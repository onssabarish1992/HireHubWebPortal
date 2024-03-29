﻿function showLoader() {
    $(".page-loader").show();
}


function hideLoader() {
    $(".page-loader").hide();
}

function displayConfirmationMessage(argMessage, argType) {
    var alertTemplate = "<div class='alert alert-dismissible #message-type#'><a href='#' class='close' data-bs-dismiss='alert' aria-label='close'>&times;</a>#message#</div>";
    var resultTemplate = alertTemplate.replace("#message-type#", argType).replace("#message#", argMessage);
    $(".confirmation-message").html(resultTemplate);
}