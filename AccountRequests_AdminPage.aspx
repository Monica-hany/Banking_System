<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.Master" AutoEventWireup="true" CodeBehind="AccountRequests_AdminPage.aspx.cs" Inherits="MiniBank.AccountRequests_AdminPage" %>
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
     Account Requests
 </div>

 <div style="text-align:center; margin-top: 20px;">
     <span style="font-weight:bold; margin-right:10px;">Filter by Account Status:</span>
     <asp:DropDownList ID="AccountStatusDropDown" runat="server" AutoPostBack="true" OnSelectedIndexChanged="AccountStatusDropDown_SelectedIndexChanged" Height="31px" />
 </div>

 <div style="text-align:center; margin-top: 10px;">
     <span style="font-weight:bold; margin-right:10px;">Currency:</span>
     <asp:DropDownList ID="ddlCurrencyFilter" runat="server" AutoPostBack="true" 
                       OnSelectedIndexChanged="ddlCurrencyFilter_SelectedIndexChanged" Height="31px" />
 </div>

 <div style="display: flex; justify-content: center; margin-top: 20px;">
     <asp:GridView ID="AccountRequestsGrid" runat="server" AutoGenerateColumns="False"
         AllowPaging="true" PageSize="10"
         PagerStyle-CssClass="grid-pager"
         PagerSettings-Mode="NumericFirstLast"
         PagerSettings-FirstPageText="⟨⟨"
         PagerSettings-LastPageText="⟩⟩"
         PagerSettings-NextPageText="Next ⟩"
         PagerSettings-PreviousPageText="⟨ Prev"
         OnPageIndexChanging="AccountRequestsGrid_PageIndexChanging"
         OnRowCommand="AccountRequestsGrid_RowCommand"
         OnRowDataBound="AccountRequestsGrid_RowDataBound"
         CellPadding="4" ForeColor="#333333" GridLines="None"
         Width="100%">

         <AlternatingRowStyle BackColor="White" />

         <Columns>
             <asp:BoundField DataField="Account_ID" HeaderText="Account ID" />
             <asp:BoundField DataField="Email" HeaderText="User Email" />
             <asp:BoundField DataField="AccountType_name" HeaderText="Account Type" />
             <asp:TemplateField HeaderText="Balance">
                 <ItemTemplate>
                     <%# String.Format("{0:N2} {1}", Eval("Balance"), Eval("Currency_name")) %>
                 </ItemTemplate>
             </asp:TemplateField>
             <asp:BoundField DataField="BalanceInEGP" HeaderText="Balance in EGP" DataFormatString="{0:C}" />
             <asp:BoundField DataField="Date_opened" HeaderText="Date Opened" DataFormatString="{0:yyyy-MM-dd}" />
             <asp:BoundField DataField="AccountStatus" HeaderText="Status" />
             <asp:TemplateField HeaderText="Actions">
                 <ItemTemplate>
                     <asp:Button ID="btnApproveAccount" runat="server" Text="Approve" CommandName="ApproveAccount" CommandArgument='<%# Eval("Account_ID") %>' />
                     &nbsp;
                     <asp:Button ID="btnRejectAccount" runat="server" Text="Reject" CommandName="RejectAccount" CommandArgument='<%# Eval("Account_ID") %>' />
                     &nbsp;
                     <asp:Button ID="btnActivate" runat="server" Text="Activate" CommandName="ActivateAccount" CommandArgument='<%# Eval("Account_ID") %>' />
                     &nbsp;
                     <asp:Button ID="btnSuspend" runat="server" Text="Suspend" CommandName="SuspendAccount" CommandArgument='<%# Eval("Account_ID") %>' />
                     &nbsp;
                     <asp:Button ID="btnClose" runat="server" Text="Close" CommandName="CloseAccount" CommandArgument='<%# Eval("Account_ID") %>' />
                 </ItemTemplate>
             </asp:TemplateField>
         </Columns>

         <EmptyDataTemplate>
             <div style="padding: 15px; color: red; font-weight: bold; text-align: center;">
                 No account requests found.
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
                 CssClass="action-button" OnClick="BackToAdmin_btn_Click" Width="226px" />
 </div>
</asp:Content>
