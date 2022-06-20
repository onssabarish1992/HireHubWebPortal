
function SaveData(argType) {
    showLoader();

    $.ajax({
        type: "POST",
        url: URLConstants["ProcessData"],
        data: jQuery.param({ argRequestType: argType }),
        success: function (response) {
            displayConfirmationMessage('Data processed successfully', 'alert-success alert-dismissible');
            hideLoader();
        },
        error: function (response) {
            displayConfirmationMessage('Error in Application', 'alert-danger');
            hideLoader();
        }
    });
}