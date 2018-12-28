<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin.aspx.cs" Inherits="TheParsnipWeb.Admin" %>
<%@ Register Src="~/CustomControls/UacApi/UserForm.ascx" TagPrefix="uc1" TagName="UserForm" %>
<%@ Register Src="~/CustomControls/admin/adminMenu.ascx" TagPrefix="adminControls" TagName="adminMenu" %>


<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Admin</title>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" type="text/css" href="ParsnipStyle.css" />
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.2/css/bootstrap.min.css" integrity="sha384-Smlep5jCw/wG7hdkwQ/Z5nLIefveQRIY9nfy6xoR1uRYBtpZgI6339F5dgvm/e9B" crossorigin="anonymous" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.1.2/js/bootstrap.min.js" integrity="sha384-o+RDsa0aLu++PJvFqy8fFScvbHFLtbvScb8AjopnFD+iEQ7wo/CG0xlczd+2O/em" crossorigin="anonymous"></script>
</head>
<body style="padding-bottom:2.5%; padding-top:1.5%">
    <form runat="server">
        <div class="container">
            <div class="jumbotron">
                <h1>Admin</h1>
                <p>Website management suite</p>
            </div>
            <adminControls:adminMenu runat="server" id="adminMenu" />
        </div>
    </form>
</body>
</html>
