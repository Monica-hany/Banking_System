<%@ Page Title="Transfer" Language="C#" MasterPageFile="~/MainMaster.Master" AutoEventWireup="true" CodeBehind="Transfer.aspx.cs" Inherits="MiniBank.Transfer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
    .transfer-container {
        display: flex;
        justify-content: center;
        align-items: center;
        min-height: 90vh;
    }

    .transfer-box {
        width: 500px;
        padding: 35px 45px;
        background-color: #fff;
        border-radius: 10px;
        box-shadow: 0 6px 16px rgba(0, 0, 0, 0.1);
        border-top: 8px solid #1C5E55;
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

    .transfer-title {
        text-align: center;
        font-size: 26px;
        font-weight: bold;
        color: #1C5E55;
        margin-bottom: 25px;
    }

    .transfer-box label {
        display: block;
        margin-top: 12px;
        font-weight: bold;
        color: #1C5E55;
    }

    .asp-input, .asp-dropdown {
        width: 100%;
        padding: 8px 10px;
        margin-top: 5px;
        border-radius: 6px;
        border: 1px solid #ccc;
    }

    .asp-button {
        width: 100%;
        padding: 12px;
        margin-top: 20px;
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

    .result-message {
        margin-top: 10px;
        font-size: 14px;
    }

    .modal-overlay {
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(28, 94, 85, 0.6);
        display: flex;
        align-items: center;
        justify-content: center;
        z-index: 1000;
    }

    .modal-box {
        background: #fff;
        padding: 30px;
        border-radius: 10px;
        text-align: center;
        box-shadow: 0 4px 20px rgba(0,0,0,0.3);
        width: 350px;
        animation: fadeIn 0.3s ease-in-out;
    }

    .ok-button {
        margin-top: 20px;
        padding: 10px 20px;
        background-color: #1C5E55;
        border: none;
        color: white;
        font-weight: bold;
        border-radius: 5px;
        cursor: pointer;
    }

    #okBtn {
        display: none;
    }

    .ok-button:hover {
        background-color: #16624f;
    }

    @keyframes fadeIn {
        from {opacity: 0;}
        to {opacity: 1;}
    }

    /* ✅ Checkmark animation */
    .checkmark-circle {
        width: 80px;
        height: 80px;
        margin: 0 auto 20px;
        position: relative;
    }

    .checkmark-circle .background {
        width: 80px;
        height: 80px;
        border-radius: 50%;
        background: #1C5E55;
        position: absolute;
        top: 0;
        left: 0;
    }

    .checkmark-circle .checkmark {
        position: absolute;
        top: 24px;
        left: 27px;
        width: 18px;
        height: 35px;
        transform: rotate(45deg);
        border-right: 4px solid #fff;
        border-bottom: 4px solid #fff;
        animation: drawCheck 0.5s ease-out forwards;
    }

    @keyframes drawCheck {
        0% {
            width: 0;
            height: 0;
            opacity: 0;
        }
        50% {
            width: 0;
            height: 18px;
            opacity: 1;
        }
        100% {
            width: 18px;
            height: 35px;
            opacity: 1;
        }
    }
</style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div style="text-align: right; margin: 20px; margin-bottom: 0;">
        <asp:LinkButton 
            ID="Logout_Link" 
            runat="server" 
            OnClick="Logout_Link_Click" 
            CausesValidation="false"
            Style="text-decoration: none; color: #1C5E55; font-weight: bold; font-size: 16px;">
            Logout
            <img src="Images/logout.jpg" alt="Logout" style="vertical-align: middle; width: 24px; height: 24px; margin-right: 5px;" />
        </asp:LinkButton>
    </div>


    <div class="transfer-container">
        <!-- The message shown if there are zero active accounts -->
        <div style="text-align:center; margin-bottom: 30px;">
            <asp:Label 
                ID="lblNoActiveAccounts" 
                runat="server" 
                Text="⚠️ You don’t have any active accounts available to make a transfer."
                ForeColor="Red"
                Font-Bold="True"
                CssClass="message-label"
                Visible="false"
                Style="font-size: 20px; display: block; margin-bottom: 15px;"
            />
            <asp:Button 
                ID="btnBackHome" 
                runat="server" 
                Text="Back to Home Page" 
                OnClick="btnBackHome_Click" 
                Visible="false" 
                CssClass="action-button" 
            />
        </div>

        <!-- The panel that holds the transfer form -->
        <asp:Panel ID="pnlTransferForm" runat="server" Visible="false">
            <div class="transfer-box">
                <div class="transfer-title">Transfer Money</div>

                <!-- From Account -->
                <label for="SenderAccounts_ddl">From</label>
                <asp:DropDownList ID="SenderAccounts_ddl" runat="server" CssClass="asp-dropdown" />
                <asp:RequiredFieldValidator ID="rfvSender" runat="server" ControlToValidate="SenderAccounts_ddl" InitialValue="" ErrorMessage="Please select a sender account" CssClass="error-label" Display="Dynamic" />

                <!-- Recipient Email -->
                <label for="RecepientEmail_txtBox">Recipient Email Address</label>
                <asp:TextBox ID="RecepientEmail_txtBox" runat="server" CssClass="asp-input" Placeholder="example@email.com" />
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="RecepientEmail_txtBox" ErrorMessage="Email address is required" CssClass="error-label" Display="Dynamic" />
                <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="RecepientEmail_txtBox" ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" ErrorMessage="Please enter a valid email address" CssClass="error-label" Display="Dynamic" />

                <asp:Button 
                    ID="SearchRecepient_btn" 
                    runat="server" 
                    Text="Search Recipient Accounts" 
                    CssClass="asp-button" 
                    OnClick="SearchRecepient_btn_Click" 
                    CausesValidation="false" 
                />

                <!-- This button is hidden by default, only shown after a valid search -->
                <asp:Button 
                    ID="ChangeEmail_btn" 
                    runat="server" 
                    Text="Change Email" 
                    CssClass="asp-button" 
                    OnClick="ChangeEmail_btn_Click" 
                    Visible="false" 
                />

                <!-- Recipient Search Result Messages -->
                <div class="result-message">
                    <asp:Label ID="lblRecipientNotFound" runat="server" Text="❌ This recipient email is not registered." ForeColor="Red" Font-Bold="True" Visible="false" />
                    <asp:Label ID="lblNoRecipientAccounts" runat="server" Text="⚠️ This recipient has no active accounts." ForeColor="OrangeRed" Font-Bold="True" Visible="false" />
                    <asp:Label ID="lblNoMoreRecipientAccounts" runat="server" 
                    Text="⚠️ This recipient has no other active accounts available for transfer." 
                    ForeColor="OrangeRed" Font-Bold="True" Visible="false" />
                </div>

                <!-- To Account -->
                <label for="RecepientAccounts_ddl">To</label>
                <asp:DropDownList ID="RecepientAccounts_ddl" runat="server" CssClass="asp-dropdown" />
                <asp:RequiredFieldValidator ID="rfvRecepient" runat="server" ControlToValidate="RecepientAccounts_ddl" InitialValue="" ErrorMessage="Please select a recipient account" CssClass="error-label" Display="Dynamic" EnableClientScript="false" EnableViewState="false" />

                <!-- Transfer Amount -->
                <label for="TransferAmount_txtBox">Transfer Amount</label>
                <asp:TextBox ID="TransferAmount_txtBox" runat="server" CssClass="asp-input" Placeholder="e.g. 100.00" />
                <asp:RequiredFieldValidator ID="rfvAmount" runat="server" ControlToValidate="TransferAmount_txtBox" ErrorMessage="Transfer amount is required" CssClass="error-label" Display="Dynamic" EnableClientScript="false" EnableViewState="false" />
                <asp:RegularExpressionValidator ID="revAmount" runat="server" ControlToValidate="TransferAmount_txtBox" ValidationExpression="^\d+(\.\d{1,2})?$" ErrorMessage="Amount must be a valid number (e.g. 100 or 100.50)" CssClass="error-label" Display="Dynamic" />

                <!-- Scheduled Date -->
                <label for="ScheduledDate_txtBox">Scheduled Transfer Date</label>
                <asp:TextBox ID="ScheduledDate_txtBox" runat="server" CssClass="asp-input" TextMode="Date" />
                <asp:CustomValidator 
                    ID="cvScheduledDate" 
                    runat="server" 
                    ControlToValidate="ScheduledDate_txtBox" 
                    OnServerValidate="cvScheduledDate_ServerValidate"
                    ErrorMessage="Date must be today or in the future" 
                    CssClass="error-label" 
                    Display="Dynamic" />

                <!-- Submit Button -->
                <asp:Button ID="Transfer_btn" runat="server" Text="Transfer" CssClass="asp-button" CausesValidation="true" OnClick="Transfer_btn_Click" />

                <!--Back to Home page button-->
                 <div style="display: flex; gap: 15px; margin-top: 10px;">
                     <asp:Button ID="backToHome_btn" runat="server" Text="Back To Home"
                     CssClass="asp-button" OnClick="backToHome_btn_Click" CausesValidation="false" />

                 </div>

                <!-- Success Modal -->
                <div id="successModal" class="modal-overlay" style="display:none;">
                    <div class="modal-box">
                        <div class="checkmark-circle">
                            <div class="background"></div>
                            <div class="checkmark"></div>
                        </div>
                        <h3>Transfer Successful!</h3>
                        <button id="okBtn" class="ok-button">OK</button>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>

    <script type="text/javascript">
        function showSuccessModal() {
            document.getElementById("successModal").style.display = "flex";
        }
    </script>
</asp:Content>