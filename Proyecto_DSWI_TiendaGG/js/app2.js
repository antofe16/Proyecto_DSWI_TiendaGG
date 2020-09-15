$(document).ready(function () {
    $('.menu-image').on('click', function () {
        var sidebar = $('.sidebar');
        if (sidebar.css('display') == 'none') {
            sidebar.fadeIn(300);
        } else {
            sidebar.fadeOut(300);
        }
    });
    $('.down').on('click', function () {
        var menudrop = $(this).parent().next('ul');
        menudrop.slideToggle();
    });
    $('.sidebar-menu-item').hover(
        function () {
            var menuitem = $(this).children();
            menuitem.css('color', 'white');
        },
        function () {
            var menuitem = $(this).children();
            menuitem.css('color', '#343A40');
        }
    );
});