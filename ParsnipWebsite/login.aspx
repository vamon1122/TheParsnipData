<%@ Page Async="true" Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ParsnipWebsite.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.2/css/bootstrap.min.css" integrity="sha384-Smlep5jCw/wG7hdkwQ/Z5nLIefveQRIY9nfy6xoR1uRYBtpZgI6339F5dgvm/e9B" crossorigin="anonymous" />
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.1.2/js/bootstrap.min.js" integrity="sha384-o+RDsa0aLu++PJvFqy8fFScvbHFLtbvScb8AjopnFD+iEQ7wo/CG0xlczd+2O/em" crossorigin="anonymous"></script>
    <!--<script src="bootstrap-4.1.2-dist/js/bootstrap.js"></script>-->
    
    <script src="../Javascript/Useful_Functions.js"></script>
    <link id="link_style" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="Css/Shared_Style.css" />
    <script src="../Javascript/Apply_Style.js"></script>

    <title>Log In</title>
</head>
<body style="padding-top: 0px; text-align:center">
    <div style="padding-top: 1.5%; padding-left:1.5%; padding-right:1.5%;">
        <div class="alert alert-warning alert-dismissible" runat="server" style="display:none;" id="Warning">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <strong>Confirm Identity</strong> You must log in first!
        </div>

        <div class="alert alert-danger alert-dismissible" runat="server" style="display:none;" id="Alert_LogInError">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <strong>Login Failed</strong> No user was found with those details.
        </div>
    </div>
    <div class="center_form">
    <form runat="server" >        
        <img src="Resources/Media/Images/Local/Fat_Kieron_Cutout.JPG" style="max-width:100px; display:block; margin-left: auto; margin-right:auto;" />
        <div style="padding-right:5%;padding-left:5%;">
                <br />    
                <label style="text-align:left; width:100%">Username</label>
                <asp:TextBox runat="server" CssClass="form-control login" ID="inputUsername"  />
                <div class="form-group">
                    <label style="text-align:left; width:100%">Password</label>
                    <asp:TextBox runat="server" TextMode="password" CssClass="form-control login" ID="inputPwd" />
                </div>
                <div class="form-check" style="text-align:left; width:100%">
                    <asp:CheckBox runat="server" CssClass="form-check-input login" ID="RememberPwd" />
                    <label class="form-check-label" >Remember Password</label>
                </div>
                <br />
                <div style="float:left;">
                    <asp:Button runat="server" ID="ButLogIn" OnClick="ButLogIn_Click" CssClass="btn btn-primary" Text="Log In"></asp:Button>
                </div>
            </div>
    </form>
        </div>
</body>
</html>

