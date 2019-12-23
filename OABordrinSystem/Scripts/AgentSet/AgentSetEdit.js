var $AgentSetEdit = $("#AgentSetEdit");
AgentSetParam = app.util.getEditParam($AgentSetEdit);

function GetSaveParam() {
    var para = app.util.serializeParamArray(AgentSetParam)
    para.AgentModuleList = [];
    $("div.icheckbox_square-green").each(function () {
        if ($(this).hasClass("checked")) {
            para.AgentModuleList.push($(this).find("input").first().val());
        }
    });
    return para;
}

$("#SaveAgentSet").click(function () {
    if (app.util.ValidationFromData($AgentSetEdit)) {
        $.ajax({
            url: '/AgentSet/SaveAgentSet',
            data: GetSaveParam(),
            dataType: "json",
            success: function (ans) {
                if (ans.error.length > 0) {
                    app.util.onError(ans.error, $AgentSetEdit);
                    return;
                }
                $AgentSetEdit.modal("hide");
                AgentSetDataTableObj.fnDraw();
            }
        });
    }
});

//app.util.serializeParamArray(AgentSetParam)
$(document).ready(function () {
    GetAutocompleteData();
    AgentSetParam.b_StartDate.datepicker({ autoclose: true, format: 'yyyy-mm-dd' }).on('changeDate', function (e) {
        var startTime = e.date;
        AgentSetParam.b_EndDate.datepicker('setStartDate', startTime);
    });;
    AgentSetParam.b_EndDate.datepicker({ autoclose: true, format: 'yyyy-mm-dd' }).on('changeDate', function (e) {
        var endTime = e.date;
        AgentSetParam.b_StartDate.datepicker('setEndDate', endTime);
    });
    $('.clockpicker').clockpicker();
    GetAgentModuleList();
});

AgentSetParam.b_AgentName.autocomplete({
    source: function (request, response) {
        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
        response($.grep(choiceUserList, function (value) {
            return matcher.test(value);
        }));
    }
});


var choiceUserList = [];

function GetAutocompleteData() {
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


function GetAgentModuleList() {
    $.ajax({
        url: '/AgentSet/GetAgentModuleList',
        dataType: "json",
        success: function (ans) {
            if (ans.error.length > 0) {
                return false;
            }
            $("#AgentModuleList").empty();
            if (ans.data != null && ans.data.length > 0) {
                for (var i = 0; i < ans.data.length; i++) {
                    var item = ans.data[i];
                    $("#AgentModuleList").append(" <label><input class='i-checks' type='checkbox' value='" + item.Value + "'> <i></i>&nbsp;" + item.Text + "&nbsp;</label>");
                }
            }
            $('.i-checks').iCheck({
                checkboxClass: 'icheckbox_square-green',
                radioClass: 'iradio_square-green',
            });
        }
    });
}