using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MiniBank
{
    public partial class MainMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Session["UserID"] != null)
            {
                if (Session["AuthToken"].ToString() != Request.Cookies["AuthToken"].Value)
                {
                    Response.Redirect("Login.aspx");
                }
            }
            
        }

    }
}