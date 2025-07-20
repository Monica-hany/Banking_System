using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MiniBank
{
    public partial class MyAccounts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    int userId = (int)Session["UserID"];
                    LoadUserAccounts(userId);
                    LoadUserName(userId); // Set the username for welcome message
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }
        private void LoadUserName(int userId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT Fname FROM Users WHERE User_id = @UserId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                object result = cmd.ExecuteScalar();
                conn.Close();

                if (result != null)
                {
                    lblUsername.Text = result.ToString();
                }
            }
        }

        private void LoadUserAccounts(int userId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                SELECT A.Account_ID,
                       T.AccountType_name,
                       A.Balance,
                       C.Currency_name,
                       A.Balance * C.Currency_value AS BalanceInEGP,
                       A.Date_opened,
                       S.Status_name
                FROM Accounts A
                JOIN Look_AccountType T ON A.AccountType_id = T.AccountType_id
                JOIN Look_AccountStatus S ON A.AccountStatus_id = S.Status_id
                JOIN Look_Currency C ON A.Currency_id = C.Currency_id
                WHERE A.User_id = @UserId";


                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                bool hasAccounts = false;
                bool hasActiveAccount = false;

                DataTable dt = new DataTable();
                dt.Load(reader);

                if (dt.Rows.Count > 0)
                {
                    hasAccounts = true;
                    foreach (DataRow row in dt.Rows)
                    {
                        string status = row["Status_name"].ToString();
                        if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                        {
                            hasActiveAccount = true;
                            break;
                        }
                    }
                }

                gvAccounts.DataSource = dt;
                gvAccounts.DataBind();

                lblNoAccounts.Visible = !hasAccounts;

                // Enable/disable buttons based on active account
                btnTransfer.Enabled = hasActiveAccount;
                // btnViewTransfers.Enabled = hasActiveAccount;

                conn.Close();
            }
        }

        protected void btnCreateAccount_Click(object sender, EventArgs e)
        {
            Response.Redirect("Create_Account.aspx");
        }

        protected void btnTransfer_Click(object sender, EventArgs e)
        {
            Response.Redirect("Transfer.aspx"); // 🔁 Update this if your transfer page has a different name
        }

        protected void Logout_Link_Click(object sender, EventArgs e)
        {
            Session.Abandon(); // Destroys the session
            Response.Redirect("Login.aspx"); // Redirects to login page
        }

        protected void btnViewTransfers_Click(object sender, EventArgs e)
        {
            Response.Redirect("TransferHistory.aspx");
        }

        protected void backToHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("Home.aspx");
        }
    }
}