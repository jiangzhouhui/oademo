var $ExpenseReimbursementDetailModel = $("#ExpenseReimbursementDetailModel");
var detailParam = app.util.getEditParam($ExpenseReimbursementDetailModel);

var $ReimbursementItemTrDetail = $("#ReimbursementItemTrDetail");

var htmlReimbursementDetail = $ReimbursementItemTrDetail.html();

var $LoanItemDetail = $("#LoanItemDetail");
var $LoanItemTrDetail = $("#LoanItemTrDetail");
var htmlLoanItemDetail = $LoanItemTrDetail.html();


function LoadDetail(id) {
    $.ajax({
        url: '/ExpenseReimbursement/GetExpenseReimbursementById',
        data: { "id": id },
        dataType: "json",
        success: function (ans) {
            if (ans.error.length > 0) {
                app.util.onError(ans.error, $ExpenseReimbursementDetailModel);
                return;
            }
            if (ans.data.b_IsBudgetary) {
                ans.data.b_IsBudgetary = "是";
            }
            else {
                ans.data.b_IsBudgetary = "否";
            }

            if (ans.data.b_Type == "Non Project") {
                ans.data.b_Type = "非项目";
                $("#departmentLeaderInfoDetail").show();
            } else {
                ans.data.b_Type = "项目";
                $("#departmentLeaderInfoDetail").hide();
            }
            $ExpenseReimbursementDetailModel.find("h4.modal-title").empty().append("<label style='color:rgb(88,136,193)'>" + ans.data.b_RecordNo + "</label>");
            app.util.clearError($ExpenseReimbursementDetailModel);
            app.util.bindValue(detailParam, ans);

            $ReimbursementItemTrDetail.empty();
            if (ans.data != null && ans.data.ReimbursementItems != null && ans.data.ReimbursementItems.length > 0) {
                for (var i = 0; i < ans.data.ReimbursementItems.length; i++) {
                    var item = ans.data.ReimbursementItems[i];
                    var $tr = $(htmlReimbursementDetail);
                    $tr.find(".b_Date").html(item.b_Date);
                    $tr.find(".b_CategoryNumber").html(item.b_CategoryNumber);
                    $tr.find(".b_ProjectName").html(item.b_ProjectName);
                    $tr.find(".b_BudgetNumber").html(item.b_BudgetNumber);
                    $tr.find(".b_Currency").html(item.b_Currency);
                    $tr.find(".b_Rate").html(item.b_Rate);
                    $tr.find(".b_OriginalCurrency").html(item.b_OriginalCurrency);
                    $tr.find(".b_Count").html(item.b_Count);
                    $tr.find(".b_TaxRate").html(item.b_TaxRate);
                    $tr.find(".b_Tax").html(item.b_Tax);
                    $tr.find(".b_TaxFreeAmount").html(item.b_TaxFreeAmount);
                    $tr.find(".b_CNYSubtotal").html(item.b_CNYSubtotal);
                    $ReimbursementItemTrDetail.append($tr);
                }
            }

            $LoanItemTrDetail.empty();
            if (ans.data != null && ans.data.LoanItems != null && ans.data.LoanItems.length > 0) {
                for (var i = 0; i < ans.data.LoanItems.length; i++) {
                    var item = ans.data.LoanItems[i];
                    var $tr = $(htmlLoanItemDetail);
                    $tr.find(".b_LoanOrderNo").html(item.b_LoanOrderNo);
                    $tr.find(".b_Date").html(item.b_Date);
                    $tr.find(".b_Borrower").html(item.b_Borrower);
                    $tr.find(".b_LoanAmount").html(item.b_LoanAmount);
                    $tr.find(".b_LoanReason").html(item.b_LoanReason);
                    $LoanItemTrDetail.append($tr);
                }
                $LoanItemDetail.show();
            }
            else {
                $LoanItemDetail.hide();
            }


            //附件信息
            $("#fileListDetail").empty();
            if (ans.data != null && ans.data.Files != null && ans.data.Files.length > 0) {
                for (var i = 0; i < ans.data.Files.length; i++) {
                    var item = ans.data.Files[i];
                    $("#fileListDetail").append("<div><label><a href='#' id='" + item.id + "' onclick='DownloadAttachment(this)'>" + item.fileName + "</a></label></div>");
                }
            }
            $ExpenseReimbursementDetailModel.modal("show");
        }
    });

}


//下载附件
function DownloadAttachment(obj) {
    location.href = 'FileManage/DownloadAttachment' + "?id=" + $(obj).attr("Id");
}

