using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

// NPOI namespaces
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace MiniBank
{
    public partial class Insert_Transaction_File : System.Web.UI.Page
    {


        private bool IsRowEmpty(IRow row)
        {
            if (row == null) return true;

            foreach (var cell in row.Cells)
            {
                if (cell != null && !string.IsNullOrWhiteSpace(cell.ToString()))
                    return false;
            }

            return true;
        }

        private const string TemplatePath = "~/Templates/TransactionsTemplate.xlsx";
        private const string SessionKey = "UploadedTransactions";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                GridView1.Visible = false;
                btnRemoveRejected.Visible = false;
                btnMakeAllTransactions.Visible = false;
                lblUploadMessage.Text = string.Empty;
            }
        }


        private DataTable GetAllAccounts()
        {
            DataTable dt = new DataTable();
            string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Accounts", conn))
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        protected void Logout_Link_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        protected void btnDownloadTemplate_Click(object sender, EventArgs e)
        {
            string fullPath = Server.MapPath(TemplatePath);
            if (File.Exists(fullPath))
            {
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-Disposition", $"attachment; filename=TransactionTemplate.xlsx");
                Response.TransmitFile(fullPath);
                Response.End();
            }
            else
            {
                lblUploadMessage.Text = "Template file not found.";
            }
        }

        protected void btnUploadFile_Click(object sender, EventArgs e)
        {
            if (!fileUpload.HasFile)
            {
                lblUploadMessage.Text = "Please select a file to upload.";
                return;
            }

            string fileExt = Path.GetExtension(fileUpload.FileName);
            if (fileExt != ".xlsx")
            {
                lblUploadMessage.Text = "Only .xlsx files are allowed.";
                return;
            }

            try
            {
                int userId = Convert.ToInt32(Session["UserID"]);
                List<Transaction> transactions = new List<Transaction>();

                DataTable accountsTable = GetAllAccounts();
                Dictionary<int, decimal> accountBalances = accountsTable.AsEnumerable()
                    .ToDictionary(r => Convert.ToInt32(r["Account_id"]),
                                  r => Convert.ToDecimal(r["Balance"]));

                HashSet<int> validAccountIds = new HashSet<int>(accountBalances.Keys);

                using (var stream = fileUpload.PostedFile.InputStream)
                {
                    IWorkbook workbook = new XSSFWorkbook(stream);
                    ISheet sheet = workbook.GetSheetAt(0);

                    int serial = 0;

                    for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                    {
                        IRow currentRow = sheet.GetRow(rowIndex);
                        if (IsRowEmpty(currentRow)) continue;


                        serial++;  // Increment serial for every row read (preserve order)

                        var tx = new Transaction();
                        bool isValid = true;
                        string rejectReason = "";

                        string GetCellString(int index) =>
                            currentRow.GetCell(index)?.ToString().Trim() ?? "";

                        tx.RawSenderAccountId = GetCellString(0);
                        tx.RawRecipientAccountId = GetCellString(1);
                        tx.RawValueDate = GetCellString(2);
                        tx.RawAmount = GetCellString(3);

                        // Sender validation
                        if (!int.TryParse(tx.RawSenderAccountId, out int senderId))
                        {
                            isValid = false;
                            rejectReason += "Invalid Sender Account ID. ";
                        }
                        else if (!validAccountIds.Contains(senderId))
                        {
                            isValid = false;
                            rejectReason += "Sender Account ID does not exist. ";
                        }
                        else if (!SenderAccountBelongsToUser(senderId, userId))
                        {
                            isValid = false;
                            rejectReason += "Sender Account does not belong to current user. ";
                        }
                        else
                        {
                            tx.SenderAccountId = senderId;
                        }

                        // Recipient validation
                        if (!int.TryParse(tx.RawRecipientAccountId, out int recipientId))
                        {
                            isValid = false;
                            rejectReason += "Invalid Recipient Account ID. ";
                        }
                        else if (!validAccountIds.Contains(recipientId))
                        {
                            isValid = false;
                            rejectReason += "Recipient Account ID does not exist. ";
                        }
                        else
                        {
                            tx.RecipientAccountId = recipientId;
                        }

                        // Value Date
                        if (!DateTime.TryParse(tx.RawValueDate, out DateTime valueDate))
                        {
                            isValid = false;
                            rejectReason += "Invalid Value Date. ";
                        }
                        else if (valueDate < DateTime.Today)
                        {
                            isValid = false;
                            rejectReason += "Value Date cannot be in the past. ";
                        }
                        else
                        {
                            tx.ValueDate = valueDate;
                        }

                        // Amount + Balance Check
                        if (!decimal.TryParse(tx.RawAmount, out decimal amount))
                        {
                            isValid = false;
                            rejectReason += "Invalid Amount. ";
                        }
                        else
                        {
                            tx.Amount = amount;

                            if (tx.SenderAccountId != 0 &&
                                accountBalances.ContainsKey(tx.SenderAccountId) &&
                                accountBalances[tx.SenderAccountId] < amount)
                            {
                                isValid = false;
                                rejectReason += "Insufficient Balance. ";
                            }
                        }

                        tx.Status = isValid ? "Valid" : "Rejected";
                        tx.RejectionReason = isValid ? "" : rejectReason.Trim();

                        // Use serial to keep original order regardless of validity
                        tx.Serial = serial;

                        transactions.Add(tx);
                    }

                    // Detect duplicates
                    var seen = new Dictionary<string, Transaction>();

                    foreach (var tx in transactions)
                    {
                        string key = $"{tx.RawSenderAccountId}|{tx.RawRecipientAccountId}|{tx.RawValueDate}|{tx.RawAmount}";

                        if (seen.ContainsKey(key))
                        {
                            tx.Status = "Rejected";
                            tx.RejectionReason = "Duplicate transaction.";
                            // Keep original serial to preserve order
                        }
                        else
                        {
                            seen[key] = tx;
                        }
                    }
                }

                // Store transactions in session
                Session[SessionKey] = transactions;

                // Bind GridView using helper method
                BindTransactions(transactions);

                GridView1.Visible = true;
                btnRemoveRejected.Visible = true;
                btnMakeAllTransactions.Visible = true;

                lblUploadMessage.ForeColor = System.Drawing.Color.Green;
                lblUploadMessage.Text = "File uploaded and processed.";
            }
            catch (Exception ex)
            {
                lblUploadMessage.Text = "Error processing file: " + ex.Message;
            }
        }


        private bool SenderAccountBelongsToUser(int accountId, int userId)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString))
            {
                con.Open();
                string query = "SELECT COUNT(*) FROM Accounts WHERE Account_id = @accountId AND User_id = @userId";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@accountId", accountId);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;

            if (Session[SessionKey] is List<Transaction> txList)
            {
                List<Transaction> filteredList;

                if (ddlSort.SelectedValue == "rejected")
                {
                    filteredList = txList
                        .OrderBy(t => t.Status == "Valid" ? 1 : 0)
                        .ThenBy(t => t.Serial)
                        .ToList();
                }
                else // original or any other value
                {
                    filteredList = txList
                        .OrderBy(t => t.Serial)
                        .ToList();
                }

                GridView1.DataSource = filteredList.Select(t => new
                {
                    SenderAccountId = t.RawSenderAccountId,
                    RecipientAccountId = t.RawRecipientAccountId,
                    ValueDate = t.RawValueDate,
                    Amount = t.RawAmount,
                    t.Status,
                    t.RejectionReason
                }).ToList();

                GridView1.DataBind();
            }
        }



        protected void btnRemoveRejected_Click(object sender, EventArgs e)
        {
            if (Session[SessionKey] is List<Transaction> txList)
            {
                var validOnly = txList.Where(t => t.Status == "Valid").ToList();
                GridView1.DataSource = validOnly.Select(t => new
                {
                    SenderAccountId = t.RawSenderAccountId,
                    RecipientAccountId = t.RawRecipientAccountId,
                    ValueDate = t.RawValueDate,
                    Amount = t.RawAmount,
                    t.Status,
                    t.RejectionReason
                }).ToList();
                GridView1.DataBind();
                Session[SessionKey] = validOnly;
            }
        }

        protected void btnMakeAllTransactions_Click(object sender, EventArgs e)
        {
            if (Session[SessionKey] is List<Transaction> txList)
            {
                int insertedCount = 0;

                string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    foreach (var tx in txList.Where(t => t.Status == "Valid"))
                    {
                        bool isToday = tx.ValueDate.HasValue && tx.ValueDate.Value.Date == DateTime.Today;
                        int needsJob = isToday ? 0 : 1;

                        if (isToday)
                        {
                            // Check balance again
                            string balanceQuery = "SELECT Balance FROM Accounts WHERE Account_id = @SenderId";
                            decimal currentBalance = 0;

                            using (SqlCommand cmd = new SqlCommand(balanceQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@SenderId", tx.SenderAccountId);
                                object result = cmd.ExecuteScalar();
                                if (result == null || !decimal.TryParse(result.ToString(), out currentBalance))
                                    continue;
                            }

                            if (currentBalance < tx.Amount)
                            {
                                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Insufficient balance for one or more transactions.');", true);
                                return;
                            }

                            // Insert + balance update
                            string insertQuery = @"
                            INSERT INTO Transactions
                            (TimeStamp, Amount, Sender_Account_id, Receiver_Account_id, IsExtracted, ValueDate, NeedsJob)
                            VALUES
                            (GETDATE(), @Amount, @Sender, @Receiver, 0, @ValueDate, 0);

                            UPDATE Accounts SET Balance = Balance - @Amount WHERE Account_id = @Sender;
                            UPDATE Accounts SET Balance = Balance + @Amount WHERE Account_id = @Receiver;
        ";
                            using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@Amount", tx.Amount);
                                cmd.Parameters.AddWithValue("@Sender", tx.SenderAccountId);
                                cmd.Parameters.AddWithValue("@Receiver", tx.RecipientAccountId);
                                cmd.Parameters.AddWithValue("@ValueDate", tx.ValueDate);

                                try
                                {
                                    cmd.ExecuteNonQuery();
                                    insertedCount++;
                                }
                                catch
                                {
                                    // Optional: log
                                }
                            }
                        }
                        else
                        {
                            // Scheduled for future — only insert, no balance update
                            string insertQuery = @"
                                INSERT INTO Transactions
                                (TimeStamp, Amount, Sender_Account_id, Receiver_Account_id, IsExtracted, ValueDate, NeedsJob)
                                VALUES
                                (GETDATE(), @Amount, @Sender, @Receiver, 0, @ValueDate, 1);
                            ";
                            using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@Amount", tx.Amount);
                                cmd.Parameters.AddWithValue("@Sender", tx.SenderAccountId);
                                cmd.Parameters.AddWithValue("@Receiver", tx.RecipientAccountId);
                                cmd.Parameters.AddWithValue("@ValueDate", tx.ValueDate);

                                try
                                {
                                    cmd.ExecuteNonQuery();
                                    insertedCount++;
                                }
                                catch
                                {
                                    // Optional: log
                                }
                            }
                        }
                    }

                }

                // ✅ Clear session and grid
                Session[SessionKey] = null;
                GridView1.DataSource = null;
                GridView1.DataBind();

                // ✅ Hide grid and show message
                pnlGridContainer.Visible = false;
                lblSuccessMessage.Text = $"{insertedCount} transactions inserted successfully.";
                lblSuccessMessage.Visible = true;
            }
        }

        private void BindTransactions(List<Transaction> transactions)
        {
            GridView1.DataSource = transactions.Select(t => new
            {
                SenderAccountId = t.RawSenderAccountId,
                RecipientAccountId = t.RawRecipientAccountId,
                ValueDate = t.RawValueDate,
                Amount = t.RawAmount,
                t.Status,
                t.RejectionReason
            }).ToList();
            GridView1.DataBind();
        }

        protected void ddlSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            var transactions = Session[SessionKey] as List<Transaction>;
            if (transactions == null) return;

            if (ddlSort.SelectedValue == "rejected")
            {
                // Rejected first, then valid; within group, original order
                transactions = transactions
                    .OrderBy(t => t.Status == "Valid" ? 1 : 0)
                    .ThenBy(t => t.Serial)
                    .ToList();
            }
            else if (ddlSort.SelectedValue == "original")
            {
                // Just original order by serial
                transactions = transactions
                    .OrderBy(t => t.Serial)
                    .ToList();
            }

            BindTransactions(transactions);
        }



        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.DataItem != null)
            {
                dynamic dataItem = e.Row.DataItem;

                string status = "";
                string rejectionReason = "";

                try { status = dataItem.Status ?? ""; } catch { }
                try { rejectionReason = dataItem.RejectionReason ?? ""; } catch { }

                if (e.Row.Cells.Count > 5)
                {
                    TableCell statusCell = e.Row.Cells[4];
                    statusCell.CssClass = (status == "Valid") ? "status-valid" : "status-rejected";

                    TableCell rejectionCell = e.Row.Cells[5];
                    if (!string.IsNullOrWhiteSpace(rejectionReason))
                        rejectionCell.CssClass = "rejection-text";
                }
            }
        }

        protected void backHome_btn_Click(object sender, EventArgs e)
        {
            Response.Redirect("Home.aspx");
        }
    }




    public class Transaction
    {
        public int Serial { get; set; }
        public string Status { get; set; }
        public string RejectionReason { get; set; }

        public int SenderAccountId { get; set; }
        public int RecipientAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? ValueDate { get; set; }

        public string RawSenderAccountId { get; set; }
        public string RawRecipientAccountId { get; set; }
        public string RawValueDate { get; set; }
        public string RawAmount { get; set; }


    }
}
