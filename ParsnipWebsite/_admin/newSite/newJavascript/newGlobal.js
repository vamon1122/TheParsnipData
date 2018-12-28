/*
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
www.theparsnip.co.uk: global javascript file (as of 23/05/2016 16:00)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*/

/*
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Function List
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


***USER AGENT DETECT***

isMobile() returns true/false (true it is a mobile, false it isn't a mobile)

deviceDetect() returns OS name (Android, webOS, iPhone, iPad, iPod, BlackBerry, Windows Phone, Windows, MacOS, UNIX, Linux or unknown)


~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


***RANDOM VALUES***

randomColor() returns random hexadecimal number wihth "#" prefix

randomLetter() returns a random lower case letter

randomLetterCAPS returns a random CAPITAL letter

randomNumber() returns a random number between 0 and 9

randomNumber99() returns a random number between 0 and 99

randomNumber999() returns a random number between 0 and 999


~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


***TIME AND DATE***

day() returns the day (Monday, Tuesday etc)

dayShort() returns the day (Mon, Tue etc)

hourMinuteSecond() returns time (HH:MM:SS) 

hourMinute() returns time (HH:MM) 

hour() returns time (HH) 

minute() returns time (MM) 

second() returns time (SS) 


~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


***YOUTUBE EFFICIENT EMBEDDING***

Use this snippet of code instead of the default youtube code

<div class="youtube-container">
   <div class="youtube-player" data-id="VIDEOID"></div>
</div>


~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*/
/*
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Defining Properties of elements
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*/

//LATEST_VIDEO_PROPERTIES START

/* Title */        var lvpTitle = "LATEST VIDEO: Ben's BBQ Video!!!";

/* Description */  var lvpDescription = "Thanks for coming :P";

/* Video ID */     var lvpVideoID = "wLnPNr4WpkA";

//LATEST_VIDEO_PROPERTIES END


//TITLE_PROPERTIES START
createPage();
function createPage()//This is needed for the clock
//Title
{
    {
        var Title = "#TheParsnip";
        title(Title);
        hr();
    }

    var text;
    var href;
    //BUTTON_PROPERTIES START
    {
        //Home button
         text = "Home";
         href = "../HTML/home.html";
        createButton(text, href);
    }

    {
        //Youtube button
        text = "Youtube";
        href = "../HTML/youtube.html";
        createButton(text, href);
    }

    {
        //Camping button
        text = "Camping";
        href = "../HTML/camping.html";
        createButton(text, href);
    }

    {
        //Admin
        {
            if (checkCookie("username")) {
                if (getCookie("username") === "admin1234") {
                    text = "Admin";
                    href = "../HTML/admin.html";
                    createButton(text, href);
                }
            }
        }
    }
}


/*
{
    //new button
    var text = "New";
    var href = "../HTML/page.html"
    createButton(text, href);
}
*/


timeOne();

/*
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*/
/*
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Functions
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*/

/*{
    var loc = window.location;
    var locTwo = loc.toString().substr(-10);
    if (locTwo == 'admin.html');
    {
        alert("window.location (" + locTwo + ") == admin.html")
        if (getCookie("username") != "admin1234");
        {
            alert("not admin");
            window.location.replace("_default.html");
        }
    }
}*/


//Checks if element exists
function elementExists(element)
{
    if (document.getElementById(element) !== null) return true; else return false;
}


//Creates random hexadecimal color code (e.g. #123abc)
var colCode = "#"; //The color being made
var colCodeTemp = "#"; //Complete Color
var stringCharacter = 1; //This goes up every time a new character is added to the string (color value e.g. #123abc)
resetAuto(); //Auto set values to default

function randomColor() {
    resetAuto();
    return colCode;
}

function resetAuto() //Resets all values to default
{
    colCode = "#";
    colCodeTemp = "#";
    stringCharacter = 1;
    createNumber();
}


function createNumber() //Creates a random number between 1 & 15
{
    var x = Math.round(Math.random() * 100);
    if (x < 16) { letterSwitch(x); } else { createNumber(); }
}

function letterSwitch(x) {
    switch (x) {
        case 10:
            stringAdd('a');
            break;
        case 11:
            stringAdd('b');
            break;
        case 12:
            stringAdd('c');
            break;
        case 13:
            stringAdd('d');
            break;
        case 14:
            stringAdd('e');
            break;
        case 15:
            stringAdd('f');
            break;
        default:
            stringAdd(x);
            break;
    }
}

function stringAdd(x) //Collates the characters for the hex number code
{
    if (stringCharacter <= 6) {
        stringCharacter++;
        colCodeTemp = colCodeTemp + x;
        createNumber();
    }
    else {
        colCode = colCodeTemp;

    }
}


//Random Letter Generator (lower)

function randomLetter() {
    var x = 99;
    while (x > 25)
    { x = randomNumber99(); }
    switch (x) {
        case 0:
            return "a";
        case 1:
            return "b";
        case 2:
            return "c";
        case 3:
            return "d";
        case 4:
            return "e";
        case 5:
            return "f";
        case 6:
            return "g";
        case 7:
            return "h";
        case 8:
            return "i";
        case 9:
            return "j";
        case 10:
            return "k";
        case 11:
            return "l";
        case 12:
            return "m";
        case 13:
            return "n";
        case 14:
            return "o";
        case 15:
            return "p";
        case 16:
            return "q";
        case 17:
            return "r";
        case 18:
            return "d";
        case 19:
            return "t";
        case 20:
            return "u";
        case 21:
            return "v";
        case 22:
            return "w";
        case 23:
            return "x";
        case 24:
            return "y";
        case 25:
            return "z";
        default:
            alert("ERROR! x = " + x);
            break;
    }
}

//Random Letter Generator (upper)
function randomLetterCAPS() {
    var x = 99;
    while (x > 25)
    { x = randomNumber99(); }
    switch (x) {
        case 0:
            return "A";
        case 1:
            return "B";
        case 2:
            return "C";
        case 3:
            return "D";
        case 4:
            return "E";
        case 5:
            return "F";
        case 6:
            return "G";
        case 7:
            return "H";
        case 8:
            return "I";
        case 9:
            return "J";
        case 10:
            return "K";
        case 11:
            return "L";
        case 12:
            return "M";
        case 13:
            return "N";
        case 14:
            return "O";
        case 15:
            return "P";
        case 16:
            return "Q";
        case 17:
            return "R";
        case 18:
            return "S";
        case 19:
            return "T";
        case 20:
            return "U";
        case 21:
            return "V";
        case 22:
            return "W";
        case 23:
            return "X";
        case 24:
            return "Y";
        case 25:
            return "Z";
        default:
            return "ERROR";
    }
}


//Random Number Generator (0-9)
function randomNumber() {
    var x = 100;
    while (x > 9)
    { x = Math.round(Math.random() * 10); }
    return x;
}


//Random Number Generator (0-99)
function randomNumber99() {
    var x = 100;
    while (x > 99)
    { x = Math.round(Math.random() * 100); }
    return x;
}


//Random Number Generator (0-999)
function randomNumber999() {
    var x = 100;
    while (x > 999)
    { x = Math.round(Math.random() * 1000); }
    return x;
}


//Day (long)
function day() {
    var day = new Date().getDay();
    {
        switch (day) {
            case 0:
                return "Sunday";

            case 1:
                return "Monday";

            case 2:
                return "Tuesday";

            case 3:
                return "Wednesday";

            case 4:
                return "Thursday";

            case 5:
                return "Friday";

            case 6:
                return "Saturday";

            default:
                return "broken";
        }
    }
}


//Day (short)
function dayShort() {
    var day = new Date().getDay();
    {
        switch (day) {
            case 0:
                return "Sun";

            case 1:
                return "Mon";
            case 2:
                return "Tue";
            case 3:
                return "Wed";
            case 4:
                return "Thu";
            case 5:
                return "Fri";
            case 6:
                return "Sat";
            default:
                return "error";
        }
    }
}


//Time (HH:MM:SS)
function hourMinuteSecond() {
    var hour = new Date().getHours();
    var minute = new Date().getMinutes();
    var second = new Date().getSeconds();
    if (hour < 10) { hour = '0' + +hour; }
    if (minute < 10) { minute = '0' + +minute; }
    if (second < 10) { second = '0' + +second; }

    {
        return hour + ":" + minute + ":" + second;
    }
}


//Time (HH:MM)
function hourMinute() {
    var hour = new Date().getHours();
    var minute = new Date().getMinutes();
    if (hour < 10) { hour = '0' + +hour; }
    if (minute < 10) { minute = '0' + +minute; }
    {
        return hour + ":" + minute;
    }
}


//Time (HH)
function hour() {
    var hour = new Date().getHours();
    if (hour < 10) { hour = '0' + +hour; }
    {
        return hour;
    }
}


//Time (MM)
function minute() {
    var minute = new Date().getMinutes();
    if (minute < 10) { minute = '0' + +minute; }

    {
        return minute;
    }
}


//Time (SS)
function second() {
    var second = new Date().getSeconds();
    if (second < 10) { second = '0' + +second; }

    {
        return second;
    }
}


//Mobile / Desktop CSS ----- KEYWORDS: Style, sheet,  stylesheet
{
    if (elementExists("link_style")) {
        if (navigator.userAgent.match(/Android/i)
        || navigator.userAgent.match(/webOS/i)
        || navigator.userAgent.match(/iPhone/i)
        || navigator.userAgent.match(/iPad/i)
        || navigator.userAgent.match(/iPod/i)
        || navigator.userAgent.match(/BlackBerry/i)
        || navigator.userAgent.match(/Windows Phone/i)
        ) {
            link_style = document.getElementById("link_style");
            link_style.setAttribute("href", "../CSS/m_style.css");
        }
        else {
            link_style = document.getElementById("link_style");
            link_style.setAttribute("href", "../CSS/style.css");
        }
    }
}


//Mobile / Desktop Check ----- KEYWORDS: Device, phone, pc
function isMobile() {
    if (navigator.userAgent.match(/Android/i)
    || navigator.userAgent.match(/webOS/i)
    || navigator.userAgent.match(/iPhone/i)
    || navigator.userAgent.match(/iPad/i)
    || navigator.userAgent.match(/iPod/i)
    || navigator.userAgent.match(/BlackBerry/i)
    || navigator.userAgent.match(/Windows Phone/i)
    ) {
        return true;
    }
    else {
        return false;
    }
}


//Device Check ----- KEYWORDS: Device, phone, pc, user, agent, useragent
function deviceDetect() {
    if (navigator.userAgent.match(/Android/i)) { return 'Android'; }
    else if (navigator.userAgent.match(/webOS/i)) { return 'webOS'; }
    else if (navigator.userAgent.match(/iPhone/i)) { return 'iPhone'; }
    else if (navigator.userAgent.match(/iPad/i)) { return 'iPad'; }
    else if (navigator.userAgent.match(/iPod/i)) { return 'iPod'; }
    else if (navigator.userAgent.match(/BlackBerry/i)) { return 'BlackBerry'; }
    else if (navigator.userAgent.match(/Windows Phone/i)) { return 'Windows Phone'; }
    else if (navigator.appVersion.indexOf("Win") !== -1) { return "Windows"; }
    else if (navigator.appVersion.indexOf("Mac") !== -1) { return "MacOS"; }
    else if (navigator.appVersion.indexOf("X11") !== -1) { return "UNIX"; }
    else if (navigator.appVersion.indexOf("Linux") !== -1) { return "Linux"; }
}

//Device Check ----- KEYWORDS: Device, phone, pc, user, agent, useragent
function userAgent() {
    if (navigator.userAgent.match(/Android/i)) { return Android; }
    else if (navigator.userAgent.match(/webOS/i)) { return webOS; }
    else if (navigator.userAgent.match(/iPhone/i)) { return iPhone; }
    else if (navigator.userAgent.match(/iPad/i)) { return iPad; }
    else if (navigator.userAgent.match(/iPod/i)) { return iPod; }
    else if (navigator.userAgent.match(/BlackBerry/i)) { return BlackBerry; }
    else if (navigator.userAgent.match(/Windows Phone/i)) { return WindowsPhone; }
    else if (navigator.appVersion.indexOf("Win") !== -1) { return Windows; }
    else if (navigator.appVersion.indexOf("Mac") !== -1) { return MacOS; }
    else if (navigator.appVersion.indexOf("X11") !== -1) { return UNIX; }
    else if (navigator.appVersion.indexOf("Linux") !== -1) { return Linux; }
}


/*
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*/
/*
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Page Creation Functions
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*/


//Title Creation
function title(Title) {
    if (elementExists("titleAndMenu"))
    {
    var div = document.getElementById("titleAndMenu");
        var parsnip = document.createElement("h1");
    parsnip.innerHTML = Title;
    div.appendChild(parsnip);
    }

}


//Title line creation
function hr() {
    if (elementExists("titleAndMenu"))
    {
    var div = document.getElementById("titleAndMenu");
        var horozontalRule = document.createElement("hr");
    div.appendChild(horozontalRule);

    }

}


//Button Creation
function createButton(text, href) {
    var buttonAnchor = document.createElement("a");
    buttonAnchor.href = href;
    var menuButton = document.createElement("input");
    menuButton.type = "button";
    menuButton.value = text;
    menuButton.className = "menu";
    buttonStyle(menuButton, buttonAnchor);
}


//styles the button
function buttonStyle(menuButton, buttonAnchor) {
    if (elementExists("titleAndMenu")) {

        var color = "#CCCCCC";
        var backgroundImage = "url(../Patterns/pattern_black1.png)";
        if (isMobile() === true) { //alert("MOBILE ! After isMob() = " + isMob()) 
            var width = "300px";
            var height = "60px";
            var borderRadius = "20px 20px";
            var fontSize = "35px";
            var marginBottom = "10px";
            var whiteSpace = "nowrap";
            var marginRight = "8px";
            menuButton.style.color = color;
            menuButton.style.backgroundImage = backgroundImage;
            menuButton.style.width = width;
            menuButton.style.height = height;
            menuButton.style.borderRadius = borderRadius;
            menuButton.style.fontSize = fontSize;
            menuButton.style.marginBottom = marginBottom;
            menuButton.style.marginRight = marginRight;
            menuButton.style.whiteSpace = whiteSpace;
            menuButton.style.cursor = "pointer";
            buttonAnchor.appendChild(menuButton);
            titleAndMenu.appendChild(buttonAnchor);
        }
        else if (isMobile() === false) {
            width = "200px";
            height = "30px";
            borderRadius = "10px 10px";
            fontSize = "18px";
            marginBottom = "5px";
            outline = "none";
            marginRight = "4px";
            menuButton.style.color = color;
            menuButton.style.backgroundImage = backgroundImage;
            menuButton.style.width = width;
            menuButton.style.height = height;
            menuButton.style.borderRadius = borderRadius;
            menuButton.style.fontSize = fontSize;
            menuButton.style.marginBottom = marginBottom;
            menuButton.style.outline = outline;
            menuButton.style.marginRight = marginRight;
            menuButton.style.cursor = "pointer";
            buttonAnchor.appendChild(menuButton);
            titleAndMenu.appendChild(buttonAnchor);
        }
    }
}


//Clock and Login
function timeOne() {
    if (elementExists("titleAndMenu")) {
        var div = document.getElementById("titleAndMenu");
        var timePost = document.createElement("label");
        timePost.style.position = "absolute";
        timePost.style.right = "10px";
        timePost.style.top = "10px";
        timePost.className = "pulse";
        timePost.id = "timePost";
        timePost.style.minWidth = "250px";
        timePost.style.textAlign = "center";
        timePost.style.borderStyle = "inset";
        timePost.style.backgroundColor = "grey";
        timePost.style.color = "white";
        timePost.style.borderRadius = "10px";
        timePost.style.padding = "3px 6px";
        timePost.style.fontWeight = "bold";

        div.appendChild(timePost);
        time();
    }
}


function testCookie()
{
        document.cookie = "test = yes";
    if (checkCookie("test")) { return true; }
}

function createCookie(cname, cvalue) {
    document.cookie = cname + "=" + cvalue;
}

function createCookiePerm(cname, cvalue) {
    document.cookie = cname + "=" + cvalue + "; expires=Thu, 18 Dec 2019 12:00:00 UTC";
}


//Signing in & out
function signInFunc() {
    if (testCookie()) {
        var person = prompt("Enter your username", "");

        //setUser(person);

        createCookiePerm("username", person);

        signInOutAnchor();
    }
    else 
    { alert("Cookies do not appear to work properly on your device. The log in function is not available"); }
}


function signOutFunc()
{
    document.cookie = "username=; expires=Thu, 01 Jan 1970 00:00:00 UTC";
    signInOutAnchor();
}


signInOutAnchor();
function signInOutAnchor()
{
    if (elementExists("titleAndMenu")) {
        var div = document.getElementById("titleAndMenu");
    
        
        if (elementExists("logInAnchor")) {
            div.innerHTML = "";
        }
        

        if (!elementExists("loginAnchor"))
        {
            var logInAnchor = document.createElement("a");
            logInAnchor.id = "logInAnchor";
            logInAnchor.style.position = "absolute";
            logInAnchor.style.right = "23px";
            logInAnchor.style.top = "45px";
            logInAnchor.style.color = "blue";
            logInAnchor.style.cursor = "pointer";
            if (isMobile())
            {
                logInAnchor.style.top = "60px";
            }
        }

        if (!checkCookie("username")) {
            logInAnchor.onclick = signInFunc;
            logInAnchor.innerHTML = "(Log In)";
            div.appendChild(logInAnchor);
        }
        else {
            logInAnchor.onclick = signOutFunc;
            logInAnchor.innerHTML = "(Log Out)";
            div.appendChild(logInAnchor);
        }
    }
}

function lastPage()
{
    alert(window.location);
    createCookie("lastPage", window.location);
}


//Change username on clock
function time() {
    var div = document.getElementById("titleAndMenu");
    if (!document.getElementById("timePost"))
    {
        createPage();
        
        timeOne();
    }
    var timePost = document.getElementById("timePost");
    
    if (checkCookie('username')) {
        timePost.innerHTML = 'Welcome back ' + getCookie('username') + "!" + " " + day() + " " + hourMinuteSecond();
    }
    else {
        timePost.innerHTML = 'Welcome!' + " " + day() + " " + hourMinuteSecond();
    }
    setTimeout(time, 10);
}


//Set username cookie
function setUser(username) {
    document.cookie = "username= " + username;
    //alert("Username Saved!")
}


//Get cookie
function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) === 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}


//Check Cookie
function checkCookie(p) {
    //alert('got here')
    var checkThis = getCookie(p);
    if (checkThis !== "") {
        return true;
    } else {
        return false;
    }
}


//Latest Video
latestVideo();
function latestVideo() {
    var title = lvpTitle;
    var videoID = lvpVideoID;
    var description = lvpDescription;

    //Title
    var titleCreate = document.createElement("h3");
    titleCreate.innerHTML = title;
    latestVideoAppend(titleCreate);

    //Description
    var descriptionCreate = document.createElement("div");
    descriptionCreate.innerHTML = description;
    latestVideoAppend(descriptionCreate);

    //div
    var containerDiv = document.createElement("div");
    containerDiv.className = "youtube-container";
    containerDiv.style.display = "block";
    containerDiv.style.margin = "20px auto";
    containerDiv.style.width = "100%";
    containerDiv.style.maxWidth = "600px";

    //inner div
    var playerDiv = document.createElement("div");
    playerDiv.className = "youtube-player";
    playerDiv.setAttribute("data-id", videoID);
    playerDiv.style.display = "block";
    playerDiv.style.width = "100%";
    playerDiv.style.paddingBottom = "56.25%";
    playerDiv.style.overflow = "hidden";
    playerDiv.style.position = "relative";
    playerDiv.style.width = "100%";
    playerDiv.style.height = "100%";
    playerDiv.style.cursor = "hand";
    playerDiv.style.display = "block";
    containerDiv.appendChild(playerDiv);
    latestVideoAppend(containerDiv);

    //hr
    var horozontalRule = document.createElement("hr");
    horozontalRule.className = "break";
    latestVideoAppend(horozontalRule);
}

//latest video create
function latestVideoAppend(element) {
    if (elementExists("latestVideo")) {
        var target = document.getElementById("latestVideo");
        target.appendChild(element);
    }

}


//Camping Password
function onclick_password() {
    if (testCookie()) {
        var info = document.getElementById("info");
        var x = document.getElementById("passw").value;
        if (x === 'loldred' || x === 'loldred ' || x === 'Loldred' || x === 'Loldred ') {
            //document.cookie = "password = yes"
            createCookiePerm("password", "yes");
            unlockCamping();
        }
        else {
            document.getElementById("passw").setAttribute("style", "background-color:lightsalmon");
            window.alert("Sorry you entered the wrong password, please try again");
        }

    }
    else
    {
        alert("Cookies do not appear to work properly on your device. The log in function is not available");
        }
}


//Unlocks camping if cookie has been created
function unlockCamping()
{
        if (checkCookie("password"))
        {
            //alert("cookie exists")
            if (getCookie("password") === "yes")
            {
                info.setAttribute('style', 'display: block');
                document.getElementById("hide_me").setAttribute('style', 'display: none');
            }
        }
}


/*
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*/
/*
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
External Functions
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*/


//GOOGLE ANALYTICS START
(function (i, s, o, g, r, a, m) {
    i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
        (i[r].q = i[r].q || []).push(arguments);
    }, i[r].l = 1 * new Date(); a = s.createElement(o),
        m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m);
})(window, document, 'script', 'https://www.google-analytics.com/analytics.js', 'ga');

ga('create', 'UA-77788621-1', 'auto');
ga('send', 'pageview');


//Youtube Functions
(function () {
    var v = document.getElementsByClassName("youtube-player");
    for (var n = 0; n < v.length; n++) {
        var p = document.createElement("div");
        p.innerHTML = labnolThumb(v[n].dataset.id);
        p.onclick = labnolIframe;
        v[n].appendChild(p);
    }
})();

function labnolThumb(id) {
    return '<img class="youtube-thumb" src="//i.ytimg.com/vi/' + id + '/hqdefault.jpg"><div class="play-button"></div>';
}

function labnolIframe() {
    var iframe = document.createElement("iframe");
    iframe.setAttribute("src", "//www.youtube.com/embed/" + this.parentNode.dataset.id + "?autoplay=1&autohide=2&border=0&wmode=opaque&enablejsapi=1&controls=0&showinfo=0");
    iframe.setAttribute("frameborder", "0");
    iframe.setAttribute("id", "youtube-iframe");
    this.parentNode.replaceChild(iframe, this);
}
/*
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*/





