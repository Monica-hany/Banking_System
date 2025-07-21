<%@ Page Title="Forgot Password" Language="C#" MasterPageFile="~/MainMaster.Master" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="MiniBank.ForgotPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        body {
            font-family: 'Segoe UI', sans-serif;
            background-color: white;
            margin: 0;
            padding: 0;
        }

        .form-container {
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
        }

        .form-box {
            width: 500px;
            background-color: white;
            padding: 40px;
            border-radius: 10px;
            box-shadow: 0 6px 20px rgba(0, 0, 0, 0.1);
            border-top: 6px solid #1C5E55;
        }

        h2, h3 {
            text-align: center;
            color: #1C5E55;
            margin-bottom: 25px;
        }

        table {
            width: 100%;
        }

        td {
            padding: 10px;
            vertical-align: top;
        }

        label {
            font-weight: bold;
            color: #1C5E55;
        }

        .asp-input {
            width: 100%;
            padding: 10px;
            border-radius: 6px;
            border: 1px solid #ccc;
            font-size: 14px;
        }

        .asp-button {
            width: 100%;
            padding: 12px;
            margin-top: 12px;
            background-color: #1C5E55;
            color: white;
            font-weight: bold;
            border: none;
            border-radius: 6px;
            cursor: pointer;
            transition: background-color 0.3s ease;
        }

        .asp-button:hover {
            background-color: #16624f;
        }

        .error-label {
            color: red;
            font-size: 12px;
        }

        .status-message {
            text-align: center;
            color: red;
            margin-bottom: 15px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="form-container">
        <div class="form-box">
            <h2>Reset Your Password</h2>

            <asp:Label ID="lblMessage" runat="server" CssClass="status-message" />

            <!-- Panel 1: Email -->
            <asp:Panel ID="pnlVerify" runat="server">
                <table>
                    <tr>
                        <td><label for="txtEmail">Email:</label></td>
                        <td>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="asp-input" Placeholder="Enter your registered email" />
                            <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
                                ControlToValidate="txtEmail" ErrorMessage="Email is required"
                                CssClass="error-label" Display="Dynamic" />
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>
                            <asp:Button ID="btnGetQuestion" runat="server" Text="Get Secret Question" CssClass="asp-button" OnClick="btnGetQuestion_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <!-- Panel 2: Secret Q&A -->
            <asp:Panel ID="pnlQA" runat="server" Visible="false">
                <table>
                    <tr>
                        <td><label>Secret Question:</label></td>
                        <td>
                            <asp:Label ID="lblSecretQuestion" runat="server" ForeColor="#1C5E55" Font-Bold="true" />
                        </td>
                    </tr>
                    <tr>
                        <td><label for="txtSecretAnswer">Secret Answer:</label></td>
                        <td>
                            <asp:TextBox ID="txtSecretAnswer" runat="server" CssClass="asp-input" />
                            <asp:RequiredFieldValidator ID="rfvAnswer" runat="server"
                                ControlToValidate="txtSecretAnswer" ErrorMessage="Answer is required"
                                CssClass="error-label" Display="Dynamic" />
                            <asp:RegularExpressionValidator ID="revAnswer" runat="server"
                                ControlToValidate="txtSecretAnswer" ValidationExpression=".{3,}"
                                ErrorMessage="Answer must be at least 3 characters" CssClass="error-label"
                                Display="Dynamic" />
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>
                            <asp:Button ID="btnVerify" runat="server" Text="Verify Answer" CssClass="asp-button" OnClick="btnVerify_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <!-- Panel 3: New Password -->
            <asp:Panel ID="pnlReset" runat="server" Visible="false">
                <h3>Enter New Password</h3>
                <table>
                    <tr>
                        <td><label for="txtNewPassword">New Password:</label></td>
                        <td>
                            <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" CssClass="asp-input" />
                            <asp:RequiredFieldValidator ID="rfvPassword" runat="server"
                                ControlToValidate="txtNewPassword" ErrorMessage="Password is required"
                                CssClass="error-label" Display="Dynamic" />

                            <asp:RegularExpressionValidator ID="revNewPassword" runat="server"
                            ControlToValidate="txtNewPassword"
                            ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$"
                            ErrorMessage="Password must be at least 8 characters and include at least 1 uppercase, lowercase, digit, and special character"
                            CssClass="error-label"
                            Display="Dynamic" />


                        </td>
                    </tr>
                    <tr>
                        <td><label for="txtConfirmPassword">Confirm Password:</label></td>
                        <td>
                            <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="asp-input" />
                            <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server"
                                ControlToValidate="txtConfirmPassword" ErrorMessage="Please confirm your password"
                                CssClass="error-label" Display="Dynamic" />
                            <asp:CompareValidator ID="cvPasswords" runat="server"
                                ControlToCompare="txtNewPassword" ControlToValidate="txtConfirmPassword"
                                ErrorMessage="Passwords do not match" CssClass="error-label" Display="Dynamic" />
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>
                            <asp:Button ID="btnReset" runat="server" Text="Reset Password" CssClass="asp-button" OnClick="btnReset_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
