<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.Master" AutoEventWireup="true" CodeBehind="Insert_Transaction_File.aspx.cs" Inherits="MiniBank.Insert_Transaction_File" %>

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

        .transaction-grid {
            width: 100%;
            border-collapse: collapse;
            margin: 0 auto;
            font-size: 15px;
            table-layout: fixed;
            word-wrap: break-word;
        }

        .transaction-grid th, .transaction-grid td {
            padding: 12px 10px;
            text-align: center;
            word-wrap: break-word;
        }

        .action-button {
            padding: 14px 0;
            font-size: 15px;
            width: 250px;
            line-height: 1.3;
            border-style: none;
            font-weight: bold;
            color: white;
            background-color: #1C5E55;
            border-radius: 8px;
            cursor: pointer;
            transition: background-color 0.3s ease;
            margin: 0 10px;
        }

        .action-button:hover {
            background-color: #14463F;
        }

        .file-upload-box {
            border: 2px dashed #1C5E55;
            padding: 30px;
            text-align: center;
            border-radius: 10px;
            color: #1C5E55;
            margin-bottom: 20px;
        }

        #<%= GridView1.ClientID %> td, #<%= GridView1.ClientID %> th {
            padding: 8px 15px;
        }

        .status-valid,
        .status-rejected {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            padding: 4px 10px;
            font-size: 13px;
            font-weight: 600;
            border-radius: 15px;
            min-width: 80px;
            height: 28px;
            text-align: center;
            vertical-align: middle;
            white-space: nowrap;
        }

        .status-valid {
            background-color: #c8e6c9; /* softer green */
            color: #1C5E55;
        }

        .status-rejected {
            background-color: #333;
            color: #fff;
        }


        #<%= GridView1.ClientID %> td {
            vertical-align: middle;
            text-align: center;
        }

        .rejection-text {
            color: red;
            font-weight: bold;
        }

        .transaction-grid td, .transaction-grid th {
            padding-top: 10px;
            padding-bottom: 10px;
        }

        .valid-row {
            background-color: #dff0d8;
        }

        .rejected-row {
            background-color: #f2dede;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div style="text-align: right; margin: 20px; margin-bottom: 0;">
        <asp:LinkButton ID="Logout_Link" runat="server" OnClick="Logout_Link_Click" Style="text-decoration: none; color: #1C5E55; font-weight: bold; font-size: 16px;">
            Logout
            <img src="Images/logout.jpg" alt="Logout" style="vertical-align: middle; width: 24px; height: 24px; margin-right: 5px;" />
        </asp:LinkButton>
    </div>

    <div style="text-align:center; font-size: 22px; font-weight: bold; color: #1C5E55;">
        Upload Transactions File
    </div>

    <div style="text-align: center; margin-top: 20px;">
        <asp:Button ID="btnDownloadTemplate" runat="server" 
            Text="Download Template" 
            CssClass="action-button" 
            OnClick="btnDownloadTemplate_Click" 
            CausesValidation="false" />
    </div>

    <div class="file-upload-box">
        <asp:FileUpload ID="fileUpload" runat="server" CssClass="asp-button" />
        <br /><br />
        <asp:Button ID="btnUploadFile" runat="server" Text="Upload Transactions File" CssClass="action-button" OnClick="btnUploadFile_Click" />
        <br />
        <asp:Label ID="lblUploadMessage" runat="server" ForeColor="Red" Font-Bold="true" />
    </div>

    <!-- Dropdown container -->
    <div style="text-align:center; margin: 20px auto;">
        <asp:DropDownList ID="ddlSort" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSort_SelectedIndexChanged">
            <asp:ListItem Text="Original Order" Value="original" />
            <asp:ListItem Text="Rejected First" Value="rejected" />
        </asp:DropDownList>
    </div>

    <!-- GridView container -->
    <asp:Panel ID="pnlGridContainer" runat="server">
        <div style="overflow-x:auto; margin: 20px auto; max-width: 95%; text-align: center;">
            <asp:GridView ID="GridView1" runat="server" CssClass="transaction-grid"
                AutoGenerateColumns="False"
                AllowPaging="true" PageSize="10"
                PagerStyle-CssClass="grid-pager"
                PagerSettings-Mode="NumericFirstLast"
                PagerSettings-FirstPageText="⟨⟨"
                PagerSettings-LastPageText="⟩⟩"
                PagerSettings-NextPageText="Next ⟩"
                PagerSettings-PreviousPageText="⟨ Prev"
                CellPadding="4" ForeColor="#333333" GridLines="None"
                OnRowDataBound="GridView1_RowDataBound"
                OnPageIndexChanging="GridView1_PageIndexChanging">

                <AlternatingRowStyle BackColor="White" />

                <Columns>
                    <asp:BoundField DataField="SenderAccountId" HeaderText="Sender Account ID" />
                    <asp:BoundField DataField="RecipientAccountId" HeaderText="Recipient Account ID" />
                    <asp:BoundField DataField="ValueDate" HeaderText="Value Date" DataFormatString="{0:yyyy-MM-dd}" />
                    <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="{0:N2}" />  
                    <asp:BoundField DataField="Status" HeaderText="Status" HeaderStyle-Font-Bold="true" />
                    <asp:BoundField DataField="RejectionReason" HeaderText="Rejection Reason" />
                    <asp:BoundField DataField="Serial" HeaderText="Serial" Visible="False" />
                </Columns>

                <EmptyDataTemplate>
                    <div style="padding: 15px; color: red; font-weight: bold; text-align: center;">
                        No transactions to display.
                    </div>
                </EmptyDataTemplate>

                <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                <RowStyle BackColor="#E3EAEB" />
            </asp:GridView>
        </div>
    </asp:Panel>

    <asp:Label ID="lblSuccessMessage" runat="server" 
        Text="" 
        Visible="false"
        ForeColor="Green" 
        Font-Bold="true" 
        Font-Size="Large"
        Style="display:block; text-align:center; margin-top: 20px;" />

    <div style="text-align: center; margin-top: 20px;">
        <asp:Button ID="btnRemoveRejected" runat="server" Text="Remove All Rejected" CssClass="action-button" OnClick="btnRemoveRejected_Click" />
        <asp:Button ID="btnMakeAllTransactions" runat="server" Text="Make All Transactions" CssClass="action-button" OnClick="btnMakeAllTransactions_Click" />
        <asp:Button ID="backHome_btn" runat="server" Text="Back To Home Page" CssClass="action-button" OnClick="backHome_btn_Click" />
    </div>

</asp:Content>
