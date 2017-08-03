
var emailupnInputText = document.getElementById('emailupn_input_text');
var processButton = document.getElementById('process_button');
var hiddenMailAccountDomain = document.getElementById('hiddenMailAccountDomain');

emailupnInputText.addEventListener('change', changeState);

function changeState() {
    if (emailupnInputText.value.length != 0 ) {
        processButton.removeAttribute("disabled");
        componentHandler.upgradeElement(processButton);
    }
    else {
        processButton.setAttribute("disabled", "");
        componentHandler.upgradeElement(processButton);
    }
}

