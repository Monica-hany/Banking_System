using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;


namespace MiniBank
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSecretQuestions();
            }
        }

        private void LoadSecretQuestions()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT Secret_Question_id, Secret_Question_Text FROM Look_Secret_Questions";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    ddlSecretQuestion.DataSource = cmd.ExecuteReader();
                    ddlSecretQuestion.DataTextField = "Secret_Question_Text";
                    ddlSecretQuestion.DataValueField = "Secret_Question_id";
                    ddlSecretQuestion.DataBind();
                }
                catch (Exception ex)
                {
                    ErrorLabel.Text = "Error loading secret questions: " + ex.Message;
                }
            }
        }

        protected void Fname_txtBox_TextChanged(object sender, EventArgs e)
        {

        }
        protected void Register_btn_Click(object sender, EventArgs e)
        {
            string fname = Fname_txtBox.Text.Trim();
            string lname = Lname_txtBox.Text.Trim();
            string email = email_txtBox.Text.Trim();
            string phone = PhoneNo_txtBox.Text.Trim();
            string address = Address_txtBox.Text.Trim();
            string password = password_txtBox.Text.Trim();

            // Validate password complexity
            if (!IsValidPassword(password))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Password must be at least 8 characters long and contain uppercase, lowercase, digit, and special character.');", true);
                return;
            }

            string hashedPassword = HashPassword(password);
            string secretQ = ddlSecretQuestion.SelectedValue;
            string secretA = txtSecretAnswer.Text.Trim().ToLower();
            if (secretA.Length < 3)
            {
                ErrorLabel.Text = "Secret answer must be at least 3 characters long.";
                return;
            }
            string hashedSecretA = HashPassword(secretA);  // For security



            string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // Check if email already exists
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@Email", email);

                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    string script = "alert('This email is already registered. Please login instead.'); window.location='Login.aspx';";
                    ClientScript.RegisterStartupScript(this.GetType(), "alertRedirect", script, true);
                }
                else
                {
                    // INSERT full user details including secret Q/A
                    string insertQuery = @"
                    INSERT INTO Users 
                    (Fname, Lname, Email, Phone_no, Address, Password, userType_id, userStatus_id, Secret_Question_id, Secret_Answer) 
                    VALUES 
                    (@Fname, @Lname, @Email, @Phone, @Address, @Password, 2, 1, @SecretQ, @SecretA)";


                    SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@Fname", fname);
                    insertCmd.Parameters.AddWithValue("@Lname", lname);
                    insertCmd.Parameters.AddWithValue("@Email", email);
                    insertCmd.Parameters.AddWithValue("@Phone", phone);
                    insertCmd.Parameters.AddWithValue("@Address", address);
                    insertCmd.Parameters.AddWithValue("@Password", hashedPassword);
                    insertCmd.Parameters.AddWithValue("@SecretQ", secretQ); // selected question ID
                    insertCmd.Parameters.AddWithValue("@SecretA", hashedSecretA); // hashed answer


                    try
                    {
                        insertCmd.ExecuteNonQuery();

                        Response.Redirect("Status.aspx?type=pending");

                    }
                    catch (Exception ex)
                    {
                        string errorMessage = ex.Message.Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "");
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Error: {errorMessage}');", true);
                        ErrorLabel.Text = "Detailed Error: " + ex.ToString();
                    }
                }

                conn.Close();
            }
        }

        private bool IsValidPassword(string password)
        {
            // At least 8 characters, 1 uppercase, 1 lowercase, 1 digit, 1 special character
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            return Regex.IsMatch(password, pattern);
        }


        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // convert to hex
                }
                return builder.ToString();
            }
        }

        protected void password_txtBox_TextChanged(object sender, EventArgs e)
        {

        }
    }

}