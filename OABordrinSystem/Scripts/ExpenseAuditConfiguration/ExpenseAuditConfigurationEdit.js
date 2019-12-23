var $ExpenseAuditConfigurationEdit = $("#ExpenseAuditConfigurationEdit");
var ExpenseAuditConfigurationParam = app.util.getEditParam($ExpenseAuditConfigurationEdit);

$(document).ready(function () {
    //加载成本中心
    LoadCostCenter();

    //加载公司代码
    GetStructureCompanyCode();

    //获取项目列表
    GetProjectList();

});


//autocomplete
var availableTags = [];
function SetAutocomplete() {
    $("#b_HandlePersons")
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
        url: '/AutoComplete/GetDelegateToNames',
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

function LoadCostCenter() {
    $.ajax({
        url: '/AutoComplete/GetCostCenterList',
        dataType: "json",
        async: false,
        success: function (ans) {
            if (ans.error.length > 0) {
                return false;
            }
            ExpenseAuditConfigurationParam.b_CostCenters.empty();
            ExpenseAuditConfigurationParam.b_CostCenters.append("<option value=''>请选择</option>");
            if (ans != null && ans.data != null) {
                for (var i = 0; i < ans.data.length; i++) {
                    var value = ans.data[i];
                    ExpenseAuditConfigurationParam.b_CostCenters.append("<option value='" + value + "'>" + value + "</option>");
                }
            }

            var config = {
                '.chosen-select': {},
                '.chosen-select-deselect': { allow_single_deselect: true },
                '.chosen-select-no-single': { disable_search_threshold: 10 },
                '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
                '.chosen-select-width': { width: "95%" }
            }
            for (var selector in config) {
                $(selector).chosen(config[selector]);
            }

            $(".chosen-container").css("width", "100%");
        }
    });
}


//获取公司代码列表
function GetStructureCompanyCode() {
    $.ajax({
        url: '/AutoComplete/GetStructureCompanyCode',
        dataType: "json",
        async: false,
        success: function (ans) {
            if (ans.error.length > 0) {
                return false;
            }
            ExpenseAuditConfigurationParam.b_CompanyCode.empty();
            ExpenseAuditConfigurationParam.b_CompanyCode.append("<option value=''>选择公司代码</option>");
            if (ans.data != null && ans.data.length > 0) {
                for (var i = 0; i < ans.data.length; i++) {
                    var value = ans.data[i];
                    ExpenseAuditConfigurationParam.b_CompanyCode.append("<option value='" + value + "'>" + value + "</option>");
                }
            }
        }
    });
}

function GetProjectList() {
    $.ajax({
        url: '/AutoComplete/GetProjectList',
        dataType: "json",
        success: function (ans) {
            if (ans.error.length > 0) {
                return false;
            }
            ExpenseAuditConfigurationParam.b_ProjectName.empty();
            ExpenseAuditConfigurationParam.b_ProjectName.append("<option value=''>选择项目名称</option>");
            if (ans.data != null && ans.data.length > 0) {
                for (var i = 0; i < ans.data.length; i++) {
                    var value = ans.data[i];
                    ExpenseAuditConfigurationParam.b_ProjectName.append("<option value='" + value + "'>" + value + "</option>")
                }
            }
        }
    });
}

ExpenseAuditConfigurationParam.b_Type.change(function () {
    var value = $(this).val();
    if (value == "Project")
    {
        $("#b_CostCenters").hide();
        $("#nProjectName").show();
    }
    else
    {
        $("#b_CostCenters").show();
        $("#nProjectName").hide();
    }
});

function GetSaveParam()
{
    var param = app.util.serializeParamArray(ExpenseAuditConfigurationParam);
    return param;
}








