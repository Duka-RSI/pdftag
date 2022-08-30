(($) => {
  const $contact_us = $('#contact-us')
        ,$contact_us_box = $('#contact-us-box')
        ,$nav_item = $('ul.navbar-nav li.nav-item');
  $contact_us.on('click', (e) => {
      e.preventDefault()
      const $body = window.opera ? document.compatMode == 'CSS1Compat' ? $('html') : $('body') :
          $('html,body')
      $body.animate({
          scrollTop: $contact_us_box.offset().top
      }, 500)
  })
  $nav_item.click(function(event) {
    $(this).addClass('active').siblings().removeClass('active');
  });
})($)
