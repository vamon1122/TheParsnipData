<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccountTiny.ascx.cs" Inherits="TheParsnipWeb.AccountTiny" %>

<form runat="server">
<asp:Label ID="Name" runat="server"></asp:Label>
<asp:Button runat="server" ID="LogOut" Text="Log Out" OnClick="LogOut_Click"/>
    </form>