<%@ Page Title="Register" Language="C#" MasterPageFile="~/MainMaster.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="MiniBank.Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .register-container {
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 90vh;
        }

        .register-box {
            width: 550px;
            padding: 35px 45px;
            background-color: #ffffff;
            border-radius: 10px;
            box-shadow: 0 6px 18px rgba(0, 0, 0, 0.1);
            border-top: 8px solid #1C5E55;
        }

        .register-title {
            text-align: center;
            font-size: 26px;
            font-weight: bold;
            color: #1C5E55;
            margin-bottom: 25px;
        }

        .register-box label {
            display: block;
            margin-top: 12px;
            font-weight: bold;
            color: #1C5E55;
        }

        .asp-textbox, .asp-dropdown {
            width: 100%;
            padding: 8px 10px;
            margin-top: 5px;
            border-radius: 6px;
            border: 1px solid #ccc;
        }

        .asp-button {
            width: 100%;
            padding: 12px;
            margin-top: 25px;
            background-color: #1C5E55;
            color: white;
            font-weight: bold;
            border: none;
            border-radius: 6px;
            cursor: pointer;
        }

        .asp-button:hover {
            background-color: #16624f;
        }

        .error-label {
            color: red;
            font-size: 12px;
            display: block;
        }

        .gray-tip {
            color: gray;
            font-size: 12px;
            margin-top: 4px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="register-container">
        <div class="register-box">
            <div class="register-title">Registration Form</div>

            <asp:Label ID="ErrorLabel" runat="server" ForeColor="Red" />

            <!-- First Name -->
            <label for="Fname_txtBox">First Name</label>
            <asp:TextBox ID="Fname_txtBox" runat="server" CssClass="asp-textbox" />
            <asp:RequiredFieldValidator ID="rfvFname" runat="server" ControlToValidate="Fname_txtBox" ErrorMessage="First name is required" CssClass="error-label" Display="Dynamic" />

            <!-- Last Name -->
            <label for="Lname_txtBox">Last Name</label>
            <asp:TextBox ID="Lname_txtBox" runat="server" CssClass="asp-textbox" />
            <asp:RequiredFieldValidator ID="rfvLname" runat="server" ControlToValidate="Lname_txtBox" ErrorMessage="Last name is required" CssClass="error-label" Display="Dynamic" />

            <!-- Email -->
            <label for="email_txtBox">Email</label>
            <asp:TextBox ID="email_txtBox" runat="server" CssClass="asp-textbox" Placeholder="example@email.com" />
            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="email_txtBox" ErrorMessage="Email is required" CssClass="error-label" Display="Dynamic" />
            <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="email_txtBox" ErrorMessage="Invalid email format" CssClass="error-label" Display="Dynamic" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" />

            <!-- Phone -->
            <label for="PhoneNo_txtBox">Phone Number</label>
            <asp:TextBox ID="PhoneNo_txtBox" runat="server" CssClass="asp-textbox" Placeholder="01222734481" />
            <asp:RequiredFieldValidator ID="rfvPhone" runat="server" ControlToValidate="PhoneNo_txtBox" ErrorMessage="Phone number is required" CssClass="error-label" Display="Dynamic" />
            <asp:RegularExpressionValidator ID="revPhone" runat="server" ControlToValidate="PhoneNo_txtBox" ErrorMessage="Phone number must be exactly 11 digits, e.g. 01222734481" CssClass="error-label" Display="Dynamic" ValidationExpression="^\d{11}$" />

            <!-- Address -->
            <label for="Address_txtBox">Address</label>
            <asp:TextBox ID="Address_txtBox" runat="server" CssClass="asp-textbox" />
            <asp:RequiredFieldValidator ID="rfvAddress" runat="server" ControlToValidate="Address_txtBox" ErrorMessage="Address is required" CssClass="error-label" Display="Dynamic" />

            <!-- Password -->
            <label for="password_txtBox">Password</label>
            <asp:TextBox ID="password_txtBox" runat="server" CssClass="asp-textbox" TextMode="Password" OnTextChanged="password_txtBox_TextChanged" />
            <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="password_txtBox" ErrorMessage="Password is required" CssClass="error-label" Display="Dynamic" />
            <asp:RegularExpressionValidator ID="revPassword" runat="server"
             ControlToValidate="Password_txtBox"
             ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$"
             ErrorMessage="Password must be at least 8 characters and include at least 1 uppercase, lowercase, digit, and special character"
             CssClass="error-label"
             Display="Dynamic" />

            <!-- Confirm Password -->
            <label for="confirmPassword_txtBox">Confirm Password</label>
            <asp:TextBox ID="confirmPassword_txtBox" runat="server" CssClass="asp-textbox" TextMode="Password" />
            <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server" ControlToValidate="confirmPassword_txtBox" ErrorMessage="Please confirm your password" CssClass="error-label" Display="Dynamic" />
            <asp:CompareValidator ID="cvPasswords" runat="server" ControlToCompare="password_txtBox" ControlToValidate="confirmPassword_txtBox" ErrorMessage="Passwords do not match" CssClass="error-label" Display="Dynamic" />

            <!-- Secret Question -->
            <label for="ddlSecretQuestion">Secret Question</label>
            <asp:DropDownList ID="ddlSecretQuestion" runat="server" AppendDataBoundItems="true" CssClass="asp-dropdown">
                <asp:ListItem Text="-- Select a question --" Value="" />
            </asp:DropDownList>
            <div class="gray-tip">💡 Choose something only you know. Avoid obvious answers.</div>
            <asp:RequiredFieldValidator ID="rfvSecretQ" runat="server" ControlToValidate="ddlSecretQuestion" InitialValue="" ErrorMessage="Please select a secret question" CssClass="error-label" Display="Dynamic" />

            <!-- Secret Answer -->
            <label for="txtSecretAnswer">Secret Answer</label>
            <asp:TextBox ID="txtSecretAnswer" runat="server" CssClass="asp-textbox" />
            <asp:RequiredFieldValidator ID="rfvSecretA" runat="server" ControlToValidate="txtSecretAnswer" ErrorMessage="Secret answer is required" CssClass="error-label" Display="Dynamic" />
            <asp:RegularExpressionValidator ID="revSecretA" runat="server" ControlToValidate="txtSecretAnswer" ValidationExpression=".{3,}" ErrorMessage="Secret answer must be at least 3 characters long" CssClass="error-label" Display="Dynamic" />

            <!-- Submit Button -->
            <asp:Button ID="Register_btn" runat="server" Text="Register" CssClass="asp-button" CausesValidation="true" OnClick="Register_btn_Click" />

            <!-- Validation Summary -->
            <asp:ValidationSummary ID="vsSummary" runat="server" HeaderText="Please fix the following errors:" ForeColor="Red" DisplayMode="BulletList" />
        </div>
    </div>
</asp:Content>
