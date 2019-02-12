﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="home.aspx.cs" Inherits="ParsnipWebsite.Home" %>
<%@ Register Src="~/CustomControls/AccountTiny.ascx" TagPrefix="uc1" TagName="AccountTiny" %>



<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    
    <link id="link_style" rel="stylesheet" type="text/css" />

    <!--
    <link rel="stylesheet" type="text/css" href="../css/old/style.css" />-->
    <link rel="stylesheet" type="text/css" href="../css/style.css" />
        
    <link rel="stylesheet" type="text/css" href="css/parsnipStyle.css" />
        
    <title>Home</title>
    <link rel="stylesheet" type="text/css" href="css/parsnipStyle.css" />
    
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
    <div id="latestVideo" style="width:97.5%"></div>

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
            body.style = "margin-top:10%"
        }
    </script>
</body>
</html>
