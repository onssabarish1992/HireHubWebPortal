


function loadCombinations() {

    var parameters = {};

    $.ajax({
        type: "POST",
        url: URLConstants["GetAHPJobPairs"],
        data: parameters,
        success: function (response) {
            $("#div_slidersection").html(response);
        },
        error: function (response) {
            console.log("Error in application")
        }
    })
}

//Reset isEqual radio button
function resetEqualWeightage(argPairId) {
    $("#" + argPairId + "_col2 input[type=radio]").prop("checked", false);
}


//Reset numeric scale of 2-9
function resetNumericScale(argPairId) {
    $("#" + argPairId + "_col3 input[type=radio]").prop("checked", false);
}


function SaveWeightage() {
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

    $.ajax({
        type: "POST",
        url: URLConstants["SaveAHPRatings"],
        data: JSON.stringify(AHPPairs),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response != null && response) {
                loadCombinations();
            }
        },
        error: function (a, b, c) {
            console.log("Error!!!")
        }
    });


}


