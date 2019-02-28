try {
    var url_string = window.location.href
    var url = new URL(url_string);
    var id = document.querySelector('[id*="' + url.searchParams.get("imageid") + '"]').id;
    document.getElementById(id).scrollIntoView();
}
catch (e) {
    //This does not work on older browsers.
}