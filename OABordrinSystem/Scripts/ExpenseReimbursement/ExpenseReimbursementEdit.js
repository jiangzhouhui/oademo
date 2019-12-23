var $ExpenseReimbursementEdit = $("#ExpenseReimbursementEdit"), $ReimbursementItem = $("#ReimbursementItem"), $ReimbursementItemTr = $("#ReimbursementItemTr"), ReimbursementItemHtml = $ReimbursementItemTr.html();
//报销明细
var $LoanItem = $("#LoanItem"), $LoanItemTr = $("#LoanItemTr"), LoanItemHtml = $LoanItemTr.html();

ExpenseReimbursementEditParam = app.util.getEditParam($ExpenseReimbursementEdit);

$(document).ready(function () {
    ExpenseReimbursementEditParam.b_AttachmentsQuantity.number();

    GetAutocompleteChoiceUserData();

    //加载项目列表
    GetProjectList();

    //加载地区列表
    GetRegionList();

    //加载费用类别
    GetExpenseCategoryList();

    //加载货币种类列表
    GetCurrencyTypeList();

    //加载公司代码列表
    GetStructureCompanyCode();

    //获取税率码
    GetTaxCodeConfigure();
});


function GetSaveParam(operation) {
    //获取基础信息
    var param = app.util.serializeParamArray(ExpenseReimbursementEditParam);
    param.operation = operation;
    param.b_AdvancedAmount = ExpenseReimbursementEditParam.b_AdvancedAmount.html()
    param.b_TotalExpense = ExpenseReimbursementEditParam.b_TotalExpense.html();
    param.b_DueCompany = ExpenseReimbursementEditParam.b_DueCompany.html();

    //获取单选按钮的值
    var b_IsBudgetary;
    $ExpenseReimbursementEdit.find("input[name='b_IsBudgetary']").each(function () {
        var $this = $(this);
        if ($this.closest("div").hasClass("checked")) {
            b_IsBudgetary = $this.val();
        }
    });
    param.b_IsBudgetary = b_IsBudgetary;

    var b_Type = "";
    $ExpenseReimbursementEdit.find("input[name='b_Type']").each(function () {
        var $this = $(this);
        if ($this.closest("div").hasClass("checked")) {
            b_Type = $this.val();
        }
    });
    param.b_Type = b_Type;

    //获取报销明细
    var items = [];
    $ReimbursementItemTr.find("tr").each(function () {
        var $this = $(this);
        var Id = $.trim($this.find("input.Id").val());
        var b_Date = $.trim($this.find("input.b_Date").val());
        var b_CategoryNumber = $.trim($this.find("select.b_CategoryNumber").val());
        var b_ProjectName = $.trim($this.find("select.b_ProjectName").val());
        if ($.trim($this.find("input.b_ProjectName").val())!="")
        {
            b_ProjectName = $.trim($this.find("input.b_ProjectName").val());
        }
        var b_BudgetNumber = $.trim($this.find("input.b_BudgetNumber").val());
        var b_Currency = $.trim($this.find("select.b_Currency").val());
        var b_Rate = $.trim($this.find("input.b_Rate").val());
        var b_OriginalCurrency = $.trim($this.find("input.b_OriginalCurrency").val());
        var b_Count = $.trim($this.find("input.b_Count").val());
        var b_TaxRate = $.trim($this.find("select.b_TaxRate").val());
        var b_Tax = $.trim($this.find("input.b_Tax").val());
        var b_TaxFreeAmount = $.trim($this.find("input.b_TaxFreeAmount").val());
        var b_CNYSubtotal = $.trim($this.find("label.b_CNYSubtotal").html());
        if (b_Date != "" && b_CategoryNumber != "" && b_BudgetNumber != "" && b_Currency != "" && b_Rate != "" && b_OriginalCurrency != "" && b_Count != "" && b_TaxRate != "") {
            items.push({ "Id": Id, "b_Date": b_Date, "b_CategoryNumber": b_CategoryNumber, "b_ProjectName": b_ProjectName, "b_BudgetNumber": b_BudgetNumber, "b_Currency": b_Currency, "b_Rate": b_Rate, "b_OriginalCurrency": b_OriginalCurrency, "b_Count": b_Count, "b_TaxRate": b_TaxRate, "b_Tax": b_Tax, "b_TaxFreeAmount": b_TaxFreeAmount, "b_CNYSubtotal": b_CNYSubtotal });
        }
    });
    param.ReimbursementItems = items;

    //获取借款明细
    var LoanItems = [];
    $LoanItemTr.find("tr").each(function () {
        var $this = $(this);
        var Id = $.trim($this.find("input.Id").val());
        var b_LoanOrderNo = $.trim($this.find("input.b_LoanOrderNo").val());
        var b_Date = $.trim($this.find("input.b_Date").val());
        var b_Borrower = $.trim($this.find("input.b_Borrower").val());
        var b_LoanAmount = $.trim($this.find("input.b_LoanAmount").val());
        var b_LoanReason = $.trim($this.find("input.b_LoanReason").val());
        if (b_LoanOrderNo != "" && b_Date != "" && b_Borrower != "" && b_LoanAmount != "" && b_LoanReason != "") {
            LoanItems.push({ "b_LoanOrderNo": b_LoanOrderNo, "b_Date": b_Date, "b_Borrower": b_Borrower, "b_LoanAmount": b_LoanAmount, "b_LoanReason": b_LoanReason });
        }
    });
    param.LoanItems = LoanItems;

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

//获取审核的数据
function GetAuditExpenseReimbursement() {
    //获取基础信息
    var param = app.util.serializeParamArray(ExpenseReimbursementEditParam);
    param.b_AdvancedAmount = ExpenseReimbursementEditParam.b_AdvancedAmount.html();
    param.b_TotalExpense = ExpenseReimbursementEditParam.b_TotalExpense.html();
    param.b_DueCompany = ExpenseReimbursementEditParam.b_DueCompany.html();

    //获取单选按钮的值
    var b_IsBudgetary;
    $ExpenseReimbursementEdit.find("input[name='b_IsBudgetary']").each(function () {
        var $this = $(this);
        if ($this.closest("div").hasClass("checked")) {
            b_IsBudgetary = $this.val();
        }
    });
    param.b_IsBudgetary = b_IsBudgetary;

    var b_Type = "";
    $ExpenseReimbursementEdit.find("input[name='b_Type']").each(function () {
        var $this = $(this);
        if ($this.closest("div").hasClass("checked")) {
            b_Type = $this.val();
        }
    });
    param.b_Type = b_Type;

    return param;

}

//小窗口显示样式
function displayModelSetting() {
    var status = ExpenseReimbursementEditParam.status.val();
    $ExpenseReimbursementEdit.find("div.modal-footer").find("button").each(function () {
        $(this).hide();
    });
    $("#btnClose").show();
    if (status == "" || status == "Start") {
        app.util.ModelToEdit($ExpenseReimbursementEdit);
        if (ExpenseReimbursementEditParam.b_IsHangUp.val() == "true") {
            app.util.ModelToDetails(ExpenseReimbursementEditParam.b_ReimbursementPlace.closest("div"));
            app.util.ModelToDetails(ExpenseReimbursementEditParam.b_CompanyCode.closest("div"));
        }

        //显示操作按钮
        $ReimbursementItemTr.find(".AddReimbursementItem,.deleteReimbursementItem").show();
        $LoanItemTr.find(".AddLoanItem,.deleteLoanItem").show();
        $("#SaveExpenseReimbursement").show();
        $("#SubmitExpenseReimbursement").show();
    }
    else if (status == "Dept.Manager" || status == "Dept.Director" || status == "Division VP" || status == "Project Manager" || status == "Project Director" || status == "Project VP" || status == "GM") {
        app.util.ModelToDetails($ExpenseReimbursementEdit);
        //显示操作按钮
        $ReimbursementItemTr.find(".AddReimbursementItem,.deleteReimbursementItem").hide();
        $LoanItemTr.find(".AddLoanItem,.deleteLoanItem").hide();
        $("#AuditApprove").show();
        $("#SendRefuse").show();
    }
    else if (status == "Expense Accountant Check" || status == "Expense Accountant Creation") {
        app.util.ModelToDetails($ExpenseReimbursementEdit);
        app.util.ModelToEdit($ReimbursementItem);
        app.util.ModelToEdit($LoanItem);
        app.util.ModelToEdit(ExpenseReimbursementEditParam.b_AttachmentsQuantity.closest("div"));
        app.util.ModelToEdit(ExpenseReimbursementEditParam.b_AmountInWords.closest("div"));
        app.util.ModelToEdit(ExpenseReimbursementEditParam.b_TotalAmount.closest("div"));
        app.util.ModelToDetails($ReimbursementItemTr.find(".b_BudgetNumber").closest("td"));
        app.util.ModelToDetails($ReimbursementItemTr.find(".b_ProjectName").closest("td"));
        //显示操作按钮
        $ReimbursementItemTr.find(".AddReimbursementItem,.deleteReimbursementItem").hide();
        $LoanItemTr.find(".AddLoanItem,.deleteLoanItem").hide();
        $("#AuditApprove").show();
        $("#SendRefuse").show();
        $("#SendHangUp").show();
        $("#TurnToDo").show();
    }
    else if (status == "Financial Analyst") {
        app.util.ModelToDetails($ExpenseReimbursementEdit);
        //显示操作按钮
        $ReimbursementItemTr.find(".AddReimbursementItem,.deleteReimbursementItem").hide();
        $LoanItemTr.find(".AddLoanItem,.deleteLoanItem").hide();
        app.util.ModelToEdit($ReimbursementItemTr.find(".b_BudgetNumber").closest("td"));
        $("#AuditApprove").show();
        $("#SendRefuse").show();
        $("#SendHangUp").show();
        $("#TurnToDo").show();
    }
    else if (status == "Financial Director") {
        app.util.ModelToDetails($ExpenseReimbursementEdit);
        //显示操作按钮
        $ReimbursementItemTr.find(".AddReimbursementItem,.deleteReimbursementItem").hide();
        $LoanItemTr.find(".AddLoanItem,.deleteLoanItem").hide();
        $("#AuditApprove").show();
        $("#SendRefuse").show();
        $("#SendHangUp").show();
        $("#TurnToDo").show();
    }
    else {
        app.util.ModelToDetails($ExpenseReimbursementEdit);
        //显示操作按钮
        $ReimbursementItemTr.find(".AddReimbursementItem,.deleteReimbursementItem").hide();
        $LoanItemTr.find(".AddLoanItem,.deleteLoanItem").hide();
        $("#AuditApprove").show();
        $("#SendRefuse").show();
        $("#SendHangUp").show();
    }
    app.util.ModelToEdit(ExpenseReimbursementEditParam.b_Remark.closest("div"));
}



//部门领导AutoComplete
var choiceUserList = [];
ExpenseReimbursementEditParam.b_LineLeader.autocomplete({
    source: function (request, response) {
        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
        response($.grep(choiceUserList, function (value) {
            return matcher.test(value);
        }));
    }
});

ExpenseReimbursementEditParam.b_DepartmentLeader.autocomplete({
    source: function (request, response) {
        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
        response($.grep(choiceUserList, function (value) {
            return matcher.test(value);
        }));
    }
});


ExpenseReimbursementEditParam.b_DivisionVP.autocomplete({
    source: function (request, response) {
        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
        response($.grep(choiceUserList, function (value) {
            return matcher.test(value);
        }));
    }
});

ExpenseReimbursementEditParam.b_Employee.autocomplete({
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




//报销明细委托事件
$ReimbursementItem.delegate("a", "click", function () {
    var $this = $(this);
    if ($this.hasClass("AddReimbursementItem")) {
        $this.closest("tr").after(AddNewReimbursementItemTr());
        return false;
    }
    if ($this.hasClass("deleteReimbursementItem")) {
        $this.closest("tr").remove();
        if ($ReimbursementItemTr.find("tr").length <= 0) {
            $ReimbursementItemTr.prepend(AddNewReimbursementItemTr());
        }
        CalculateData($this);
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
    $this.val($this.val().replace(/[^\d]/g, ''));
}).delegate("input.b_TaxRate", "keyup", function () {
    var $this = $(this);
    $this.val($this.val().replace(/[^\d\.]/g, ''));
}).delegate("input.b_Tax", "keyup", function () {
    var $this = $(this);
    $this.val($this.val().replace(/[^\d\.]/g, ''));
}).delegate("input.b_TaxFreeAmount", "keyup", function () {
    var $this = $(this);
    $this.val($this.val().replace(/[^\d\.]/g, ''));
}).delegate("input.b_OriginalCurrency,input.b_Rate,input.b_Count", "blur", function () {
    var $this = $(this);
    CalculateTax($this);
    CalculateAdvancedAmount();
}).delegate("input.b_Tax,input.b_TaxFreeAmount", "blur", function () {
    var $this = $(this);
    CalculateData($this);
    CalculateAdvancedAmount();
}).delegate("select.b_TaxRate", "change", function () {
    var $this = $(this);
    CalculateTax($this);
    CalculateAdvancedAmount();
});

//借款明细
$LoanItem.delegate("input.b_LoanAmount", "keyup", function () {
    var $this = $(this);
    $this.val($this.val().replace(/[^\d\.]/g, ''));
}).delegate("input.b_LoanAmount", "blur", function () {
    var $this = $(this);
    CalculateAdvancedAmount();
});

//计算税额
function CalculateTax($this) {
    var ItemTr = $this.closest("tr");
    //税率
    var b_TaxRate = $.trim(ItemTr.find("select.b_TaxRate").val());
    //原币单价
    var b_OriginalCurrency = $.trim(ItemTr.find("input.b_OriginalCurrency").val());
    //汇率
    var b_Rate = $.trim(ItemTr.find("input.b_Rate").val());
    //数量
    var b_Count = $.trim(ItemTr.find("input.b_Count").val());
    if (b_TaxRate != "" && b_OriginalCurrency != "" && b_Rate != "" && b_Count != "") {
        //税额
        var b_Tax = (parseFloat(parseFloat(b_TaxRate) * parseFloat(b_OriginalCurrency) * parseFloat(b_Rate) * parseFloat(b_Count))) / (1 + parseFloat(b_TaxRate));
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

        ////大写
        //var b_AmountInWords = convertCurrency(b_CNYSubtotal);
        //ItemTr.find("label.b_AmountInWords").html(b_AmountInWords);
    }
    var b_TotalAmount = 0, b_AmountInWords;
    //计算合计（大写）、合计（小写）
    $ReimbursementItemTr.find("tr").each(function () {
        var $this = $(this);
        b_TotalAmount = parseFloat(b_TotalAmount) + parseFloat($this.find("label.b_CNYSubtotal").html());
    });
    //合计（小写）
    ExpenseReimbursementEditParam.b_TotalAmount.val(b_TotalAmount);

    //合计（大写）
    b_AmountInWords = convertCurrency(b_TotalAmount);
    ExpenseReimbursementEditParam.b_AmountInWords.val(b_AmountInWords);

    //总费用
    ExpenseReimbursementEditParam.b_TotalExpense.html(b_TotalAmount);

    //向公司预支现金
    var b_AdvancedAmount = 0;
    $LoanItemTr.find("tr").each(function () {
        var $this = $(this);
        var b_LoanAmount = $.trim($this.find("input.b_LoanAmount").val());
        if (b_LoanAmount == "") {
            b_LoanAmount = 0;
        }
        b_AdvancedAmount = parseFloat(b_AdvancedAmount) + parseFloat(b_LoanAmount);
        b_AdvancedAmount = toDecimal2(b_AdvancedAmount);
    });
    ExpenseReimbursementEditParam.b_AdvancedAmount.html(b_AdvancedAmount);
    //应退还公司
    var b_DueCompany = 0;
    b_DueCompany = parseFloat(ExpenseReimbursementEditParam.b_TotalExpense.html()) - parseFloat(b_AdvancedAmount);
    ExpenseReimbursementEditParam.b_DueCompany.html(b_DueCompany);
}


//计算预支现金  和 应退还公司
function CalculateAdvancedAmount() {
    //向公司预支现金
    var b_AdvancedAmount = 0;
    $LoanItemTr.find("tr").each(function () {
        var $this = $(this);
        var b_LoanAmount = $.trim($this.find("input.b_LoanAmount").val());
        if (b_LoanAmount == "") {
            b_LoanAmount = 0;
        }
        b_AdvancedAmount = parseFloat(b_AdvancedAmount) + parseFloat(b_LoanAmount);
        b_AdvancedAmount = toDecimal2(b_AdvancedAmount);
    });
    ExpenseReimbursementEditParam.b_AdvancedAmount.html(b_AdvancedAmount);

    //应退还公司
    var b_DueCompany = 0;
    b_DueCompany = parseFloat(ExpenseReimbursementEditParam.b_TotalExpense.html()) - parseFloat(b_AdvancedAmount);
    ExpenseReimbursementEditParam.b_DueCompany.html(b_DueCompany);
}


//当手动修改税额  和不含税金额时触发
function CalculateData($this) {
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

    var b_TotalAmount = 0, b_AmountInWords;
    //计算合计（大写）、合计（小写）
    $ReimbursementItemTr.find("tr").each(function () {
        var $this = $(this);
        b_TotalAmount = parseFloat(b_TotalAmount) + parseFloat($this.find("label.b_CNYSubtotal").html());
    });
    //合计（小写）
    ExpenseReimbursementEditParam.b_TotalAmount.val(b_TotalAmount);

    //合计（大写）
    b_AmountInWords = convertCurrency(b_TotalAmount);
    ExpenseReimbursementEditParam.b_AmountInWords.val(b_AmountInWords);

    //总费用
    ExpenseReimbursementEditParam.b_TotalExpense.html(b_TotalAmount);
}




function AddNewReimbursementItemTr(operatorType) {
    var html = "";
    if (operatorType == "audit")
    {
        html = $(ReimbursementItemHtml);
        html.find(".b_ProjectName").first().closest("td").empty().append("<input type='text' class='form-control b_ProjectName'/>")
    }
    else
    {
        html = $(ReimbursementItemHtml);
    }
   
    html.find(".b_Date").datepicker({ autoclose: true, format: 'yyyy-mm-dd' });
    var b_Type = "";
    $ExpenseReimbursementEdit.find("input[name='b_Type']").each(function () {
        var $this = $(this);
        if ($this.closest("div").hasClass("checked")) {
            b_Type = $this.val();
        }
    });
    if (b_Type == "Non Project")
    {
        html.find(".b_ProjectName").attr("disabled", "disabled");
    }

    //html.find(".b_ProjectName").autocomplete({
    //    source: function (request, response) {
    //        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
    //        response($.grep(projectList, function (value) {
    //            return matcher.test(value);
    //        }));
    //    }, scrollHeight: 300
    //});
    html.find(".b_ProjectName").first().empty();
    if (projectList.length > 0) {
        html.find(".b_ProjectName").first().append("<option value=''></option>");
        for (var i = 0; i < projectList.length; i++) {
            var value = projectList[i];
            html.find(".b_ProjectName").first().append("<option value='" + value + "'>" + value + "</option>");
        }
    }

    //费用类别
    html.find(".b_CategoryNumber").first().empty();
    if (costNamelist.length > 0) {
        html.find(".b_CategoryNumber").first().append("<option value=''></option>");
        for (var i = 0; i < costNamelist.length; i++) {
            var value = costNamelist[i];
            html.find(".b_CategoryNumber").first().append("<option value='" + value + "'>" + value + "</option>");
        }
    }

    //货币种类
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

function AddNewLoanItemTr(userName) {
    var html = $(LoanItemHtml);
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

//项目列表AutoComplate
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
    formData.append("myfile", document.getElementById("Erfile").files[0]);
    formData.append("id", ExpenseReimbursementEditParam.Id.val());
    formData.append("status", ExpenseReimbursementEditParam.status.val());
    formData.append("relationTableName", "R_ExpenseReimbursementFile");
    $.ajax({
        url: '/FileManage/UploadFile',
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (ans) {
            if (ans.error.length > 0) {
                app.util.onError(ans.error, $ExpenseReimbursementEdit);
                return;
            }
            if (ans.data != null) {
                $("#fileList").append("<div><label><a href='#' class='fileObj' id='" + ans.data.id + "' onclick='DownloadAttachment(this)'>" + ans.data.fileName + "</a>&nbsp;&nbsp;&nbsp;<a href='#' class='glyphicon glyphicon-remove text-danger' id='" + ans.data.id + "' relationId='" + ans.data.relationId + "' onclick='DeleteFile(this)'></a></label></div>");
            }
        }
    });
});


///删除附件
function DeleteFile(obj) {
    if (confirm("您确定要删除改附件吗？")) {
        $.ajax({
            url: '/FileManage/DeleteFile',
            data: { "id": $(obj).attr("Id"), "relationId": $(obj).attr("relationId"), "relationTableName": "R_ExpenseReimbursementFile" },
            dataType: "json",
            success: function (ans) {
                if (ans.error.length > 0) {
                    app.util.onError(ans.error, $ExpenseReimbursementEdit);
                    return;
                }
                $(obj).closest("div").remove();
            }
        })
    }
}


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
            ExpenseReimbursementEditParam.b_ReimbursementPlace.empty();
            ExpenseReimbursementEditParam.b_ReimbursementPlace.append("<option value=''>选择地区</option>")
            if (ans.data != null && ans.data.length > 0) {
                for (var i = 0; i < ans.data.length; i++) {
                    var value = ans.data[i];
                    ExpenseReimbursementEditParam.b_ReimbursementPlace.append("<option value='" + value + "'>" + value + "</option>");
                }
            }
        }
    });
}

//获取费用类别
var costNamelist = [];
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


//获取币种列表
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
$ExpenseReimbursementEdit.delegate("input[name='b_Employee']", "blur", function () {
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
                ExpenseReimbursementEditParam.b_StaffNo.val(item.b_JobNumber);
                ExpenseReimbursementEditParam.b_Dept.val(item.b_Department);
                ExpenseReimbursementEditParam.b_LineLeader.val(item.b_SeniorManager);
                ExpenseReimbursementEditParam.b_DepartmentLeader.val(item.b_Director);
                ExpenseReimbursementEditParam.b_DivisionVP.val(item.b_VP);
                if (item.b_Department != null && item.b_Department != "") {
                    ExpenseReimbursementEditParam.b_Dept.val(item.b_Department);
                }
                else if (item.b_Centre != null && item.b_Dept != "") {
                    ExpenseReimbursementEditParam.b_Dept.val(item.b_Centre);
                }
                ExpenseReimbursementEditParam.b_CompanyCode.val(item.b_CompanyCode);
                ExpenseReimbursementEditParam.b_CostCenter.val(item.b_CostCenter);
                $LoanItemTr.find("tr").each(function () {
                    $(this).find("input.b_Borrower").first().val(name);
                });
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
            ExpenseReimbursementEditParam.b_CompanyCode.empty();
            ExpenseReimbursementEditParam.b_CompanyCode.append("<option value=''>选择公司代码</option>")
            if (ans.data != null && ans.data.length > 0) {
                for (var i = 0; i < ans.data.length; i++) {
                    var value = ans.data[i];
                    ExpenseReimbursementEditParam.b_CompanyCode.append("<option value='" + value + "'>" + value + "</option>");
                }
            }
        }
    });
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
        $ReimbursementItem.find("select.b_ProjectName").each(function () {
            $(this).removeAttr("disabled");
        });
        $("#departmentLeaderInfo").hide();
    }
    else {
        $ReimbursementItem.find("select.b_ProjectName").each(function () {
            $(this).attr("disabled", "disabled");
            $(this).val("");
        });
        $("#departmentLeaderInfo").show();
    }
});






