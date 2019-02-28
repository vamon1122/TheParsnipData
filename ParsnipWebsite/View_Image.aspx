<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="View_Image.aspx.cs" Inherits="ParsnipWebsite.View_Image" %>

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

    <script src="Javascript/Intersection_Observer.js"></script>

    <style>
        .width100{
            width:100%;
        }
    </style>

    <title>New Title</title>
</head>
<body class="fade0p5" id="body" style="text-align:center">
    <label class="censored" id="pageId">Edit-Photo.html</label>

    <!--FOR JS DYNAMIC PAGE CREATION DO NOT MOVE START-->

    <div id="titleAndMenu"></div>
    <div id="menuDiv"></div>

    <!--FOR JS DYNAMIC PAGE CREATION DO NOT MOVE END-->
  
    

    
        <!--
  <div class="form-group">
    <label>Description</label>
    <input type="text" class="form-control login" id="InputDescription" />
  </div>
        -->
    <div class="center_form">
        <div class="input-group mb-3" style="padding-left:5%; padding-right:5%">
  <div class="input-group-prepend">
    <span class="input-group-text" id="inputGroup-sizing-default">Link</span>
  </div>
  <input type="text" id="ShareLink" class="form-control" onclick="this.setSelectionRange(0, this.value.length)" />
</div>
    <h2 id="ImageTitle"></h2>

    <form id="form1" runat="server">
        <asp:Image runat="server" ID="ImagePreview" CssClass="width100" />
    </form>

        
    
        </div>
        <script>

            var url_string = window.location.href
            url = new URL(url_string);
            document.getElementById("ImageTitle").innerHTML = url.searchParams.get("title");

            document.getElementById("ShareLink").value = "https://www.theparsnip.co.uk/photos?imageid=" + url.searchParams.get("imageid");
    </script>
        
    <script src="../Javascript/Menu.js"></script>

    
    
</body>
</html>


