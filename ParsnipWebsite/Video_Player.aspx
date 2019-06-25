<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Video_Player.aspx.cs" Inherits="ParsnipWebsite.Video_Player" %>

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

    <title>Video Player</title>
</head>
<body class="fade0p5" id="body" style="text-align:center">
    <menuControls:Menu runat="server" ID="Menu" />
    <div runat="server" id="ShareLinkContainer" class="input-group mb-3" style="padding-left:5%; padding-right:5%">
  <div class="input-group-prepend">
    <span class="input-group-text" id="inputGroup-sizing-default">Link</span>
  </div>
  <input runat="server" type="text" id="ShareLink" class="form-control" onclick="this.setSelectionRange(0, this.value.length)" />
</div>
    <div runat="server" class="alert alert-danger alert-dismissible parsnip-alert" Visible="false" id="NotExistError">
        <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
        <strong>Error:</strong> Could not access image. The image which you are trying to access has been deleted or the link which you are using has expired!
    </div>
    <h2 runat="server" id="VideoTitle"></h2>
        <video runat="server" id="video_container" controls="controls" style="width:100%; max-width:1280px">
            <source runat="server" id="VideoSource" type="video/mp4" />
            Your browser does not support HTML5 video.
        </video>
    <div runat="server" id="youtube_video_container" class="youtube-container" visible="false">
        <div runat="server" id="youtube_video" class="youtube-player"></div>
        
    </div>
    <form id="form1" runat="server">
        <div style="padding-left:2.5%; padding-right:2.5%">
        <asp:Button runat="server" ID="Button_ViewAlbum" class="btn btn-info btn-lg btn-block" Text="CLICK for more like this!" OnClick="Button_ViewAlbum_Click"></asp:Button>
            </div>
    </form>
    <script>
        (function () {
    var v = document.getElementsByClassName("youtube-player");
    for (var n = 0; n < v.length; n++) {
        var p = document.createElement("div");
        p.innerHTML = labnolThumb(v[n].dataset.id);
        p.onclick = labnolIframe;
        v[n].appendChild(p);

            }})();

            function labnolThumb(id) {
    return '<img class="youtube-thumb" src="//i.ytimg.com/vi/' + id + '/hqdefault.jpg"><div class="play-button"></div>';
}



function labnolIframe() {
    var iframe = document.createElement("iframe");
    iframe.setAttribute("src", "//www.youtube.com/embed/" + this.parentNode.dataset.id + "?autoplay=1&autohide=2&border=0&wmode=opaque&enablejsapi=1&controls=0&showinfo=0");
    iframe.setAttribute("frameborder", "0");
    iframe.setAttribute("id", "youtube-iframe");
    this.parentNode.replaceChild(iframe, this);
}

    


    </script>
</body>
</html>
