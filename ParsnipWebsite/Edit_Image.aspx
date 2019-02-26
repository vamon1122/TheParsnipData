<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit_Image.aspx.cs" Inherits="ParsnipWebsite.Edit_Image" %>

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

    <title>Edit Image</title>
</head>
<body class="fade0p5" id="body" style="text-align:center" >
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
    <form id="form1" runat="server" defaultbutton="ButtonSave">
        <div class="form-group" style="padding-left:5%; padding-right: 5%;" >
      <label style="text-align:left; width:100%">Title</label>
            <asp:TextBox CssClass="form-control" runat="server" ID="InputTitleTwo" />
  </div>

        
        <div runat="server" id="DropDownDiv" visible="false">
        <label>Select an album:</label>
            <asp:DropDownList ID="NewAlbumsDropDown" runat="server" AutoPostBack="True" CssClass="form-control" 
                onselectedindexchanged="SelectAlbum_Changed">
            </asp:DropDownList>
            <br />
        </div>


        <asp:Image runat="server" ID="ImagePreview" CssClass="image-preview" Width="100%" />
        <br />
        <br />
        <div style="width:100%; padding-left:5%; padding-right:5%">
            <asp:Button runat="server" ID="ButtonSave" class="btn btn-primary float-right" Text="Save" Width="100px" OnClick="ButtonSave_Click"></asp:Button>
        <asp:Button runat="server" ID="btn_AdminDelete"  CssClass="btn btn-primary float-left" Width="100px" Text="Delete" Visible="false" data-toggle="modal" data-target="#confirmDelete" OnClientClick="return false;"></asp:Button>
        </div>

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
      </div>
    <!--    
    <script>
            
            var url_string = window.location.href
            url = new URL(url_string);
            document.getElementById("InputTitle").value = url.searchParams.get("title");


        function SavePhoto() {
            var url_string = window.location.href
            var url;
            var redirect = "edit_image?"
            var InputTitle = document.getElementById("InputTitle").value;

            try {
                //More efficient but does not work on older browsers
                url = new URL(url_string);
                redirect += "redirect=" + url.searchParams.get("redirect") + "&imageid=" + url.searchParams.get("imageid")+ "&title=" + InputTitle + "&save=true";
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
        -->
        
    
    <script src="../Javascript/Menu.js"></script>
    
</body>
</html>

