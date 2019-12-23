var $WorkflowActivitySignModel = $("#WorkflowActivitySign");
WorkflowActivitySignParam = app.util.getEditParam($WorkflowActivitySignModel);

$(document).ready(function () {
    LoadAutocompleteData();
    SetAutocomplete();
});

//autocomplete
var availableTags = [];
function SetAutocomplete() {
    $("#PersonSignList")
     // don't navigate away from the field on tab when selecting an item
    .on("keydown", function (event) {
        if (event.keyCode === $.ui.keyCode.TAB &&
            $(this).autocomplete("instance").menu.active) {
            event.preventDefault();
        }
    })
    .autocomplete({
        minLength: 0,
        source: function (request, response) {
            // delegate back to autocomplete, but extract the last term
            response($.ui.autocomplete.filter(
              availableTags, extractLast(request.term)));
        },
        focus: function () {
            // prevent value inserted on focus
            return false;
        },
        select: function (event, ui) {
            var terms = split(this.value);
            // remove the current input
            terms.pop();
            // add the selected item
            terms.push(ui.item.value);
            // add placeholder to get the comma-and-space at the end
            terms.push("");
            this.value = terms.join(";");
            return false;
        }
    });
}

function split(val) {
    return val.split(/;\s*/);
}
function extractLast(term) {
    return split(term).pop();
}


function LoadAutocompleteData() {
    //获取User数据
    $.ajax({
        url: '/AutoComplete/GetUserList',
        dataType: "json",
        async: false,
        success: function (ans) {
            if (ans.error.length > 0) {
                return false;
            }
            availableTags = [];
            if (ans.data != null && ans.data.length > 0) {
                for (var i = 0; i < ans.data.length; i++) {
                    availableTags.push(ans.data[i]);
                }
            }
        }
    });
}
