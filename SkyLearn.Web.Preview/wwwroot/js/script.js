"use strict";
AOS.init();

var a = 0;
var b = 0;
var oTop = 0;
var progress = $("#progress");
$(window).scroll(function () {
  if (progress.length) {
    var oTop = progress.offset().top - window.innerHeight;
    if (b == 0 && $(window).scrollTop() > oTop) {
      var max = -219.99078369140625;
      forEach(document.querySelectorAll(".progress"), function (index, value) {
        var percent = value.getAttribute("data-progress");
        value
          .querySelector(".fill")
          .setAttribute(
            "style",
            "stroke-dashoffset: " + ((100 - percent) / 100) * max
          );
        value.querySelector(".value").innerHTML = percent + "%";
      });

      b = 1;
    }
  }
});


var counter = $("#counter");
$(window).scroll(function () {
  if (counter.length) {
    var scrolTop = counter.offset().top - window.innerHeight;
    if (a == 0 && $(window).scrollTop() > scrolTop) {
      $(".counter-value").each(function () {
        var $this = $(this),
          countTo = $this.attr("data-count");
        $({
          countNum: $this.text()
        }).animate(
          {
            countNum: countTo
          },
          {
            duration: 2000,
            easing: "swing",
            step: function () {
              $this.text(Math.floor(this.countNum));
            },
            complete: function () {
              $this.text(this.countNum);
            }
          }
        );
      });
      a = 1;
    }
  }
});


var btn = $("#return-to-top");

$(window).scroll(function () {
  if ($(window).scrollTop() > 300) {
    btn.fadeIn(500);
  } else {
    btn.fadeOut(500);
  }
});

btn.on("click", function (evnt) {
  evnt.preventDefault();
  $("html, body").animate({ scrollTop: 0 }, "500");
});


$(".dropdown").on("show.bs.dropdown", function () {
  $(this).find(".dropdown-menu").first().stop(true, true).slideDown();
});

$(".dropdown").on("hide.bs.dropdown", function () {
  $(this).find(".dropdown-menu").first().stop(true, true).slideUp();
});


/* Hero slider js */
var owl = $(".hero_slider");
owl.owlCarousel({
  margin: 0,
  loop: true,
  dots: true,
  nav: true,
  smartSpeed: 1000,
  autoplay: false,
  items: 5,
  responsive: {
    0: {
      items: 1
    },
    600: {
      items: 1
    },
    767: {
      items: 1
    },
    992: {
      items: 1
    },
    1000: {
      items: 1
    }
  }
});

/* Hero slider other pages js */
var owl = $(".hero_slider_2, .hero_slider_3, .hero_slider_4");
owl.owlCarousel({
  margin: 0,
  loop: true,
  dots: false,
  nav: true,
  smartSpeed: 1000,
  autoplay: false,
  items: 5,
  responsive: {
    0: {
      items: 1
    },
    600: {
      items: 1
    },
    767: {
      items: 1
    },
    992: {
      items: 1
    },
    1000: {
      items: 1
    }
  }
});

/* Team slider js */
var owl = $(".team_slider");
owl.owlCarousel({
  margin: 0,
  loop: true,
  dots: false,
  nav: true,
  smartSpeed: 1000,
  autoplay: false,
  items: 5,
  responsive: {
    0: {
      items: 1,
      margin: 28,
      dots: true,
      nav: false
    },
    481: {
      items: 2,
      margin: 28,
      dots: true,
      nav: false
    },
    576: {
      items: 2,
      dots: true,
      nav: false
    },
    768: {
      items: 3,
      dots: true,
      nav: false
    },
    992: {
      items: 3,
      dots: true,
      nav: false
    },
    1000: {
      items: 3,
      dots: true,
      nav: false
    },
    1200: {
      items: 4,
      dots: false,
      nav: true
    },
    1367: {
      items: 4,
      dots: false,
      nav: true
    }
  }
});

/* News slider js */
var owl = $(".news_slider");
owl.owlCarousel({
  margin: 0,
  loop: true,
  dots: false,
  nav: true,
  smartSpeed: 1000,
  autoplay: false,
  items: 3,
  responsiveClass: true,
  responsive: {
    0: {
      items: 1,
      dots: true,
      nav: false
    },
    576: {
      items: 2,
      dots: true,
      nav: false
    },
    768: {
      items: 2,
      dots: true,
      nav: false
    },
    992: {
      items: 3,
      dots: true,
      nav: false
    },
    1200: {
      items: 3,
      dots: true,
      nav: false
    },
    1367: {
      items: 3,
      dots: true,
      nav: false
    }
  }
});

/* Photo Gallery js */
$(document).on('click', '[data-toggle="lightbox"]', function(event) {
  event.preventDefault();
  $(this).ekkoLightbox();
});
