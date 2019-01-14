<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="getDeviceInfo.aspx.cs" Inherits="TheParsnipWeb.getDeviceInfo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../javascript/globalBodyV1.4.js"></script>
    <script>
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
        createCookiePerm("deviceType", deviceDetect());
        window.location.replace(redirect);
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
    </form>
</body>
</html>
