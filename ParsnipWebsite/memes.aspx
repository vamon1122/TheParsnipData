<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Memes.aspx.cs" Inherits="ParsnipWebsite.Memes" %>

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
    
    <title>Memes</title>
</head>
<body class="fade0p5" id="body">
    <label class="censored" id="pageId">memes.html</label>

    <!--FOR JS DYNAMIC PAGE CREATION DO NOT MOVE START-->

    <div id="titleAndMenu"></div>
    <div id="menuDiv"></div>

    <!--FOR JS DYNAMIC PAGE CREATION DO NOT MOVE END-->
    <!--<br class="nomobile" />-->
    
    <h2>Memes</h2>
    <hr class="break" />

    <form runat="server">
        <div class="alert alert-warning alert-dismissible" runat="server" style="display:none; position:fixed; top:55px; width:98%; margin-left:1%;" id="Warning">

                 <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                <strong>Access Denied</strong> You cannot edit photos which other people have uploaded!
            </div>
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
        <div class="padded-text">
        <h3>Loldred back in the good old days (of abuse)</h3>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/Loldred_Abuse.JPG" />
        <hr class="break" />
    </div>

    <div>
        <div class="padded-text">
        <h3>How to spot a Fuckboy *cough* Kieron *cough*</h3>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/kieron_how_to_spot_a_fuckboy.JPG" />
        <hr class="break" />
    </div>

    <div>
        <div class="padded-text">
    <h3>Ben get's so much pussy!</h3>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/ben_pussy.JPG" />
        <hr class="break" />
    </div>
    
    
    <div>
        <div class="padded-text">
        <h3>Loldred's Drinking Problems Carlsberg</h3>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/loldred_carlsberg.JPG" />
        <hr class="break" />
    </div>

    <div>
        <div class="padded-text">
        <h3>It can't have been easy...</h3>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/loldred_control_yourself.JPG" />
        <hr class="break" />
    </div>

    <div>
        <div class="padded-text">
        <h3>When you get a free house...</h3>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/loldred_free_house.JPG" />
        <hr class="break" />
    </div>

    <div>
        <div class="padded-text">
        <h3>Loldred's grooming techniques are on point guys</h3>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/loldred_lawnmowers.JPG" />
        <hr class="break" />
    </div>

    <div>
        <div class="padded-text">
        <h3>Loldred Shag Me</h3>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/loldred_shag_me.JPG" />
        <hr class="break" />
    </div>

    <div>
        <div class="padded-text">
        <h3>The similarities are uncanny...</h3>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/marshy_rat.JPG" />
        <hr class="break" />
    </div>

    <div>
        <div class="padded-text">
        <h3>Mason The Pimp</h3>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/mason_pimp.JPG" />
        <hr class="break" />
    </div>

    <div>
        <div class="padded-text">
        <h3>Tom is (ligitimately) done</h3>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/tom_done.JPG" />
        <hr class="break" />
    </div>
   

    <div>
        <div class="padded-text">
        <h3>Kieron with BAE</h3>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/kieron_with_BAE.JPG" />
        <hr class="break" />
    </div>

    <div>
        <div class="padded-text">
        <h3>Age is just a number eh kids?</h3>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/loldred_age_is_just_a_number.JPG" />
        <hr class="break" />
    </div>

    <div>
        <div class="padded-text">
        <h3>Loldred Approves</h3>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/loldred_approves.JPG" />
        <hr class="break" />
    </div>  

    <div>
        <div class="padded-text">
        <h3>At Least I can Pull</h3>
            </div>
        <img class="meme" src="http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477058839/Photos/memes/loldred_at_least_I_can_pull.JPG" />
        <hr class="break" />
    </div>
       

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

