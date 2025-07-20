<%@ Page Title="My Accounts" Language="C#" MasterPageFile="~/MainMaster.Master" AutoEventWireup="true" CodeBehind="MyAccounts.aspx.cs" Inherits="MiniBank.MyAccounts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .page-title {
            text-align: center;
            margin-top: 30px;
            font-size: 24px;
            font-weight: bold;
            color: #1C5E55;
        }

        .grid-wrapper {
            display: flex;
            justify-content: center;
            margin-top: 30px;
        }

        .account-table th, .account-table td {
            border: 1px solid #ccc;
            padding: 10px;
            text-align: center;
            vertical-align: middle;
        }

        .account-table th {
            background-color: #1C5E55;
            color: white;
            font-weight: bold;
        }

        .account-table {
            border-collapse: collapse;
            width: 100%;
        }

        .account-table .alt-row {
            background-color: #E3EAEB;
        }

        .action-button {
            width: 200px;
            padding: 12px 0;
            font-size: 16px;
            font-weight: bold;
            color: white;
            background-color: #1C5E55;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            transition: background-color 0.3s ease;
            margin: 0 10px;
        }

        .action-button:hover {
            background-color: #16624f;
        }

        .action-button:disabled {
            background-color: #a5bcb8;
            cursor: not-allowed;
            color: white;
            opacity: 0.7;
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


    <!-- Welcome Message -->
    <div class="page-title">
        Welcome, <asp:Label ID="lblUsername" runat="server" Text=""></asp:Label>
    </div>

    <!-- Empty account message -->
    <div style="text-align: center; margin-top: 10px;">
        <asp:Label ID="lblNoAccounts" runat="server" Text="You don't have any accounts yet." 
                   ForeColor="Red" Font-Bold="True" Visible="False"></asp:Label>
    </div>

    <!-- Accounts Table -->
    <div class="grid-wrapper">
        <asp:GridView ID="gvAccounts" runat="server" CssClass="account-table"
            AutoGenerateColumns="False"
            AlternatingRowStyle-CssClass="alt-row"
            HeaderStyle-BackColor="#1C5E55"
            HeaderStyle-ForeColor="White"
            HeaderStyle-Font-Bold="True"
            GridLines="None">
            <Columns>
                <asp:BoundField DataField="Account_ID" HeaderText="Account ID" />
                <asp:BoundField DataField="AccountType_name" HeaderText="Account Type" />

                <asp:TemplateField HeaderText="Balance">
                    <ItemTemplate>
                        <%# String.Format("{0:N2} {1}", Eval("Balance"), Eval("Currency_name")) %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="BalanceInEGP" HeaderText="Balance in EGP" DataFormatString="{0:C}" />

                <asp:BoundField DataField="Date_opened" HeaderText="Date Opened" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="Status_name" HeaderText="Status" />
            </Columns>

        </asp:GridView>
    </div>

    <!-- Action Buttons -->
    <div style="text-align: center; margin-top: 20px;">
        <asp:Button ID="btnCreateAccount" runat="server" Text="Create New Account"
                    OnClick="btnCreateAccount_Click"
                    CssClass="action-button" />

        <asp:Button ID="btnTransfer" runat="server" Text="Transfer Money"
                    OnClick="btnTransfer_Click"
                    Enabled="false"
                    CssClass="action-button" />

        <asp:Button ID="btnViewTransfers" runat="server" Text="View Transfer History"
                    Enabled="true"
                    CssClass="action-button" OnClick="btnViewTransfers_Click" />
    </div>

    <div style="text-align: center; margin-top: 20px;">
        <asp:Button ID="backToHome" runat="server" Text="Back To Home"
            Enabled="true"
            CssClass="action-button" OnClick="backToHome_Click" />
    </div>


</asp:Content>
