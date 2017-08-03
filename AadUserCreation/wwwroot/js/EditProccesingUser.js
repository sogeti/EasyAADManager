$(document).ready(function () {
 
    var emailupnInputText = document.getElementById('emailupn_input_text');

    var dialog = document.querySelector('#dialog');
    var dialogError = document.querySelector('#dialogError');

    $("#divProcessing").hide();
    $("#errorNotify").hide();

    $("#userPost_form").on("submit", function (event) {
        event.preventDefault();
        $("#userPost_form").hide();
        $("#errorNotify").hide();
        $("#divProcessing").show();

        var url = '/home/EditUser';

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
   

                $("#jobId").text(response);

                $("#userPost_form").show();
                dialog.showModal();

               
            }
        })
    });
});