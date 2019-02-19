<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit-Image.aspx.cs" Inherits="ParsnipWebsite.Edit_Image" %>

<head runat="server">
    <!-- BOOTSTRAP START -->
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.2/css/bootstrap.min.css" integrity="sha384-Smlep5jCw/wG7hdkwQ/Z5nLIefveQRIY9nfy6xoR1uRYBtpZgI6339F5dgvm/e9B" crossorigin="anonymous" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.1.2/js/bootstrap.min.js" integrity="sha384-o+RDsa0aLu++PJvFqy8fFScvbHFLtbvScb8AjopnFD+iEQ7wo/CG0xlczd+2O/em" crossorigin="anonymous"></script>
    <!-- BOOTSTRAP END -->

    <link id="link_style" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="css/shared-style.css" />

    <script src="javascript/intersection-observer.js"></script>

    <title>New Title</title>
</head>
<body class="fade0p5" id="body">
    <label class="censored" id="pageId">Edit-Photo.html</label>

    <!--FOR JS DYNAMIC PAGE CREATION DO NOT MOVE START-->

    <div id="titleAndMenu"></div>
    <div id="menuDiv"></div>

    <!--FOR JS DYNAMIC PAGE CREATION DO NOT MOVE END-->

    <div style="position:absolute; left: 50%; margin-left:-150px;">
                
  <div class="form-group" style="width:300px"  >
    
          
      <br />    
      <label>Title</label>
    <input type="text" class="form-control login" id="InputTitle"  />
  </div>
        <!--
  <div class="form-group">
    <label>Description</label>
    <input type="text" class="form-control login" id="InputDescription" />
  </div>
        -->
            <br />
  


    <form id="form1" runat="server">
        <asp:Image runat="server" ID="ImagePreview" />
        <br />
        <asp:Button runat="server" ID="btn_AdminDelete"  CssClass="btn btn-primary" Text="Delete" Visible="false" data-toggle="modal" data-target="#confirmDelete" OnClientClick="return false;"></asp:Button>
        

        <!-- Modal -->
        <div class="modal fade" id="confirmDelete" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="exampleModalLongTitle">Confirm Delete</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        Are you sure that you want to this photo from all albums?
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                        <asp:Button ID="BtnDeleteImage" runat="server" class="btn btn-primary" OnClick="BtnDeleteImage_Click" Text="Confirm"></asp:Button>
                    </div>
                </div>
            </div>
        </div>
    </form>
        <button id="ButtonSave" class="btn btn-primary" onclick="SavePhoto()">Save</button>

    
        
        
    </div>
    <script src="../javascript/globalBodyV1.6.js"></script>
    <script src="../javascript/menuV1.14.js"></script>
    <script>
        function SavePhoto() {
            var url_string = window.location.href
            var url;
            var redirect = "edit-image?"
            var InputTitle = document.getElementById("InputTitle").value;

            try {
                //More efficient but does not work on older browsers
                url = new URL(url_string);
                redirect += "redirect=" + url.searchParams.get("redirect") + "&imageid=" + url.searchParams.get("imageid")+ "&title=" + InputTitle;
            }
            catch (e) {
                //More compatible method
                url = window.location.href;
                redirect += "imageid=" + url.split('=')[url.split('=').length - 1];
            }

            if (redirect === "" || redirect === null) {
                redirect = "home?error=true";
            }

            //Use window.location.replace if possible because 
            //you don't want the current url to appear in the 
            //history, or to show - up when using the back button.
            try { window.location.replace(redirect); }
            catch (e) { window.location = redirect; }
        }
    </script>
</body>
</html>

