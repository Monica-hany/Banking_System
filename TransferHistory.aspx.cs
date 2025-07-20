using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace MiniBank
{
    public partial class TransferHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                LoadTransferHistory(); // Initial full load
            }
        }

        private void LoadTransferHistory()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;
            int userId = Convert.ToInt32(Session["UserID"]);

            string query = @"
            SELECT 
                T.Transaction_ID, 
                T.TimeStamp, 
                T.Amount, 
                LC.Currency_name AS Currency,  -- ✅ Sender's currency name
                LEFT(U1.Email, CHARINDEX('@', U1.Email)-1) + '@****' + ' (' + LAT1.AccountType_name + ')' AS FromAccount,
                LEFT(U2.Email, CHARINDEX('@', U2.Email)-1) + '@****' + ' (' + LAT2.AccountType_name + ')' AS ToAccount
            FROM Transactions T
            INNER JOIN Accounts A1 ON T.Sender_Account_id = A1.Account_ID
            INNER JOIN Accounts A2 ON T.Receiver_Account_id = A2.Account_ID
            INNER JOIN Users U1 ON A1.User_ID = U1.User_ID
            INNER JOIN Users U2 ON A2.User_ID = U2.User_ID
            INNER JOIN Look_AccountType LAT1 ON A1.AccountType_id = LAT1.AccountType_id
            INNER JOIN Look_AccountType LAT2 ON A2.AccountType_id = LAT2.AccountType_id
            INNER JOIN Look_Currency LC ON A1.Currency_id = LC.Currency_id  -- ✅ Join to get currency name
            WHERE A1.User_id = @UserId OR A2.User_id = @UserId
            ORDER BY T.TimeStamp DESC

            ";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(reader);
                ViewState["TransferData"] = dt;

                if (dt.Rows.Count > 0)
                {
                    gvTransfers.DataSource = dt;
                    gvTransfers.DataBind();
                    lblNoTransfersAll.Visible = false;
                }
                else
                {
                    gvTransfers.DataSource = null;
                    gvTransfers.DataBind();
                    lblNoTransfersAll.Visible = true;
                }

                lblNoTransfersFiltered.Visible = false;
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            int userId = Convert.ToInt32(Session["UserID"]);
            string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;

            string query = @"
            SELECT 
                T.Transaction_ID, 
                T.TimeStamp, 
                T.Amount, 
                LC.Currency_name AS Currency,  -- ✅ Sender's currency name
                LEFT(U1.Email, CHARINDEX('@', U1.Email)-1) + '@****' + ' (' + LAT1.AccountType_name + ')' AS FromAccount,
                LEFT(U2.Email, CHARINDEX('@', U2.Email)-1) + '@****' + ' (' + LAT2.AccountType_name + ')' AS ToAccount
            FROM Transactions T
            INNER JOIN Accounts A1 ON T.Sender_Account_id = A1.Account_ID
            INNER JOIN Accounts A2 ON T.Receiver_Account_id = A2.Account_ID
            INNER JOIN Users U1 ON A1.User_ID = U1.User_ID
            INNER JOIN Users U2 ON A2.User_ID = U2.User_ID
            INNER JOIN Look_AccountType LAT1 ON A1.AccountType_id = LAT1.AccountType_id
            INNER JOIN Look_AccountType LAT2 ON A2.AccountType_id = LAT2.AccountType_id
            INNER JOIN Look_Currency LC ON A1.Currency_id = LC.Currency_id  -- ✅ Join to get currency name
            WHERE A1.User_id = @UserId OR A2.User_id = @UserId
            ORDER BY T.TimeStamp DESC

            ";

            DateTime fromDate, toDate;
            bool hasFrom = DateTime.TryParse(txtFromDate.Text, out fromDate);
            bool hasTo = DateTime.TryParse(txtToDate.Text, out toDate);

            if (hasFrom)
                query += " AND T.TimeStamp >= @FromDate";
            if (hasTo)
                query += " AND T.TimeStamp <= @ToDate";

            query += " ORDER BY T.TimeStamp DESC";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                if (hasFrom)
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                if (hasTo)
                    cmd.Parameters.AddWithValue("@ToDate", toDate.AddDays(1));  // include full day

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(reader);
                ViewState["TransferData"] = dt;

                if (dt.Rows.Count > 0)
                {
                    gvTransfers.PageIndex = 0;
                    gvTransfers.DataSource = dt;
                    gvTransfers.DataBind();
                    lblNoTransfersFiltered.Visible = false;
                }
                else
                {
                    gvTransfers.DataSource = null;
                    gvTransfers.DataBind();
                    lblNoTransfersFiltered.Visible = true;
                }

                lblNoTransfersAll.Visible = false;
            }
        }

        protected void btnBackToHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("Home.aspx");
        }

        protected void gvTransfers_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            gvTransfers.PageIndex = e.NewPageIndex;
            if (ViewState["TransferData"] != null)
            {
                gvTransfers.DataSource = ViewState["TransferData"];
                gvTransfers.DataBind();
            }
        }

        protected void Logout_Link_Click(object sender, EventArgs e)
        {
            Session.Abandon(); // Destroys the session
            Response.Redirect("Login.aspx"); // Redirects to login page
        }
    }
}
