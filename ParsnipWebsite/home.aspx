<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="home.aspx.cs" Inherits="ParsnipWebsite.Home" %>
<%@ Register Src="~/CustomControls/AccountTiny.ascx" TagPrefix="uc1" TagName="AccountTiny" %>



<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- BOOTSTRAP START -->
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.2/css/bootstrap.min.css" integrity="sha384-Smlep5jCw/wG7hdkwQ/Z5nLIefveQRIY9nfy6xoR1uRYBtpZgI6339F5dgvm/e9B" crossorigin="anonymous" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.1.2/js/bootstrap.min.js" integrity="sha384-o+RDsa0aLu++PJvFqy8fFScvbHFLtbvScb8AjopnFD+iEQ7wo/CG0xlczd+2O/em" crossorigin="anonymous"></script>
    <!-- BOOTSTRAP END -->

    <link id="link_style" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="css/shared-style.css" />
        
    <title>Home</title>
</head>
<body class="fade0p5" id="body">
    <label class="censored" id="pageId">home.html</label>
    
    <!--FOR JS DYNAMIC PAGE CREATION DO NOT MOVE START-->

    <div id="titleAndMenu"></div>
    <div id="menuDiv"></div>

    <!--FOR JS DYNAMIC PAGE CREATION DO NOT MOVE END-->
    <!--<img src="../rounded.png" />-->
    <br /><br />
    <div class="padding" />
    <!--<h2>Try the new look!</h2>
    Warning, you will be logged into a test account, however yout account details will be remembered and will revert when you click on the try button again. The new look may change regularly and may often be broken.
    <br />
    <br />
    <button onclick='{
    if (!testCookie())
    {
        alert("Cookies do not appear to be wotking on your device. You cannot access this part of the website.")
    }
    else if (getCookie("username") !== "test") {
        document.cookie = "oldUN =" + getCookie("username")
        document.cookie = "username = test";
    }
    else
    {
        signOutFunc()
        externalSignInFunc(getCookie("oldUN"))
    }


        document.location.reload()}'>Try</button>
    <hr class="break" />-->

    <h2>Home</h2>
    <asp:Label runat="server" ID="WelcomeLabel"></asp:Label>
    
    <!--
    <div style="position:fixed; top:5px; right:2px; z-index:99999">
    <uc1:AccountTiny runat="server" id="AccountTiny" />
        </div>
    -->
    <!--LATEST VIDEO START-->
    <hr class="break" />
    <div id="latestVideo" style="width:100%"></div>

    <!--LATEST VIDEO END-->
    <!--DEVICE DETECT START-->

    <h3>Should show what device you're using lol</h3>
    <button class="menu" onclick="{alert('You are using ' + deviceDetect()) }">Device</button>

    <hr class="break" />

    <!--DEVICE DETECT END-->
    <!--SCRIPTS-->
    <script src="../javascript/globalBodyV1.6.js"></script>
    <script src="../javascript/menuV1.14.js"></script>
    <script>
        if(isMobile())
        {
            var body = document.getElementById("body")
            body.style = "margin-top:5%"
        }
    </script>
</body>
</html>
