<%@ Page Async="true" Language="C#" AutoEventWireup="true" CodeBehind="View_Image.aspx.cs" Inherits="ParsnipWebsite.View_Image" %>
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

    <script src="Javascript/Intersection_Observer.js"></script>

    <style>
        .width100{
            width:100%;
        }
    </style>

    <title>View Image</title>
</head>
<body class="fade0p5" id="body" style="text-align:center">
    <menuControls:Menu runat="server" ID="Menu" />

    <div class="center_form">
        <div class="input-group mb-3" style="padding-left:5%; padding-right:5%">
  <div class="input-group-prepend">
    <span class="input-group-text" id="inputGroup-sizing-default">Link</span>
  </div>
  <input type="text" id="ShareLink" class="form-control" onclick="this.setSelectionRange(0, this.value.length)" />
</div>
    <h2 id="ImageTitle"></h2>
    
    <form id="form1" runat="server">
        <asp:Label runat="server" id="Album"></asp:Label>
        <asp:Label runat="server" ID="AlbumId" Visible="false"></asp:Label>
        <asp:Image runat="server" ID="ImagePreview" CssClass="width100" />
    </form>
        <br />
        <button class="btn btn-primary" onclick="ViewInAlbum()">View In Album</button>
        
    
        </div>
        <script>

            var url_string = window.location.href
            url = new URL(url_string);
            document.getElementById("ImageTitle").innerHTML = url.searchParams.get("title");

            document.getElementById("ShareLink").value = "https://www.theparsnip.co.uk/view_image?imageid=" + url.searchParams.get("imageid");
    </script>

    <script>
        function ViewInAlbum() {
            var url_string = window.location.href
            url = new URL(url_string);

            var albumUrl = document.getElementById("Album").innerHTML.toLowerCase();

            /*
            var albumId = document.getElementById("AlbumId").innerHTML.toUpperCase();
            switch (albumId) {
                case "4B4E450A-2311-4400-AB66-9F7546F44f4E":
                    albumUrl = "photos";
                    break;
                case "5F15861A-689C-482A-8E31-2F13429C36E5":
                    albumUrl = "memes";
                    break;
                default:
                    albumId = "errorAlbum";
                    break;
            }
            */
            //Href doesn't delete this page from the history wheras redirect would.
            window.location.href = "https://www.theparsnip.co.uk/"+ albumUrl +"?imageid=" + url.searchParams.get("imageid");
        }
    </script>

    
    
</body>
</html>


