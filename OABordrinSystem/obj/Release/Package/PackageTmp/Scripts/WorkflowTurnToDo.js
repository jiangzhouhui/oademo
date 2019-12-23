var $WorkflowTurnToDoModel = $("#WorkflowTurnToDo");

WorkflowTurnToDoParam = app.util.getEditParam($WorkflowTurnToDoModel);



$(document).ready(function () {
    GetAutocompleteChoiceIdentityData();
});


var choiceIdentityList = [];
function GetAutocompleteChoiceIdentityData() {
    $.ajax({
        url: '/AutoComplete/GetUserList',
        dataType: "json",
        success: function (ans) {
            if (ans.error.length > 0) {
                return false;
            }
            choiceIdentityList = [];
            if (ans.data != null && ans.data.length > 0) {
                for (var i = 0; i < ans.data.length; i++) {
                    choiceIdentityList.push(ans.data[i]);
                }
            }
        }
    });
}

WorkflowTurnToDoParam.delegateToName.autocomplete({
    source: function (request, response) {
        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
        response($.grep(choiceIdentityList, function (value) {
            return matcher.test(value);
        }));
    }
});


