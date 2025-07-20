<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.Master" AutoEventWireup="true" CodeBehind="AdminPage.aspx.cs" Inherits="MiniBank.AdminPage" %>

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


    <div style="text-align:center; margin-top: 20px; font-size: 24px; font-weight: bold; color: #1C5E55;">
        Welcome to NBE Mini Bank <br /> Admin Page
    </div>

        <!-- Button Panel (Stacked vertically and centered) -->
   <div class="button-panel">
       <div style="text-align:center; margin-top: 20px;">
            <asp:Button ID="UserRequests_btn" runat="server" Text="User Requests" CssClass="action-button" OnClick="UserRequests_btn_Click"  />
       </div>
       <div style="text-align:center; margin-top: 20px;">
            <asp:Button ID="AccountRequests_btn" runat="server" Text="Account Requests" CssClass="action-button" OnClick="AccountRequests_btn_Click"  />
       </div>
   </div>

</asp:Content>
