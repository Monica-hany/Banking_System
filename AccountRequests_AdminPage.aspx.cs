using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MiniBank
{
    public partial class AccountRequests_AdminPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["admin"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            // Ensure no caching of this page
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));

            if (!IsPostBack)
            {
                LoadCurrencies();
                LoadAccountStatusFilter();
                LoadAccountRequests();
            }
        }

        private void LoadCurrencies()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT Currency_id, Currency_name FROM Look_Currency";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                ddlCurrencyFilter.Items.Clear();
                ddlCurrencyFilter.Items.Add(new ListItem("All", "All")); // Default option

                while (reader.Read())
                {
                    ddlCurrencyFilter.Items.Add(new ListItem(reader["Currency_name"].ToString(), reader["Currency_id"].ToString()));
                }

                conn.Close();
            }
        }


        private void LoadAccountStatusFilter()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT Status_id, Status_name FROM Look_AccountStatus";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                AccountStatusDropDown.Items.Clear();
                AccountStatusDropDown.Items.Add(new ListItem("All", "All"));

                while (reader.Read())
                {
                    string name = reader["Status_name"].ToString();
                    string id = reader["Status_id"].ToString();
                    AccountStatusDropDown.Items.Add(new ListItem(name, id));
                }

                conn.Close();
            }
        }
        protected void AccountStatusDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAccountRequests();
        }


        private void LoadAccountRequests()
        {
            string selectedStatus = AccountStatusDropDown.SelectedValue;
            string selectedCurrency = ddlCurrencyFilter.SelectedValue;

            string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                SELECT 
                    A.Account_ID, 
                    U.Email, 
                    T.AccountType_name, 
                    A.Balance, 
                    C.Currency_name,
                    A.Balance * C.Currency_value AS BalanceInEGP,
                    A.Date_opened,
                    S.Status_name AS AccountStatus
                FROM Accounts A
                JOIN Users U ON A.User_ID = U.User_ID
                JOIN Look_AccountType T ON A.AccountType_id = T.AccountType_id
                JOIN Look_AccountStatus S ON A.AccountStatus_id = S.Status_id
                JOIN Look_Currency C ON A.Currency_id = C.Currency_id
                WHERE 1 = 1";

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (selectedStatus != "All")
                {
                    query += " AND A.AccountStatus_id = @Status";
                    cmd.Parameters.AddWithValue("@Status", selectedStatus);
                }

                if (selectedCurrency != "All")
                {
                    query += " AND A.Currency_id = @CurrencyId";
                    cmd.Parameters.AddWithValue("@CurrencyId", selectedCurrency);
                }

                cmd.CommandText = query;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                AccountRequestsGrid.DataSource = dt;
                AccountRequestsGrid.DataBind();
            }
        }

        protected void AccountRequestsGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int accountId = Convert.ToInt32(e.CommandArgument);
            int newStatus = 0;

            switch (e.CommandName)
            {
                case "ApproveAccount": newStatus = 2; break;
                case "RejectAccount": newStatus = 5; break;
                case "ActivateAccount": newStatus = 3; break;
                case "SuspendAccount": newStatus = 4; break;
                case "CloseAccount": newStatus = 5; break;
            }

            if (newStatus > 0)
            {
                string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string updateQuery = "UPDATE Accounts SET AccountStatus_id = @Status WHERE Account_ID = @AccountId";
                    SqlCommand cmd = new SqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@Status", newStatus);
                    cmd.Parameters.AddWithValue("@AccountId", accountId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                LoadAccountRequests();
            }
        }

        protected void AccountRequestsGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string status = DataBinder.Eval(e.Row.DataItem, "AccountStatus").ToString();

                Button btnApprove = (Button)e.Row.FindControl("btnApproveAccount");
                Button btnReject = (Button)e.Row.FindControl("btnRejectAccount");
                Button btnActivate = (Button)e.Row.FindControl("btnActivate");
                Button btnSuspend = (Button)e.Row.FindControl("btnSuspend");
                Button btnClose = (Button)e.Row.FindControl("btnClose");

                if (btnApprove != null) btnApprove.Visible = false;
                if (btnReject != null) btnReject.Visible = false;
                if (btnActivate != null) btnActivate.Visible = false;
                if (btnSuspend != null) btnSuspend.Visible = false;
                if (btnClose != null) btnClose.Visible = false;

                switch (status)
                {
                    case "Pending":
                        btnApprove.Visible = true;
                        btnReject.Visible = true;
                        break;
                    case "Approved":
                        btnActivate.Visible = true;
                        btnClose.Visible = true;
                        break;
                    case "Active":
                        btnSuspend.Visible = true;
                        btnClose.Visible = true;
                        break;
                    case "Suspended":
                        btnActivate.Visible = true;
                        btnClose.Visible = true;
                        break;
                }
            }
        }

       
        protected void AccountRequestsGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            AccountRequestsGrid.PageIndex = e.NewPageIndex;
            LoadAccountRequests();
        }


        protected void ddlCurrencyFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAccountRequests(); // reload grid with selected currency
        }

        protected void BackToAdmin_btn_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminPage.aspx");
        }

        protected void Logout_Link_Click(object sender, EventArgs e)
        {
            Session.Abandon(); // Destroys the session
            Response.Redirect("Login.aspx"); // Redirects to login page
        }

    }

}