var $PrManageDetailModel = $("#PrManageDetailModel");
var detailParam = app.util.getEditParam($PrManageDetailModel);
var $PrManageItemTrDetail = $("#PrManageItemTrDetail");
var htmlItem = $PrManageItemTrDetail.html();

var $PrQuotationItemTrDetail = $("#PrQuotationItemTrDetail");
var QuotationItemDetailHtml = $PrQuotationItemTrDetail.html();

var $PrRepeateItemTrDetail = $("#PrRepeateItemTrDetail");
var RepeateItemDetailHtml = $PrRepeateItemTrDetail.html();

var $PrChoiceSuppliersTrDetail = $("#PrChoiceSuppliersTrDetail");
var ChoiceSupplierDetailHtml = $PrChoiceSuppliersTrDetail.html();




function LoadDetail(id) {
    $.ajax({
        url: '/PrManage/GetPrManageById',
        data: { "id": id },
        dataType: "json",
        success: function (ans) {
            if (ans.error.length > 0) {
                app.util.onError(ans.error, $PrManageDetailModel);
                return;
            }

            $PrManageDetailModel.find("h4.modal-title").empty().append("<label style='color:rgb(88,136,193)'>" + ans.data.b_PrRecordNo + "</label>");
            app.util.clearError($PrManageDetailModel);
            app.util.bindValue(detailParam, ans);

            if (ans != null && ans.data != null && ans.data.b_Budget != null)
            {
                detailParam.b_Budget.html(ans.data.b_Budget.formatMoney());
            }

            //获取单据状态
            var status = ans.data.status;
            if (ans.data.Remark != "") {
                $("label[name='remarkDetail']").html(ans.data.Remark);
                $("#PrManageRemarkDetail").show();
            }
            else {
                $("#PrManageRemarkDetail").hide();
            }

            if (ans.data.b_AdditionalBudget != "" && ans.data.b_AdditionalBudget != null) {

                $("div.AdditionalBudgetDetail").show();
            }
            else {
                $("div.AdditionalBudgetDetail").hide();
            }

            //采购类型
            $PrManageDetailModel.find("input[name='b_PrType']").each(function () {
                var $this = $(this);
                if ($this.val() == ans.data.b_PrType) {
                    $this.closest("div").addClass("checked");
                }
                else {
                    $this.closest("div").removeClass("checked");
                }
            });

            ////采购类型
            //$PrManageDetailModel.find("input[name='b_ContractParty']").each(function () {
            //    var $this = $(this);
            //    if ($this.val() == ans.data.b_ContractParty) {
            //        $this.closest("div").addClass("checked");
            //    }
            //    else {
            //        $this.closest("div").removeClass("checked");
            //    }
            //});

            if (ans.data.b_PrType == "project") {
                $(".ProjectLeaderInfoDetail").show();
                $("#departmentLeaderInfoDetail").hide();
            }
            else {
                $(".ProjectLeaderInfoDetail").hide();
                $("#departmentLeaderInfoDetail").show();
            }
            $PrManageItemTrDetail.empty();
            //显示需求信息
            if (ans.data != null && ans.data.PrManageItems != null && ans.data.PrManageItems.length > 0) {
                for (var i = 0; i < ans.data.PrManageItems.length; i++) {
                    var Item = ans.data.PrManageItems[i];
                    var $tr = $(htmlItem);
                    $tr.find(".b_RequestList").html(Item.b_RequestList);
                    $tr.find(".b_SpecificationQuantity").html(Item.b_SpecificationQuantity);
                    $tr.find(".b_ProjectNo").html(Item.b_ProjectNo);
                    $tr.find(".b_TaskNo").html(Item.b_TaskNo);
                    $tr.find(".b_Qty").html(Item.b_Qty);
                    $tr.find(".b_Unit").html(Item.b_Unit);
                    $PrManageItemTrDetail.append($tr);
                }
            }
            $PrQuotationItemTrDetail.empty();
            //显示报价信息
            if (ans.data != null && ans.data.PrQuotationItems != null && ans.data.PrQuotationItems.length > 0) {
                for (var i = 0; i < ans.data.PrQuotationItems.length; i++) {
                    var item = ans.data.PrQuotationItems[i];
                    var $tr = $(QuotationItemDetailHtml);
                    $tr.find(".b_Supplier").html(item.b_Supplier);
                    $tr.find(".b_Quotation").html(item.b_Quotation);
                    $tr.find(".b_Remarks").html(item.b_Remarks);
                    $PrQuotationItemTrDetail.append($tr);
                    $("#PurchasingInquiryDetail").show();
                }
            }
            else {
                $("#PurchasingInquiryDetail").hide();
            }

            //重复采购信息
            $PrRepeateItemTrDetail.empty();
            if (ans.data != null && ans.data.PrRepeateItems != null && ans.data.PrQuotationItems.length > 0) {
                for (var i = 0; i < ans.data.PrRepeateItems.length; i++) {
                    var item = ans.data.PrRepeateItems[i];
                    var $tr = $(RepeateItemDetailHtml);
                    $tr.find(".b_PrRecordNo").text(item.b_PrRecordNo);
                    $tr.find(".b_PreviousSupplier").text(item.b_PreviousSupplier);
                    $tr.find(".b_ContractNo").text(item.b_ContractNo);
                    //$tr.find(".b_ContractPrice").text(item.b_ContractPrice);
                    $tr.find(".b_PreviousBuyer").text(item.b_PreviousBuyer);
                    $PrRepeateItemTrDetail.append($tr);
                    $("#PrRepeateItemDetail").show();
                }
            }
            else {
                $("#PrRepeateItemDetail").hide();
            }

            //选择供应商信息
            $PrChoiceSuppliersTrDetail.empty();
            if (ans.data != null && ans.data.PrChoiceSupplierItems != null && ans.data.PrChoiceSupplierItems.length > 0)
            {
                for(var i=0;i<ans.data.PrChoiceSupplierItems.length;i++)
                {
                    var item = ans.data.PrChoiceSupplierItems[i];
                    var $tr = $(ChoiceSupplierDetailHtml);
                    $tr.find(".b_Supplier").text(item.b_Supplier);
                    $tr.find(".b_ContractPrice").text(item.b_ContractPrice);
                    $tr.find(".b_PoNo").text(item.b_PoNo);
                    if (item.b_ContractProperty == "openContract")
                    {
                        $tr.find(".b_ContractProperty").text("开口");
                    }
                    else
                    {
                        $tr.find(".b_ContractProperty").text("闭口");
                    }
                    $tr.find(".b_PaymentClause").text(item.b_PaymentClause);
                    $PrChoiceSuppliersTrDetail.append($tr);
                    $("#PrChoiceSuppliersDetail").show();
                }
            }
            else
            {
                $("#PrChoiceSuppliersDetail").hide();
            }


            ////合同性质
            //if (ans.data.b_ContractProperty != null && ans.data.b_ContractProperty != "") {
            //    $PrManageDetailModel.find("input[value='" + ans.data.b_ContractProperty + "']").iCheck('check');
            //}



            //合同类型
            if (ans.data.b_ContractType != null && ans.data.b_ContractType != "") {
                $PrManageDetailModel.find("input[value='" + ans.data.b_ContractType + "']").iCheck('check');
            }


            //附件信息
            $("#fileListDetail").empty();
            if (ans.data != null && ans.data.Files != null && ans.data.Files.length > 0) {
                for (var i = 0; i < ans.data.Files.length; i++) {
                    var item = ans.data.Files[i];
                    if (item.comments != "Contract Management" || (item.comments == "Contract Management" && ans.data.IsPurchasingAuth)) {
                        $("#fileListDetail").append("<div><label><a href='#' id='" + item.id + "' onclick='DownloadAttachmentDetail(this)'>" + item.fileName + "</a></label></div>");
                    }
                }
            }

            //详情中Radio 或者 checkbox 不可操作
            $PrManageDetailModel.find("input[type=radio],input[type=checkbox]").each(function () {
                $(this).attr("disabled", "disabled");
            });
            showDetailByStatus(ans.data, status);
            $PrManageDetailModel.modal("show");
        }
    })

}




function showDetailByStatus(data, status) {

    if (status == "Financial Analyst" || status == "Financial Manager" || status == "Financial Director" || status == "CFO") {
        $("#b_BudgetStatusDetail").show();
        $("#b_BuyerDetail").hide();
        $("#PurchasingInquiryDetail").hide();
        $("#ContractRegisterDetail").hide();
        $("#ContractManagementDetail").hide();
    }
    else if (status == "Receive PR") {
        $("#b_BudgetStatusDetail").show();
        $("#b_BuyerDetail").show();
        $("#PurchasingInquiryDetail").hide();
        $("#ContractRegisterDetail").hide();
        $("#ContractManagementDetail").hide();
    }
    else if (status == "Buyer Inquiry") {
        $("#b_BudgetStatusDetail").show();
        $("#b_BuyerDetail").show();
        if (data.IsPurchasingAuth) {
            $("#PurchasingInquiryDetail").show();
        }
        else {
            $("#PurchasingInquiryDetail").hide();
        }
        $("#ContractRegisterDetail").hide();
        $("#ContractManagementDetail").hide();
    }
    else if (status == "Contract Registration") {
        $("#b_BudgetStatusDetail").show();
        $("#b_BuyerDetail").show();
        if (data.IsPurchasingAuth) {
            $("#PurchasingInquiryDetail").show();
            $("#ContractRegisterDetail").show();
        }
        else {
            $("#PurchasingInquiryDetail").hide();
            $("#ContractRegisterDetail").hide();
        }
        $("#ContractManagementDetail").hide();
    }
    else if (status == "End" || status == "Contract Management" || status == "Purchase Manager" || status == "Purchase Director") {
        $("#b_BudgetStatusDetail").show();
        $("#b_BuyerDetail").show();
        if (data.IsPurchasingAuth) {
            $("#PurchasingInquiryDetail").show();
            $("#ContractRegisterDetail").show();
            $("#ContractManagementDetail").show();
        }
        else {
            $("#PurchasingInquiryDetail").hide();
            $("#ContractRegisterDetail").hide();
            $("#ContractManagementDetail").hide();
        }

    }
    else {
        $("#b_BudgetStatusDetail").hide();
        $("#b_BuyerDetail").hide();
        $("#PurchasingInquiryDetail").hide();
        $("#ContractRegisterDetail").hide();
        $("#ContractManagementDetail").hide();
    }
}

//下载附件
function DownloadAttachmentDetail(obj) {
    location.href = '/PrManage/DownloadAttachment' + "?id=" + $(obj).attr("id");
}








