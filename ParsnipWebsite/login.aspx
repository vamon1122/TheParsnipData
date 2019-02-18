<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="ParsnipWebsite.LogInBarrier" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title></title>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.2/css/bootstrap.min.css" integrity="sha384-Smlep5jCw/wG7hdkwQ/Z5nLIefveQRIY9nfy6xoR1uRYBtpZgI6339F5dgvm/e9B" crossorigin="anonymous" />
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.1.2/js/bootstrap.min.js" integrity="sha384-o+RDsa0aLu++PJvFqy8fFScvbHFLtbvScb8AjopnFD+iEQ7wo/CG0xlczd+2O/em" crossorigin="anonymous"></script>
    <!--<script src="bootstrap-4.1.2-dist/js/bootstrap.js"></script>-->

</head>
<body>
    
    <form runat="server">
        <div style="padding-top: 1.5%; padding-left:1.5%; padding-right:1.5%;">
        <div class="alert alert-warning alert-dismissible" runat="server" style="display:none;" id="Warning">

                 <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                <strong>Confirm Identity</strong> You must log in first!
            </div>
            </div>
        <script src="../javascript/globalBodyV1.6.js"></script>
        
            <div style="width:300px; ">
            

            <div style="position:absolute; left: 50%; margin-left:-150px;">
            <img src="resources/media/images/fat_kieron_cutout.JPG" style="max-width:100px; display:block; margin-left: auto; margin-right:auto;" />
                
  <div class="form-group" style="width:300px"  >
    
          
      <br />    
      <label>Username</label>
    <asp:TextBox runat="server" CssClass="form-control login" ID="inputUsername"  />
  </div>
  <div class="form-group">
    <label>Password</label>
    <asp:TextBox runat="server" TextMode="password" CssClass="form-control login" ID="inputPwd" />
  </div>
  <div class="form-check">
    <asp:CheckBox runat="server" CssClass="form-check-input login" ID="RememberPwd" />
    <label class="form-check-label">Remember Password</label>
  </div>
            <br />
  <asp:Button runat="server" ID="ButLogIn" OnClick="ButLogIn_Click" CssClass="btn btn-primary" Text="Log In"></asp:Button>
</div>
                </div>
</form>
    
</body>
</html>
