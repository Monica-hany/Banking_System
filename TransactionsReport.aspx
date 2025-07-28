<%@ Page Title="Transactions Report" Language="C#" MasterPageFile="~/MainMaster.Master" AutoEventWireup="true" CodeBehind="TransactionsReport.aspx.cs" Inherits="MiniBank.TransactionsReport" %>

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
    <!-- Logout Link -->
    <div style="text-align: right; margin: 20px; margin-bottom: 0;">
        <asp:LinkButton ID="Logout_Link" runat="server" OnClick="Logout_Link_Click"
                        Style="text-decoration: none; color: #1C5E55; font-weight: bold; font-size: 16px;">
            Logout
            <img src="Images/logout.jpg" alt="Logout" style="vertical-align: middle; width: 24px; height: 24px; margin-right: 5px;" />
        </asp:LinkButton>
    </div>

    <!-- Page Title -->
    <div style="text-align: center; font-size: 22px; font-weight: bold; color: #1C5E55; margin-top: 20px;">
        Transactions Report
    </div>

    <!-- Transactions Grid -->
    <div style="display: flex; justify-content: center; margin-top: 30px;">
       <asp:GridView ID="TransactionsGrid" runat="server" AutoGenerateColumns="true"
            AllowPaging="true" PageSize="10"
            PagerStyle-CssClass="grid-pager"
            PagerSettings-Mode="NumericFirstLast"
            PagerSettings-FirstPageText="⟨⟨"
            PagerSettings-LastPageText="⟩⟩"
            PagerSettings-NextPageText="Next ⟩"
            PagerSettings-PreviousPageText="⟨ Prev"
            OnPageIndexChanging="TransactionsGrid_PageIndexChanging"
            CellPadding="4" ForeColor="#333333" GridLines="None"
            Width="100%">
            
            <AlternatingRowStyle BackColor="White" />
            <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
            <RowStyle BackColor="#E3EAEB" />
            <EmptyDataTemplate>
                <div style="padding: 15px; color: red; font-weight: bold; text-align: center;">
                    No transactions found.
                </div>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>

    <!-- Action Button -->
    <div style="text-align: center; margin-top: 20px;">
        <asp:Button ID="BackToAdmin_btn" runat="server" Text="Back to Admin Dashboard"
                    PostBackUrl="AdminPage.aspx"
                    CssClass="action-button" Width="306px" />
    </div>
</asp:Content>
