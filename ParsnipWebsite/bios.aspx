﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="bios.aspx.cs" Inherits="ParsnipWebsite.bios" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="../css/old/style.css" />
    <link rel="stylesheet" type="text/css" href="../css/style.css" />
    <title>Bios</title>
</head>
<body class="fade0p5" id="body">
    <label class="censored" id="pageId">bios.html</label>

    <!--FOR JS DYNAMIC PAGE CREATION DO NOT MOVE START-->

    <div id="titleAndMenu"></div>
    <div id="menuDiv"></div>

    <!--FOR JS DYNAMIC PAGE CREATION DO NOT MOVE END-->
    <br /><br />
    <br class="nomobile" />
    <div class="cens_req"><label>Certain elements of this page were removed by request. </label><a href="content removal.html">Click here</a><label> to learn more.</label></div>
    

    <h2>Bios!!!</h2>
    <h3>All credit goes to Kieron 'Gaz Beadle' Howarth</h3>
    
    <img class="censored" id="kieron" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477059052/Photos/Bios/Fat Kieron.JPG" style="width:300px" />
    <hr class="break" />
    <br />

    <div>
        Kieron - Angel <br />
        Ben - Angel (I assume you didn't aim this at me Kieron, I appreciate it :P)<br />
        Loldred - Cunt<br />
        Marshy - Cunt<br />
        Aaron - Cunt<br />
        Raul - Cunt<br />
        Tom - Cunt<br />
        Dan - Cunt<br />
        Mason - Cunt<br />
        <br />
        Source below \/<br />
    </div>
    <br />
    <img src="resources/media/images/photos/Photos/kieron_chat.png" id="Kieron_chat" />
    <br />
    <br />
    <script src="../javascript/globalBodyV1.6.js"></script>
    <script src="../javascript/menuV1.13.js"></script>
    <script>
        var chat = document.getElementById("Kieron_chat")
        var kieron = document.getElementById("kieron")
        if(isMobile())
        {
            var body = document.getElementById("body")
            body.style = "margin-top:10%"
            chat.style.width = "90%"
            kieron.style.width = "25%"
        }
        else
        {
            chat.style.width = "20%"
            kieron.style.width = "5%"
        }
    </script>
</body>
</html>
