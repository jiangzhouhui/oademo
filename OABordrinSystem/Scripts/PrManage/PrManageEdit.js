var $PrManageEdit = $("#PrManageEdit"), $PrManageItem = $("#PrManageItem"), $PrManageItemTr = $("#PrManageItemTr");
//采购询价对象
var $PrQuotationItem = $("#PrQuotationItem"), $PrQuotationItemTr = $("#PrQuotationItemTr"), QuotationHtmlTr = $PrQuotationItemTr.html();
//重复采购
var $PrRepeateItem = $("#PrRepeateItem"), $PrRepeateItemTr = $("#PrRepeateItemTr"), RepeateHtmlTr = $PrRepeateItemTr.html();
//选择供应商
var $PrChoiceSuppliers = $("#PrChoiceSuppliers"), $PrChoiceSuppliersTr = $("#PrChoiceSuppliersTr"), ChoiceSupplierHtmlTr = $PrChoiceSuppliersTr.html();

var htmlTr = $PrManageItemTr.html();
PrManageEditParam = app.util.getEditParam($PrManageEdit);


$(document).ready(function () {
    GetAutocompleteChoiceUserData();

    $('.i-checks').iCheck({
        checkboxClass: 'icheckbox_square-green',
        radioClass: 'iradio_square-green',
    });

    PrManageEditParam.b_Budget.digital();
    PrManageEditParam.b_AdditionalBudget.digital();

    $("input[name='b_PrType']").on('ifChecked', function (event) {
        var $this = $(this);
        if ($this.val() == "project") {
            $(".ProjectLeaderInfo").show();
            $("#departmentLeaderInfo").show();
        }
        else {
            $(".ProjectLeaderInfo").hide();
            $("#departmentLeaderInfo").show();
        }
    });

    $("input[name='b_RepetitivePurchase']").on('ifChanged', function (event) {
        var $this = $(this);
        if ($this[0].checked == true) {
            $PrRepeateItem.show();
        }
        else {
            $PrRepeateItem.hide();
        }
    });

    $("input[name='b_ContractType']").on('ifChanged', function (event) {
        var $this = $(this);
        if ($this[0].checked == true && $this.val() == "Other Contract") {
            $("#DownloadContract").hide();
        }
        else {
            $("#DownloadContract").show();
        }
    });

    $PrManageItem.delegate("a", "click", function () {
        var $this = $(this);
        if ($this.hasClass("AddPrManageItem")) {
            $this.closest("tr").after(AddNewPrManageItem());
            return false;
        }

        if ($this.hasClass("deletePrManageItem")) {
            $this.closest("tr").remove();
            if ($PrManageItemTr.find("tr").length <= 0) {
                $PrManageItemTr.prepend(AddNewPrManageItem());
            }
            return false;
        }
    }).delegate("input.b_Qty", "keyup", function () {
        var $this = $(this);
        $this.val($this.val().replace(/[^\d]/g, ''));
    });

    //项目列表
    GetProjectList();

    //加载合同编号
    LoadContractNoList();

});



//部门领导AutoComplete
var choiceUserList = [];
PrManageEditParam.b_DeptManager.autocomplete({
    source: function (request, response) {
        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
        response($.grep(choiceUserList, function (value) {
            return matcher.test(value);
        }));
    }
});
PrManageEditParam.b_DeptDirector.autocomplete({
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
PrManageEditParam.b_ProjectName.autocomplete({
    source: function (request, response) {
        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
        response($.grep(projectList, function (value) {
            return matcher.test(value);
        }));
    }, scrollHeight: 300
});

var contractNoList = [];
function LoadContractNoList() {
    $.ajax({
        url: '/AutoComplete/GetContractNoList',
        dataType: "json",
        success: function (ans) {
            if (ans.error.length > 0) {
                return false;
            }
            contractNoList = [];
            if (ans.data != null && ans.data.length > 0) {
                for (var i = 0; i < ans.data.length; i++) {
                    var item = ans.data[i];
                    contractNoList.push(item);
                }
            }
        }
    });
}

//采购询价委托事件
$PrQuotationItem.delegate("a", "click", function () {
    var $this = $(this);
    if ($this.hasClass("AddItem")) {
        $this.closest("tr").after($(QuotationHtmlTr));
        return false;
    }
    if ($this.hasClass("deleteItem")) {
        $this.closest("tr").remove();
        if ($PrQuotationItemTr.find("tr").length <= 0) {
            $PrQuotationItemTr.prepend($(QuotationHtmlTr));
        }
        return false;
    }
});

function AddNewPrManageItem() {
    var html = $(htmlTr);
    return html;
}


//重复采购
$PrRepeateItem.delegate("a", "click", function () {
    var $this = $(this);
    if ($this.hasClass("AddItem")) {
        $this.closest("tr").after(AddNewRepeateItem());
        return false;
    }
    if ($this.hasClass("deleteItem")) {
        $this.closest("tr").remove();
        if ($PrRepeateItemTr.find("tr").length <= 0) {
            $PrRepeateItemTr.prepend(AddNewRepeateItem());
        }
        return false;
    }
});

function AddNewRepeateItem() {
    var html = $(RepeateHtmlTr);
    if (PrManageEditParam.status.val() == "Buyer Inquiry") {
        html.find(".b_ContractNo").autocomplete({
            source: function (request, response) {
                var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
                response($.grep(contractNoList, function (value) {
                    return matcher.test(value);
                }));
            }, scrollHeight: 300
        });
    }
    return html;
}


//选择供应商
$PrChoiceSuppliers.delegate("a", "click", function () {
    var $this = $(this);
    if ($this.hasClass("AddItem")) {
        $this.closest("tr").after(AddChoiceSupplierItem());
        return false;
    }
    if ($this.hasClass("deleteItem")) {
        $this.closest("tr").remove();
        if ($PrChoiceSuppliersTr.find("tr").length <= 0) {
            $PrChoiceSuppliersTr.prepend(AddChoiceSupplierItem());
        }
        ContractBudgetHandle();
        return false;
    }
}).delegate("input.b_ContractPrice", "keyup", function () {
    var $this = $(this);
    $this.val($this.val().replace(/[^\d\.]/g, ''));
}).delegate("input.b_ContractPrice", "blur", function () {
    ContractBudgetHandle();
});


function AddChoiceSupplierItem() {
    var html = $(ChoiceSupplierHtmlTr);
    $.ajax({
        url: '/PrManage/GetContractPoNo',
        data: { "b_Buyer": PrManageEditParam.b_Buyer.val() },
        dataType: "json",
        async: false,
        success: function (ans) {
            if (ans.error.length > 0) {
                return false;
            }
            html.find(".b_PoNo").val(ans.data);
        }
    });
    return html;
}


function GetSaveParam(operation) {
    var param = app.util.serializeParamArray(PrManageEditParam);
    param.operation = operation;
    //获取单选按钮的值
    var b_PrType;
    $PrManageEdit.find("input[name='b_PrType']").each(function () {
        var $this = $(this);
        if ($this.closest("div").hasClass("checked")) {
            b_PrType = $this.val();
        }
    });

    param.b_PrType = b_PrType;
    var items = [];
    $PrManageItemTr.find("tr").each(function () {
        var $this = $(this);
        var id = $.trim($this.find("input.id").val());
        var b_RequestList = $.trim($this.find("input.b_RequestList").val());
        var b_SpecificationQuantity = $.trim($this.find("input.b_SpecificationQuantity").val());
        var b_ProjectNo = $.trim($this.find("input.b_ProjectNo").val());
        var b_TaskNo = $.trim($this.find("input.b_TaskNo").val());
        var b_Qty = $.trim($this.find("input.b_Qty").val());
        var b_Unit = $.trim($this.find("input.b_Unit").val());
        if (b_RequestList != "" && b_SpecificationQuantity != "" && b_Qty != "" && b_Unit != "") {
            items.push({ "id": id, "b_RequestList": b_RequestList, "b_SpecificationQuantity": b_SpecificationQuantity, "b_ProjectNo": b_ProjectNo, "b_TaskNo": b_TaskNo, "b_Qty": b_Qty, "b_Unit": b_Unit });
        }
    });
    param.PrManageItems = items;

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


//获取审核的界面数据
function GetAuditPrManageParam() {
    var param = app.util.serializeParamArray(PrManageEditParam);
    if (PrManageEditParam.status.val() == "Receive PR") {
        var AuthorizedPurchase = $("input[name=b_AuthorizedPurchase]").closest("div").hasClass('checked');
        param.b_AuthorizedPurchase = AuthorizedPurchase;
    }

    //获取单选按钮的值
    var b_PrType;
    $PrManageEdit.find("input[name='b_PrType']").each(function () {
        var $this = $(this);
        if ($this.closest("div").hasClass("checked")) {
            b_PrType = $this.val();
        }
    });
    param.b_PrType = b_PrType;

    if (PrManageEditParam.status.val() == "Buyer Inquiry") {
        var prQuotationItems = [];
        $PrQuotationItemTr.find("tr").each(function () {
            var $this = $(this);
            var id = $.trim($this.find("input.id").val());
            var b_Supplier = $.trim($this.find("input.b_Supplier").val());
            var b_Quotation = $.trim($this.find("input.b_Quotation").val());
            var b_Remarks = $.trim($this.find("input.b_Remarks").val());
            if (b_Supplier != "") {
                prQuotationItems.push({ "id": id, "b_Supplier": b_Supplier, "b_Quotation": b_Quotation, "b_Remarks": b_Remarks });
            }
        });
        param.PrQuotationItems = prQuotationItems;
        //获取选择项
        var UrgentPurchase = $("input[name=b_UrgentPurchase]").closest("div").hasClass('checked');
        var RepetitivePurchase = $("input[name=b_RepetitivePurchase]").closest("div").hasClass('checked');
        var IsSingleSupplier = $("input[name=b_IsSingleSupplier]").closest("div").hasClass('checked');
        param.b_UrgentPurchase = UrgentPurchase;
        param.b_RepetitivePurchase = RepetitivePurchase;
        param.b_IsSingleSupplier = IsSingleSupplier;

        //获取重复采购数据
        var prRepeateItems = [];
        if (param.b_RepetitivePurchase) {
            $PrRepeateItemTr.find("tr").each(function () {
                var $this = $(this);
                var id = $.trim($this.find(".id").val());
                var b_PrRecordNo = $.trim($this.find(".b_PrRecordNo").text());
                var b_PreviousSupplier = $.trim($this.find(".b_PreviousSupplier").text());
                var b_ContractNo = $.trim($this.find(".b_ContractNo").val());
                var b_PreviousBuyer = $.trim($this.find(".b_PreviousBuyer").text());
                if (b_PrRecordNo != "" && b_PreviousSupplier != "" && b_ContractNo != "" && b_PreviousBuyer != "" && b_PreviousBuyer != "") {
                    prRepeateItems.push({ "id": id, "b_PrRecordNo": b_PrRecordNo, "b_PreviousSupplier": b_PreviousSupplier, "b_ContractNo": b_ContractNo, "b_PreviousBuyer": b_PreviousBuyer })
                }
            });
        }
        param.prRepeateItems = prRepeateItems;
    }

    if (PrManageEditParam.status.val() == "Contract Registration") {
        var prChoiceSupplierItems = [];
        $PrChoiceSuppliersTr.find("tr").each(function () {
            var $this = $(this);
            var id = $.trim($this.find("input.id").val());
            var b_Supplier = $.trim($this.find("input.b_Supplier").val());
            var b_ContractPrice = $.trim($this.find("input.b_ContractPrice").val());
            var b_PoNo = $.trim($this.find("input.b_PoNo").val());
            var b_ContractProperty = $.trim($this.find("select.b_ContractProperty").val());
            var b_PaymentClause = $.trim($this.find("input.b_PaymentClause").val());

            if (b_Supplier != "" && b_ContractPrice != "" && b_PoNo != "" && b_ContractProperty != "" && b_PaymentClause != "") {
                prChoiceSupplierItems.push({ "id": id, "b_Supplier": b_Supplier, "b_ContractPrice": b_ContractPrice, "b_PoNo": b_PoNo, "b_ContractProperty": b_ContractProperty, "b_PaymentClause": b_PaymentClause });
            }
        });
        param.prChoiceSupplierItems = prChoiceSupplierItems;
    }


    if (PrManageEditParam.status.val() == "Contract Management") {
        var b_ContractType;
        $PrManageEdit.find("input[name='b_ContractType']").each(function () {
            var $this = $(this);
            if ($this.closest("div").hasClass("checked")) {
                b_ContractType = $this.val();
            }
        });
        param.b_ContractType = b_ContractType;
    }
    return param;
}


//采购人员自动AutoComplate
var chioceBuyer = [];
$("input[name=b_Buyer]").autocomplete({
    source: function (request, response) {
        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
        response($.grep(chioceBuyer, function (value) {
            return matcher.test(value);
        }));
    }
});

function GetAutocompleteData() {
    $.ajax({
        url: '/AutoComplete/GetBuyerList',
        dataType: "json",
        success: function (ans) {
            if (ans.error.length > 0) {
                return false;
            }
            chioceBuyer = [];
            if (ans.data != null && ans.data.length > 0) {
                for (var i = 0; i < ans.data.length; i++) {
                    chioceBuyer.push(ans.data[i]);
                }
            }
        }
    });
}


//退回代码
$PrManageEdit.delegate("#SendBack", "click", function () {
    //获取单选按钮的值
    var b_PrType;
    $PrManageEdit.find("input[name='b_PrType']").each(function () {
        var $this = $(this);
        if ($this.closest("div").hasClass("checked")) {
            b_PrType = $this.val();
        }
    });

    $.ajax({
        url: '/PrManage/GetWorkflowProcessPathByActivityId',
        data: { "activityId": PrManageEditParam.activityId.val(), "b_PrType": b_PrType, "b_DeptManager": PrManageEditParam.b_DeptManager.val(), "b_DeptDirector": PrManageEditParam.b_DeptDirector.val() },
        dataType: "json",
        success: function (ans) {
            if (ans.error.length > 0) {
                return;
            }
            LoadActivityList(PrManageEditParam.id.val(), PrManageEditParam.activityId.val(), PrManageEditParam.activityAssignmentId.val(), ans.data);
            app.util.clearError($WorkflowActivityModel);
            $("#WorkflowActivityModel").modal("show");
        }
    });
});



