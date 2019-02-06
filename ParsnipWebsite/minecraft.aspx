﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="minecraft.aspx.cs" Inherits="ParsnipWebsite.minecraft" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="../css/old/style.css" />
    <link rel="stylesheet" type="text/css" href="../css/style.css" />
    <title>Minecraft</title>
</head>
<body class="fade0p5" id="body">
    <label class="censored" id="pageId">minecraft.html</label>

    <!--FOR JS DYNAMIC PAGE CREATION DO NOT MOVE START-->

    <div id="titleAndMenu"></div>
    <div id="menuDiv"></div>

    <!--FOR JS DYNAMIC PAGE CREATION DO NOT MOVE END-->
    <br /><br />
    <h1>IP: mc.theparsnip.co.uk</h1>

    <script src="../javascript/globalBodyV1.6.js"></script>
    <script src="../javascript/menuV1.13.js"></script>
    <script>
        if(isMobile())
        {
            var body = document.getElementById("body")
            body.style = "margin-top:10%"
        }
    </script>
</body>
</html>
