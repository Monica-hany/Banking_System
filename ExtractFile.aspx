<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExtractFile.aspx.cs" Inherits="MiniBank.ExtractFile" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
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
            border-style: none;
            border-color: inherit;
            border-width: medium;
            padding: 12px 0;
            font-size: 16px;
            font-weight: bold;
            color: white;
            background-color: #1C5E55;
            border-radius: 8px;
            cursor: pointer;
            transition: background-color 0.3s ease;
            margin: 0 10px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <!-- ✅ Required for popup alerts -->
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        
        <div style="text-align: center; margin-top: 20px;">
            <asp:Button ID="ExtractFile_btn" runat="server" Text="Extract Transactions Report"
                        Enabled="true"
                        CssClass="action-button" OnClick="ExtractFile_btn_Click" Width="306px" />
        </div>
    </form>
</body>
</html>
