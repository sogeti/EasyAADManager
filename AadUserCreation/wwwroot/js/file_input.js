
var fileInputTextDiv = document.getElementById('file_input_text_div');
var fileInput = document.getElementById('file_input_file');
var fileInputText = document.getElementById('file_input_text');
var filefullpath = document.getElementById('filefullpath_input_text');
var processButton = document.getElementById('process_button');


fileInput.addEventListener('change', changeInputText);
fileInput.addEventListener('change', changeState);

function changeInputText() {
    var str = fileInput.value;
    var i;
    if (str.lastIndexOf('\\')) {
        i = str.lastIndexOf('\\') + 1;
    } else if (str.lastIndexOf('/')) {
        i = str.lastIndexOf('/') + 1;
    }
    fileInputText.value = str.slice(i, str.length);
    filefullpath.value = str;
}

function changeState() {
    if (fileInputText.value.length != 0) {
        if (!fileInputTextDiv.classList.contains("is-focused")) {
            fileInputTextDiv.classList.add('is-focused');
            processButton.removeAttribute("disabled");
            componentHandler.upgradeElement(processButton);

        }
    } else {
        if (fileInputTextDiv.classList.contains("is-focused")) {
            fileInputTextDiv.classList.remove('is-focused');
            processButton.setAttribute("disabled", "");
            componentHandler.upgradeElement(processButton);

        }
    }
}