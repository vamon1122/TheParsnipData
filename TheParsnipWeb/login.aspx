﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="TheParsnipWeb.LogInBarrier" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title></title>
    <link rel="stylesheet" type="text/css" href="ParsnipStyle.css" />
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.2/css/bootstrap.min.css" integrity="sha384-Smlep5jCw/wG7hdkwQ/Z5nLIefveQRIY9nfy6xoR1uRYBtpZgI6339F5dgvm/e9B" crossorigin="anonymous" />
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.1.2/js/bootstrap.min.js" integrity="sha384-o+RDsa0aLu++PJvFqy8fFScvbHFLtbvScb8AjopnFD+iEQ7wo/CG0xlczd+2O/em" crossorigin="anonymous"></script>
</head>
<body style="padding-top:5%;">
    
    <form runat="server">
        <div class="center_div">
            <img src="Images/fat_kieron_cutout.JPG" style="max-width:150px; display:block; margin-left: auto; margin-right:auto;" />
                
  <div class="form-group">
    
          
      <br />    
      <label>Username</label>
    <asp:TextBox runat="server" CssClass="form-control login" ID="inputUsername"  />
  </div>
  <div class="form-group">
    <label for="exampleInputPassword1">Password</label>
    <asp:TextBox runat="server" TextMode="password" CssClass="form-control login" ID="inputPwd"  />
  </div>
  <div class="form-check">
    <asp:CheckBox runat="server" CssClass="form-check-input login" ID="RememberPwd" />
    <label class="form-check-label">Remember Password</label>
  </div>
            <br />
  <asp:Button runat="server" ID="ButLogIn" OnClick="ButLogIn_Click" CssClass="btn btn-primary" Text="Log In"></asp:Button>
</form>
    </div>
</body>
</html>
