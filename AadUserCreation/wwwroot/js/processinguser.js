$(document).ready(function () {
    var firstnameInputText = document.getElementById('firstname_input_text');
    var lastnameInputText = document.getElementById('lastname_input_text');
    var displaynameInputText = document.getElementById('displayname_input_text');
    var emailupnInputText = document.getElementById('emailupn_input_text');

    var suffixInputText = document.getElementById('suffix_input_text');
    var dialog = document.querySelector('#dialog');
    var dialogError = document.querySelector('#dialogError');

    $("#divProcessing").hide();
    $("#errorNotify").hide();

    
    $("#userPost_form").on("submit", function (event) {
        event.preventDefault();
        $("#userPost_form").hide();
        $("#errorNotify").hide();
        $("#divProcessing").show();

        var url = '/home';

        var formData = $(this).serialize();
        $.ajax({
            url: url,
            type: "POST",
            data: formData,
            dataType: "json",
            success: function (response) {
                if (response == "Error, please notify administrator.") {
                    $("#divProcessing").hide();
                    $("#errorNotify").show();
                }
                $("#divProcessing").hide();

                firstnameInputText.value = "";
                lastnameInputText.value = "";
                displaynameInputText.value = "";
                emailupnInputText.value = "";
                suffixInputText.value = "";

                $("#jobId").text(response);

                $("#userPost_form").show();
                dialog.showModal();
            }
        })
    });
});