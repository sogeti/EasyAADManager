(function () {
    'use strict';

    var dialog = document.querySelector('#dialogRemove');
    

    if (!dialog.showModal) {
        dialogPolyfill.registerDialog(dialog);
    }

    dialog.querySelector('#OkayButton')
        .addEventListener('click', function () {
            
            var upn = document.querySelector('#emailupn_input_text').value;
            dialog.close();
            document.querySelector('#emailupn_input_text').value = '';
            var url = '/home/DeleteUser';

                $.ajax({
                    url: url,
                    type: "POST",
                    data: { "jsonData": upn },
                    dataType: "json",
                    success: function (response) {
                        $("#RemoveUserPost_form").show();
                        $("#errorNotify").hide();
                        $("#divProcessing").hide();
                        if (response == "Error, please notify administrator.") {
                            $("#divProcessing").hide();
                            $("#errorNotify").show();
                        }
                    }
                });
        });

    dialog.querySelector('#NoButton')
        .addEventListener('click', function () {
            dialog.close();
            $("#RemoveUserPost_form").show();
            $("#errorNotify").hide();
            $("#divProcessing").hide();
    });

}());