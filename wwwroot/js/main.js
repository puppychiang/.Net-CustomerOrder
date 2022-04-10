
$(document).ready(function () {
    $("input").click(function () {
        //alert("Hi");
        //$("H1").text("Hello");
        let numberListItem = $("li").length;
        let randomChidNumber = Math.floor(Math.random() * numberListItem);
        $("H1").text($("li").eq(randomChidNumber).text());
    });
});

