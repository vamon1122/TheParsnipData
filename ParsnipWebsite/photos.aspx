<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Photos.aspx.cs" Inherits="ParsnipWebsite.Photos" %>

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
    <link rel="stylesheet" type="text/css" href="Css/Shared_Style.css" />

    <script src="Javascript/Intersection_Observer.js"></script>

    <title>Photos</title>
</head>
<body class="fade0p5" id="body">
    <label class="censored" id="pageId">photos.html</label>

    <!--FOR JS DYNAMIC PAGE CREATION DO NOT MOVE START-->

    <div id="titleAndMenu"></div>
    <div id="menuDiv"></div>

    <!--FOR JS DYNAMIC PAGE CREATION DO NOT MOVE END-->

    
            <div class="cens_req padded-text"><label>Certain elements of this page were removed by request. <a href="Content_Removal.html">Click here</a> to learn more.</label></div>
    
            <h2>Photos</h2>
            <hr class="break" />
            <a href ="https://photos.app.goo.gl/GgkSHa8FvichfXRZ7">

            <img src="Resources/Media/Images/Web_Media/google-photos.svg" width="100" />
            </a>

            <br />
    <div class="padded-text">
            <label>Click <a href="https://photos.app.goo.gl/GgkSHa8FvichfXRZ7">here</a> to view 'The Parsnip Collection' on google photos. This includes most of the photos on this page and was curated by Tom.</label>
        </div>
            <hr class="break" />
        
            <form runat="server">
        <div class="alert alert-warning alert-dismissible" runat="server" style="display:none; position:fixed; top:55px; width:98%; margin-left:1%;" id="Warning">

                 <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                <strong>Access Denied</strong> You cannot edit photos which other people have uploaded!
            </div>
                <div runat="server" id="UploadDiv" class="form-group" style="display:none; ">
                    <label class="file-upload btn">
                        
                        <span><strong>Upload Photo</strong></span>
                        <asp:FileUpload ID="PhotoUpload" runat="server" class="form-control-file" onchange="this.form.submit()" />
                    </label>

                </div>
                <br />
                <div runat="server" id="DynamicPhotosDiv">

                </div>
            </form>
    
    
            <script src="../Javascript/Useful_Functions.js"></script>
            <script src="../Javascript/Menu.js"></script>
            <script>

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
                        //I used Javascript/intersection-observer as a fallback
                    }
                });

       
            </script>
</body>
</html>
