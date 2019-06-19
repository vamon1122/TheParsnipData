<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Share_Stats.aspx.cs" Inherits="ParsnipWebsite.Share_Stats" %>

<%@ Register Src="~/Custom_Controls/Admin/adminMenu.ascx" TagPrefix="adminControls" TagName="adminMenu" %>
<%@ Register Src="~/Custom_Controls/Uac_Api/AdminUserForm.ascx" TagPrefix="admin" TagName="AdminUserForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.2/css/bootstrap.min.css" integrity="sha384-Smlep5jCw/wG7hdkwQ/Z5nLIefveQRIY9nfy6xoR1uRYBtpZgI6339F5dgvm/e9B" crossorigin="anonymous" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.1.2/js/bootstrap.min.js" integrity="sha384-o+RDsa0aLu++PJvFqy8fFScvbHFLtbvScb8AjopnFD+iEQ7wo/CG0xlczd+2O/em" crossorigin="anonymous"></script>

    <script src="../Javascript/Useful_Functions.js"></script>
    <link id="link_style" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="Css/Shared_Style.css" />
    <script src="../Javascript/Apply_Style.js"></script>

    <title>Create User</title>
</head>
<body style="padding-bottom:2.5%; padding-top:4%">
    <form runat="server">
        <div class="container">
            <!-- Alerts -->
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
                <h1 class="display-4">Share Stats</h1>
                <p class="lead">View the statistics of shared links</p>
                <hr class="my-4" />
                <p><adminControls:adminMenu runat="server" id="adminMenu1" /></p>
            </div>   
           
            <asp:Table runat="server" ID="Table_Stats" CssClass="table table-striped" >
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell>Title</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Uploaded By</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Shared By</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Times Used</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Media Directory</asp:TableHeaderCell>
                </asp:TableHeaderRow>
                
            </asp:Table>
            
        </div>

        
    </form>
</body>
</html>
