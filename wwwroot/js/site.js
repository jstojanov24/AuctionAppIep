// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function filter(){
    var min = $("#min").val ( )
    var max = $("#max").val ( )
    var word = $("#word").val ( )

    $.ajax ({
        type: "GET",
        url: "/Home/Filter",
        data: {
            "min" : min,
            "max" : max,
            "word":word
           
        },
        dataType: "text",
        success: function ( response ) {
           $("#auctions").html ( response )
        },
        error: function ( response ) {
            alert ( response )
        }
    })
}

function next (s1,current ) {
    $.ajax({
        type: "GET",
        url: "/Home/Next",
        data: {
            "currentpage":current,
            "word":s1
        },
        dataType: "text",
        success: function ( response ) {
            $("#ubicuse").html ( response )
        },
        error: function ( response ) {
            alert ( response )
        }
    })
}

function updateTimer ( ) {
    // document.getElementById ( "timer" )
    var hidd=$("#count").val( );
    //cnt=parseInt(hidd);
    for( i=1;i<=hidd; i++){
    var string = $("#"+i).text ( )

    var array = string.split ( ":" )

    var days= parseInt ( array[0] )
    var hours = parseInt ( array[1] )
    var minutes = parseInt ( array[2] )
    var seconds = parseInt ( array[3] )

    var timeInSeconds = 86400*days+hours * 3600 + minutes * 60 + seconds - 1

    seconds = timeInSeconds % 60
    minutes = Math.floor((timeInSeconds%3600)/60)
    hours = Math.floor((timeInSeconds%86400)/3600);
    days= Math.floor ( timeInSeconds / 86400 )

    if ( seconds < 10 ) {
        seconds = "0" + seconds
    }

    if ( minutes < 10 ) {
        minutes = "0" + minutes
    }

    if ( hours < 10 ) {
        hours = "0" + hours
    }
    if ( days < 10 ) {
        days = "0" + days
    }

    $("#"+i).text ( days+":"+hours + ":" + minutes + ":" + seconds )
}
}

setInterval ( updateTimer, 1500 )