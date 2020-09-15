$(document).ready(function(){
    $('#menu').on('click', function(){
        var sidebar = $('#sidebar');
        var header = $('header');
        var main = $('main');
        if (sidebar.css('display') == 'none'){
            sidebar.fadeIn(300);
            header.css('opacity','0.8');
            main.css('opacity','0.8');
        }
    });

    $('#close').on('click', function(){
        var sidebar = $('#sidebar');
        var header = $('header');
        var main = $('main');
        if (!(sidebar.css('display') == 'none')){
            sidebar.fadeOut(300);
            header.css('opacity','1');
            main.css('opacity','1');
        }
    });
    
   /* $('#efect1')*/
});