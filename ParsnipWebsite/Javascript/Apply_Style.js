ApplyStyle();

function ApplyStyle() {

    var link_style = document.getElementById("link_style");

    if (isMobile()) {
        var w = Math.max(document.documentElement.clientWidth, window.innerWidth || 0);
        var h = Math.max(document.documentElement.clientHeight, window.innerHeight || 0);

        if (h > w) {
            link_style.setAttribute("href", "../Css/Mobile_Portrait_Style.css");
        }
        else {
            link_style.setAttribute("href", "../Css/Mobile_Landscape_Style.css");
        }
    }
    else {
        link_style.setAttribute("href", "../Css/Desktop_Style.css");
    }
}

window.addEventListener("orientationchange", function () {
    ApplyStyle();
});