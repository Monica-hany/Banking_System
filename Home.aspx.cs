using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MiniBank
{
    public partial class Home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Prevent browser caching 3shan bybawaz el redirection
            Response.Cache.SetCacheability(HttpCacheability.NoCache); // hena ba ensures that Home.aspx is never loaded from cache
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetNoStore();

            if (!IsPostBack)
            {
                if (Session["UserID"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                lblUsername.Text = Session["Fname"]?.ToString() ?? "User";
            }
        }

        protected void TransferHistory_btn_Click(object sender, EventArgs e)
        {
            Response.Redirect("TransferHistory.aspx");
        }


        protected void CreateAccount_btn_Click(object sender, EventArgs e)
        {
            Response.Redirect("Create_Account.aspx");
        }

        protected void Transfer_btn_Click(object sender, EventArgs e)
        {
            Response.Redirect("Transfer.aspx");
        }

        protected void MyAccounts_btn_Click(object sender, EventArgs e)
        {
            Response.Redirect("MyAccounts.aspx");
        }

        protected void Logout_Link_Click(object sender, EventArgs e)
        {
            Session.Abandon(); // Destroys the session
            Response.Redirect("Login.aspx"); // Redirects to login page
        }

        protected void Insert_Transaction_File_Click(object sender, EventArgs e)
        {
            Response.Redirect("Insert_Transaction_File.aspx");
        }
    }
}

