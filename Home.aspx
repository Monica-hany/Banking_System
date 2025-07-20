<%@ Page Title="Home" Language="C#" MasterPageFile="~/MainMaster.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="MiniBank.Home" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Cache-Control" content="no-store, no-cache, must-revalidate" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="Expires" content="0" />

    <style>
        .home-container {
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            height: 70vh; /* vertical centering */
        }

        .page-title {
            font-size: 28px;
            font-weight: bold;
            color: #1C5E55;
            margin-bottom: 30px;
            text-align: center;
        }

        .button-panel {
            display: flex;
            flex-direction: column;
            gap: 20px;
            align-items: center;
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
        }

        .action-button:hover {
            background-color: #16624f;
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

    <div class="home-container">
        <div class="page-title">
            Welcome, <asp:Label ID="lblUsername" runat="server" />
        </div>

        <!-- Button Panel (Stacked vertically and centered) -->
       <div class="button-panel">
            <asp:Button ID="CreateAccount_btn" runat="server" Text="Create Account" CssClass="action-button" OnClick="CreateAccount_btn_Click" />
            <asp:Button ID="MyAccounts_btn" runat="server" Text="My Accounts" CssClass="action-button" OnClick="MyAccounts_btn_Click" />
            <asp:Button ID="Transfer_btn" runat="server" Text="Transfer" CssClass="action-button" OnClick="Transfer_btn_Click" />
           <asp:Button ID="TransferHistory_btn" runat="server" Text="Transfer History" CssClass="action-button" OnClick="TransferHistory_btn_Click" />
        </div>
    </div> 

        <!--
        <div>
            <button type="button" class="action-button" onclick="location.href='Create_Account.aspx'">Create Account</button>
            <button type="button"  class="action-button" onclick="location.href='MyAccounts.aspx'">My Accounts</button>
            <button type="button"  class="action-button" onclick="location.href='Transfer.aspx'">Transfer</button>
        </div>

        -->
    <script>
    window.addEventListener('pageshow', function (event) {
        if (event.persisted) {
            window.location.reload();
        }
    });
    </script>

</asp:Content>
