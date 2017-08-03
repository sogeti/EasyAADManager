$(document).ready(function () {


    var divProccesing = document.getElementById('divProcessing');
    var divState = document.getElementById('divState');

    $("#divProcessing").hide();

});

function RefreshBlob() {

    var divProccesing = document.getElementById('divProcessing');
    var divState = document.getElementById('divState');

    $("#divState").hide();
    $("#divProcessing").show();

    $.ajax({
        type: "POST",
        url: "/Home/PostRefreshCollectionBlob", // the URL of the controller action method
        success: function (result) {
            var querystring = "RefreshCollectionBlob";
            document.location = querystring;
        },
        error: function (req, status, error) {
            // do something with error   
        }
    });
}
