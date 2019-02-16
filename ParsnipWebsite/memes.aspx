<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="memes.aspx.cs" Inherits="ParsnipWebsite.memes" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link id="link_style" rel="stylesheet" type="text/css" />

    <!--
    <link rel="stylesheet" type="text/css" href="../css/old/style.css" />-->
    <link rel="stylesheet" type="text/css" href="../css/style.css" />

    
    <link href="css/fileupload.css" rel="stylesheet" type ="text/css" />
    <title>Memes</title>
    <style>
        image
        {
            max-width:500px;
        }
    </style>

    <script src="javascript/intersection-observer.js"></script>
</head>
<body class="fade0p5" id="body">
    <label class="censored" id="pageId">memes.html</label>

    <!--FOR JS DYNAMIC PAGE CREATION DO NOT MOVE START-->

    <div id="titleAndMenu"></div>
    <div id="menuDiv"></div>

    <!--FOR JS DYNAMIC PAGE CREATION DO NOT MOVE END-->
    <!--<br class="nomobile" />-->
    <br /><br />
    
    <h2>DANK Memes</h2>
    I'm trying to make an archive of all our memes. Send them to benbartontablet@gmail.com :P
    
    <div>Loldred you definitely have the most (and the dankest) memes :P</div>
    <hr class="break" />

    <form runat="server">

                <div runat="server" id="UploadDiv" class="form-group" style="display:none">
                    <label class="file-upload">
                        
                        <span><strong>Upload Meme</strong></span>
                        <asp:FileUpload ID="PhotoUpload" runat="server" class="form-control-file" onchange="this.form.submit()" />
                    </label>

                </div>
                <br />
                <div runat="server" id="DynamicPhotosDiv">

                </div>
            </form>

    <div>
        <h3>Loldred back in the good old days (of abuse)</h3>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/Loldred_Abuse.JPG" />
        <hr class="break" />
    </div>

    <div>
        <h3>How to spot a Fuckboy *cough* Kieron *cough*</h3>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/kieron_how_to_spot_a_fuckboy.JPG" />
        <hr class="break" />
    </div>

    <div>
    <h3>Ben get's so much pussy!</h3>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/ben_pussy.JPG" />
        <hr class="break" />
    </div>
    
    
    <div>
        <h3>Loldred's Drinking Problems Carlsberg</h3>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/loldred_carlsberg.JPG" />
        <hr class="break" />
    </div>

    <div>
        <h3>It can't have been easy...</h3>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/loldred_control_yourself.JPG" />
        <hr class="break" />
    </div>

    <div>
        <h3>When you get a free house...</h3>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/loldred_free_house.JPG" />
        <hr class="break" />
    </div>

    <div>
        <h3>Loldred's grooming techniques are on point guys</h3>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/loldred_lawnmowers.JPG" />
        <hr class="break" />
    </div>

    <div>
        <h3>Loldred Shag Me</h3>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/loldred_shag_me.JPG" />
        <hr class="break" />
    </div>

    <div>
        <h3>The similarities are uncanny...</h3>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/marshy_rat.JPG" />
        <hr class="break" />
    </div>

    <div>
        <h3>Mason The Pimp</h3>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/mason_pimp.JPG" />
        <hr class="break" />
    </div>

    <div>
        <h3>Tom is (ligitimately) done</h3>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/tom_done.JPG" />
        <hr class="break" />
    </div>
   

    <!--<div>
        <h3>Kieron with BAE</h3>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/kieron_with_BAE.JPG" />
        <hr class="break" />
    </div>-->

    <div>
        <h3>Age is just a number eh kids?</h3>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/loldred_age_is_just_a_number.JPG" />
        <hr class="break" />
    </div>

    <div>
        <h3>Loldred Approves</h3>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/loldred_approves.JPG" />
        <hr class="break" />
    </div>  

    <div>
        <h3>At Least I can Pull</h3>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/loldred_at_least_I_can_pull.JPG" />
        <hr class="break" />
    </div>
       

    <script src="../javascript/globalBodyV1.6.js"></script>
    <script src="../javascript/menuV1.14.js"></script>
    <script>
                if (isMobile()) {
                    var body = document.getElementById("body")
                    body.style = "margin-top:5%"


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
