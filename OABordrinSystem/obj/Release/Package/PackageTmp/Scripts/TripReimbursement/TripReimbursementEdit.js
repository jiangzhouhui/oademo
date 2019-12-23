//差旅报销
var $TripReimbursementEdit = $("#TripReimbursementEdit");
//住宿费
var $HotelExpenseItem = $("#HotelExpenseItem"), $HotelExpenseItemTr = $("#HotelExpenseItemTr"), HotelExpenseItemHtml = $HotelExpenseItemTr.html();
//交通费
var $TrafficExpenseItem = $("#TrafficExpenseItem"), $TrafficExpenseItemTr = $("#TrafficExpenseItemTr"), TrafficExpenseItemHtml = $TrafficExpenseItemTr.html();
//餐费及固定补贴
var $MealsSubsidiesItem = $("#MealsSubsidiesItem"), $MealsSubsidiesItemTr = $("#MealsSubsidiesItemTr"), MealsSubsidiesItemHtml = $MealsSubsidiesItemTr.html();
//其他（签证/其他费用）
var $OthersItem = $("#OthersItem"), $OthersItemTr = $("#OthersItemTr"), OthersItemHtml = $OthersItemTr.html();
//借款明细
var $LoanItems = $("#LoanItems"), $LoanItemsTr = $("#LoanItemsTr"), LoanItemsHtml = $LoanItemsTr.html();

TripReimbursementParam = app.util.getEditParam($TripReimbursementEdit);

$(document).ready(function () {

    TripReimbursementParam.b_AttachmentsQuantity.number();

    GetAutocompleteChoiceUserData();

    //加载项目名称
    GetProjectList();

    //加载地区列表
    GetRegionList();

    //加载币种种类列表
    GetCurrencyTypeList();

    //加载公司代码列表
    GetStructureCompanyCode();

    //加载交通类别
    GetTrafficCategoryList();

    //加载费用类别
    //GetExpenseCategoryList();

    //获取税率码
    GetTaxCodeConfigure();


});


function GetSaveParam(operation) {
    var param = app.util.serializeParamArray(TripReimbursementParam);
    param.operation = operation;
    param.b_AdvancedAmount = TripReimbursementParam.b_AdvancedAmount.html()
    param.b_TotalExpense = TripReimbursementParam.b_TotalExpense.html();
    param.b_AmountInTotal = TripReimbursementParam.b_AmountInTotal.html();
    param.b_IsBeyondBudget = TripReimbursementParam.b_IsBeyondBudget.html();

    //获取单选按钮的值
    var b_IsBudgetary;
    $TripReimbursementEdit.find("input[name='b_IsBudgetary']").each(function () {
        var $this = $(this);
        if ($this.closest("div").hasClass("checked")) {
            b_IsBudgetary = $this.val();
        }
    });
    param.b_IsBudgetary = b_IsBudgetary;
    //获取单选按钮的值
    var b_IntalBusiness;
    $TripReimbursementEdit.find("input[name='b_IntalBusiness']").each(function () {
        var $this = $(this);
        if ($this.closest("div").hasClass("checked")) {
            b_IntalBusiness = $this.val();
        }
    });
    param.b_IntalBusiness = b_IntalBusiness;
    //获取单选按钮的值
    var b_Type = "";
    $TripReimbursementEdit.find("input[name='b_Type']").each(function () {
        var $this = $(this);
        if ($this.closest("div").hasClass("checked")) {
            b_Type = $this.val();
        }
    });
    param.b_Type = b_Type;

    //获取住宿费
    var HEitems = [];
    $HotelExpenseItemTr.find("tr").each(function () {
        var $this = $(this);
        var id = $.trim($this.find("input.id").val());
        var b_StartDate = $.trim($this.find("input.b_StartDate").val());
        var b_EndDate = $.trim($this.find("input.b_EndDate").val());
        var b_ProjectName = $.trim($this.find("select.b_ProjectName").val());
        if ($.trim($this.find("input.b_ProjectName").val()) != "") {
            b_ProjectName = $.trim($this.find("input.b_ProjectName").val());
        }
        var b_City = $.trim($this.find("input.b_City").val());
        var b_Hotel = $.trim($this.find("input.b_Hotel").val());
        var b_Currency = $.trim($this.find("select.b_Currency").val());
        var b_Rate = $.trim($this.find("input.b_Rate").val());
        var b_OriginalCurrency = $.trim($this.find("input.b_OriginalCurrency").val());
        var b_Count = $.trim($this.find("input.b_Count").val());
        var b_TaxRate = $.trim($this.find("select.b_TaxRate").val());
        var b_Tax = $.trim($this.find("input.b_Tax").val());
        var b_TaxFreeAmount = $.trim($this.find("input.b_TaxFreeAmount").val());
        var b_CNYSubtotal = $.trim($this.find("label.b_CNYSubtotal").html());
        if (b_Currency != "" && b_Rate != "" && b_OriginalCurrency != "" && b_Count != "" && b_TaxRate != "") {
            HEitems.push({ "id": id, "b_StartDate": b_StartDate, "b_EndDate": b_EndDate, "b_ProjectName": b_ProjectName, "b_City": b_City, "b_Hotel": b_Hotel, "b_Currency": b_Currency, "b_Rate": b_Rate, "b_OriginalCurrency": b_OriginalCurrency, "b_Count": b_Count, "b_TaxRate": b_TaxRate, "b_Tax": b_Tax, "b_TaxFreeAmount": b_TaxFreeAmount, "b_CNYSubtotal": b_CNYSubtotal });
        }
    });

    param.HotelExpenseItems = HEitems;

    //获取交通费
    var TEitems = [];
    $TrafficExpenseItemTr.find("tr").each(function () {
        var $this = $(this);
        var id = $.trim($this.find("input.id").val());
        var b_StartDate = $.trim($this.find("input.b_StartDate").val());
        var b_EndDate = $.trim($this.find("input.b_EndDate").val());
        var b_ProjectName = $.trim($this.find("select.b_ProjectName").val());
        if ($.trim($this.find("input.b_ProjectName").val()) != "") {
            b_ProjectName = $.trim($this.find("input.b_ProjectName").val());
        }
        var b_Type = $.trim($this.find("select.b_Type").val());
        var b_StartPoint = $.trim($this.find("input.b_StartPoint").val());
        var b_EndPoint = $.trim($this.find("input.b_EndPoint").val());
        var b_Currency = $.trim($this.find("select.b_Currency").val());
        var b_Rate = $.trim($this.find("input.b_Rate").val());
        var b_OriginalCurrency = $.trim($this.find("input.b_OriginalCurrency").val());
        var b_Count = $.trim($this.find("input.b_Count").val());
        var b_TaxRate = $.trim($this.find("select.b_TaxRate").val());
        var b_Tax = $.trim($this.find("input.b_Tax").val());
        var b_TaxFreeAmount = $.trim($this.find("input.b_TaxFreeAmount").val());
        var b_CNYSubtotal = $.trim($this.find("label.b_CNYSubtotal").html());
        if (b_StartDate != "" && b_EndDate != "" && b_Currency != "" && b_Rate != "" && b_OriginalCurrency != "" && b_Count != "" && b_TaxRate != "") {
            TEitems.push({ "id": id, "b_StartDate": b_StartDate, "b_EndDate": b_EndDate, "b_ProjectName": b_ProjectName, "b_Type": b_Type, "b_StartPoint": b_StartPoint, "b_EndPoint": b_EndPoint, "b_Currency": b_Currency, "b_Rate": b_Rate, "b_OriginalCurrency": b_OriginalCurrency, "b_Count": b_Count, "b_TaxRate": b_TaxRate, "b_Tax": b_Tax, "b_TaxFreeAmount": b_TaxFreeAmount, "b_CNYSubtotal": b_CNYSubtotal });
        }
    });
    param.TrafficExpenseItems = TEitems;

    //获取餐费及固定补贴
    var MBitems = [];
    $MealsSubsidiesItemTr.find("tr").each(function () {
        var $this = $(this);
        var id = $.trim($this.find("input.id").val());
        var b_StartDate = $.trim($this.find("input.b_StartDate").val());
        var b_EndDate = $.trim($this.find("input.b_EndDate").val());
        var b_ProjectName = $.trim($this.find("select.b_ProjectName").val());
        if ($.trim($this.find("input.b_ProjectName").val()) != "") {
            b_ProjectName = $.trim($this.find("input.b_ProjectName").val());
        }
        var b_Place = $.trim($this.find("input.b_Place").val());
        var b_CompanionAmount = $.trim($this.find("input.b_CompanionAmount").val());
        var b_CompanionName = $.trim($this.find("input.b_CompanionName").val());
        var b_Currency = $.trim($this.find("select.b_Currency").val());
        var b_Rate = $.trim($this.find("input.b_Rate").val());
        var b_FixedSubsidy = $.trim($this.find("input.b_FixedSubsidy").val());
        var b_TaxRate = $.trim($this.find("select.b_TaxRate").val());
        var b_Tax = $.trim($this.find("input.b_Tax").val());
        var b_TaxFreeAmount = $.trim($this.find("input.b_TaxFreeAmount").val());
        var b_CNYSubtotal = $.trim($this.find("label.b_CNYSubtotal").html());
        if (b_StartDate != "" && b_EndDate != "" && b_Currency != "" && b_Rate != "" && b_TaxRate != "") {
            MBitems.push({ "id": id, "b_StartDate": b_StartDate, "b_EndDate": b_EndDate, "b_ProjectName": b_ProjectName, "b_Place": b_Place, "b_CompanionAmount": b_CompanionAmount, "b_CompanionName": b_CompanionName, "b_Currency": b_Currency, "b_Rate": b_Rate, "b_FixedSubsidy": b_FixedSubsidy, "b_TaxRate": b_TaxRate, "b_Tax": b_Tax, "b_TaxFreeAmount": b_TaxFreeAmount, "b_CNYSubtotal": b_CNYSubtotal });
        }
    });
    param.MealsSubsidiesItems = MBitems;

    //获取其他
    var OSitems = [];
    $OthersItemTr.find("tr").each(function () {
        var $this = $(this);
        var id = $.trim($this.find("input.id").val());
        var b_StartDate = $.trim($this.find("input.b_StartDate").val());
        var b_EndDate = $.trim($this.find("input.b_EndDate").val());
        var b_ProjectName = $.trim($this.find("select.b_ProjectName").val());
        if ($.trim($this.find("input.b_ProjectName").val()) != "") {
            b_ProjectName = $.trim($this.find("input.b_ProjectName").val());
        }
        var b_Place = $.trim($this.find("input.b_Place").val());
        var b_Type = $.trim($this.find("input.b_Type").val());
        var b_Reason = $.trim($this.find("input.b_Reason").val());
        var b_Currency = $.trim($this.find("select.b_Currency").val());
        var b_Rate = $.trim($this.find("input.b_Rate").val());
        var b_OriginalCurrency = $.trim($this.find("input.b_OriginalCurrency").val());
        var b_Count = $.trim($this.find("input.b_Count").val());
        var b_TaxRate = $.trim($this.find("select.b_TaxRate").val());
        var b_Tax = $.trim($this.find("input.b_Tax").val());
        var b_TaxFreeAmount = $.trim($this.find("input.b_TaxFreeAmount").val());
        var b_CNYSubtotal = $.trim($this.find("label.b_CNYSubtotal").html());

        if (b_StartDate != "" && b_EndDate != "" && b_Currency != "" && b_Rate != "" && b_OriginalCurrency != "" && b_Count != "" && b_TaxRate != "") {
            OSitems.push({ "id": id, "b_StartDate": b_StartDate, "b_EndDate": b_EndDate, "b_ProjectName": b_ProjectName, "b_Place": b_Place, "b_Type": b_Type, "b_Reason": b_Reason, "b_Currency": b_Currency, "b_Rate": b_Rate, "b_OriginalCurrency": b_OriginalCurrency, "b_Count": b_Count, "b_TaxRate": b_TaxRate, "b_Tax": b_Tax, "b_TaxFreeAmount": b_TaxFreeAmount, "b_CNYSubtotal": b_CNYSubtotal });
        }
    });

    param.OthersItems = OSitems;

    //获取借款明细
    var LNitems = [];
    $LoanItemsTr.find("tr").each(function () {
        var $this = $(this);
        var id = $.trim($this.find("input.id").val());
        var b_LoanOrderNo = $.trim($this.find("input.b_LoanOrderNo").val());
        var b_Date = $.trim($this.find("input.b_Date").val());
        var b_Borrower = $.trim($this.find("input.b_Borrower").val());
        var b_LoanAmount = $.trim($this.find("input.b_LoanAmount").val());
        var b_LoanReason = $.trim($this.find("input.b_LoanReason").val());
        if (b_LoanOrderNo != "" && b_Date != "" && b_Borrower != "" && b_LoanAmount != "" && b_LoanReason != "") {
            LNitems.push({ "id": id, "b_LoanOrderNo": b_LoanOrderNo, "b_Date": b_Date, "b_Borrower": b_Borrower, "b_LoanAmount": b_LoanAmount, "b_LoanReason": b_LoanReason });
        }
    });
    param.LoanItems = LNitems;

    //上传的文件集
    var fileIds = [];
    $("#fileList").find("a.fileObj").each(function () {
        var id = $(this).attr("id");
        var source_id = $(this).attr("source_id");
        if (source_id == "" || source_id == undefined) {
            fileIds.push(id);
        }
    });
    param.fileIds = fileIds;

    return param;
}

//获取审核数据
function GetAuditTripReimbursement() {
    var param = app.util.serializeParamArray(TripReimbursementParam);
    param.b_AdvancedAmount = TripReimbursementParam.b_AdvancedAmount.html()
    param.b_TotalExpense = TripReimbursementParam.b_TotalExpense.html();
    param.b_AmountInTotal = TripReimbursementParam.b_AmountInTotal.html();

    //获取单选按钮的值
    var b_IsBudgetary;
    $TripReimbursementEdit.find("input[name='b_IsBudgetary']").each(function () {
        var $this = $(this);
        if ($this.closest("div").hasClass("checked")) {
            b_IsBudgetary = $this.val();
        }
    });
    param.b_IsBudgetary = b_IsBudgetary;
    //获取单选按钮的值
    var b_IntalBusiness;
    $TripReimbursementEdit.find("input[name='b_IntalBusiness']").each(function () {
        var $this = $(this);
        if ($this.closest("div").hasClass("checked")) {
            b_IntalBusiness = $this.val();
        }
    });
    param.b_IntalBusiness = b_IntalBusiness;
    //获取单选按钮的值
    var b_Type = "";
    $TripReimbursementEdit.find("input[name='b_Type']").each(function () {
        var $this = $(this);
        if ($this.closest("div").hasClass("checked")) {
            b_Type = $this.val();
        }
    });
    param.b_Type = b_Type;

    return param;
}


//子表小窗口样式显示
function displayModelSetting() {
    var status = TripReimbursementParam.status.val();

    $TripReimbursementEdit.find("div.modal-footer").find("button").each(function () {
        $(this).hide();
    });
    $("#btnClose").show();
    if (status == "" || status == "Start") {
        app.util.ModelToEdit($TripReimbursementEdit);
        if (TripReimbursementParam.b_IsHangUp.val() == "true") {
            app.util.ModelToDetails(TripReimbursementParam.b_ReimbursementPlace.closest("div"));
            app.util.ModelToDetails(TripReimbursementParam.b_CompanyCode.closest("div"));
        }

        //显示操作按钮
        $HotelExpenseItemTr.find(".AddHotelExpenseItem,.delHotelExpenseItem").show();
        $TrafficExpenseItemTr.find(".AddTrafficExpenseItem,.delTrafficExpenseItem").show();
        $MealsSubsidiesItemTr.find(".AddMealsSubsidiesItem,.delMealsSubsidiesItem").show();
        $OthersItemTr.find(".AddOthersItem,.delOthersItem").show();
        $LoanItemsTr.find(".AddLoanItems,.delLoanItems").show();
        $("#SaveTripReimbursement").show();
        $("#SubmitTripReimbursement").show();
    }
    else if (status == "Dept.Manager" || status == "Dept.Director" || status == "Division VP" || status == "Project Manager" || status == "Project Director" || status == "Project VP" || status == "GM") {
        app.util.ModelToDetails($TripReimbursementEdit);
        //隐藏操作按钮
        $HotelExpenseItemTr.find(".AddHotelExpenseItem,.delHotelExpenseItem").hide();
        $TrafficExpenseItemTr.find(".AddTrafficExpenseItem,.delTrafficExpenseItem").hide();
        $MealsSubsidiesItemTr.find(".AddMealsSubsidiesItem,.delMealsSubsidiesItem").hide();
        $OthersItemTr.find(".AddOthersItem,.delOthersItem").hide();
        $LoanItemsTr.find(".AddLoanItems,.delLoanItems").hide();
        $("#Approve").show();
        $("#SendRefuse").show();
    }
    else if (status == "Expense Accountant Check" || status == "Expense Accountant Creation") {
        app.util.ModelToDetails($TripReimbursementEdit);
        app.util.ModelToEdit($HotelExpenseItem);
        app.util.ModelToEdit($TrafficExpenseItem);
        app.util.ModelToEdit($MealsSubsidiesItem);
        app.util.ModelToEdit($OthersItem);
        app.util.ModelToEdit($LoanItems);
        app.util.ModelToEdit(TripReimbursementParam.b_AttachmentsQuantity.closest("div"));
        //app.util.ModelToEdit(TripReimbursementParam.b_BudgetNumber.closest("div"));
        app.util.ModelToEdit(TripReimbursementParam.b_HotelInWords.closest("div"));
        app.util.ModelToEdit(TripReimbursementParam.b_HotelAmount.closest("div"));
        app.util.ModelToEdit(TripReimbursementParam.b_TrafInWords.closest("div"));
        app.util.ModelToEdit(TripReimbursementParam.b_TrafAmount.closest("div"));
        app.util.ModelToEdit(TripReimbursementParam.b_MealInWords.closest("div"));
        app.util.ModelToEdit(TripReimbursementParam.b_MealAmount.closest("div"));
        app.util.ModelToEdit(TripReimbursementParam.b_OthInWords.closest("div"));
        app.util.ModelToEdit(TripReimbursementParam.b_OthAmount.closest("div"));
        app.util.ModelToDetails($HotelExpenseItemTr.find(".b_ProjectName").closest("td"));
        app.util.ModelToDetails($TrafficExpenseItemTr.find(".b_ProjectName").closest("td"));
        app.util.ModelToDetails($MealsSubsidiesItemTr.find(".b_ProjectName").closest("td"));
        app.util.ModelToDetails($OthersItemTr.find(".b_ProjectName").closest("td"));

        //显示操作按钮
        $HotelExpenseItemTr.find(".AddHotelExpenseItem,.delHotelExpenseItem").hide();
        $TrafficExpenseItemTr.find(".AddTrafficExpenseItem,.delTrafficExpenseItem").hide();
        $MealsSubsidiesItemTr.find(".AddMealsSubsidiesItem,.delMealsSubsidiesItem").hide();
        $OthersItemTr.find(".AddOthersItem,.delOthersItem").hide();
        $LoanItemsTr.find(".AddLoanItems,.delLoanItems").hide();
        $("#Approve").show();
        $("#SendRefuse").show();
        $("#SendHangUp").show();
        $("#TurnToDo").show();
    }
    else if (status == "Financial Analyst") {
        app.util.ModelToDetails($TripReimbursementEdit);
        //显示操作按钮
        $HotelExpenseItemTr.find(".AddHotelExpenseItem,.delHotelExpenseItem").hide();
        $TrafficExpenseItemTr.find(".AddTrafficExpenseItem,.delTrafficExpenseItem").hide();
        $MealsSubsidiesItemTr.find(".AddMealsSubsidiesItem,.delMealsSubsidiesItem").hide();
        $OthersItemTr.find(".AddOthersItem,.delOthersItem").hide();
        $LoanItemsTr.find(".AddLoanItems,.delLoanItems").hide();
        app.util.ModelToEdit(TripReimbursementParam.b_BudgetNumber.closest("div"));
        $("#Approve").show();
        $("#SendRefuse").show();
        $("#SendHangUp").show();
        $("#TurnToDo").show();
    }
    else {
        app.util.ModelToDetails($TripReimbursementEdit);
        //隐藏操作按钮
        $HotelExpenseItemTr.find(".AddHotelExpenseItem,.delHotelExpenseItem").hide();
        $TrafficExpenseItemTr.find(".AddTrafficExpenseItem,.delTrafficExpenseItem").hide();
        $MealsSubsidiesItemTr.find(".AddMealsSubsidiesItem,.delMealsSubsidiesItem").hide();
        $OthersItemTr.find(".AddOthersItem,.delOthersItem").hide();
        $LoanItemsTr.find(".AddLoanItems,.delLoanItems").hide();
        $("#Approve").show();
        $("#SendRefuse").show();
        $("#SendHangUp").show();
    }
    app.util.ModelToEdit(TripReimbursementParam.b_Remark.closest("div"));
}


var choiceUserList = [];
//直属主管
TripReimbursementParam.b_LineLeader.autocomplete({
    source: function (request, response) {
        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
        response($.grep(choiceUserList, function (value) {
            return matcher.test(value);
        }));
    }
});

//部门领导
TripReimbursementParam.b_DeptLeader.autocomplete({
    source: function (request, response) {
        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
        response($.grep(choiceUserList, function (value) {
            return matcher.test(value);
        }));
    }
});

//中心领导
TripReimbursementParam.b_DivisionVP.autocomplete({
    source: function (request, response) {
        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
        response($.grep(choiceUserList, function (value) {
            return matcher.test(value);
        }));
    }
});

//申请人
TripReimbursementParam.b_Employee.autocomplete({
    source: function (request, response) {
        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
        response($.grep(choiceUserList, function (value) {
            return matcher.test(value);
        }));
    }
});

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

//出差单号触发autocomplete
TripReimbursementParam.b_BTRecordNo.autocomplete({
    source: function (request, response) {
        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
        response($.grep(travelRecordNolist, function (value) {
            return matcher.test(value);
        }));
    },
    select: function (event, ui) {
        var value = ui.item.value;
        ui.item.value = $.trim(ui.item.value.split(" ")[0].split("：")[1]);
    },
    focus: function (event, ui) {
        return false;
    }
}).focus(function () {
    $(this).autocomplete("search", "b");
});

//获取出差单号
var travelRecordNolist = [];
function GetAutocompleteTravelRecordNo() {
    var b_Employee = TripReimbursementParam.b_Employee.val();
    $.ajax({
        url: '/AutoComplete/GetTravelRecordNo',
        data: { "b_Employee": b_Employee },
        dataType: "json",
        success: function (ans) {
            if (ans.error.length > 0) {
                return false;
            }
            travelRecordNolist = [];
            if (ans.data != null && ans.data.length > 0) {
                for (var i = 0; i < ans.data.length; i++) {
                    var item = ans.data[i];
                    travelRecordNolist.push("单号：" + item.b_DocumentNo + "   " + "申请时间：" + item.b_ApplicationDate + "   " + "是否项目：" + item.b_Type + "   " + "项目名称：" + item.b_ProjectName + "  " + "目的地：" + item.b_Destination);
                }
            }
        }
    });

}

//出差单号失去焦点触发事件
TripReimbursementParam.b_BTRecordNo.blur(function () {
    var b_BTRecordNo = TripReimbursementParam.b_BTRecordNo.val();
    var b_Employee = TripReimbursementParam.b_Employee.val();
    if (b_BTRecordNo != "" && b_BTRecordNo != null) {
        $.ajax({
            url: '/TripReimbursement/GetBusinessTravelByParam',
            data: { "b_BTRecordNo": b_BTRecordNo, "b_Employee": b_Employee },
            dataType: "json",
            success: function (ans) {
                if (ans.error.length > 0) {
                    return false;
                }
                if (ans.data != null) {
                    TripReimbursementParam.b_TravelBudget.val(ans.data.B_TRAVELBUDGET);
                    //计算是否超出预算
                    CalculationBeyondBudget();
                }
            }
        });
    }
    else {
        TripReimbursementParam.b_TravelBudget.val("");
        TripReimbursementParam.b_IsBeyondBudget.html("");
        TripReimbursementParam.b_BeyondReason.val("");
        $("#BeyondReasonIsDisplay").hide();
    }
});

//计算是否超出预算
function CalculationBeyondBudget() {
    var b_BTRecordNo = $.trim(TripReimbursementParam.b_BTRecordNo.val());
    var b_TotalExpense = $.trim(TripReimbursementParam.b_TotalExpense.html());
    var b_TravelBudget = $.trim(TripReimbursementParam.b_TravelBudget.val());
    if (b_BTRecordNo != "" && b_TotalExpense != "" && b_TravelBudget != "") {
        b_TotalExpense = b_TotalExpense == "" ? 0 : parseFloat(b_TotalExpense);
        b_TravelBudget = b_TravelBudget == "" ? 0 : parseFloat(b_TravelBudget);
        if (b_TravelBudget < b_TotalExpense) {
            TripReimbursementParam.b_IsBeyondBudget.html("是");
            $("#BeyondReasonIsDisplay").show();
            $("label.AddRedLabel").css("color", "red");
        }
        else {
            TripReimbursementParam.b_IsBeyondBudget.html("否");
            TripReimbursementParam.b_BeyondReason.val("");
            $("#BeyondReasonIsDisplay").hide();
            $("label.AddRedLabel").css("color", "");
        }
    }
    else if (b_BTRecordNo=="")
    {
        TripReimbursementParam.b_IsBeyondBudget.html("");
        TripReimbursementParam.b_BeyondReason.val("");
        TripReimbursementParam.b_TravelBudget.val("");
        $("#BeyondReasonIsDisplay").hide();
        $("label.AddRedLabel").css("color", "");
    }
}

//住宿委托事件
$HotelExpenseItem.delegate("a", "click", function () {
    var $this = $(this);
    if ($this.hasClass("AddHotelExpenseItem")) {
        $this.closest("tr").after(AddHotelExpenseItemTr());
        return false;
    }
    if ($this.hasClass("delHotelExpenseItem")) {
        $this.closest("tr").remove();
        CalculateTax($this, "Hote");
        if ($HotelExpenseItemTr.find("tr").length <= 0) {
            $HotelExpenseItemTr.prepend(AddHotelExpenseItemTr());
        }
        CalculateAdvancedAmount();
        return false;
    }
}).delegate("input.b_Rate", "keyup", function () {
    var $this = $(this);
    $this.val($this.val().replace(/[^\d\.]/g, ''));
}).delegate("input.b_OriginalCurrency", "keyup", function () {
    var $this = $(this);
    $this.val($this.val().replace(/[^\d\.]/g, ''));
}).delegate("input.b_Count", "keyup", function () {
    var $this = $(this);
    $this.val($this.val().replace(/[^\d\.]/g, ''));
}).delegate("input.b_TaxRate", "keyup", function () {
    var $this = $(this);
    $this.val($this.val().replace(/[^\d\.]/g, ''));
}).delegate("input.b_Tax", "keyup", function () {
    var $this = $(this);
    $this.val($this.val().replace(/[^\d\.]/g, ''));
}).delegate("input.b_Rate,input.b_OriginalCurrency,input.b_Count", "blur", function () {
    var $this = $(this);
    CalculateTax($this, "Hote");
    CalculateAdvancedAmount();
}).delegate("input.b_Tax,input.b_TaxFreeAmount", "blur", function () {
    var $this = $(this);
    CalculateHoteData($this, "Hote");
    CalculateAdvancedAmount();
}).delegate("input.b_StartDate,input.b_EndDate", "change", function () {
    var $this = $(this).closest("tr");
    var startDate = $this.find("input.b_StartDate").first().val();
    var endDate = $this.find("input.b_EndDate").first().val();
    if (startDate != "" && endDate != "") {
        var days = differenceDate(startDate, endDate);
    }
    $this.find("input.b_Count").first().val(days);
    CalculateTax($this, "Hote");
}).delegate("select.b_TaxRate", "change", function () {
    var $this = $(this);
    CalculateTax($this, "Hote");
    CalculateAdvancedAmount();
});

//借款明细
$LoanItems.delegate("input.b_LoanAmount", "keyup", function () {
    var $this = $(this);
    $this.val($this.val().replace(/[^\d\.]/g, ''));
}).delegate("input.b_LoanAmount", "blur", function () {
    var $this = $(this);
    CalculateAdvancedAmount();
});

//计算金额
function CalculateTax($this, type) {
    if (type == "Hote" || type == "Other") {
        var ItemTr = $this.closest("tr");
        //税率
        var b_TaxRate = $.trim(ItemTr.find("select.b_TaxRate").val());
        //原币单价
        var b_OriginalCurrency = $.trim(ItemTr.find("input.b_OriginalCurrency").val());
        //汇率
        var b_Rate = $.trim(ItemTr.find("input.b_Rate").val());
        //天数
        var b_Count = $.trim(ItemTr.find("input.b_Count").val());

        if (b_TaxRate != "" && b_OriginalCurrency != "" && b_Rate != "" && b_Count != "") {
            //税额
            var b_Tax = (parseFloat(b_TaxRate) * parseFloat(b_OriginalCurrency) * parseFloat(b_Rate) * parseFloat(b_Count)) / (1 + parseFloat(b_TaxRate));
            b_Tax = toDecimal2(b_Tax);
            ItemTr.find("input.b_Tax").val(b_Tax);
            //不含税金额
            var b_TaxFreeAmount = (parseFloat(b_OriginalCurrency) * parseFloat(b_Rate) * parseFloat(b_Count)) / (1 + parseFloat(b_TaxRate));
            b_TaxFreeAmount = toDecimal2(b_TaxFreeAmount);
            ItemTr.find("input.b_TaxFreeAmount").val(b_TaxFreeAmount);
            //小计
            var b_CNYSubtotal = parseFloat(b_Tax) + parseFloat(b_TaxFreeAmount);
            b_CNYSubtotal = toDecimal2(b_CNYSubtotal);
            ItemTr.find("label.b_CNYSubtotal").html(b_CNYSubtotal);

        }
    }
    else if (type == "Traf") {
        var ItemTr = $this.closest("tr");
        //税率
        var b_TaxRate = $.trim(ItemTr.find("select.b_TaxRate").val());
        //原币单价
        var b_OriginalCurrency = $.trim(ItemTr.find("input.b_OriginalCurrency").val());
        //汇率
        var b_Rate = $.trim(ItemTr.find("input.b_Rate").val());

        if (b_TaxRate != "" && b_OriginalCurrency != "" && b_Rate != "") {
            //税额
            var b_Tax = (parseFloat(b_TaxRate) * parseFloat(b_OriginalCurrency) * parseFloat(b_Rate)) / (1 + parseFloat(b_TaxRate));
            b_Tax = toDecimal2(b_Tax);
            ItemTr.find("input.b_Tax").val(b_Tax);

            //不含税金额
            var b_TaxFreeAmount = (parseFloat(b_OriginalCurrency) * parseFloat(b_Rate)) / (1 + parseFloat(b_TaxRate));
            b_TaxFreeAmount = toDecimal2(b_TaxFreeAmount);
            ItemTr.find("input.b_TaxFreeAmount").val(b_TaxFreeAmount);

            //小计
            var b_CNYSubtotal = parseFloat(b_Tax) + parseFloat(b_TaxFreeAmount);
            b_CNYSubtotal = toDecimal2(b_CNYSubtotal);
            ItemTr.find("label.b_CNYSubtotal").html(b_CNYSubtotal);
        }
    }
    else if (type == "Meal") {
        var ItemTr = $this.closest("tr");
        //税率
        var b_TaxRate = $.trim(ItemTr.find("select.b_TaxRate").val());
        //汇率
        var b_Rate = $.trim(ItemTr.find("input.b_Rate").val());
        //固定补贴
        var b_FixedSubsidy = $.trim(ItemTr.find("input.b_FixedSubsidy").val());
        b_FixedSubsidy = b_FixedSubsidy == "" ? 0 : b_FixedSubsidy;

        if (b_TaxRate != "" && b_Rate != "") {
            //税额
            var b_Tax = parseFloat(b_TaxRate) * (parseFloat(b_FixedSubsidy)) * parseFloat(b_Rate);
            b_Tax = toDecimal2(b_Tax);
            ItemTr.find("input.b_Tax").val(b_Tax);

            //不含税金额
            var b_TaxFreeAmount = (parseFloat(b_FixedSubsidy)) * parseFloat(b_Rate) - parseFloat(b_Tax);
            b_TaxFreeAmount = toDecimal2(b_TaxFreeAmount);
            ItemTr.find("input.b_TaxFreeAmount").val(b_TaxFreeAmount);
            //小计
            var b_CNYSubtotal = parseFloat(b_Tax) + parseFloat(b_TaxFreeAmount);
            b_CNYSubtotal = toDecimal2(b_CNYSubtotal);
            ItemTr.find("label.b_CNYSubtotal").html(b_CNYSubtotal);
        }
    }
    if (type == "Hote") {
        var b_HotelAmount = 0, b_HotelInWords;

        //计算合计大、小写   
        $HotelExpenseItemTr.find("tr").each(function () {
            var $this = $(this);
            b_HotelAmount = parseFloat(b_HotelAmount) + parseFloat($this.find("label.b_CNYSubtotal").html());
        });
        //合计大写    
        b_HotelInWords = convertCurrency(b_HotelAmount);
        TripReimbursementParam.b_HotelInWords.val(b_HotelInWords);
        //合计小写    
        TripReimbursementParam.b_HotelAmount.val(b_HotelAmount);

    }
    else if (type == "Traf") {
        var b_TrafAmount = 0, b_TrafInWords;

        //计算合计大、小写   
        $TrafficExpenseItemTr.find("tr").each(function () {
            var $this = $(this);
            b_TrafAmount = parseFloat(b_TrafAmount) + parseFloat($this.find("label.b_CNYSubtotal").html());
        });
        //合计大写 
        b_TrafInWords = convertCurrency(b_TrafAmount);
        TripReimbursementParam.b_TrafInWords.val(b_TrafInWords);
        //合计小写
        TripReimbursementParam.b_TrafAmount.val(b_TrafAmount);
    }
    else if (type == "Meal") {
        var b_MealAmount = 0, b_MealInWords;

        //计算合计大、小写   
        $MealsSubsidiesItemTr.find("tr").each(function () {
            var $this = $(this);
            b_MealAmount = parseFloat(b_MealAmount) + parseFloat($this.find("label.b_CNYSubtotal").html());
        });
        //合计大写 
        b_MealInWords = convertCurrency(b_MealAmount);
        TripReimbursementParam.b_MealInWords.val(b_MealInWords);
        //合计小写
        TripReimbursementParam.b_MealAmount.val(b_MealAmount);

    }
    else if (type == "Other") {
        var b_OthAmount = 0, b_OthInWords;

        //计算合计大、小写   
        $OthersItemTr.find("tr").each(function () {
            var $this = $(this);
            b_OthAmount = parseFloat(b_OthAmount) + parseFloat($this.find("label.b_CNYSubtotal").html());
        });
        //合计大写 
        b_OthInWords = convertCurrency(b_OthAmount);
        TripReimbursementParam.b_OthInWords.val(b_OthInWords);
        //合计小写
        TripReimbursementParam.b_OthAmount.val(b_OthAmount);
    }

    //总费用
    var b_HotelAmount = TripReimbursementParam.b_HotelAmount.val() == "" ? 0 : TripReimbursementParam.b_HotelAmount.val();
    b_HotelAmount = b_HotelAmount == "NaN" ? 0 : b_HotelAmount;
    var b_TrafAmount = TripReimbursementParam.b_TrafAmount.val() == "" ? 0 : TripReimbursementParam.b_TrafAmount.val();
    b_TrafAmount = b_HotelAmount == "NaN" ? 0 : b_TrafAmount;
    var b_MealAmount = TripReimbursementParam.b_MealAmount.val() == "" ? 0 : TripReimbursementParam.b_MealAmount.val();
    b_MealAmount = b_MealAmount == "NaN" ? 0 : b_MealAmount;
    var b_OthAmount = TripReimbursementParam.b_OthAmount.val() == "" ? 0 : TripReimbursementParam.b_OthAmount.val();
    b_OthAmount = b_OthAmount == "NaN" ? 0 : b_OthAmount;
    var b_TotalExpense = parseFloat(parseFloat(b_HotelAmount) + parseFloat(b_TrafAmount) + parseFloat(b_MealAmount) + parseFloat(b_OthAmount));
    TripReimbursementParam.b_TotalExpense.html(toDecimal2(b_TotalExpense));

    //向公司预支现金
    var b_AdvancedAmount = 0;
    $LoanItemsTr.find("tr").each(function () {
        var $this = $(this);
        var b_LoanAmount = $.trim($this.find("input.b_LoanAmount").val());
        if (b_LoanAmount == "") {
            b_LoanAmount = 0;
        }
        b_AdvancedAmount = parseFloat(b_AdvancedAmount) + parseFloat(b_LoanAmount);
        b_AdvancedAmount = toDecimal2(b_AdvancedAmount);
    });
    TripReimbursementParam.b_AdvancedAmount.html(b_AdvancedAmount);

    //应退还公司
    var b_AmountInTotal = 0;
    b_AmountInTotal = parseFloat(TripReimbursementParam.b_TotalExpense.html()) - parseFloat(b_AdvancedAmount);
    TripReimbursementParam.b_AmountInTotal.html(b_AmountInTotal);
    //计算是否超出预算
    CalculationBeyondBudget();
}

//计算预知现金和应退还公司
function CalculateAdvancedAmount() {
    //向公司预支现金
    var b_AdvancedAmount = 0;
    $LoanItemsTr.find("tr").each(function () {
        var $this = $(this);
        var b_LoanAmount = $.trim($this.find("input.b_LoanAmount").val());
        if (b_LoanAmount == "") {
            b_LoanAmount = 0;
        }
        b_AdvancedAmount = parseFloat(b_AdvancedAmount) + parseFloat(b_LoanAmount);
        b_AdvancedAmount = toDecimal2(b_AdvancedAmount);
    });
    TripReimbursementParam.b_AdvancedAmount.html(b_AdvancedAmount);

    //应退还公司
    var b_AmountInTotal = 0;
    b_AmountInTotal = parseFloat(TripReimbursementParam.b_TotalExpense.html()) - parseFloat(b_AdvancedAmount);
    TripReimbursementParam.b_AmountInTotal.html(b_AmountInTotal);

}

//当手动修改  税额和不含税金额时触发
function CalculateHoteData($this, type) {
    var ItemTr = $this.closest("tr");
    //税额
    var b_Tax = ItemTr.find("input.b_Tax").val();
    b_Tax = toDecimal2(b_Tax);

    //不含税金额
    var b_TaxFreeAmount = ItemTr.find("input.b_TaxFreeAmount").val();
    b_TaxFreeAmount = toDecimal2(b_TaxFreeAmount);

    //小计
    var b_CNYSubtotal = parseFloat(b_Tax) + parseFloat(b_TaxFreeAmount);
    b_CNYSubtotal = toDecimal2(b_CNYSubtotal);
    ItemTr.find("label.b_CNYSubtotal").html(b_CNYSubtotal);

    if (type == "Hote") {
        var b_HotelAmount = 0, b_HotelInWords;
        $HotelExpenseItemTr.find("tr").each(function () {
            var $this = $(this);
            b_HotelAmount = parseFloat(b_HotelAmount) + parseFloat($this.find("label.b_CNYSubtotal").html());
        });

        //合计（小写）
        TripReimbursementParam.b_HotelAmount.val(b_HotelAmount);

        //合计（大写）
        b_HotelInWords = convertCurrency(b_HotelAmount);
        TripReimbursementParam.b_HotelInWords.val(b_HotelInWords);
    }
    else if (type == "Traf") {
        var b_TrafAmount = 0, b_TrafInWords;
        $TrafficExpenseItemTr.find("tr").each(function () {
            var $this = $(this);
            b_TrafAmount = parseFloat(b_TrafAmount) + parseFloat($this.find("label.b_CNYSubtotal").html());
        });

        //合计（小写）
        TripReimbursementParam.b_TrafAmount.val(b_TrafAmount);

        //合计（大写）
        b_TrafInWords = convertCurrency(b_TrafAmount);
        TripReimbursementParam.b_TrafInWords.val(b_TrafInWords);



    }
    else if (type == "Meal") {
        var b_MealAmount = 0, b_MealInWords;
        $MealsSubsidiesItemTr.find("tr").each(function () {
            var $this = $(this);
            b_MealAmount = parseFloat(b_MealAmount) + parseFloat($this.find("label.b_CNYSubtotal").html());
        });

        //合计（小写）
        TripReimbursementParam.b_MealAmount.val(b_MealAmount);

        //合计（大写）
        b_MealInWords = convertCurrency(b_MealAmount);
        TripReimbursementParam.b_MealInWords.val(b_MealInWords);

    }
    else if (type == "Other") {
        var b_OthAmount = 0, b_OthInWords;
        $OthersItemTr.find("tr").each(function () {
            var $this = $(this);
            b_OthAmount = parseFloat(b_OthAmount) + parseFloat($this.find("label.b_CNYSubtotal").html());
        });

        //合计（小写）
        TripReimbursementParam.b_OthAmount.val(b_OthAmount);

        //合计（大写）
        b_OthInWords = convertCurrency(b_OthAmount);
        TripReimbursementParam.b_OthInWords.val(b_OthInWords);
    }

    //总费用
    var b_HotelAmount = TripReimbursementParam.b_HotelAmount.val() == "" ? 0 : TripReimbursementParam.b_HotelAmount.val();
    b_HotelAmount = b_HotelAmount == "NaN" ? 0 : b_HotelAmount;
    var b_TrafAmount = TripReimbursementParam.b_TrafAmount.val() == "" ? 0 : TripReimbursementParam.b_TrafAmount.val();
    b_TrafAmount = b_TrafAmount == "NaN" ? 0 : b_TrafAmount;
    var b_MealAmount = TripReimbursementParam.b_MealAmount.val() == "" ? 0 : TripReimbursementParam.b_MealAmount.val();
    b_MealAmount = b_MealAmount == "NaN" ? 0 : b_MealAmount;
    var b_OthAmount = TripReimbursementParam.b_OthAmount.val() == "" ? 0 : TripReimbursementParam.b_OthAmount.val();
    b_OthAmount = b_OthAmount == "NaN" ? 0 : b_OthAmount;
    var b_TotalExpense = parseFloat(parseFloat(b_HotelAmount) + parseFloat(b_TrafAmount) + parseFloat(b_MealAmount) + parseFloat(b_OthAmount));
    TripReimbursementParam.b_TotalExpense.html(toDecimal2(b_TotalExpense));

    //判断是否超出预算
    CalculationBeyondBudget();
}

//交通委托事件
$TrafficExpenseItem.delegate("a", "click", function () {
    var $this = $(this);
    if ($this.hasClass("AddTrafficExpenseItem")) {
        $this.closest("tr").after(AddTrafficExpenseItemTr());
        return false;
    }
    if ($this.hasClass("delTrafficExpenseItem")) {
        $this.closest("tr").remove();
        CalculateTax($this, "Traf");
        if ($TrafficExpenseItemTr.find("tr").length <= 0) {
            $TrafficExpenseItemTr.prepend(AddTrafficExpenseItemTr());
        }
        CalculateAdvancedAmount();
        return false;
    }
}).delegate("input.b_Rate", "keyup", function () {
    var $this = $(this);
    $this.val($this.val().replace(/[^\d\.]/g, ''));
}).delegate("input.b_OriginalCurrency", "keyup", function () {
    var $this = $(this);
    $this.val($this.val().replace(/[^\d\.]/g, ''));
}).delegate("input.b_TaxRate", "keyup", function () {
    var $this = $(this);
    $this.val($this.val().replace(/[^\d\.]/g, ''));
}).delegate("input.b_Count", "keyup", function () {
    var $this = $(this);
    $this.val($this.val().replace(/[^\d\.]/g, ''));
}).delegate("input.b_Tax", "keyup", function () {
    var $this = $(this);
    $this.val($this.val().replace(/[^\d\.]/g, ''));
}).delegate("input.b_Rate,input.b_OriginalCurrency", "blur", function () {
    var $this = $(this);
    CalculateTax($this, "Traf");
    CalculateAdvancedAmount();
}).delegate("input.b_Tax,input.b_TaxFreeAmount", "blur", function () {
    var $this = $(this);
    CalculateHoteData($this, "Traf");
    CalculateAdvancedAmount();
}).delegate("select.b_TaxRate", "change", function () {
    var $this = $(this);
    CalculateTax($this, "Traf");
    CalculateAdvancedAmount();
});

//餐费及固定补贴委托事件
$MealsSubsidiesItem.delegate("a", "click", function () {
    var $this = $(this);
    if ($this.hasClass("AddMealsSubsidiesItem")) {
        $this.closest("tr").after(AddMealsSubsidiesItemTr());
        return false;
    }
    if ($this.hasClass("delMealsSubsidiesItem")) {
        $this.closest("tr").remove();
        CalculateTax($this, "Meal");
        if ($MealsSubsidiesItemTr.find("tr").length <= 0) {
            $MealsSubsidiesItemTr.prepend(AddMealsSubsidiesItemTr());
        }
        CalculateAdvancedAmount();
        return false;
    }
}).delegate("input.b_Rate", "keyup", function () {
    var $this = $(this);
    $this.val($this.val().replace(/[^\d\.]/g, ''));
}).delegate("input.b_TaxRate", "keyup", function () {
    var $this = $(this);
    $this.val($this.val().replace(/[^\d\.]/g, ''));
}).delegate("input.b_Tax", "keyup", function () {
    var $this = $(this);
    $this.val($this.val().replace(/[^\d\.]/g, ''));
}).delegate("input.b_Rate,input.b_FixedSubsidy", "blur", function () {
    var $this = $(this);
    CalculateTax($this, "Meal");
    CalculateAdvancedAmount();
}).delegate("input.b_Tax,input.b_TaxFreeAmount", "blur", function () {
    var $this = $(this);
    CalculateHoteData($this, "Meal");
    CalculateAdvancedAmount();
}).delegate("select.b_TaxRate", "change", function () {
    var $this = $(this);
    CalculateTax($this, "Meal");
    CalculateAdvancedAmount();
});

//其他（签证/其他费用）委托事件
$OthersItem.delegate("a", "click", function () {
    var $this = $(this);
    if ($this.hasClass("AddOthersItem")) {
        $this.closest("tr").after(AddOthersItemTr());
        return false;
    }
    if ($this.hasClass("delOthersItem")) {
        $this.closest("tr").remove();
        CalculateTax($this, "Other");
        if ($OthersItemTr.find("tr").length <= 0) {
            $OthersItemTr.prepend(AddOthersItemTr());
        }
        CalculateAdvancedAmount();
        return false;
    }
}).delegate("input.b_Rate", "keyup", function () {
    var $this = $(this);
    $this.val($this.val().replace(/[^\d\.]/g, ''));
}).delegate("input.b_OriginalCurrency", "keyup", function () {
    var $this = $(this);
    $this.val($this.val().replace(/[^\d\.]/g, ''));
}).delegate("input.b_Count", "keyup", function () {
    var $this = $(this);
    $this.val($this.val().replace(/[^\d\.]/g, ''));
}).delegate("input.b_TaxRate", "keyup", function () {
    var $this = $(this);
    $this.val($this.val().replace(/[^\d\.]/g, ''));
}).delegate("input.b_Tax", "keyup", function () {
    var $this = $(this);
    $this.val($this.val().replace(/[^\d\.]/g, ''));
}).delegate("input.b_Rate,input.b_OriginalCurrency,input.b_Count", "blur", function () {
    var $this = $(this);
    CalculateTax($this, "Other");
    CalculateAdvancedAmount();
}).delegate("input.b_Tax,input.b_TaxFreeAmount", "blur", function () {
    var $this = $(this);
    CalculateHoteData($this, "Other");
    CalculateAdvancedAmount();
}).delegate("select.b_TaxRate", "change", function () {
    var $this = $(this);
    CalculateTax($this, "Other");
    CalculateAdvancedAmount();
});

////借款明细委托事件
//$LoanItemsTr.delegate("a", "click", function () {
//    var $this = $(this);
//    if ($this.hasClass("AddLoanItems")) {
//        $this.closest("tr").after(AddLoanItemsTr());
//        return false;
//    }
//    if ($this.hasClass("delLoanItems")) {
//        $this.closest("tr").remove();
//        if ($LoanItemsTr.find("tr").length <= 0) {
//            $LoanItemsTr.prepend(AddLoanItemsTr());
//        }
//    }
//});

//住宿事件
function AddHotelExpenseItemTr(operatorType) {
    var html = $(HotelExpenseItemHtml);

    var b_Type = "";
    $TripReimbursementEdit.find("input[name='b_Type']").each(function () {
        var $this = $(this);
        if ($this.closest("div").hasClass("checked")) {
            b_Type = $this.val();
        }
    });
    if (b_Type == "Non Project") {
        html.find(".b_ProjectName").attr("disabled", "disabled");
    }


    html.find('.b_StartDate').datepicker({
        format: "yyyy-mm-dd",
        autoclose: true,
        todayHighlight: true,
        endDate: new Date()
    }).on('changeDate', function (e) {
        var startTime = e.date;
        html.find('.b_EndDate').datepicker('setStartDate', startTime);
    });

    html.find('.b_EndDate').datepicker({
        format: "yyyy-mm-dd",
        autoclose: true,
        todayHighlight: true,
        //startDate: new Date()
    }).on('changeDate', function (e) {
        var endTime = e.date;
        html.find('.b_StartDate').datepicker('setEndDate', endTime);
    });

    html.find(".b_ProjectName").first().empty();
    if (projectList.length > 0) {
        html.find(".b_ProjectName").first().append("<option value=''></option>");
        for (var i = 0; i < projectList.length; i++) {
            var value = projectList[i];
            html.find(".b_ProjectName").first().append("<option value='" + value + "'>" + value + "</option>");
        }
    }

    if (operatorType == "audit") {
        html.find(".b_ProjectName").first().closest("td").empty().append("<input type='text' class='form-control b_ProjectName'/>")
    }

    html.find(".b_Currency").first().empty();
    if (currencyTypeList.length > 0) {
        html.find(".b_Currency").first().append("<option value=''></option>");
        for (var i = 0; i < currencyTypeList.length; i++) {
            var value = currencyTypeList[i];
            html.find(".b_Currency").first().append("<option value='" + value + "'>" + value + "</option>");
        }
    }

    html.find(".b_TaxRate").first().empty();
    if (TaxCodeList.length > 0) {
        html.find(".b_TaxRate").first().append("<option value=''>请选择</option>")
        for (var i = 0; i < TaxCodeList.length; i++) {
            var item = TaxCodeList[i];
            html.find(".b_TaxRate").first().append("<option value='" + item.Value + "'>" + item.Text + "</option>");
        }
    }

    html.find(".b_Currency").val("RMB");
    html.find(".b_Rate").val(1);

    return html;
}



//交通事件
function AddTrafficExpenseItemTr(operatorType) {
    var html = $(TrafficExpenseItemHtml);
    var b_Type = "";
    $TripReimbursementEdit.find("input[name='b_Type']").each(function () {
        var $this = $(this);
        if ($this.closest("div").hasClass("checked")) {
            b_Type = $this.val();
        }
    });
    if (b_Type == "Non Project") {
        html.find(".b_ProjectName").attr("disabled", "disabled");
    }

    html.find('.b_StartDate').datepicker({
        format: "yyyy-mm-dd",
        autoclose: true,
        todayHighlight: true,
        endDate: new Date()
    }).on('changeDate', function (e) {
        var startTime = e.date;
        html.find('.b_EndDate').datepicker('setStartDate', startTime);
    });

    html.find('.b_EndDate').datepicker({
        format: "yyyy-mm-dd",
        autoclose: true,
        todayHighlight: true,
        //startDate: new Date()
    }).on('changeDate', function (e) {
        var endTime = e.date;
        html.find('.b_StartDate').datepicker('setEndDate', endTime);
    });

    html.find(".b_ProjectName").first().empty();
    if (projectList.length > 0) {
        html.find(".b_ProjectName").first().append("<option value=''></option>");
        for (var i = 0; i < projectList.length; i++) {
            var value = projectList[i];
            html.find(".b_ProjectName").first().append("<option value='" + value + "'>" + value + "</option>");
        }
    }

    if (operatorType == "audit") {
        html.find(".b_ProjectName").first().closest("td").empty().append("<input type='text' class='form-control b_ProjectName'/>")
    }

    html.find(".b_Type").first().empty();
    if (TrafficCategoryList.length > 0) {
        html.find(".b_Type").first().append("<option value=''></option>");
        for (var i = 0; i < TrafficCategoryList.length; i++) {
            var value = TrafficCategoryList[i];
            html.find(".b_Type").first().append("<option value='" + value + "'>" + value + "</option>");
        }
    }

    html.find(".b_Currency").first().empty();
    if (currencyTypeList.length > 0) {
        html.find(".b_Currency").first().append("<option value=''></option>");
        for (var i = 0; i < currencyTypeList.length; i++) {
            var value = currencyTypeList[i];
            html.find(".b_Currency").first().append("<option value='" + value + "'>" + value + "</option>");
        }
    }

    html.find(".b_TaxRate").first().empty();
    if (TaxCodeList.length > 0) {
        html.find(".b_TaxRate").first().append("<option value=''>请选择</option>")
        for (var i = 0; i < TaxCodeList.length; i++) {
            var item = TaxCodeList[i];
            html.find(".b_TaxRate").first().append("<option value='" + item.Value + "'>" + item.Text + "</option>");
        }
    }

    html.find(".b_Currency").val("RMB");
    html.find(".b_Rate").val(1);

    return html;
}

//餐费及固定补贴事件
function AddMealsSubsidiesItemTr(operatorType) {
    var html = $(MealsSubsidiesItemHtml);
    var b_Type = "";
    $TripReimbursementEdit.find("input[name='b_Type']").each(function () {
        var $this = $(this);
        if ($this.closest("div").hasClass("checked")) {
            b_Type = $this.val();
        }
    });
    if (b_Type == "Non Project") {
        html.find(".b_ProjectName").attr("disabled", "disabled");
    }

    html.find('.b_StartDate').datepicker({
        format: "yyyy-mm-dd",
        autoclose: true,
        todayHighlight: true,
        endDate: new Date()
    }).on('changeDate', function (e) {
        var startTime = e.date;
        html.find('.b_EndDate').datepicker('setStartDate', startTime);
    });

    html.find('.b_EndDate').datepicker({
        format: "yyyy-mm-dd",
        autoclose: true,
        todayHighlight: true,
        //startDate: new Date()
    }).on('changeDate', function (e) {
        var endTime = e.date;
        html.find('.b_StartDate').datepicker('setEndDate', endTime);
    });

    html.find(".b_ProjectName").first().empty();
    if (projectList.length > 0) {
        html.find(".b_ProjectName").first().append("<option value=''></option>");
        for (var i = 0; i < projectList.length; i++) {
            var value = projectList[i];
            html.find(".b_ProjectName").first().append("<option value='" + value + "'>" + value + "</option>");
        }
    }

    if (operatorType == "audit") {
        html.find(".b_ProjectName").first().closest("td").empty().append("<input type='text' class='form-control b_ProjectName'/>")
    }

    html.find(".b_Currency").first().empty();
    if (currencyTypeList.length > 0) {
        html.find(".b_Currency").first().append("<option value=''></option>");
        for (var i = 0; i < currencyTypeList.length; i++) {
            var value = currencyTypeList[i];
            html.find(".b_Currency").first().append("<option value='" + value + "'>" + value + "</option>");
        }
    }

    html.find(".b_TaxRate").first().empty();
    if (TaxCodeList.length > 0) {
        html.find(".b_TaxRate").first().append("<option value=''>请选择</option>")
        for (var i = 0; i < TaxCodeList.length; i++) {
            var item = TaxCodeList[i];
            html.find(".b_TaxRate").first().append("<option value='" + item.Value + "'>" + item.Text + "</option>");
        }
    }

    html.find(".b_Currency").val("RMB");
    html.find(".b_Rate").val(1);
    html.find(".b_TaxRate").val("0");
    return html;
}

//其他（签证/其他费用）事件
function AddOthersItemTr(operatorType) {
    var html = $(OthersItemHtml);
    var b_Type = "";
    $TripReimbursementEdit.find("input[name='b_Type']").each(function () {
        var $this = $(this);
        if ($this.closest("div").hasClass("checked")) {
            b_Type = $this.val();
        }
    });
    if (b_Type == "Non Project") {
        html.find(".b_ProjectName").attr("disabled", "disabled");
    }

    html.find('.b_StartDate').datepicker({
        format: "yyyy-mm-dd",
        autoclose: true,
        todayHighlight: true,
        endDate: new Date()
    }).on('changeDate', function (e) {
        var startTime = e.date;
        html.find('.b_EndDate').datepicker('setStartDate', startTime);
    });

    html.find('.b_EndDate').datepicker({
        format: "yyyy-mm-dd",
        autoclose: true,
        todayHighlight: true,
        //startDate: new Date()
    }).on('changeDate', function (e) {
        var endTime = e.date;
        html.find('.b_StartDate').datepicker('setEndDate', endTime);
    });


    html.find(".b_ProjectName").first().empty();
    if (projectList.length > 0) {
        html.find(".b_ProjectName").first().append("<option value=''></option>");
        for (var i = 0; i < projectList.length; i++) {
            var value = projectList[i];
            html.find(".b_ProjectName").first().append("<option value='" + value + "'>" + value + "</option>");
        }
    }

    if (operatorType == "audit") {
        html.find(".b_ProjectName").first().closest("td").empty().append("<input type='text' class='form-control b_ProjectName'/>")
    }

    html.find(".b_Currency").first().empty();
    if (currencyTypeList.length > 0) {
        html.find(".b_Currency").first().append("<option value=''></option>");
        for (var i = 0; i < currencyTypeList.length; i++) {
            var value = currencyTypeList[i];
            html.find(".b_Currency").first().append("<option value='" + value + "'>" + value + "</option>");
        }
    }

    html.find(".b_TaxRate").first().empty();
    if (TaxCodeList.length > 0) {
        html.find(".b_TaxRate").first().append("<option value=''>请选择</option>")
        for (var i = 0; i < TaxCodeList.length; i++) {
            var item = TaxCodeList[i];
            html.find(".b_TaxRate").first().append("<option value='" + item.Value + "'>" + item.Text + "</option>");
        }
    }

    html.find(".b_Currency").val("RMB");
    html.find(".b_Rate").val(1);

    return html;
}

//借款明细事件
function AddLoanItemsTr(userName) {
    var html = $(LoanItemsHtml);
    html.find(".b_Date").datepicker({ autoclose: true, format: 'yyyy-mm-dd' });
    html.find(".b_Borrower").autocomplete({
        source: function (request, response) {
            var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
            response($.grep(choiceUserList, function (value) {
                return matcher.test(value);
            }));
        }
    });
    html.find(".b_Borrower").first().val(userName);
    html.find(".b_Borrower").first().attr("disabled", "disabled");
    return html;
}

//AutoComplete
var projectList = [];
function GetProjectList() {
    $.ajax({
        url: '/AutoComplete/GetProjectList',
        dataType: "json",
        success: function (ans) {
            if (ans.error.length > 0) {
                return false;
            }
            if (ans.data != null && ans.data.length > 0) {
                for (var i = 0; i < ans.data.length; i++) {
                    var item = ans.data[i];
                    projectList.push(item);

                }
            }
        }
    });
}



//上传附件
$("#upload").click(function () {
    var formData = new FormData();
    formData.append("myfile", document.getElementById("Trfile").files[0]);
    formData.append("id", TripReimbursementParam.id.val());
    formData.append("status", TripReimbursementParam.status.val());
    formData.append("relationTableName", "R_TripReimbursementFile");
    $.ajax({
        url: '/FileManage/UploadFile',
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (ans) {
            if (ans.error.length > 0) {
                app.util.onError(ans.error, $TripReimbursementEdit);
                return;
            }
            if (ans.data != null) {
                $("#fileList").append("<div><label><a href='#' class='fileObj' id='" + ans.data.id + "' onclick='DownloadAttachment(this)'>" + ans.data.fileName + "</a>&nbsp;&nbsp;&nbsp;<a href='#' class='glyphicon glyphicon-remove text-danger' id='" + ans.data.id + "' relationId='" + ans.data.relationId + "' onclick='DeleteFile(this)'></a></label></div>");
            }
        }
    });
});

//删除附件
function DeleteFile(obj) {
    if (confirm("确定要删除吗？")) {
        $.ajax({
            url: '/FileManage/DeleteFile',
            data: { "id": $(obj).attr("id"), "relationId": $(obj).attr("relationId"), "relationTableName": "R_TripReimbursementFile" },
            dataType: "json",
            success: function (ans) {
                if (ans.error.length > 0) {
                    app.util.onError(ans.error, $TripReimbursementEdit);
                    return;
                }
                $(obj).closest("div").remove();
            }
        });
    };
};

//获取地区列表
function GetRegionList() {
    $.ajax({
        url: '/AutoComplete/GetRegionList',
        dataType: "json",
        async: false,
        success: function (ans) {
            if (ans.error.length > 0) {
                return false;
            }
            TripReimbursementParam.b_ReimbursementPlace.empty();
            TripReimbursementParam.b_ReimbursementPlace.append("<option value=''>选择地区</option>");
            if (ans.data != null && ans.data.length > 0) {
                for (var i = 0; i < ans.data.length; i++) {
                    var value = ans.data[i];
                    TripReimbursementParam.b_ReimbursementPlace.append("<option value='" + value + "'>" + value + "</option>");
                }
            }
        }
    });
}

//获取费用类别
function GetExpenseCategoryList() {
    $.ajax({
        url: '/AutoComplete/GetExpenseCategoryList',
        dataType: "json",
        async: false,
        success: function (ans) {
            if (ans.error.length > 0) {
                return false;
            }
            if (ans.data != null && ans.data.length > 0) {
                for (var i = 0; i < ans.data.length; i++) {
                    var value = ans.data[i];
                    costNamelist.push(value);

                }
            }
        }
    });
}

//获取交通类别
var TrafficCategoryList = [];
function GetTrafficCategoryList() {
    $.ajax({
        url: '/AutoComplete/GetTrafficCategoryList',
        dataType: "json",
        async: false,
        success: function (ans) {
            if (ans.error.length > 0) {
                return false;
            }
            if (ans.data != null && ans.data.length > 0) {
                for (var i = 0; i < ans.data.length; i++) {
                    var value = ans.data[i];
                    TrafficCategoryList.push(value);
                }
            }
        }
    });
}

//获取币种类别
var currencyTypeList = [];
function GetCurrencyTypeList() {
    $.ajax({
        url: '/AutoComplete/GetCurrencyTypeList',
        dataType: "json",
        async: false,
        success: function (ans) {
            if (ans.error.length > 0) {
                return false;
            }
            if (ans.data != null && ans.data.length > 0) {
                for (var i = 0; i < ans.data.length; i++) {
                    var value = ans.data[i];
                    currencyTypeList.push(value);
                }
            }
        }
    });
}

//申请人信息联动
$TripReimbursementEdit.delegate("input[name='b_Employee']", "blur", function () {
    LoadEmployeeInfo($(this).val());
});

function LoadEmployeeInfo(name) {
    $.ajax({
        url: '/Common/GetUserByName',
        dataType: "json",
        async: false,
        data: { "name": name },
        success: function (ans) {
            if (ans.error.length > 0) {
                return false;
            }
            if (ans.data != null) {
                var item = ans.data;
                TripReimbursementParam.b_StaffNo.val(item.b_JobNumber);
                TripReimbursementParam.b_LineLeader.val(item.b_SeniorManager);
                TripReimbursementParam.b_DeptLeader.val(item.b_Director);
                TripReimbursementParam.b_DivisionVP.val(item.b_VP);
                TripReimbursementParam.b_Dept.val(item.b_Department);
                if (item.b_Department != null && item.b_Department != "") {
                    TripReimbursementParam.b_Dept.val(item.b_Department);
                }
                else if (item.b_Centre != null && item.b_Dept != "") {
                    TripReimbursementParam.b_Dept.val(item.b_Centre);
                }
                TripReimbursementParam.b_CompanyCode.val(item.b_CompanyCode);
                TripReimbursementParam.b_CostCenter.val(item.b_CostCenter);
                $LoanItemsTr.find("tr").each(function () {
                    $(this).find("input.b_Borrower").first().val(name);
                })
            }
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
            TripReimbursementParam.b_CompanyCode.empty();
            TripReimbursementParam.b_CompanyCode.append("<option value=''>选择公司代码</option>");
            if (ans.data != null && ans.data.length > 0) {
                for (var i = 0; i < ans.data.length; i++) {
                    var value = ans.data[i];
                    TripReimbursementParam.b_CompanyCode.append("<option value='" + value + "'>" + value + "</option>");
                }
            }
        }
    });
}

//计算时间差
function differenceDate(date1, date2) {
    var dateOne = new Date(date1);
    var dateTwo = new Date(date2);
    var dateThree = dateTwo.getTime() - dateOne.getTime();

    //计算出相差天数
    var days = Math.floor(dateThree / (24 * 3600 * 1000));
    return days;
}


var TaxCodeList = [];
//获取税率码
function GetTaxCodeConfigure() {
    $.ajax({
        url: '/AutoComplete/GetTaxCodeList',
        dataType: "json",
        async: false,
        success: function (ans) {
            if (ans.error.length > 0) {
                return false;
            }
            if (ans.data != null && ans.data.length > 0) {
                for (var i = 0; i < ans.data.length; i++) {
                    var item = ans.data[i];
                    TaxCodeList.push({ "Text": item.Text, "Value": item.Value });
                }
            }
        }
    });
}


$("input[name='b_Type']").on('ifChecked', function (event) {
    var $this = $(this);
    if ($this.val() == "Project") {
        $HotelExpenseItem.find("select.b_ProjectName").each(function () {
            $(this).removeAttr("disabled");
        });

        $TrafficExpenseItem.find("select.b_ProjectName").each(function () {
            $(this).removeAttr("disabled");
        });

        $MealsSubsidiesItem.find("select.b_ProjectName").each(function () {
            $(this).removeAttr("disabled");
        });

        $OthersItem.find("select.b_ProjectName").each(function () {
            $(this).removeAttr("disabled");
        });
        $("#departmentLeaderInfo").hide();
    }
    else {
        $HotelExpenseItem.find("select.b_ProjectName").each(function () {
            $(this).attr("disabled", "disabled");
            $(this).val("");
        });

        $TrafficExpenseItem.find("select.b_ProjectName").each(function () {
            $(this).attr("disabled", "disabled");
            $(this).val("");
        });
        $MealsSubsidiesItem.find("select.b_ProjectName").each(function () {
            $(this).attr("disabled", "disabled");
            $(this).val("");
        });

        $OthersItem.find("select.b_ProjectName").each(function () {
            $(this).attr("disabled", "disabled");
            $(this).val("");
        });
        $("#departmentLeaderInfo").show();
    }

});