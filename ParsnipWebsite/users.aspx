<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="users.aspx.cs" Inherits="TheParsnipWeb.create_user" %>
<%@ Register Src="~/CustomControls/admin/adminMenu.ascx" TagPrefix="adminControls" TagName="adminMenu" %>
<%@ Register Src="~/CustomControls/UacApi/UserForm1.ascx" TagPrefix="uc1" TagName="UserForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Create User</title>
    
    <meta name="viewport" content="width=device-width, initial-scale=1">
    
    <link rel="stylesheet" type="text/css" href="css/parsnipStyle.css" />
        
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.2/css/bootstrap.min.css" integrity="sha384-Smlep5jCw/wG7hdkwQ/Z5nLIefveQRIY9nfy6xoR1uRYBtpZgI6339F5dgvm/e9B" crossorigin="anonymous" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
    
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.1.2/js/bootstrap.min.js" integrity="sha384-o+RDsa0aLu++PJvFqy8fFScvbHFLtbvScb8AjopnFD+iEQ7wo/CG0xlczd+2O/em" crossorigin="anonymous"></script>
        

</head>
<body style="padding-bottom:2.5%; padding-top:4%">
    <form runat="server">
    <div class="container">

        <div class="alert alert-danger alert-dismissible" runat="server" style="display:none" id="Error">
             <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
             <asp:Label runat="server" ID="ErrorText"></asp:Label>
        </div>
        <div class="alert alert-warning alert-dismissible" runat="server" style="display:none" id="Warning">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label runat="server" ID="WarningText"></asp:Label>
        </div>
        <div class="alert alert-success alert-dismissible" runat="server" style="display:none" id="Success">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label runat="server" ID="SuccessText"></asp:Label>
        </div>

        <div class="jumbotron">
            <h1 class="display-4">Users</h1>
            <p class="lead">Create & edit theparsnip.co.uk users</p>
            <hr class="my-4" />
            <p><adminControls:adminMenu runat="server" id="adminMenu1" /></p>
        </div>   
           
        <div class="center_div">
             <label>Users</label>
        <asp:DropDownList ID="selectUser" runat="server" AutoPostBack="True" CssClass="form-control" 
        onselectedindexchanged="SelectUser_Changed">
    </asp:DropDownList>
            </div>
            
        <uc1:UserForm1 runat="server" ID="UserForm" />
        <asp:Button runat="server" ID="btnAction" OnClick="btnAction_Click" CssClass="btn btn-primary" Text="Action"></asp:Button>
    </div>
        </form>
    
</body>
</html>
