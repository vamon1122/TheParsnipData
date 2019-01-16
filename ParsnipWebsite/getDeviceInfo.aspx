<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="getdeviceinfo.aspx.cs" Inherits="ParsnipWebsite.getdeviceinfo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <label id="errorLabel" style="visibility:hidden">default</label>
            <label id="redirectLabel" style="visibility:hidden">default redirect</label>
        </div>
    </form>
    <script src="../javascript/globalBodyV1.5.js"></script>
    <script>
        var errorLabel = document.getElementById("errorLabel");
        errorLabel.innerHTML = "Creating deviceType cookie..." + deviceDetect();
        createCookie("deviceType", deviceDetect());
        errorLabel.innerHTML = "deviceType cooke created successfully! Doing redirect...";

        var url_string = window.location.href
        var url = new URL(url_string);
        var redirect = url.searchParams.get("url");
        var redirectLabel = document.getElementById("redirectLabel");
        redirectLabel.innerHTML = redirect;

        if (redirect === "" || redirect === null)
        {
            errorLabel.innerHTML = "Redirect is null!!";
            window.location.replace("home");
        }
        else
        {
            errorLabel.innerHTML = "all fine. deviceType = " + deviceDetect() + "Redirect = " + redirect;
            window.location.replace(redirect);
        }

        
        
    </script>
</body>
</html>
