<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.Master" AutoEventWireup="true" CodeBehind="Status.aspx.cs" Inherits="MiniBank.Status" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
    .status-container {
        text-align: center;
        margin-top: 100px;
        padding: 20px;
    }

    .status-title {
        font-size: 28px;
        color: #1C5E55;
        font-weight: bold;
        display: block;
        margin-bottom: 12px;
    }

    .status-message {
        font-size: 18px;
        color: #444;
        display: block;
    }

    .status-button {
        background-color: #1C5E55;
        color: white;
        padding: 12px 24px;
        font-size: 16px;
        border: none;
        border-radius: 6px;
        cursor: pointer;
    }

    .status-button:hover {
        background-color: #16624f;
    }

    .status-tip {
    font-size: 15px;
    color: #b34700; /* orange-brown for a warning look */
    margin-top: 10px;
    display: block;
}


</style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="status-container">
        <asp:Label ID="lblStatusTitle" runat="server" CssClass="status-title" />
        <br />
        <asp:Label ID="lblStatusMessage" runat="server" CssClass="status-message" />
        <br /><br />
        <asp:Button ID="btnAction" runat="server" CssClass="status-button" />
        <br /><br />
        <asp:Label ID="lblTip" runat="server" CssClass="status-tip" Visible="false" />
    </div>
</asp:Content>

