using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;

namespace MiniBank
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {

            string email = email_txtBox.Text.Trim();
            string password = Password_txtBox.Text.Trim();
            string hashedPassword = HashPassword(password);

            string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT User_ID, userType_id, userStatus_id, Fname
                    FROM Users 
                    WHERE Email = @Email AND Password = @Password";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", hashedPassword);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    int userIdFromDB = Convert.ToInt32(reader["User_ID"]);
                    int userTypeId = Convert.ToInt32(reader["userType_id"]);
                    int statusId = Convert.ToInt32(reader["userStatus_id"]);
                    string firstName = reader["Fname"].ToString();

                    // Store data in session
                    Session["UserID"] = userIdFromDB;
                    Session["Fname"] = firstName;

                    #region Test 
                    string gid = Guid.NewGuid().ToString();

                    Session["AuthToken"] = gid;

                    var appcookie = new HttpCookie("AuthToken");
                    appcookie.Value = gid;
                    Response.Cookies.Add(appcookie);

                    #endregion


                    if (userTypeId == 1) // Admin
                    {
                        Session["admin"] = userIdFromDB; // Store admin session key
                        Response.Redirect("AdminPage.aspx");
                    }
                    else
                    {
                        switch (statusId)
                        {
                            case 1: // Pending
                                Response.Redirect("Status.aspx?type=pending");
                                break;
                            case 2: // Approved
                                Response.Redirect("Home.aspx");
                                break;
                            case 3: // Rejected
                                Response.Redirect("Status.aspx?type=rejected");
                                break;
                            default:
                                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Unknown user status.');", true);
                                break;
                        }
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid email or password');", true);
                }

                conn.Close();
            }
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        protected void Password_txtBox_TextChanged(object sender, EventArgs e)
        {
        }

    }
}
