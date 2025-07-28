using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace MiniBank
{
    public partial class ExtractFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ExtractFile_btn_Click(object sender, EventArgs e)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;

            string folderPath = @"C:\Users\DELL\Desktop\MiniBank\Extracted Transaction Files";
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
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('CSV file successfully created at: " + filePath.Replace("\\", "\\\\") + "');", true);
        }


    }
}
