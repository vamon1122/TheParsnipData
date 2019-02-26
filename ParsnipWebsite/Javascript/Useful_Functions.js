/*
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
www.theparsnip.co.uk: global javascript file (as of 10/06/2016 10:39)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Function List
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


***COOKIES***

testCookie() Checks if cookies work on that webpage

cookieCreate(cname, cvalue) creates cookie

deleteCookie(cname) deletes cookie

getCookie(cname) returns cookie value

checkCookie(cname) returns true or false. Checks if cookie exists


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
*/




function dateAdd(date, interval, units) {
    var ret = new Date(date); //don't change original date
    var checkRollover = function () { if (ret.getDate() !== date.getDate()) ret.setDate(0); };
    switch (interval.toLowerCase()) {
        case 'year': ret.setFullYear(ret.getFullYear() + units); checkRollover(); break;
        case 'quarter': ret.setMonth(ret.getMonth() + 3 * units); checkRollover(); break;
        case 'month': ret.setMonth(ret.getMonth() + units); checkRollover(); break;
        case 'week': ret.setDate(ret.getDate() + 7 * units); break;
        case 'day': ret.setDate(ret.getDate() + units); break;
        case 'hour': ret.setTime(ret.getTime() + units * 3600000); break;
        case 'minute': ret.setTime(ret.getTime() + units * 60000); break;
        case 'second': ret.setTime(ret.getTime() + units * 1000); break;
        default: ret = undefined; break;
    }
    return ret;
}

function testCookie() {
    document.cookie = "test = yes";
    if (checkCookie("test")) { return true; }
}


function createCookie(cname, cvalue) {
    document.cookie = cname + "=" + cvalue;
}


function createCookiePerm(cname, cvalue) {
    document.cookie = cname + "=" + cvalue + "; expires=Thu, 2 Aug 2020 20:47:11 UTC;";
}


function getLocation() {
    createCookie("deviceLocation", "Geolocation function was called.");
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(showPosition, showError);
    } else {
        createCookie("deviceLocation", "Geolocation is not supported by this browser.");
    }
}



function showPosition(position) {
    createCookie("deviceLatitude", position.coords.latitude);
    createCookie("deviceLongitude", position.coords.longitude);
}

function showError(error) {
    switch (error.code) {
        case error.PERMISSION_DENIED:
            createCookie("deviceLatitude", "User denied the request for Geolocation.");
            createCookie("deviceLongitude", "User denied the request for Geolocation.");
            break;
        case error.POSITION_UNAVAILABLE:
            createCookie("deviceLatitude", "Location information is unavailable.");
            createCookie("deviceLongitude", "Location information is unavailable.");
            break;
        case error.TIMEOUT:
            createCookie("deviceLatitude", "The request to get user location timed out.");
            createCookie("deviceLongitude", "The request to get user location timed out.");
            break;
        case error.UNKNOWN_ERROR:
            createCookie("deviceLatitude", "An unknown error occurred.");
            createCookie("deviceLongitude", "An unknown error occurred.");
            break;
    }
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


/*
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Functions
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*/


//Checks if element exists
function elementExists(element) {
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
                return "broken";
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


ApplyStyle();

function ApplyStyle() {

    var link_style = document.getElementById("link_style");

    if (isMobile())
    {    
        var w = Math.max(document.documentElement.clientWidth, window.innerWidth || 0);
        var h = Math.max(document.documentElement.clientHeight, window.innerHeight || 0);

        if (h > w)
        {
            link_style.setAttribute("href", "../Css/Mobile_Portrait_Style.css");
        }
        else
        {
            link_style.setAttribute("href", "../Css/Mobile_Landscape_Style.css");
        }
    }
    else
    {   
        link_style.setAttribute("href", "../Css/Desktop_Style.css");
    }
}

window.addEventListener("orientationchange", function () {
    ApplyStyle();
});



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


~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
External Functions
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*/


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