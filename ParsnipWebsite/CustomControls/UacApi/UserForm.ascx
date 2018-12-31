<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserForm.ascx.cs" Inherits="TheParsnipWeb.UserForm1" %>
<!--Calendar-->
    <script type="text/javascript" src="https://formden.com/static/cdn/formden.js"></script>
    <link rel="stylesheet" href="https://formden.com/static/cdn/bootstrap-iso.css" />
    <link rel="stylesheet" href="https://formden.com/static/cdn/font-awesome/4.4.0/css/font-awesome.min.css" />
    <style>.bootstrap-iso .formden_header h2, .bootstrap-iso .formden_header p, .bootstrap-iso form{font-family: Arial, Helvetica, sans-serif; color: black}.bootstrap-iso form button, .bootstrap-iso form button:hover{color: white !important;} .asteriskField{color: red;}</style>
<form runat="server">
    <div class="center_div">
        <div class="alert alert-danger alert-dismissible" runat="server" style="display:none" id="Error">
             <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
             <strong>Error</strong> <asp:Label runat="server" ID="ErrorText"></asp:Label>
        </div>
        <div class="alert alert-warning alert-dismissible" runat="server" style="display:none" id="Warning">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <strong>Warning</strong> <asp:Label runat="server" ID="WarningText"></asp:Label>
        </div>
        <div class="alert alert-success alert-dismissible" runat="server" style="display:none" id="Success">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <strong>Success!</strong> <asp:Label runat="server" ID="SuccessText"></asp:Label>
        </div>
        <div class="form-group">
            <label>Username</label>
            <asp:TextBox runat="server" CssClass="form-control login" ID="username" MaxLength="50" />
        </div>
        
        <div class="form-group">
            <label>Email</label>
            <asp:TextBox runat="server" TextMode="email" CssClass="form-control login" ID="email"  MaxLength="254"/>
        </div>
 
        <div class="form-group">
            <label>Password</label>
            <asp:TextBox runat="server" TextMode="password" CssClass="form-control login" ID="password1" MaxLength="50" />
        </div>
 
        <div class="form-group">
            <label>Confirm Password</label>
            <asp:TextBox runat="server" TextMode="password" CssClass="form-control login" ID="password2" MaxLength="50" />
        </div>
            
        <div class="form-group">
            <label>Forename</label>
            <asp:TextBox runat="server" CssClass="form-control login" ID="forename" MaxLength ="50" />
        </div>
           
        <div class="form-group">
            <label>Surname</label>
            <asp:TextBox runat="server" CssClass="form-control login" ID="surname" MaxLength ="50" />
        </div>
  
        <div class="form-group">
            <label>Gender</label>
            <select runat="server" class="form-control" id="gender">
                <option>Male</option>
                <option>Female</option>
                <option>Other</option>
            </select>
        </div>

        <div class="form-group ">
            <label class="control-label col-sm-2 requiredField" for="date">
            Date
            <span class="asteriskField">
                *
            </span>
            </label>
            <div class="col-sm-10">
                <div class="input-group">
                    <div class="input-group-addon">
                        <i class="fa fa-calendar" ></i>
                    </div>
                    <input class="form-control" id="date" name="date" placeholder="MM/DD/YYYY" type="text"/>
                </div>
            </div>
        </div>
         
        <div class="form-group">
            <label>Address Line 1</label>
            <asp:TextBox runat="server" CssClass="form-control login" ID="address1" MaxLength ="50" />
        </div>
            
        <div class="form-group">
            <label>Address Line 2</label>
            <asp:TextBox runat="server" CssClass="form-control login" ID="address2" MaxLength ="50" />
        </div>
           
        <div class="form-group">
            <label>Address Line 3</label>
            <asp:TextBox runat="server" CssClass="form-control login" ID="address3" MaxLength ="50" />
        </div>
            
        <div class="form-group">
            <label>Post Code</label>
            <asp:TextBox runat="server" CssClass="form-control login" ID="postCode" MaxLength ="16" />
        </div>
            
        <div class="form-group">
            <label>Mobile Phone</label>
            <asp:TextBox runat="server" CssClass="form-control login" ID="mobilePhone" MaxLength ="32" />
        </div>
            
        <div class="form-group">
            <label>Home Phone</label>
            <asp:TextBox runat="server" CssClass="form-control login" ID="homePhone"  />
        </div>
           
        <div class="form-group">
            <label>Work Phone</label>
            <asp:TextBox runat="server" CssClass="form-control login" ID="workPhone"  />
        </div>
            
        <div class="form-group">
            <label>Account Type</label>
            <select runat="server" class="form-control" id="accountType">
                <option>user</option>
                <option>member</option>
                <option>admin</option>
            </select>
        </div>
            
        <div class="form-group">
            <label>Account Status</label>
            <select runat="server" class="form-control" id="accountStatus">
                <option>active</option>
                <option>suspended</option>
                <option>inactive</option>
            </select>
        </div>
         
        <div class="form-group">
            <label for="exampleFormControlFile1">Profile Picture</label>
            <input type="file" class="form-control-file" id="profilePicture">
        </div>

        <div class="form-group">
            <label class="form-check-label">Date Created</label>
            <input runat="server" class="form-control" type="text" placeholder="17/04/1999" id="dateTimeCreated" readonly>
        </div>
            
        <br />
            
        <asp:Button runat="server" ID="btnCreate" OnClick="btnCreate_Click" CssClass="btn btn-primary" Text="Create"></asp:Button>
    </div>
</form>
<!--Calendar-->
    <script type="text/javascript" src="https://code.jquery.com/jquery-1.11.3.min.js"></script>

<!-- Include Date Range Picker -->
<script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.4.1/js/bootstrap-datepicker.min.js"></script>
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.4.1/css/bootstrap-datepicker3.css"/>

<script>
	$(document).ready(function(){
		var date_input=$('input[name="date"]'); //our date input has the name "date"
		var container=$('.bootstrap-iso form').length>0 ? $('.bootstrap-iso form').parent() : "body";
		date_input.datepicker({
			format: 'mm/dd/yyyy',
			container: container,
			todayHighlight: true,
			autoclose: true,
		})
	})
</script>