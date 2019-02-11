<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Manage-Photos.aspx.cs" Inherits="ParsnipWebsite.Manage_Photos" %>

<%@ Register Src="~/CustomControls/admin/adminMenu.ascx" TagPrefix="adminControls" TagName="adminMenu" %>
<%@ Register Src="~/CustomControls/UacApi/AdminUserForm.ascx" TagPrefix="admin" TagName="AdminUserForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="css/parsnipStyle.css" />
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.2/css/bootstrap.min.css" integrity="sha384-Smlep5jCw/wG7hdkwQ/Z5nLIefveQRIY9nfy6xoR1uRYBtpZgI6339F5dgvm/e9B" crossorigin="anonymous" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.1.2/js/bootstrap.min.js" integrity="sha384-o+RDsa0aLu++PJvFqy8fFScvbHFLtbvScb8AjopnFD+iEQ7wo/CG0xlczd+2O/em" crossorigin="anonymous"></script>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
            <div class="container">
            <div class="center_div">
                <div class="jumbotron">
                <h1 class="display-4">Users</h1>
                <p class="lead">Create & edit theparsnip.co.uk users</p>
                <hr class="my-4" />
                <p><adminControls:adminMenu runat="server" id="adminMenu1" /></p>
            </div>  
            
            
            <label>Select a user:</label>
            <asp:DropDownList ID="SelectUser" runat="server" AutoPostBack="True" CssClass="form-control" 
                onselectedindexchanged="SelectUser_Changed">
            </asp:DropDownList>
                <asp:Button runat="server" ID="BtnDeleteUploads" OnClick="BtnDeleteUploads_Click" CssClass="btn btn-primary" Text="Delete"></asp:Button>
                <div runat="server" id="DisplayPhotosDiv">

                </div>

            
        </div>
        </div>
    </form>
</body>
</html>
