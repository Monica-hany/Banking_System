using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MiniBank
{
    public partial class Status : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string status = Request.QueryString["type"];

                switch (status)
                {
                    case "pending":
                        lblStatusTitle.Text = "⏳ Registration Pending";
                        lblStatusMessage.Text = "Your account is awaiting admin approval. Thank you for your patience!";
                        btnAction.Text = "Back to Login";
                        btnAction.PostBackUrl = "Login.aspx";
                        lblTip.Visible = false;

                        break;

                    case "rejected":
                        lblStatusTitle.Text = "❌ Registration Rejected";
                        lblStatusMessage.Text = "Unfortunately, your registration has been rejected.";
                        btnAction.Text = "Create Another Account";
                        btnAction.PostBackUrl = "Register.aspx";

                        lblTip.Visible = true;
                        lblTip.Text = "⚠️ Tip: Please use a different email address when registering a new account.";
                        break;

                    case "success":
                        Response.Redirect("Home.aspx");
                        lblTip.Visible = false;

                        break;

                    default:
                        lblStatusTitle.Text = "ℹ️ Status";
                        lblStatusMessage.Text = "No status information available.";
                        btnAction.Text = "Login";
                        btnAction.PostBackUrl = "Login.aspx";
                        lblTip.Visible = false;

                        break;
                }
            }
        }
    }

    
}
