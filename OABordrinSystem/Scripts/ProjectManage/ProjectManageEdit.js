var $ProjectManageEdit = $("#ProjectManageEdit");
ProjectManageParam = app.util.getEditParam($ProjectManageEdit);

$("#SaveProjectManage").click(function () {
    if (app.util.ValidationFromData($ProjectManageEdit))
    {
        //验证输入是否为数字
        if (!app.util.ValidNumber(ProjectManageParam.b_Sort)) {
            return;
        }
        $.ajax({
            url: '/ProjectManage/SaveProjectManage',
            data: app.util.serializeParamArray(ProjectManageParam),
            dataType: "json",
            success: function (ans) {
                if (ans.error.length > 0) {
                    app.util.onError(ans.error, $ProjectManageEdit);
                    return;
                }
                $ProjectManageEdit.modal("hide");
                ProjectManageDataTableObj.fnDraw();
            }
        });
    }
});

var choiceUserList = [];


ProjectManageParam.b_ProjectDirector.autocomplete({
    source: function (request, response) {
        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
        response($.grep(choiceUserList, function (value) {
            return matcher.test(value);
        }));
    }
});


$(document).ready(function () {
    GetAutocompleteData();
    ProjectManageParam.b_Sort.number();
    ProjectManageParam.b_PmtOrPatLeader.autocomplete({

        source: function (request, response) {
            var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
            response($.grep(choiceUserList, function (value) {
                return matcher.test(value);
            }));
        }

    });
});

ProjectManageParam.b_ProjectManager.autocomplete({
    source: function (request, response) {
        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
        response($.grep(choiceUserList, function (value) {
            return matcher.test(value);
        }));
    }
});

ProjectManageParam.b_ProjectVP.autocomplete({
    source: function (request, response) {
        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
        response($.grep(choiceUserList, function (value) {
            return matcher.test(value);
        }));
    }
});


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



