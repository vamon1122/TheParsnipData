<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TheParsnipWeb.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="font-family:Verdana">
    <form id="form1" runat="server">
        <h1>#TheParsnip</h1>
        <img src="https://ao.com/life/kitchen/kitchen-tips/vegetable-cookbook/images/main/parsnips.jpg"></img>
        <asp:TextBox runat="server" ID="BenTextBox" OnTextChanged="BenTextBox_TextChanged"></asp:TextBox>
        <asp:Label runat="server" ID="BenLabel"></asp:Label>
        <asp:Button runat="server" ID="BenButton" OnClick="BenButton_Click" />


        <div>
            Big Balls
        </div>
    </form>
</body>
</html>
