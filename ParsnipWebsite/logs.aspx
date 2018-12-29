<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="logs.aspx.cs" Inherits="TheParsnipWeb.logs" %>
<%@ Register Src="~/CustomControls/admin/adminMenu.ascx" TagPrefix="adminControls" TagName="adminMenu" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Logs</title>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" type="text/css" href="css/parsnipStyle.css" />
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.2/css/bootstrap.min.css" integrity="sha384-Smlep5jCw/wG7hdkwQ/Z5nLIefveQRIY9nfy6xoR1uRYBtpZgI6339F5dgvm/e9B" crossorigin="anonymous" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.1.2/js/bootstrap.min.js" integrity="sha384-o+RDsa0aLu++PJvFqy8fFScvbHFLtbvScb8AjopnFD+iEQ7wo/CG0xlczd+2O/em" crossorigin="anonymous"></script>
</head>
<body style="padding-bottom:2.5%; padding-top:4%">
    <form id="form1" runat="server">
        <div class="container">
            <div class="jumbotron">
            <h1 class="display-4">Logs</h1>
            <p class="lead">View theparsnip.co.uk logs</p>
            <hr class="my-4" />
            <p><adminControls:adminMenu runat="server" id="adminMenu1" /></p>
                </div>
        
        <div class="table-wrapper-scroll-y">
            <asp:Table class="table table-bordered table-striped" runat="server" id="LogTable" />
        </div>
        </div>
    </form>
</body>
</html>
