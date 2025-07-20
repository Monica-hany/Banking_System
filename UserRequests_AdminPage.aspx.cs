using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MiniBank
{
    public partial class UserRequests_AdminPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["admin"] == null)
            {
                Response.Redirect("Login.aspx");
            }


            if (!IsPostBack)
            {
                LoadStatusFilter();
                LoadRequests();
            }
        }

        protected void RequestsGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            RequestsGridView.PageIndex = e.NewPageIndex;
            LoadRequests();
        }

        protected void StatusFilterDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRequests();
        }


        private void LoadStatusFilter()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT UserStatus_id, UserStatus_name FROM Look_UserStatus";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                StatusFilterDropDown.Items.Clear();
                StatusFilterDropDown.Items.Add(new ListItem("All", "All"));

                while (reader.Read())
                {
                    string name = reader["UserStatus_name"].ToString();
                    string id = reader["UserStatus_id"].ToString();
                    StatusFilterDropDown.Items.Add(new ListItem(name, id));
                }

                conn.Close();
            }
        }

        private void LoadRequests()
        {
            string selectedStatus = StatusFilterDropDown.SelectedValue;
            string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT User_id, Fname, Lname, Email, Phone_No, Address,
                        CASE UserStatus_id 
                            WHEN 1 THEN 'Pending'
                            WHEN 2 THEN 'Approved'
                            WHEN 3 THEN 'Rejected'
                            ELSE 'Unknown' 
                        END AS userStatus
                    FROM Users";

                if (selectedStatus != "All")
                {
                    query += " WHERE UserStatus_id = @Status";
                }

                SqlCommand cmd = new SqlCommand(query, conn);
                if (selectedStatus != "All")
                {
                    cmd.Parameters.AddWithValue("@Status", selectedStatus);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                RequestsGridView.DataSource = dt;
                RequestsGridView.DataBind();
            }
        }

        protected void RequestsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Approve" || e.CommandName == "Reject")
            {
                int userId = Convert.ToInt32(e.CommandArgument);
                int newStatus = (e.CommandName == "Approve") ? 2 : 3;

                string connStr = ConfigurationManager.ConnectionStrings["MiniBankConnection"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string updateQuery = "UPDATE Users SET UserStatus_id = @Status WHERE User_id = @UserId";
                    SqlCommand cmd = new SqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@Status", newStatus);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                LoadRequests();
            }
        }

        protected void BackToAdmin_btn_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminPage.aspx");
        }

        protected void Logout_Link_Click(object sender, EventArgs e)
        {
            Session.Abandon(); // Destroys the session
            Response.Redirect("Login.aspx"); // Redirects to login page
        }

    }

}