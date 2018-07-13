<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="TheParsnipWeb.LogInBarrier" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="ParsnipStyle.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <label>Username</label> <asp:TextBox runat="server" ID="inputUsername" CssClass="login"></asp:TextBox>
            <br />
            <label>Password:</label> <asp:TextBox runat="server" ID="inputPwd" TextMode="Password" CssClass="login"></asp:TextBox>
            <br />
            <asp:Button runat="server" ID="ButLogIn" OnClick="ButLogIn_Click" Text="Log In" CssClass="login" />
            <label>Remember password</label> <asp:CheckBox runat="server" ID="RememberPwd" />

            <asp:Label runat="server" ID="StatusIndicator"></asp:Label>
        </div>
    </form>
</body>
</html>
