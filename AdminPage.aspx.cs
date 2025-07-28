using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;
using System.IO;

namespace MiniBank
{
    public partial class AdminPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            if (Session["admin"] == null)
            {
                // No admin is logged in — redirect to login page
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                
            }
        }

        protected void UserRequests_btn_Click(object sender, EventArgs e)
        {
            Response.Redirect("UserRequests_AdminPage.aspx");
        }

        protected void AccountRequests_btn_Click(object sender, EventArgs e)
        {
            Response.Redirect("AccountRequests_AdminPage.aspx");
        }

        protected void Logout_Link_Click(object sender, EventArgs e)
        {
            Session.Abandon(); // Destroys the session
            Response.Redirect("Login.aspx"); // Redirects to login page
        }

        /// <summary>
        /// The method "ExtractFile_btn_Click" extracts all unmarked transactions from the database and generates a CSV file on disk.
        ///
        /// 📁 Output File Format:
        /// Each row represents a transaction, and the columns are:
        ///     1. Sender Name (First + Last)
        ///     2. Receiver Name (First + Last)
        ///     3. Amount
        ///     4. Currency (e.g., EGP, USD)
        ///     5. Timestamp (yyyy-MM-dd HH:mm:ss format)
        ///     6. Sender Email
        ///     7. Receiver Email
        ///     8. Sender Account Type (e.g., Savings, Checking)
        ///     9. Receiver Account Type
        ///
        /// ⚠️ NOTE:
        /// - The file is saved **locally** to the specified folder on the server.
        /// - Transactions marked as `IsExtracted = 1` will be excluded.
        /// - The stored procedure used is `ExportTransactionsToTextFile` (defined below).
        /// </summary>

        protected void ExtractFile_btn_Click(object sender, EventArgs e)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;

            string folderPath = @"C:\Users\DELL\Desktop\MiniBank\Extracted Transaction Files"; //If you are using my code from github; plz edit this line -> CHANGE THIS TO A FOLDER YOU WILLL SAVE THE FILES IN ON UR PC
            string fileName = "Extracted_Transactions_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
            string filePath = Path.Combine(folderPath, fileName);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ExportTransactionsToTextFile", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        using (StreamWriter sw = new StreamWriter(filePath))
                        {
                            // Optional header
                            sw.WriteLine("Sender Name;Receiver Name;Amount;Currency;Timestamp;Sender Email;Receiver Email;Sender Account Type;Receiver Account Type");

                            // Write each DataLine row to file
                            while (reader.Read())
                            {
                                sw.WriteLine(reader["DataLine"].ToString());
                            }
                        }
                    }

                    conn.Close();
                }
            }

            // ✅ Show popup on success
            // ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('CSV file successfully created at: " + filePath.Replace("\\", "\\\\") + "');", true);

            // Show popup WITHOUT full page reload
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('CSV file successfully created at: " + filePath.Replace("\\", "\\\\") + "');", true);



            /*THIS IS JUST FOR GITHUB REFERENCE -> DO NOT REMOVE IT*/
            //THE STORED PROCEDURE I CREATED IN SQL
            /*
             
            CREATE OR ALTER PROCEDURE ExportTransactionsToTextFile
            AS
            BEGIN
                SET NOCOUNT ON;

                -- Step 1: Truncate the temp table
                IF OBJECT_ID('dbo.temp_table') IS NOT NULL
                    TRUNCATE TABLE dbo.temp_table;

                -- Step 2: Insert new transaction data as delimited strings
                INSERT INTO temp_table (DataLine)
                SELECT 
                    CONCAT_WS(';',
                        -- Sender Full Name
                        ISNULL(SU.Fname, '') + ' ' + ISNULL(SU.Lname, ''),
                        -- Receiver Full Name
                        ISNULL(RU.Fname, '') + ' ' + ISNULL(RU.Lname, ''),
                        -- Transaction Amount
                        CAST(T.Amount AS NVARCHAR),
                        -- Currency Name
                        ISNULL(C.Currency_name, ''),
                        -- Timestamp
                        CONVERT(NVARCHAR, T.TimeStamp, 120),
                        -- Sender Email
                        ISNULL(SU.email, ''),
                        -- Receiver Email
                        ISNULL(RU.email, ''),
                        -- Sender Account Type
                        ISNULL(S_AT.AccountType_name, ''),
                        -- Receiver Account Type
                        ISNULL(R_AT.AccountType_name, '') --,


                        -- Sender Balance in EGP
                       -- CAST(SA.Balance * C.Currency_value AS NVARCHAR),
                        -- Receiver Balance in EGP
                      --  CAST(RA.Balance * C.Currency_value AS NVARCHAR)


                    ) AS DataLine
                FROM Transactions T
                -- Sender side
                INNER JOIN Accounts SA ON T.Sender_Account_id = SA.Account_ID
                INNER JOIN Users SU ON SA.User_ID = SU.User_id
                INNER JOIN Look_AccountType S_AT ON SA.AccountType_id = S_AT.AccountType_id

                -- Receiver side
                INNER JOIN Accounts RA ON T.Receiver_Account_id = RA.Account_ID
                INNER JOIN Users RU ON RA.User_ID = RU.User_id
                INNER JOIN Look_AccountType R_AT ON RA.AccountType_id = R_AT.AccountType_id

                -- Currency (same for both sender and receiver assumed)
                INNER JOIN Look_Currency C ON SA.Currency_id = C.Currency_id

                WHERE T.IsExtracted = 0;

                -- Step 3: Mark transactions as extracted
                UPDATE Transactions
                SET IsExtracted = 1
                WHERE IsExtracted = 0;

               -- Return the contents of temp_table
                SELECT DataLine FROM temp_table;

            END
             */

        }

    }
}
