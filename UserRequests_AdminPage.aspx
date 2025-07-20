<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.Master" AutoEventWireup="true" CodeBehind="UserRequests_AdminPage.aspx.cs" Inherits="MiniBank.UserRequests_AdminPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <style>
     .grid-pager a, .grid-pager span {
         padding: 6px 12px;
         margin: 0 4px;
         border: 1px solid #1C5E55;
         color: #1C5E55;
         background-color: white;
         text-decoration: none;
         border-radius: 4px;
         font-weight: bold;
     }

     .grid-pager a:hover {
         background-color: #1C5E55;
         color: white;
     }

     
       .action-button {
         border-style: none;
         border-color: inherit;
         border-width: medium;
         padding: 12px 0;
         font-size: 16px;
         font-weight: bold;
         color: white;
         background-color: #1C5E55;
         border-radius: 8px;
         cursor: pointer;
         transition: background-color 0.3s ease;
         margin: 0 10px;
     }

 </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="text-align: right; margin: 20px; margin-bottom: 0;">
        <asp:LinkButton ID="Logout_Link" runat="server" OnClick="Logout_Link_Click" Style="text-decoration: none; color: #1C5E55; font-weight: bold; font-size: 16px;">
            Logout
            <img src="Images/logout.jpg"" alt="Logout" style="vertical-align: middle; width: 24px; height: 24px; margin-right: 5px;" />
        
        </asp:LinkButton>
    </div>



      <div style="text-align:center; font-size: 22px; font-weight: bold; color: #1C5E55;">
      User Requests
  </div>


<div style="text-align:center; margin-top: 20px;">
    <span style="font-weight:bold; margin-right:10px;">Filter by:</span>
    <asp:DropDownList ID="StatusFilterDropDown" runat="server" AutoPostBack="true" OnSelectedIndexChanged="StatusFilterDropDown_SelectedIndexChanged" Height="31px" />
</div>

<div style="display: flex; justify-content: center; margin-top: 30px;">
    <asp:GridView ID="RequestsGridView" runat="server" AutoGenerateColumns="False"
        AllowPaging="true" PageSize="10"
        PagerStyle-CssClass="grid-pager"
        PagerSettings-Mode="NumericFirstLast"
        PagerSettings-FirstPageText="⟨⟨"
        PagerSettings-LastPageText="⟩⟩"
        PagerSettings-NextPageText="Next ⟩"
        PagerSettings-PreviousPageText="⟨ Prev"
        OnPageIndexChanging="RequestsGridView_PageIndexChanging"
        OnRowCommand="RequestsGridView_RowCommand"
        CellPadding="4" ForeColor="#333333" GridLines="None"
        Width="100%">

        <AlternatingRowStyle BackColor="White" />

        <Columns>
            <asp:BoundField DataField="User_id" HeaderText="User ID" />
            <asp:BoundField DataField="Fname" HeaderText="First Name" />
            <asp:BoundField DataField="Lname" HeaderText="Last Name" />
            <asp:BoundField DataField="Email" HeaderText="Email" />
            <asp:BoundField DataField="Phone_no" HeaderText="Phone Number" />
            <asp:BoundField DataField="Address" HeaderText="Address" />
            <asp:BoundField DataField="userStatus" HeaderText="Status" />
            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <asp:Button ID="btnApprove" runat="server" Text="Approve" CommandName="Approve" CommandArgument='<%# Eval("User_id") %>' />
                    &nbsp;
                    <asp:Button ID="btnReject" runat="server" Text="Reject" CommandName="Reject" CommandArgument='<%# Eval("User_id") %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>

        <EmptyDataTemplate>
            <div style="padding: 15px; color: red; font-weight: bold; text-align: center;">
                No pending requests at the moment.
            </div>
        </EmptyDataTemplate>

        <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
        <RowStyle BackColor="#E3EAEB" />
    </asp:GridView>
</div>

     <!-- Action Buttons -->
 <div style="text-align: center; margin-top: 20px;">

     <asp:Button ID="BackToAdmin_btn" runat="server" Text="Back to Admin Dashboard"
                 PostBackUrl="AdminPage.aspx"
                 Enabled="true"
                 CssClass="action-button" OnClick="BackToAdmin_btn_Click" Width="306px" />
 </div>


</asp:Content>
