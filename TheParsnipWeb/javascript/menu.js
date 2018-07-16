
/////Unchanging/////
    var menuDiv = document.getElementById("menuDiv");
    
var height;
var buttWidth;
var fSize;
var titleFontSize;
var dropDownWidth;
var buttonPadding;

/////Color Scheme/////
var colLightest = "#E2FFFE";
var colLighter = "#A5D0D6";
var colLight = "#6DA9B1";
var colDark = "grey";
var colDarkest = "dimgrey";

/////Font/////
var font = "Verdana";

/////Title/////
var title = "#TheParsnip";
var titleColor = "white";



/////Menu List/////
var fontCol = "white";

/////Mobile / Desktop Variables/////
if (isMobile() == true){
    height = "112.5px";
    buttFontSize = "50px";
    buttonPadding = "30px";
    titleFontSize = "87.5px";
    dropDownWidth = "50%";
}
else{
    height = "45px";
    buttFontSize = "17px";
    buttonPadding = "20px";
    titleFontSize = "35px";
    dropDownWidth = "300px";
}

/////Buttons/////
createButton("Home", "home.aspx");
createButton("Videos", "youtube.aspx");
createButton("Memes", "memes.aspx");
createButton("Photos", "photos.aspx");
createButton("Minecraft", "minecraft.aspx");
createButton("Bios", "bios.aspx");
createButton("Log Out", "logout.aspx");

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//document.getElementById("body").style.backgroundColor = "white";
document.getElementById("body").style.backgroundImage = "url('../Patterns 2/Black & White/brickwall.png')";
document.getElementById("body").style.color = colDarkest;

menuDiv.style.zIndex = "1";
menuDiv.style.backgroundColor = colDarkest;
menuDiv.style.display = "inline-block";
menuDiv.style.height = height;
menuDiv.style.listStyle = "none";
menuDiv.style.position = "fixed"
menuDiv.style.top = "0px";
menuDiv.style.left = "0px";
menuDiv.style.padding = "0px";
menuDiv.style.width = "100%";

var pageTitle = document.createElement("label");
pageTitle.innerHTML = title;
pageTitle.style.fontSize = titleFontSize;
pageTitle.style.fontWeight = "bold";
pageTitle.style.color = titleColor;

if (isMobile()) {
    //pageTitle.style.position = "absolute";
    //pageTitle.style.right = "50px";
}

menuDiv.appendChild(pageTitle);

var menuLabel = document.createElement("label");
menuLabel.innerHTML = "≡";
menuLabel.style.cursor = "pointer";
menuLabel.style.color = titleColor;
menuLabel.style.position = "fixed";
menuLabel.style.padding = "0px";
menuLabel.style.margin = "0px";

if (isMobile()) {
    menuLabel.style.top = "-27.5px";
    menuLabel.style.left = "17.5px";
    menuLabel.style.fontSize = "125px";
}
else {
menuLabel.style.top = "-11px";
menuLabel.style.left = "7px";
menuLabel.style.fontSize = "50px";
}




menuLabel.addEventListener("click", function (e) {
menuLabel.className = "rotate";
if (isMobile()) {
    if (list.className == "mMenHide" || list.className == "menHidden") {
        list.className = "mMenVis";
    }
    else {
        //list.style.visibility = "hidden";
        list.className = "mMenHide";
    }
}
else {
    if (list.className == "menHide" || list.className == "menHidden") {
        list.className = "menVis";
    }
    else {
        //list.style.visibility = "hidden";
        list.className = "menHide";
    }


}
setTimeout(function (e) { menuLabel.className = ""; }

, 500)
}
);

menuDiv.appendChild(menuLabel);

function createButton(title, href) {
    if(document.getElementById("list")){
        funcCreateButton(title, href);
    }
    else{
        createList()
        funcCreateButton(title, href);
    }
}

if (!document.getElementById("list")) {
    createList();
}
function createList() {
    var list = document.createElement("ul");
    list.style.backgroundColor = colDark;
    list.style.position = "fixed";
    list.style.top = height;
    list.style.width = dropDownWidth;
    list.style.listStyle = "none";
    list.style.padding = "0px";
    list.style.margin = "0px";
    list.className = "menHidden";
    list.style.zIndex = "0";
    list.id = "list";
    document.getElementById("body").appendChild(list);
    
};

//list.onmouseout = function (e) { alert("Left") };





var firstButton;
function funcCreateButton(title, href) {
    var butt = document.createElement("li");
    if (firstButton == false) {
        butt.style.textAlign = "center";
    }
    else
    {
    }

    butt.style.width = "100%";
    
    
    butt.style.backgroundColor = colDark;

    var buttAnk = document.createElement("a");
    buttAnk.style.color = fontCol;
    buttAnk.innerHTML = title;
    buttAnk.href = href;
    buttAnk.style.width = "100%";
    buttAnk.style.padding = "0px"
    buttAnk.style.textDecoration = "none";
    buttAnk.style.display = "block";
    buttAnk.className = "menu"
    buttAnk.style.fontSize = buttFontSize;
    buttAnk.style.paddingTop = buttonPadding;
    buttAnk.style.paddingBottom = buttonPadding;
    
    butt.appendChild(buttAnk);
    list.appendChild(butt);
    firstButton = false;
}




function isMobile() {
    if (navigator.userAgent.match(/Android/i)
    || navigator.userAgent.match(/webOS/i)
    || navigator.userAgent.match(/iPhone/i)
    || navigator.userAgent.match(/iPad/i)
    || navigator.userAgent.match(/iPod/i)
    || navigator.userAgent.match(/BlackBerry/i)
    || navigator.userAgent.match(/Windows Phone/i)
    ) {
        return true
    }
    else {
        return false
    }
}
