using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MiniBank
{
    public partial class TransactionsReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["admin"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                LoadReportData();
            }
        }

        protected void TransactionsGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            TransactionsGrid.PageIndex = e.NewPageIndex;
            LoadReportData(); // Rebind data with the new page index
        }


        protected void Logout_Link_Click(object sender, EventArgs e)
        {
            Session.Abandon(); // Destroys the session
            Response.Redirect("Login.aspx"); // Redirects to login page
        }

        private void LoadReportData()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("ImportTransactionsFromFolder", conn)) // Your stored procedure
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    da.Fill(dt);
                    TransactionsGrid.DataSource = dt;
                    TransactionsGrid.DataBind();
                }
            }
        }
    }
}