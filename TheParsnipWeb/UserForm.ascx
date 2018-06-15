<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserForm.ascx.cs" Inherits="TheParsnipWeb.UserForm" %>
<link rel="stylesheet" type="text/css" href="ParsnipStyle.css" />


<label>Username:</label><asp:TextBox runat="server" ID="username"></asp:TextBox>
<br /><label>Email:</label><asp:TextBox runat="server" ID="email"></asp:TextBox>
<br /><label>Password:</label><asp:TextBox runat="server" ID="pwd"></asp:TextBox>
<br /><label>Forename:</label><asp:TextBox runat="server" ID="fname"></asp:TextBox>
<br /><label>Surname:</label><asp:TextBox runat="server" ID="sname"></asp:TextBox>
<br /><label>Date Of Birth:</label><asp:TextBox runat="server" ID="dob"></asp:TextBox>
<br /><label>Address Line 1:</label><asp:TextBox runat="server" ID="address1"></asp:TextBox>
<br /><label>Address Line 2:</label><asp:TextBox runat="server" ID="address2"></asp:TextBox>
<br /><label>Address Line 3:</label><asp:TextBox runat="server" ID="address3"></asp:TextBox>
<br /><label>Post Code:</label><asp:TextBox runat="server" ID="postcode"></asp:TextBox>
<br /><label>Mobile Phone:</label><asp:TextBox runat="server" ID="mobfon"></asp:TextBox>
<br /><label>Home Phone:</label><asp:TextBox runat="server" ID="homefon"></asp:TextBox>
<br /><label>Work Phone:</label><asp:TextBox runat="server" ID="workfon"></asp:TextBox>
<br /><label>Account Type:</label><asp:TextBox runat="server" ID="acctype"></asp:TextBox>
<br /><label>Account Status:</label><asp:TextBox runat="server" ID="accstatus"></asp:TextBox>
<br />

<asp:Button runat="server" ID="but_Submit" Text="Submit" OnClick="but_Submit_Click" />
