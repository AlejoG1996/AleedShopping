
$(window).scroll(function () {
    if ($("#menu").offset().top > 56) {
        $("#menu").addClass("menucolor");
    } else {
        $("#menu").removeClass("menucolor");
    }
});


(function () {
    const openButton = document.querySelector('.nav__menu');
    const menu = document.querySelector('.nav__link');
    const closeMenu = document.querySelector('.nav__close');
    const logo = document.querySelector('.logo');

    openButton.addEventListener('click', () => {
        menu.classList.add('nav__link--show');
        logo.classList.add('logoone');

    });

    closeMenu.addEventListener('click', () => {
        menu.classList.remove('nav__link--show');
        logo.classList.remove('logoone');


    });




})();

