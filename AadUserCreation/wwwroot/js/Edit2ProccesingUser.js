$(document).ready(function () {

    var emailupnInputText = document.getElementById('emailupn_input_text');

    $("#divProcessing").hide();
    
    $("#userPost_form").on("submit", function (event) {
        event.preventDefault();
        $("#userPost_form").hide();
        $("#divProcessing").show();

        var url = "/Home/Edit?upn=" + emailupnInputText.value;
        window.location.href = url;
        //kan nog net wait
    });
});