// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function elementResize(elClass) {
    var bc = $('.body-content');
    var h1 = $(window).height() - 61;

    bc.css('height', h1);
    var h2 = 0;

    var cb = $(elClass);
    cb.parent().prevAll().each(function () {
        h2 += $(this).outerHeight();
    });

    cb.css('height', bc.height() - h2 - 70);
}