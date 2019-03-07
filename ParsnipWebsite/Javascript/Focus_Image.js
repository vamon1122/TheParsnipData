try {
    var url_string = window.location.href
    var url = new URL(url_string);
    var id = document.querySelector('[id*="' + url.searchParams.get("imageid") + '"]').id;
    document.getElementById(id).scrollIntoView();

    //This is to try and compensate for 
    //the scroll which seems to go too
    //far sometimes
    window.scrollBy(0, -50);
}
catch (e) {
    //This does not work on older browsers.
}