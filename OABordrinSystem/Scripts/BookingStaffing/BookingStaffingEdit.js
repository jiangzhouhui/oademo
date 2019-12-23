var $BookingStaffingEdit = $("#BookingStaffingEdit");
var BookingStaffingParam = app.util.getEditParam($BookingStaffingEdit);

$(document).ready(function () {
    GetAutocompleteChoiceUserData();
});

function GetSaveParam() {
    var param = app.util.serializeParamArray(BookingStaffingParam);
    return param;
}

BookingStaffingParam.b_UserName.autocomplete({
    source: function (request, response) {
        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
        response($.grep(choiceUserList, function (value) {
            return matcher.test(value);
        }));
    }
});

var choiceUserList = [];
function GetAutocompleteChoiceUserData() {
    $.ajax({
        url: '/AutoComplete/GetDelegateToNames',
        dataType: "json",
        success: function (ans) {
            if (ans.error.length > 0) {
                return false;
            }
            choiceUserList = [];
            if (ans.data != null && ans.data.length > 0) {
                for (var i = 0; i < ans.data.length; i++) {
                    choiceUserList.push(ans.data[i]);
                }
            }
        }
    });
}