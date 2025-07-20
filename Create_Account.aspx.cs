using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MiniBank
{
    public partial class Create_Account : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                int userId = Convert.ToInt32(Session["UserID"]);
                LoadAvailableAccountTypes(userId);
                LoadCurrencies();
            }
        }



        private void LoadAvailableAccountTypes(int userId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                SELECT LAT.AccountType_id, LAT.AccountType_name
                FROM Look_AccountType LAT
                WHERE LAT.AccountType_id NOT IN (
                    SELECT AccountType_id
                    FROM Accounts
                    WHERE User_id = @UserId
                      AND AccountStatus_id != 5
                )";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                AccountType_ddl.Items.Clear();
                AccountType_ddl.Items.Add(new ListItem("-- Select Account Type --", ""));

                bool hasAvailableTypes = false;
                while (reader.Read())
                {
                    hasAvailableTypes = true;
                    string name = reader["AccountType_name"].ToString();
                    string id = reader["AccountType_id"].ToString();
                    AccountType_ddl.Items.Add(new ListItem(name, id));
                }

                conn.Close();

                if (!hasAvailableTypes)
                {
                    // No account types left to create
                    pnlCreateAccountForm.Visible = false;
                    pnlAllAccountTypesMsg.Visible = true;
                }
                else
                {
                    pnlCreateAccountForm.Visible = true;
                    pnlAllAccountTypesMsg.Visible = false;
                }
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

                ddlCurrency.Items.Clear();
                ddlCurrency.Items.Add(new ListItem("-- Select Currency --", ""));

                while (reader.Read())
                {
                    string name = reader["Currency_name"].ToString();
                    string id = reader["Currency_id"].ToString();
                    ddlCurrency.Items.Add(new ListItem(name, id));
                }

                conn.Close();
            }
        }

        protected void createAccount_btn_Click(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            int userId = Convert.ToInt32(Session["UserID"]);
            string accountTypeIdStr = AccountType_ddl.SelectedValue;
            string balanceStr = balance_txtBox.Text.Trim();
            string currencyIdStr = ddlCurrency.SelectedValue;

            // Validate account type
            if (!int.TryParse(accountTypeIdStr, out int accountTypeId))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Please select a valid account type.');", true);
                return;
            }

            // Validate balance
            if (!decimal.TryParse(balanceStr, out decimal balance))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Invalid balance entered.');", true);
                return;
            }

            // Validate currency
            if (!int.TryParse(currencyIdStr, out int currencyId))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Please select a valid currency.');", true);
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;

            // Check for duplicate account type (not closed)
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string checkQuery = @"
                    SELECT COUNT(*) 
                    FROM Accounts 
                    WHERE User_id = @UserId 
                      AND AccountType_id = @TypeId
                      AND AccountStatus_id != 5";

                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@UserId", userId);
                checkCmd.Parameters.AddWithValue("@TypeId", accountTypeId);

                conn.Open();
                int existingCount = (int)checkCmd.ExecuteScalar();
                conn.Close();

                if (existingCount > 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('You already have an account of this type.');", true);
                    return;
                }
            }

            // Insert account
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string insertQuery = @"
                    INSERT INTO Accounts (User_id, Balance, AccountType_id, AccountStatus_id, Date_opened, Branch_id, Currency_id)
                    VALUES (@UserId, @Balance, @TypeId, 1, @OpenedDate, 1, @CurrencyId)";

                SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@UserId", userId);
                insertCmd.Parameters.AddWithValue("@Balance", balance);
                insertCmd.Parameters.AddWithValue("@TypeId", accountTypeId);
                insertCmd.Parameters.AddWithValue("@OpenedDate", DateTime.Now);
                insertCmd.Parameters.AddWithValue("@CurrencyId", currencyId);

                conn.Open();
                insertCmd.ExecuteNonQuery();
                conn.Close();
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Account request submitted successfully. Awaiting admin approval.');", true);
            Response.Redirect("MyAccounts.aspx");
        }

        protected void AccountType_ddl_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void AccountType_ddl_SelectedIndexChanged1(object sender, EventArgs e)
        {
            
        }

        protected void btnViewMyAccounts_Click(object sender, EventArgs e)
        {
            Response.Redirect("MyAccounts.aspx");
        }

        protected void Logout_Link_Click(object sender, EventArgs e)
        {
            Session.Abandon(); // Destroys the session
            Response.Redirect("Login.aspx"); // Redirects to login page
        }

        protected void backToHome_btn_Click(object sender, EventArgs e)
        {
            Response.Redirect("Home.aspx");
        }
    }
}
