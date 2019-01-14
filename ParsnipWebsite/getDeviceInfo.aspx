<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="getDeviceInfo.aspx.cs" Inherits="TheParsnipWeb.getDeviceInfo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    
    
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <label id="errorLabel">default</label>
        </div>
    </form>
    <script src="../javascript/globalBodyV1.4.js"></script>
    <script>
        errorLabel.innerHTML = "Creating deviceType cookie...";
        createCookiePerm("deviceType", deviceDetect());
        errorLabel.innerHTML = "deviceType cooke created successfully! Doing redirect...";

        var errorLabel = document.getElementById("errorLabel");
        function getParameterByName(name, url) {
            if (!url) url = window.location.href;
            name = name.replace(/[\[\]]/g, '\\$&');
            var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
            results = regex.exec(url);
            if (!results) return null;
            if (!results[2]) return '';
            return decodeURIComponent(results[2].replace(/\+/g, ' '));
        }
        var redirect = getParameterByName('url');

        if (redirect === "" || redirect === null)
        {
            errorLabel.innerHTML = "Redirect is null!!";
        }
        else
        {
            errorLabel.innerHTML = "all fine. deviceType = " + deviceDetect() + "Redirect = " + redirect;
        }

        
        window.location.replace(redirect);
    </script>
</body>
</html>
