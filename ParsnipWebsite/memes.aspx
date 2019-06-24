﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Memes.aspx.cs" Inherits="ParsnipWebsite.Memes" %>
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
    
    <title>Memes</title>
</head>
<body class="fade0p5" id="body" style="text-align:center">
    <menuControls:Menu runat="server" ID="Menu" />
    
    <div class="alert alert-warning alert-dismissible parsnip-alert" style="display: none;" id="AccessWarning">
        <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
        <strong>Access Denied</strong> You cannot edit memes which other people have uploaded!
    </div>
    <div class="alert alert-danger alert-dismissible parsnip-alert" style="display: none;" id="VideoError">
        <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
        <strong>Upload Error</strong> You cannot upload videos to the memes page!
    </div>

    <h2>Memes</h2>
    <hr class="break" />

    <form runat="server">
                <div runat="server" id="UploadDiv" class="form-group" style="display:none">
                    <label class="file-upload">
                        
                        <span><strong>Upload Meme</strong></span>
                        <asp:FileUpload ID="PhotoUpload" runat="server" class="form-control-file" onchange="this.form.submit()" />
                    </label>

                </div>
                <br />
                <div runat="server" id="DynamicMemesDiv">

                </div>
            </form>

    <div>
        <div class="padded-text">
        <h2>Loldred abuses Mason</h2>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/Loldred_Abuse.JPG" />
        <hr class="break" />
    </div>

    <div>
        <div class="padded-text">
        <h2>How to spot a fuckboy</h2>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/kieron_how_to_spot_a_fuckboy.JPG" />
        <hr class="break" />
    </div>

    <!--
    <div>

        <div class="padded-text">
    <h2>Ben get's so much pussy!</h2>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/ben_pussy.JPG" />
        <hr class="break" />
    </div>
    -->
    <!--
    <div>
        <div class="padded-text">
        <h2>Loldred Carlsberg</h2>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/loldred_carlsberg.JPG" />
        <hr class="break" />
    </div>
    -->
    <!--
    <div>
        <div class="padded-text">
        <h2>It can't have been easy...</h2>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/loldred_control_yourself.JPG" />
        <hr class="break" />
    </div>
    -->
    <div>
        <div class="padded-text">
        <h2>WHEN BAE HAS A FREE HOUSE</h2>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/loldred_free_house.JPG" />
        <hr class="break" />
    </div>

    <div>
        <div class="padded-text">
        <h2>DID SOMEONE SAY FREE LAWNMOWERS</h2>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/loldred_lawnmowers.JPG" />
        <hr class="break" />
    </div>

    <div>
        <div class="padded-text">
        <h2>Did someone say cheeky shag?</h2>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/loldred_shag_me.JPG" />
        <hr class="break" />
    </div>

    <div>
        <div class="padded-text">
        <h2>Sid from Flushed Away</h2>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/marshy_rat.JPG" />
        <hr class="break" />
    </div>

    <div>
        <div class="padded-text">
        <h2>Pimp</h2>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/mason_pimp.JPG" />
        <hr class="break" />
    </div>

    <div>
        <div class="padded-text">
        <h2>Toms fucking done</h2>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/tom_done.JPG" />
        <hr class="break" />
    </div>
   

    <div>
        <div class="padded-text">
        <h2>Kieron with BAE</h2>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/kieron_with_BAE.JPG" />
        <hr class="break" />
    </div>

    <div>
        <div class="padded-text">
        <h2>Age is just a number</h2>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/loldred_age_is_just_a_number.JPG" />
        <hr class="break" />
    </div>

    <div>
        <div class="padded-text">
        <h2>Loldred approves!</h2>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/loldred_approves.JPG" />
        <hr class="break" />
    </div>  

    <!--
    <div>
        <div class="padded-text">
        <h2>At Least I can Pull</h2>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/loldred_at_least_I_can_pull.JPG" />
        <hr class="break" />
    </div>
    -->
    <script src="../Javascript/Focus_Image.js"></script>
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

    <script>
        var url_string = window.location.href
        var url = new URL(url_string);
        var error = url.searchParams.get("error");
        if (error !== "" && error !== null)
        {
            if (error === "video") {
                document.getElementById("VideoError").style = "display:block";
            }
            else {
                document.getElementById("AccessWarning").style = "display:block";
            }
        }
    </script>
</body>

</html>

