<%@ Page Title="Transfer History" Language="C#" MasterPageFile="~/MainMaster.Master" AutoEventWireup="true" CodeBehind="TransferHistory.aspx.cs" Inherits="MiniBank.TransferHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
    // Use sessionStorage to isolate tab sessions
    if (!sessionStorage.getItem("TabID")) {
        sessionStorage.setItem("TabID", self.crypto.randomUUID());
    }

    // Set TabID as a cookie so the server can validate it
    document.cookie = "TabID=" + sessionStorage.getItem("TabID");
    </script>

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
            flex-direction: column;
            align-items: center;
            margin-top: 30px;
        }

        .history-table th, .history-table td {
            border: 1px solid #ccc;
            padding: 10px;
            text-align: center;
        }

        .history-table th {
            background-color: #1C5E55;
            color: white;
        }

        .history-table {
            border-collapse: collapse;
            width: 100%;
        }

        .filter-container {
            text-align: center;
            margin-top: 30px;
        }

        .filter-container input[type="date"] {
            padding: 5px;
        }

        .filter-container .btn {
            padding: 6px 12px;
            font-weight: bold;
            margin-left: 10px;
        }

        .message-label {
            color: red;
            font-weight: bold;
            margin-top: 15px;
            display: inline-block;
        }

        /* Pagination styling */
        .pagination-style {
            text-align: center;
            padding: 10px;
        }

        .pagination-style a,
        .pagination-style span {
            margin: 0 4px;
            padding: 6px 12px;
            color: #1C5E55;
            text-decoration: none;
            font-weight: bold;
            border: 1px solid #1C5E55;
            border-radius: 4px;
        }

        .pagination-style a:hover {
            background-color: #1C5E55;
            color: #fff;
        }

        .pagination-style span {
            background-color: #1C5E55;
            color: #fff;
        }

        .pagination-style .aspNetDisabled {
            opacity: 0.4;
            cursor: not-allowed;
        }

        .page-info {
            margin-top: 10px;
            font-weight: bold;
            color: #1C5E55;
            text-align: center;
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


    <div class="page-title">Transfer History</div>

    <!-- Filter by Date Range -->
    <div class="filter-container">
        <asp:Label ID="lblFrom" runat="server" Text="From: " Font-Bold="true" />
        <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" Width="150px" />
        &nbsp;&nbsp;

        <asp:Label ID="lblTo" runat="server" Text="To: " Font-Bold="true" />
        <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" Width="150px" />
        &nbsp;&nbsp;

        <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="btn btn-info" OnClick="btnFilter_Click" />
    </div>

    <!-- Centered Messages Below Filter -->
    <div style="text-align: center;">
        <asp:Label ID="lblNoTransfersFiltered" runat="server"
                   Text="No transactions found in the selected period."
                   CssClass="message-label"
                   Visible="false" />
        <br />
        <asp:Label ID="lblNoTransfersAll" runat="server"
                   Text="You haven't made any transactions yet."
                   CssClass="message-label"
                   Visible="false" />
    </div>

    <!-- Transfer History Table + Pagination -->
    <div class="grid-wrapper">
        <asp:GridView ID="gvTransfers" runat="server" AutoGenerateColumns="False" 
                      CssClass="history-table"
                      AllowPaging="true" PageSize="10"
                      OnPageIndexChanging="gvTransfers_PageIndexChanging"
                      PagerStyle-CssClass="pagination-style"
                      PagerSettings-Mode="NumericFirstLast"
                      PagerSettings-Position="Bottom"
                      PagerSettings-FirstPageText="⬅️"
                      PagerSettings-LastPageText="➡️"
                      PagerSettings-PageButtonCount="5">
            <Columns>
                <asp:BoundField DataField="Transaction_ID" HeaderText="Transaction ID" />
                <asp:BoundField DataField="FromAccount" HeaderText="From" />
                <asp:BoundField DataField="ToAccount" HeaderText="To" />
                <asp:TemplateField HeaderText="Amount">
                    <ItemTemplate>
                        <%# string.Format("{0:N2} {1}", Eval("Amount"), Eval("Currency")) %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="TimeStamp" HeaderText="Time" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
            </Columns>
        </asp:GridView>

        <!-- Page X of Y -->
        <div class="page-info">
            Page <%= gvTransfers.PageIndex + 1 %> of <%= gvTransfers.PageCount %>
        </div>

        <!-- Back to Home Button -->
        <div style="text-align: center; margin-top: 20px;">
            <asp:Button ID="btnBackToHome" runat="server" Text="Back to Home Page"
            CssClass="action-button" OnClick="btnBackToHome_Click" />
        </div>

    </div>
    <script>
        window.addEventListener("beforeunload", function () {
            navigator.sendBeacon("LogoutHandler.aspx"); // implement a handler to destroy the session
        });
    </script>

</asp:Content>


