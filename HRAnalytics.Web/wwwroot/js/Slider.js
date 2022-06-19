
function loadCombinations() {

    showLoader();
    var parameters = {};

    $.ajax({
        type: "POST",
        url: URLConstants["GetAHPJobPairs"],
        data: parameters,
        success: function (response) {
            $("#div_slidersection").html(response);
            hideLoader();
        },
        error: function (response) {
            hideLoader();
            console.log("Error in application")
        }
    });
}

//On change event of dropdown
function populateCriterias() {
    var selectedValue = $("#ddl_job_role").val();

    showLoader();

    if (selectedValue) {
        $.ajax({
            type: "POST",
            url: URLConstants["GetAHHCriteriaPairs"],
            data: jQuery.param({ argJobIdParam: selectedValue }),
            success: function (response) {
                $("#div_slidersection").html(response);
                $(".btn_Save").show();

                hideLoader();
            },
            error: function (response) {
                hideLoader();
                console.log(response)
                console.log("Error in application")
            }
        });

        $("input[type=button]").show();
    }
    else {
        $("#div_slidersection").html('');
        $("input[type=button]").hide();
    }
}

//Reset isEqual radio button
function resetEqualWeightage(argPairId) {
    $("#" + argPairId + "_col2 input[type=radio]").prop("checked", false);
}


//Reset numeric scale of 2-9
function resetNumericScale(argPairId) {
    $("#" + argPairId + "_col3 input[type=radio]").prop("checked", false);
}


function SaveWeightage(argEntityID) {
    var AHPPairs = [];

    $('#table_scorings > tbody  > tr').each(function (index, tr) {
        var RatingViewModel = new Object();
        var rowid = $(this).attr('id');
        RatingViewModel.PairId = parseInt(rowid);

        //If both options are equal pass option as 1
        if ($("#" + rowid + "_col2 input[type=radio]").is(':checked')) {
            RatingViewModel.Rating = 1
        }
        else {

            //Get which radio button is checked
            var checkedoption = $("#" + rowid + "_col1 input[type=radio]:checked").val();

            var selectedOption = $("#" + rowid + "_col3 input[type=radio]:checked").val();

            if (checkedoption == "p1") {
                RatingViewModel.Rating = selectedOption;
            }
            else {
                RatingViewModel.Rating = -selectedOption;
            }
        }
        AHPPairs.push(RatingViewModel);
    });

    showLoader();
    $.ajax({
        type: "POST",
        url: URLConstants["SaveAHPRatings"],
        data: JSON.stringify(AHPPairs),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response != null && response) {
                if (argEntityID == 1) {
                    loadCombinations();
                }
                else {
                    populateCriterias();
                }
                displayConfirmationMessage('Data saved successfully', 'alert-success');
                hideLoader();
            }
        },
        error: function (a, b, c) {
            displayConfirmationMessage('Request could not be processed successfully', 'alert-danger');
            hideLoader();
        }
    });


}








