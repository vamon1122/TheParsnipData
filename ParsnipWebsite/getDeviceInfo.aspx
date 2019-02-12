<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="getdeviceinfo.aspx.cs" Inherits="ParsnipWebsite.getdeviceinfo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <label id="errorLabel">default</label>
            <label id="redirectLabel">default redirect</label>
        </div>
    </form>
    <script src="../javascript/globalBodyV1.6.js"></script>
    <script>
        var errorLabel = document.getElementById("errorLabel");
        var redirectLabel = document.getElementById("redirectLabel");
        errorLabel.innerHTML = "Creating deviceType cookie..." + deviceDetect();
        createCookie("deviceType", deviceDetect());
        errorLabel.innerHTML = "deviceType cooke created successfully! Doing redirect...";

        redirectLabel.innerHTML = "1";

        var url_string = window.location.href
        redirectLabel.innerHTML = "2";
        var url;
        var redirect

        try {
            url = new URL(url_string);
            redirect = url.searchParams.get("url");
        }
        catch (e) {
            url = window.location.href
            redirectLabel.innerHTML = "Caught error. URL now = " + url;
        }

        
        redirectLabel.innerHTML = "4";
        var redirectLabel = document.getElementById("redirectLabel");
        redirectLabel.innerHTML = "5";
        redirectLabel.innerHTML = redirect;
        redirectLabel.innerHTML = "6";

        if (redirect === "" || redirect === null) {
            redirect = "home";
        }
            

        
        redirectLabel.innerHTML = "Redirect = " + redirect;

        errorLabel.innerHTML = "all fine. deviceType = " + deviceDetect() + "Redirect = " + redirect;

        //Use window.location.replace if possible because 
        //you don't want the current url to appear in the 
        //history, or to show - up when using the back button.

        try { window.location.replace(redirect); }
        catch(e) { window.location = redirect;}
        

        
        
    </script>
</body>
</html>
