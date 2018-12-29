<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserForm.ascx.cs" Inherits="TheParsnipWeb.UserForm1" %>
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