using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace MiniBank
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        // Step 1: Get Secret Question
        protected void btnGetQuestion_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString; // comment

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT Q.Secret_Question_Text
                    FROM Users U
                    JOIN Look_Secret_Questions Q ON U.Secret_Question_id = Q.Secret_Question_id
                    WHERE U.Email = @Email";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    lblSecretQuestion.Text = reader["Secret_Question_Text"].ToString();
                    pnlQA.Visible = true;
                    pnlVerify.Enabled = false;
                    lblMessage.Text = "";
                }
                else
                {
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.Text = "Email not found.";
                }

                conn.Close();
            }
        }

        // Step 2: Verify Answer
        protected void btnVerify_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim().ToLower(); // normalize email case
            string answer = txtSecretAnswer.Text.Trim().ToLower();
            string hashedAnswer = HashPassword(answer);

            // Use per-email keys for session
            string attemptsKey = "Attempts_" + email;
            string lockoutKey = "LockoutTime_" + email;

            // 1. Check lockout time for this email
            if (Session[lockoutKey] != null && DateTime.Now < (DateTime)Session[lockoutKey])
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Too many failed attempts. Please try again after 5 minutes.";
                return;
            }

            // 2. Check previous attempts for this email
            int attempts = Session[attemptsKey] != null ? (int)Session[attemptsKey] : 0;

            if (attempts >= 3)
            {
                Session[lockoutKey] = DateTime.Now.AddMinutes(5);
                Session[attemptsKey] = 0;
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Account locked for 5 minutes due to multiple failed attempts.";
                return;
            }

            // 3. Check database for the email
            string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT Secret_Answer FROM Users WHERE Email = @Email";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                object result = cmd.ExecuteScalar();
                conn.Close();

                if (result != null)
                {
                    string storedHashedAnswer = result.ToString();

                    if (storedHashedAnswer == hashedAnswer)
                    {
                        // ✅ Correct answer
                        Session[attemptsKey] = 0;
                        Session[lockoutKey] = null;

                        pnlQA.Visible = false;
                        pnlReset.Visible = true;
                        lblMessage.ForeColor = System.Drawing.Color.Green;
                        lblMessage.Text = "Identity verified. You may now reset your password.";
                        ViewState["VerifiedEmail"] = email;
                    }
                    else
                    {
                        // ❌ Incorrect answer
                        attempts++;
                        Session[attemptsKey] = attempts;
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                        lblMessage.Text = $"Incorrect answer. Attempt {attempts} of 3.";
                    }
                }
                else
                {
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.Text = "User not found.";
                }
            }
        }


        // Step 3: Reset Password
        protected void btnReset_Click(object sender, EventArgs e)
        {
            if (ViewState["VerifiedEmail"] == null)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Unauthorized attempt. Please verify your identity again.";
                return;
            }

            string email = ViewState["VerifiedEmail"].ToString();
            string newPassword = txtNewPassword.Text.Trim();

            if (!IsValidPassword(newPassword))
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Password must be at least 8 characters long and contain uppercase, lowercase, digit, and special character.";
                return;
            }
            // Hash the new password
            string hashedPassword = HashPassword(newPassword);

            string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string updateQuery = "UPDATE Users SET Password = @Password WHERE Email = @Email";
                SqlCommand cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@Password", hashedPassword);
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            lblMessage.ForeColor = System.Drawing.Color.Green;
            lblMessage.Text = "Password reset successful. You can now login.";
            pnlReset.Visible = false;


            // Optional: wait 2 seconds then redirect (client-side)
            Response.AddHeader("REFRESH", "2;URL=Login.aspx");

            // OR Immediate redirect (uncomment if you want instant jump)
            // Response.Redirect("Login.aspx");


        }

        private bool IsValidPassword(string password)
        {
            // At least 8 characters, 1 uppercase, 1 lowercase, 1 digit, 1 special character
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            return Regex.IsMatch(password, pattern);
        }


        // Hashing Method
        private string HashPassword(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
