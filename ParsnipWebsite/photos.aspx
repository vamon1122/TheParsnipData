<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="photos.aspx.cs" Inherits="ParsnipWebsite.photos" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    
    <link id="link_style" rel="stylesheet" type="text/css" />

    <!--
    <link rel="stylesheet" type="text/css" href="../css/old/style.css" />-->
    <link rel="stylesheet" type="text/css" href="../css/style.css" />
        
        
    <title>Photos</title>
    <style>
        image
        {
            max-width:500px;
        }
    </style>

    <script src="javascript/intersection-observer.js"></script>
</head>
<body class="fade0p5" id="body">
    <label class="censored" id="pageId">photos.html</label>

    <!--FOR JS DYNAMIC PAGE CREATION DO NOT MOVE START-->

    <div id="titleAndMenu"></div>
    <div id="menuDiv"></div>

    <!--FOR JS DYNAMIC PAGE CREATION DO NOT MOVE END-->
    <br /><br />
    <br class="nomobile" />

    <div  class="container" style="width=100%">
        <div class="center_div">
            <div class="cens_req"><label>Certain elements of this page were removed by request. </label><a href="content removal.html">Click here</a><label> to learn more.</label></div>
    
            <h2>Photos</h2>
            <hr class="break" />
            <a href ="https://photos.app.goo.gl/GgkSHa8FvichfXRZ7">

            <img src="resources/media/images/webMedia/google-photos.svg" width="100" />
            </a>
            <br />
            <label>Click <a href="https://photos.app.goo.gl/GgkSHa8FvichfXRZ7">here</a> to view 'The Parsnip Collection' on google photos</label>
            <hr class="break" />
        
            <form runat="server">

                <div runat="server" id="UploadDiv" class="form-group" style="display:none">
                    <label for="exampleFormControlFile1">Upload A Photo</label>
                    <asp:FileUpload ID="PhotoUpload" runat="server" class="form-control-file" />
                    <asp:Button runat="server" ID="BtnUpload" OnClick="BtnUpload_Click" CssClass="btn btn-primary" Text="Upload"></asp:Button>
                    <asp:Button runat="server" ID="BtnDeleteUploads" OnClick="BtnDeleteUploads_Click" CssClass="btn btn-primary" Text="Delete"></asp:Button>

                </div>

                <div runat="server" id="DynamicPhotosDiv">

                </div>
            </form>
        </div>
    </div>
    
    
            <script src="../javascript/globalBodyV1.6.js"></script>
            <script src="../javascript/menuV1.13.js"></script>
            <script>
                if (isMobile()) {
                    var body = document.getElementById("body")
                    body.style = "margin-top:10%"


                }
                else
                {
                    var main = document.getElementById("main")
           
                    //main.style = "width:20%; left:60%; background-color:red"
                }

                document.addEventListener("DOMContentLoaded", function ()
                {
                    var lazyImages = [].slice.call(document.querySelectorAll("img.lazy"));

                    if ("IntersectionObserver" in window)
                    {
                        let lazyImageObserver = new IntersectionObserver(function (entries, observer)
                        {
                            entries.forEach(function (entry)
                            {
                                if (entry.isIntersecting)
                                {
                                    let lazyImage = entry.target;
                                    lazyImage.src = lazyImage.dataset.src;
                                    lazyImage.srcset = lazyImage.dataset.srcset;
                                    lazyImage.classList.remove("lazy");
                                    lazyImageObserver.unobserve(lazyImage);
                                }
                            });
                        });

                        lazyImages.forEach(function (lazyImage) {
                            lazyImageObserver.observe(lazyImage);
                        });
                    }
                    else
                    {
                        //I used javascript/intersection-observer as a fallback
                    }
                });

       
            </script>
</body>
</html>
