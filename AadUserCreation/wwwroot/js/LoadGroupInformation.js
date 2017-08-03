$(document).ready(function () {
    var mailInputText = document.getElementById('emailupn_input_text');
    mailInputText.addEventListener('change', GetData);
});

function GetData() {

    var emailupnInputText = document.getElementById('emailupn_input_text');

    $.ajax({
        type: "POST",
        url: "/Home/GetGroupsAndDepartmentsForUPN", // the URL of the controller action method
        data: { "jsonData": emailupnInputText.value },
        dataType: "json",
        success: function (result) {
            var url = '@Url.Action("Home", "Edit2")';
            window.location.href = url;
        },
        error: function (req, status, error) {
            // do something with error   
        }
    });
}