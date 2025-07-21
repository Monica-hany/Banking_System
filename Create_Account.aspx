<%@ Page Title="Create Account" Language="C#" MasterPageFile="~/MainMaster.Master" AutoEventWireup="true" CodeBehind="Create_Account.aspx.cs" Inherits="MiniBank.Create_Account" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        body {
            font-family: Arial, sans-serif;
        }

        .form-container {
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 80vh;
        }

        .form-box {
            width: 500px;
            padding: 30px 40px;
            border-radius: 10px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
            background-color: #ffffff;
            border-top: 8px solid #1C5E55;
        }

        .form-title {
            text-align: center;
            font-size: 26px;
            font-weight: bold;
            color: #1C5E55;
            margin-bottom: 30px;
        }

        .form-box label {
            display: block;
            font-weight: bold;
            margin-bottom: 5px;
            color: #1C5E55;
        }

        .form-box .asp-textbox,
        .form-box select {
            width: 100%;
            padding: 8px 10px;
            margin-bottom: 15px;
            border-radius: 6px;
            border: 1px solid #ccc;
            font-size: 14px;
        }

        .asp-button.inline-width {
            width: auto;
            padding: 8px 20px;
            display: inline-block;
            gap: 20px;
        }


        .form-box .asp-button {
            width: 100%;
            background-color: #1C5E55;
            color: white;
            font-weight: bold;
            padding: 10px;
            border: none;
            border-radius: 6px;
            cursor: pointer;
        }

        .form-box .asp-button:hover {
            background-color: #16624f;
        }

        .error-label {
            color: red;
            font-size: 12px;
            display: block;
            margin-top: -10px;
            margin-bottom: 10px;
        }

        /* Style for the message panel text */
        .message-panel-text {
            color: #1C5E55;
            font-size: 18px;
            font-weight: 600;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.4;
            margin-bottom: 25px;
        }


        .message-label {
            text-align: center;
            margin-top: 15px;
            font-weight: bold;
            color: red;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="text-align: right; margin: 20px; margin-bottom: 0;">
        <asp:LinkButton ID="Logout_Link" runat="server" OnClick="Logout_Link_Click" Style="text-decoration: none; color: #1C5E55; font-weight: bold; font-size: 16px;" CausesValidation="false";
>
            Logout
            <img src="Images/logout.jpg"" alt="Logout" style="vertical-align: middle; width: 24px; height: 24px; margin-right: 5px;" CausesValidation="false"
; />
    
        </asp:LinkButton>
    </div>

    <div class="form-container">

        <!-- Panel: Create Account Form -->
        <asp:Panel ID="pnlCreateAccountForm" runat="server">
            <div class="form-box">
                <div class="form-title">Open New Account</div>

                <!-- Account Type -->
                <label for="AccountType_ddl">Account Type</label>
                <asp:DropDownList 
                    ID="AccountType_ddl" 
                    runat="server" 
                    CssClass="asp-textbox"
                    AppendDataBoundItems="true" OnSelectedIndexChanged="AccountType_ddl_SelectedIndexChanged"
                    AutoPostBack="true">
                    <asp:ListItem Text="-- Select Account Type --" Value="" />
                </asp:DropDownList>

                <asp:RequiredFieldValidator 
                    ID="rfvAccountType" 
                    runat="server"
                    ControlToValidate="AccountType_ddl"
                    InitialValue=""
                    ErrorMessage="Please select an account type"
                    CssClass="error-label"
                    Display="Dynamic" />

                <!-- Balance -->
                <label for="balance_txtBox">Balance</label>
                <asp:TextBox ID="balance_txtBox" runat="server" CssClass="asp-textbox" Placeholder="e.g. 500.00"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvBalance" runat="server"
                    ControlToValidate="balance_txtBox"
                    ErrorMessage="Balance is required"
                    CssClass="error-label"
                    Display="Dynamic" />
                <asp:RegularExpressionValidator ID="revBalance" runat="server"
                    ControlToValidate="balance_txtBox"
                    ValidationExpression="^\d+(\.\d{1,2})?$"
                    ErrorMessage="Enter a valid number (e.g. 100 or 100.50)"
                    CssClass="error-label"
                    Display="Dynamic" />

                <!-- Currency -->
                <label for="ddlCurrency">Currency</label>
                <asp:DropDownList ID="ddlCurrency" runat="server" CssClass="asp-textbox">
                    <asp:ListItem Text="-- Select Currency --" Value="" />
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvCurrency" runat="server"
                    ControlToValidate="ddlCurrency"
                    InitialValue=""
                    ErrorMessage="Please select a currency"
                    CssClass="error-label"
                    Display="Dynamic" />

              <!-- Buttons Group -->
            <div style="display: flex; gap: 15px; margin-top: 10px;">
                <asp:Button ID="createAccount_btn" runat="server" Text="Create Account"
                    CssClass="asp-button" CausesValidation="true" OnClick="createAccount_btn_Click" />
            </div>
            <div style="display: flex; gap: 15px; margin-top: 10px;">
                <asp:Button ID="backToHome_btn" runat="server" Text="Back To Home"
                CssClass="asp-button" OnClick="backToHome_btn_Click" CausesValidation="false" />

            </div>

            <div>
                <!-- Message -->
                <asp:Label ID="lblMessage" runat="server" CssClass="message-label"></asp:Label>
            </div>
        </asp:Panel>

        <!-- Panel: Show this when user has all account types -->
        <asp:Panel ID="pnlAllAccountTypesMsg" runat="server" Visible="false" CssClass="form-box" style="text-align:center;">
            <div class="form-title message-panel-text">
                You already have accounts of all available types. You cannot create more accounts.
            </div>

        <asp:Button ID="btnViewMyAccounts" runat="server" Text="View My Accounts" CssClass="asp-button inline-width" OnClick="btnViewMyAccounts_Click" />
        </asp:Panel>

    </div>
</asp:Content>
