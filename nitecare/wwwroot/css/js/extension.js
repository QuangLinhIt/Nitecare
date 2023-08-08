$(window).on("load", function () {
  $(".carousel-products").flickity({
    contain: !0,
    cellAlign: "left",
    autoPlay: !1,
    pauseAutoPlayOnHover: !1,
  });
  $(".carousel-main-products").flickity({
    contain: !0,
    groupCells: "100%",
    pageDots: !1,
    autoPlay: 4e3,
    prevNextButtons: !1,
  });
  $(".carousel-nav-products").flickity({
    contain: !0,
    cellAlign: "center",
    asNavFor: ".carousel-main-products",
    groupCells: "100%",
    pageDots: !1,
    autoPlay: 4e3
  });
  // Swiper
  var SwiperServices = new Swiper(".swiper-feedbacks", {
    slidesPerView: "auto",
    centeredSlides: true,
    spaceBetween: 150,
    loop: true,
    speed: 1200,
    grabCursor: true,
    effect: "coverflow",
    coverflowEffect: {
      rotate: 0,
      strectch: 0,
      depth: 300,
      modifier: 1,
      slideShadows: false,
    },
    breakpoints: {
      360: {
        spaceBetween: 0,
      },
      768: {
        spaceBetween: 40,
      },
    },
    navigation: {
      nextEl: ".swiper-feedbacks-o .swiper-button-next ",
      prevEl: ".swiper-feedbacks-o .swiper-button-prev",
    },
  });
  $(".table-scroll").on("scroll", function () {
    var length = $(this).scrollLeft();
    console.log(length);
    if (length <= 0) {
      $(this).find("table thead th:nth-child(1)").removeClass("scroll");
      $(this).find("table tr td:nth-child(1)").removeClass("scroll");
    } else {
      $(this).find("table thead th:nth-child(1)").addClass("scroll");
      $(this).find("table tr td:nth-child(1)").addClass("scroll");
    }
  });
  $(".menu-vertical li").each(function () {
    let e = $(this);
    e.find("ul").length &&
      e
        .addClass("menu-vertical-drop")
        .find(">a")
        .append(
          "<i class='i-vertical i-click-vertical far fa-angle-right'></i>"
        );
  });
  $(".menu-vertical .i-click-vertical").each(function () {
    var e = $(this),
      t = e.closest("li.menu-vertical-drop");
    e.on("click", function () {
      return (
        t.parent().find(">li").not(t).removeClass("active"),
        t.toggleClass("active"),
        !1
      );
    });
  });
  $(".brief .list-color .ball").each(function () {
    let e = $(this);
    let parent = e.parent();
    e.on("click", function () {
      return (
        parent.find(".ball").removeClass("active"), e.toggleClass("active"), !1
      );
    });
  });
  $(".tab-content .content-ellips").each(function () {
    let e = $(this);
    let parent = e.parent();
    if (e.height() >= 500) {
      e.addClass("text-over");
      parent.find(".btn-toggle-content").removeClass("hidden");
    } else {
      e.removeClass("text-over");
      parent.find(".btn-toggle-content").addClass("hidden");
    }
  });
  $(".swiper-feedbacks .info").each(function () {
    let e = $(this);
    let parent = e.parent();
    if (e.height() >= 200) {
      e.addClass("text-over");
      parent.find(".btn-toggle-content").removeClass("hidden");
    } else {
      e.removeClass("text-over");
      parent.find(".btn-toggle-content").addClass("hidden");
    }
    e.on("click",function(){
      e.toggleClass("text-over");
    })
  });
  $(".form-reply").length &&
    ($(".reply-action").each(function () {
      var e = $(this).parent().next(".form-reply");
      $(this).on("click", function () {
        $(".form-reply").not(e).removeClass("active"), e.toggleClass("active");
      });
    }),
    $(".review-toggle").click(function () {
      $(this).next(".reply-group").toggleClass("active"), $(this).toggleClass("active");
    }));
});
