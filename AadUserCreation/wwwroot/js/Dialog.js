(function () {
    'use strict';
    //var dialogButton = document.querySelector('.dialog-button');
    var dialog = document.querySelector('#dialog');
    

    if (!dialog.showModal) {
        dialogPolyfill.registerDialog(dialog);
    }

    dialog.querySelector('#informationButton')
        .addEventListener('click', function () {
            var responsejobId = document.querySelector('#jobId').textContent;
            dialog.close();
            var querystring = "LogEntries/Details?id=" + responsejobId;
            document.location = querystring;
        });

    dialog.querySelector('#closeButton')
        .addEventListener('click', function () {
            dialog.close();
        });

}());