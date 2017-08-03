$(document).ready(function () {

    var emailupnInputText = document.getElementById('emailupn_input_text');
    var dialog = document.querySelector('#dialogRemove');
    var dialogError = document.querySelector('#dialogError');

    $("#divProcessing").hide();
    $("#errorNotify").hide();

    $("#RemoveUserPost_form").on("submit", function (event) {
        event.preventDefault();
        dialog.showModal();
    });
});