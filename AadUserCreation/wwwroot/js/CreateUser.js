var firstnameInputText = document.getElementById('firstname_input_text');
var lastnameInputText = document.getElementById('lastname_input_text');
var displaynameInputText = document.getElementById('displayname_input_text');
var emailupnInputText = document.getElementById('emailupn_input_text');

var suffixInputText = document.getElementById('suffix_input_text');
var departmentListSelect = document.getElementById('departmentlist_input_text');

var processButton = document.getElementById('process_button');
var hiddenMailAccountDomain = document.getElementById('hiddenMailAccountDomain');

firstnameInputText.addEventListener('change', changeInputText);
lastnameInputText.addEventListener('change', changeInputText);
suffixInputText.addEventListener('change', changeInputText);
departmentListSelect.addEventListener('change', changeInputText);

firstnameInputText.addEventListener('change', changeState);
lastnameInputText.addEventListener('change', changeState);
displaynameInputText.addEventListener('change', changeState);
emailupnInputText.addEventListener('change', changeState);


function changeInputText() {
    if (suffixInputText.value.length != 0) {
        var displayname = firstnameInputText.value + " " + suffixInputText.value +  " " + lastnameInputText.value;
        var mailadress = firstnameInputText.value + suffixInputText.value.trim()  + lastnameInputText.value + hiddenMailAccountDomain.value;
    }
    else {
        var displayname = firstnameInputText.value + " " + lastnameInputText.value;
        var mailadress = firstnameInputText.value + lastnameInputText.value + hiddenMailAccountDomain.value;
    }
    displaynameInputText.value = displayname;
    emailupnInputText.value = mailadress;
}

function changeState() {
    if (firstnameInputText.value.length != 0 && lastnameInputText.value.length != 0 && displaynameInputText.value.length != 0 && emailupnInputText.value.length != 0) {
        processButton.removeAttribute("disabled");
        componentHandler.upgradeElement(processButton);
    }
    else {
        processButton.setAttribute("disabled", "");
        componentHandler.upgradeElement(processButton);
    }
}

