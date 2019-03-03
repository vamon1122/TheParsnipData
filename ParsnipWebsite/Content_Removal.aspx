<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Content_Removal.aspx.cs" Inherits="ParsnipWebsite.Content_Removal" %>
<%@ Register Src="~/Custom_Controls/Menu/Menu.ascx" TagPrefix="menuControls" TagName="Menu" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
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

    <script src="Javascript/Intersection_Observer.js"></script>

    <title>New Title</title>
</head>
<body class="fade0p5" id="body" style="text-align:center">
    <menuControls:Menu runat="server" ID="Menu" />

    <div class="padded-text">
        In order to make the website enjoyable for everyone, I've decided to take content down on request. As much as the website is just
        meant to be a bit of fun, I understand that jokes can be taken too far and that some people may not appreciate certain things being
        on the website for very legitimate personal reasons. I'll make my best efforts to notify you if anything controversial might be
        uploaded onto the website and I will ofcourse take down anything that you deem to be offensive. Please be reasonable with your requests
        and understand if I can't act immediately.
    </div>
    <script src="../Javascript/Useful_Functions.js"></script>
    <script>
        if (isMobile()) {
            /*var body = document.getElementById("body")
            body.style = "margin-top:10%"*/


        }
        else {
            var main = document.getElementById("main")

            //main.style = "width:20%; left:60%; background-color:red"
    }
    </script>

</body >
</html>
