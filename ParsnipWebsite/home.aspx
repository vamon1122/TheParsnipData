<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="ParsnipWebsite.Home" %>
<%@ Register Src="~/Custom_Controls/Menu/Menu.ascx" TagPrefix="menuControls" TagName="Menu" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- BOOTSTRAP START -->
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.2/css/bootstrap.min.css" integrity="sha384-Smlep5jCw/wG7hdkwQ/Z5nLIefveQRIY9nfy6xoR1uRYBtpZgI6339F5dgvm/e9B" crossorigin="anonymous" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.1.2/js/bootstrap.min.js" integrity="sha384-o+RDsa0aLu++PJvFqy8fFScvbHFLtbvScb8AjopnFD+iEQ7wo/CG0xlczd+2O/em" crossorigin="anonymous"></script>
    <!-- BOOTSTRAP END -->

    <script src="../Javascript/Useful_Functions.js"></script>
    <link id="link_style" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="Css/Shared_Style.css" />
    <script src="../Javascript/Apply_Style.js"></script>
    <title>Home</title>
</head>
<body class="fade0p5" id="body" style="text-align:center">
    <label class="censored" id="pageId">home.html</label>
    
    <!--FOR JS DYNAMIC PAGE CREATION DO NOT MOVE START-->

    

    <!--FOR JS DYNAMIC PAGE CREATION DO NOT MOVE END-->
    
    
        <menuControls:Menu runat="server" ID="Menu" />

    <h2>Home</h2>
    <div class="padded-text center_div">
        <asp:Label runat="server" ID="WelcomeLabel"></asp:Label>
            <br />
        <br />
        <h3>*NEW*</h3>
        - 'Remember password' now <b><i>actually</i></b> works! You don't have to enter your password every bloody time anymore :P<br />
        - You can now upload & caption your very own <a href="photos">photos</a> & <a href="memes">memes</a>!<br />
        - Shortcut to the <a href="https://www.mixcloud.com/afternoontlive/">AfternoonT</a> podcast added to website menu. <br />
    </div>
    <hr class="break" />
    <h3>LATEST VIDEO: Aaron Gets Wavy</h3>
    <div class="cincopa-container">
        <div id="cp_widget_ec1e351e-1fd5-4c76-94f4-8abb144b50c7">Loading...</div>
        <script type="text/javascript">
            var cpo = []; cpo["_object"] ="cp_widget_ec1e351e-1fd5-4c76-94f4-8abb144b50c7"; cpo["_fid"] = "AAHA-Y-m0XgX";
            var _cpmp = _cpmp || []; _cpmp.push(cpo);
            (function() { var cp = document.createElement("script"); cp.type = "text/javascript";
            cp.async = true; cp.src = "//www.cincopa.com/media-platform/runtime/libasync.js";
            var c = document.getElementsByTagName("script")[0];
                c.parentNode.insertBefore(cp, c);
            })(); 
        </script>
        <noscript>
            <span>New Gallery 2019/2/8</span>
            <span>Aaron Gets Wavy</span>
            <span>bitrate</span>
            <span> 4364 kb/s</span>
            <span>height</span>
            <span> 720</span>
            <span>duration</span>
            <span> 00:05:22.28</span>
            <span>fps</span>
            <span> 23.98</span>
            <span>width</span>
            <span> 1280</span>
            <span>originaldate</span>
            <span> 2019-02-08T12:32:36.000000Z</span>
        </noscript>
    </div>

    <hr class="break" />

    <!--DEVICE DETECT END-->
    <!--SCRIPTS-->
</body>
</html>
