// Add this to existing site.js

// Highlight active menu item
$(document).ready(function () {
    // Get current page URL
    var url = window.location.pathname;

    // Highlight navbar links
    $('.navbar-nav .nav-link').each(function () {
        if ($(this).attr('href') === url) {
            $(this).addClass('active');
        }
    });

    // Auto-dismiss alerts after 5 seconds
    setTimeout(function () {
        $('.alert').fadeOut('slow');
    }, 5000);
});