<%@ Page Title="Login" Language="C#" MasterPageFile="~/MainMaster.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="MiniBank.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        body {
            font-family: Arial, sans-serif;
        }

        .login-container {
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 80vh;
        }

        .login-box {
            width: 400px;
            padding: 30px 40px;
            border-radius: 10px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
            background-color: #ffffff;
            border-top: 8px solid #1C5E55;
        }

        .login-title {
            text-align: center;
            font-size: 26px;
            font-weight: bold;
            color: #1C5E55;
            margin-bottom: 30px;
        }

        .login-box label {
            display: block;
            font-weight: bold;
            margin-bottom: 5px;
            color: #1C5E55;
        }

        .login-box .asp-textbox {
            width: 100%;
            padding: 8px 10px;
            margin-bottom: 15px;
            border-radius: 6px;
            border: 1px solid #ccc;
        }

        .login-box .asp-button {
            width: 100%;
            background-color: #1C5E55;
            color: white;
            font-weight: bold;
            padding: 10px;
            border: none;
            border-radius: 6px;
            cursor: pointer;
        }

        .login-box .asp-button:hover {
            background-color: #16624f;
        }

        .login-box .links {
            text-align: center;
            margin-top: 15px;
        }

        .login-box .links a {
            color: #1C5E55;
            font-weight: bold;
        }

        .error-label {
            color: red;
            font-size: 12px;
            display: block;
            margin-top: -10px;
            margin-bottom: 10px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="login-container">
        <div class="login-box">
            <div class="login-title">Welcome to NBE Mini Bank</div>

            <!-- Email -->
            <label for="email_txtBox">Email</label>
            <asp:TextBox ID="email_txtBox" runat="server" CssClass="asp-textbox" Placeholder="example@email.com"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
                ControlToValidate="email_txtBox"
                ErrorMessage="Email is required"
                CssClass="error-label"
                Display="Dynamic" />
            <asp:RegularExpressionValidator ID="revEmail" runat="server"
                ControlToValidate="email_txtBox"
                ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                ErrorMessage="Invalid email format"
                CssClass="error-label"
                Display="Dynamic" />

            <!-- Password -->
            <label for="Password_txtBox">Password</label>
            <asp:TextBox ID="Password_txtBox" runat="server" CssClass="asp-textbox" TextMode="Password" Placeholder="Enter your password" OnTextChanged="Password_txtBox_TextChanged"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvPassword" runat="server"
                ControlToValidate="Password_txtBox"
                ErrorMessage="Password is required"
                CssClass="error-label"
                Display="Dynamic" />

            <!-- Forgot Password -->
            <div class="links">
                <asp:HyperLink ID="ForgetPW_hyperlink" runat="server" NavigateUrl="~/ForgotPassword.aspx">Forgot Password?</asp:HyperLink>
            </div>

            <!-- Login Button -->
            <asp:Button ID="login_btn" runat="server" Text="Login" CssClass="asp-button" OnClick="btnLogin_Click" />


            <!-- Register -->
            <div class="links">
                New user? <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Register.aspx">Register here</asp:HyperLink>
            </div>
        </div>
    </div>
</asp:Content>
