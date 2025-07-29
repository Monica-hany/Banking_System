using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;


namespace MiniBank
{
    public partial class Transfer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ChangeEmail_btn.Visible = false;
                SearchRecepient_btn.Visible = true;
                RecepientEmail_txtBox.Enabled = true;
                ScheduledDate_txtBox.Text = DateTime.Today.ToString("yyyy-MM-dd");

                if (Session["UserID"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                if (UserHasActiveAccounts())
                {
                    pnlTransferForm.Visible = true;
                    lblNoActiveAccounts.Visible = false;
                    btnBackHome.Visible = false;
                    LoadSenderAccounts();
                }
                else
                {
                    pnlTransferForm.Visible = false;
                    lblNoActiveAccounts.Visible = true;
                    btnBackHome.Visible = true;
                }
            }
        }


        private bool UserHasActiveAccounts()
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT COUNT(*) FROM Accounts WHERE User_ID = @UserId AND AccountStatus_id = 3";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                int cnt = (int)cmd.ExecuteScalar();
                return cnt > 0;
            }
        }


        private void LoadSenderAccounts()
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                SELECT A.Account_ID,
                       LAT.AccountType_name + ' - Balance: ' + CAST(A.Balance AS VARCHAR) + ' ' + LC.Currency_name AS DisplayText
                FROM Accounts A
                INNER JOIN Look_AccountType LAT ON A.AccountType_id = LAT.AccountType_id
                INNER JOIN Look_Currency LC ON A.Currency_id = LC.Currency_id
                WHERE A.User_ID = @UserId AND A.AccountStatus_id = 3";  //only active


                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                SenderAccounts_ddl.Items.Clear();
                SenderAccounts_ddl.Items.Add(new ListItem("-- Select --", ""));

                while (reader.Read())
                {
                    string text = reader["DisplayText"].ToString();
                    string value = reader["Account_ID"].ToString();
                    SenderAccounts_ddl.Items.Add(new ListItem(text, value));
                }

                conn.Close();
            }
        }


        protected void SearchRecepient_btn_Click(object sender, EventArgs e)
        {
            lblRecipientNotFound.Visible = false;
            lblNoRecipientAccounts.Visible = false;
            lblNoMoreRecipientAccounts.Visible = false; // Reset new label visibility
            ChangeEmail_btn.Visible = false;
            SearchRecepient_btn.Visible = true; // Keep default

            string email = RecepientEmail_txtBox.Text.Trim();
            string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string getUserQuery = "SELECT User_ID FROM Users WHERE Email = @Email";
                SqlCommand getUserCmd = new SqlCommand(getUserQuery, conn);
                getUserCmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                object userIdObj = getUserCmd.ExecuteScalar();

                if (userIdObj == null)
                {
                    lblRecipientNotFound.Visible = true;
                    RecepientAccounts_ddl.Items.Clear();
                    RecepientAccounts_ddl.Items.Add(new ListItem("-- Select --", ""));
                    return;
                }

                int recipientUserId = Convert.ToInt32(userIdObj);
                RecepientAccounts_ddl.Items.Clear();
                RecepientAccounts_ddl.Items.Add(new ListItem("-- Select --", ""));

                string senderEmail = "";
                string getSenderEmailQuery = "SELECT Email FROM Users WHERE User_ID = @SenderId";
                SqlCommand getSenderEmailCmd = new SqlCommand(getSenderEmailQuery, conn);
                getSenderEmailCmd.Parameters.AddWithValue("@SenderId", Session["UserID"]);
                object senderEmailObj = getSenderEmailCmd.ExecuteScalar();
                if (senderEmailObj != null)
                    senderEmail = senderEmailObj.ToString();

                // Get the sender account selected in the dropdown
                string selectedSenderAccountId = SenderAccounts_ddl.SelectedValue;

                // First: Get total active accounts count for recipient
                string countAccountsQuery = @"
                SELECT COUNT(*) FROM Accounts 
                WHERE User_ID = @UserId AND AccountStatus_id = 3";
                SqlCommand countAccountsCmd = new SqlCommand(countAccountsQuery, conn);
                countAccountsCmd.Parameters.AddWithValue("@UserId", recipientUserId);
                int totalActiveAccounts = (int)countAccountsCmd.ExecuteScalar();

                SqlCommand getAccountsCmd;

                if (senderEmail.Equals(email, StringComparison.OrdinalIgnoreCase))
                {
                    // Recipient is the same user — exclude sender account from recipient accounts list
                    string query = @"
                    SELECT A.Account_ID, LAT.AccountType_name, A.Balance, LC.Currency_name
                    FROM Accounts A
                    INNER JOIN Look_AccountType LAT ON A.AccountType_id = LAT.AccountType_id
                    INNER JOIN Look_Currency LC ON A.Currency_id = LC.Currency_id
                    WHERE A.User_ID = @UserId AND A.AccountStatus_id = 3
                    AND A.Account_ID <> @SenderAccountId";

                    getAccountsCmd = new SqlCommand(query, conn);
                    getAccountsCmd.Parameters.AddWithValue("@UserId", recipientUserId);
                    getAccountsCmd.Parameters.AddWithValue("@SenderAccountId", selectedSenderAccountId);
                }
                else
                {
                    // Recipient is different user — show all their active accounts
                    string query = @"
                    SELECT A.Account_ID, LAT.AccountType_name, A.Balance, LC.Currency_name
                    FROM Accounts A
                    INNER JOIN Look_AccountType LAT ON A.AccountType_id = LAT.AccountType_id
                    INNER JOIN Look_Currency LC ON A.Currency_id = LC.Currency_id
                    WHERE A.User_ID = @UserId AND A.AccountStatus_id = 3";

                    getAccountsCmd = new SqlCommand(query, conn);
                    getAccountsCmd.Parameters.AddWithValue("@UserId", recipientUserId);
                }

                SqlDataReader reader = getAccountsCmd.ExecuteReader();
                bool hasAccountsAfterExclusion = false;

                while (reader.Read())
                {
                    hasAccountsAfterExclusion = true;
                    string display = "";

                    if (senderEmail.Equals(email, StringComparison.OrdinalIgnoreCase))
                    {
                        display = $"{reader["AccountType_name"]} - Balance: {reader["Balance"]} {reader["Currency_name"]}";
                    }
                    else
                    {
                        display = $"{reader["AccountType_name"]} - {reader["Currency_name"]}";
                    }

                    RecepientAccounts_ddl.Items.Add(new ListItem(display, reader["Account_ID"].ToString()));
                }

                reader.Close();

                if (totalActiveAccounts == 0)
                {
                    // Recipient has no active accounts at all
                    lblNoRecipientAccounts.Visible = true;
                }
                else if (!hasAccountsAfterExclusion)
                {
                    // Recipient has active accounts but none other than the sender's selected account
                    lblNoMoreRecipientAccounts.Visible = true;
                }
                else
                {
                    // Success: disable recipient email input and toggle buttons
                    RecepientEmail_txtBox.Enabled = false;
                    ChangeEmail_btn.Visible = true;
                    SearchRecepient_btn.Visible = false;
                }
            }
        }

        protected void btnBackHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("Home.aspx");
        }

        protected void Transfer_btn_Click(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            rfvRecepient.Enabled = true;
            rfvAmount.Enabled = true;
            revAmount.Enabled = true;
            Page.Validate();
            if (!Page.IsValid) return;

            if (!decimal.TryParse(TransferAmount_txtBox.Text.Trim(), out decimal originalAmount) || originalAmount <= 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Enter a valid positive amount.');", true);
                return;
            }

            int fromAccountId = int.Parse(SenderAccounts_ddl.SelectedValue);
            int toAccountId = int.Parse(RecepientAccounts_ddl.SelectedValue);

            string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Get sender's balance and currency
                    string senderQuery = "SELECT Balance, Currency_id FROM Accounts WHERE Account_ID = @FromId";
                    SqlCommand senderCmd = new SqlCommand(senderQuery, conn, transaction);
                    senderCmd.Parameters.AddWithValue("@FromId", fromAccountId);
                    SqlDataReader senderReader = senderCmd.ExecuteReader();

                    if (!senderReader.Read())
                    {
                        throw new Exception("Sender account not found.");
                    }

                    decimal senderBalance = Convert.ToDecimal(senderReader["Balance"]);
                    int senderCurrencyId = Convert.ToInt32(senderReader["Currency_id"]);
                    senderReader.Close();

                    // Get recipient's currency and status
                    string recipientQuery = "SELECT Currency_id, AccountStatus_id FROM Accounts WHERE Account_ID = @ToId";
                    SqlCommand recipientCmd = new SqlCommand(recipientQuery, conn, transaction);
                    recipientCmd.Parameters.AddWithValue("@ToId", toAccountId);
                    SqlDataReader recipientReader = recipientCmd.ExecuteReader();

                    if (!recipientReader.Read())
                    {
                        throw new Exception("Recipient account not found.");
                    }

                    int recipientCurrencyId = Convert.ToInt32(recipientReader["Currency_id"]);
                    int recipientStatus = Convert.ToInt32(recipientReader["AccountStatus_id"]);
                    recipientReader.Close();

                    if (recipientStatus != 3) // 3 = Active
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Recipient account is not active.');", true);
                        transaction.Rollback();
                        return;
                    }

                    // Get currency values
                    decimal senderRate = GetCurrencyValue(senderCurrencyId, conn, transaction);
                    decimal recipientRate = GetCurrencyValue(recipientCurrencyId, conn, transaction);

                    // Convert amount
                    decimal amountInEGP = originalAmount * senderRate;
                    decimal convertedAmount = amountInEGP / recipientRate;

                    if (senderBalance < originalAmount)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Insufficient balance.');", true);
                        transaction.Rollback();
                        return;
                    }
                    int needJob = 1; 
                    if (DateTime.Parse(ScheduledDate_txtBox.Text).Date == DateTime.Now.Date)
                    {
                        needJob = 0;
                        // Deduct from sender
                        string deductQuery = "UPDATE Accounts SET Balance = Balance - @Amount WHERE Account_ID = @FromId";
                        SqlCommand deductCmd = new SqlCommand(deductQuery, conn, transaction);
                        deductCmd.Parameters.AddWithValue("@Amount", originalAmount);
                        deductCmd.Parameters.AddWithValue("@FromId", fromAccountId);
                        deductCmd.ExecuteNonQuery();

                        // Add to recipient (in their currency)
                        string addQuery = "UPDATE Accounts SET Balance = Balance + @Amount WHERE Account_ID = @ToId";
                        SqlCommand addCmd = new SqlCommand(addQuery, conn, transaction);
                        addCmd.Parameters.AddWithValue("@Amount", convertedAmount);
                        addCmd.Parameters.AddWithValue("@ToId", toAccountId);
                        addCmd.ExecuteNonQuery();
                    }
                        // Insert into Transactions table
                        string insertTransactionQuery = @"
                        INSERT INTO Transactions (TimeStamp, Amount, Sender_Account_id, Receiver_Account_id,ValueDate,NeedsJob)
                        VALUES (GETDATE(), @OriginalAmount, @SenderId, @ReceiverId,@ValueDate, @NeedsJob)";
                            SqlCommand insertTransactionCmd = new SqlCommand(insertTransactionQuery, conn, transaction);
                            insertTransactionCmd.Parameters.AddWithValue("@OriginalAmount", originalAmount); // You can log the EGP amount too if you like as a future enhancement
                            insertTransactionCmd.Parameters.AddWithValue("@SenderId", fromAccountId);
                            insertTransactionCmd.Parameters.AddWithValue("@ReceiverId", toAccountId);
                            insertTransactionCmd.Parameters.AddWithValue("@ValueDate", DateTime.Parse(ScheduledDate_txtBox.Text).Date);
                            insertTransactionCmd.Parameters.AddWithValue("@NeedsJob", needJob);
                            insertTransactionCmd.ExecuteNonQuery();

                            transaction.Commit();

                    Response.AddHeader("REFRESH", "2;URL=TransferHistory.aspx");
                    ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "showSuccessModal();", true);
                 }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Transfer failed: {ex.Message}');", true);
                }
            }
        }

        // 🔄 Helper: to get exchange rate
        private decimal GetCurrencyValue(int currencyId, SqlConnection conn, SqlTransaction transaction)
        {
            string query = "SELECT Currency_value FROM Look_Currency WHERE Currency_id = @Id";
            SqlCommand cmd = new SqlCommand(query, conn, transaction);
            cmd.Parameters.AddWithValue("@Id", currencyId);
            object result = cmd.ExecuteScalar();
            return Convert.ToDecimal(result);
        }

        // 🔄 Helper: Get CurrencyId from Accounts table
        private int GetCurrencyId(int accountId, SqlConnection conn, SqlTransaction transaction)
        {
            string query = "SELECT Currency_id FROM Accounts WHERE Account_ID = @AccId";
            SqlCommand cmd = new SqlCommand(query, conn, transaction);
            cmd.Parameters.AddWithValue("@AccId", accountId);
            object result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 1; // fallback to 1 (EGP)
        }

        // 🔄 Helper: Get Currency Rate from Look_Currency
        private decimal GetCurrencyRate(int currencyId, SqlConnection conn, SqlTransaction transaction)
        {
            string query = "SELECT Currency_value FROM Look_Currency WHERE Currency_id = @CurrencyId";
            SqlCommand cmd = new SqlCommand(query, conn, transaction);
            cmd.Parameters.AddWithValue("@CurrencyId", currencyId);
            object result = cmd.ExecuteScalar();
            return result != null ? Convert.ToDecimal(result) : 1.0m;
        }

        protected void ChangeEmail_btn_Click(object sender, EventArgs e)
        {
            RecepientEmail_txtBox.Enabled = true;                            // ✅ Enable email input
            RecepientEmail_txtBox.Text = "";                                 // 🔄 Clear it

            RecepientAccounts_ddl.Items.Clear();                             // 🔄 Clear dropdown
            RecepientAccounts_ddl.Items.Add(new ListItem("-- Select --", ""));

            lblRecipientNotFound.Visible = false;
            lblNoRecipientAccounts.Visible = false;

            ChangeEmail_btn.Visible = false;                                 // ❌ Hide "Change Email"
            SearchRecepient_btn.Visible = true;                              // ✅ Show "Search"
        }

        protected void cvScheduledDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            DateTime selectedDate;
            if (DateTime.TryParse(ScheduledDate_txtBox.Text, out selectedDate))
            {
                args.IsValid = selectedDate.Date >= DateTime.Today;
            }
            else
            {
                args.IsValid = false;
            }
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