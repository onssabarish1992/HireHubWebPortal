$(document).ready(function () {
    registerChangeEvent();
});

//Register change events on page load
function registerChangeEvent() {
    $("#job-role").change(populateCriterias)
}

//On change event of job role
function populateCriterias() {
    var jobId = $("#job-role").val();
    loadCriterias(jobId);
}

//Ajax call
function loadCriterias(argJobId) {

    var parameters = { 'argJobId': argJobId };
    $.ajax({
        type: "GET",
        url: URL["GetCriteriaForJobs"],
        data: parameters,
        success: function (response) {
            var criterias = $('#job-criteria');

            // clear and add default (null) option
            criterias.empty().append($('<option></option>').val('').text('--Select--'));

            $.each(response, function (index, item) {
                criterias.append($('<option></option>').val(item.criteriaID).text(item.criteriaName));
            });
        },
        error: function (response) {
            console.log("Error in loadCriterias...")
        }
    })
}
