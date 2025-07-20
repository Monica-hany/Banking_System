using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;

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
    }
}
