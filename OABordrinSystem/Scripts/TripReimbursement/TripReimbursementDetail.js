var $TripReimbursementDetailModel = $("#TripReimbursementDetailModel");
var detailParam = app.util.getEditParam($TripReimbursementDetailModel);

var $HotelExpenseItemTrDetail = $("#HotelExpenseItemTrDetail");
var htmlHotelExpenseItemDetail = $HotelExpenseItemTrDetail.html();

var $TrafficExpenseItemTrDetail = $("#TrafficExpenseItemTrDetail");
var htmlTrafficExpenseItemDetail = $TrafficExpenseItemTrDetail.html();

var $MealsSubsidiesItemTrDetail = $("#MealsSubsidiesItemTrDetail");
var htmlMealsSubsidiesItemDetail = $MealsSubsidiesItemTrDetail.html();

var $OthersItemTrDetail = $("#OthersItemTrDetail");
var htmlOthersItemDetail = $OthersItemTrDetail.html();

var $LoanItemsDetail = $("#LoanItemsDetail");
var $LoanItemsTrDetail = $("#LoanItemsTrDetail");
var htmlLoanItemsDetail = $LoanItemsTrDetail.html();
function LoadDetail(id) {
    $.ajax({
        url: '/TripReimbursement/GetTripReimbursementById',
        data: { "id": id },
        dataType: "json",
        success: function (ans) {
            if (ans.error.length > 0) {
                app.util.onError(ans.error, $TripReimbursementDetailModel);
                return;
            }
            if (ans.data.b_IsBudgetary) {
                ans.data.b_IsBudgetary = "是";
            }
            else {
                ans.data.b_IsBudgetary = "否";
            }
            if (ans.data.b_IntalBusiness) {
                ans.data.b_IntalBusiness = "是";
            }
            else {
                ans.data.b_IntalBusiness = "否";
            }
            if (ans.data.b_Type == "Non Project") {
                ans.data.b_Type = "非项目"
                $("#departmentLeaderInfoDetail").show();

            } else {
                ans.data.b_Type = "项目";
                $("#departmentLeaderInfoDetail").hide();
            }
            $TripReimbursementDetailModel.find("h4.modal-title").empty().append("<label style='color:rgb(88,136,193)'>" + ans.data.b_RecordNo + "</label>");
            app.util.clearError($TripReimbursementDetailModel);
            app.util.bindValue(detailParam, ans);


            if (ans.data.b_IsBeyondBudget == "是")
            {
                $("label.AddRedLabelDetail").css("color", "red");
                $("#BeyondReasonIsDisplayDetail").show();
            }
            else
            {
                $("label.AddRedLabelDetail").css("color", "");
                $("#BeyondReasonIsDisplayDetail").hide();
            }


            //住宿详情
            $HotelExpenseItemTrDetail.empty();
            if (ans.data != null && ans.data.HotelExpenseItems != null && ans.data.HotelExpenseItems.length > 0) {
                for (var i = 0; i < ans.data.HotelExpenseItems.length; i++) {
                    var item = ans.data.HotelExpenseItems[i];
                    var $tr = $(htmlHotelExpenseItemDetail);
                    $tr.find(".b_StartDate").html(item.b_StartDate);
                    $tr.find(".b_EndDate").html(item.b_EndDate);
                    $tr.find(".b_ProjectName").html(item.b_ProjectName);
                    $tr.find(".b_City").html(item.b_City);
                    $tr.find(".b_Hotel").html(item.b_Hotel);
                    $tr.find(".b_Currency").html(item.b_Currency);
                    $tr.find(".b_Rate").html(item.b_Rate);
                    $tr.find(".b_OriginalCurrency").html(item.b_OriginalCurrency);
                    $tr.find(".b_Count").html(item.b_Count);
                    $tr.find(".b_TaxRate").html(item.b_TaxRate);
                    $tr.find(".b_Tax").html(item.b_Tax);
                    $tr.find(".b_TaxFreeAmount").html(item.b_TaxFreeAmount);
                    $tr.find(".b_CNYSubtotal").html(item.b_CNYSubtotal);
                    $HotelExpenseItemTrDetail.append($tr);
                }
                $("#HotelDetailID").show();
            }
            else
            {
                 $("#HotelDetailID").hide();
            }

            //交通详情
            $TrafficExpenseItemTrDetail.empty();
            if (ans.data != null && ans.data.TrafficExpenseItems != null && ans.data.TrafficExpenseItems.length > 0) {
                for (var i = 0; i < ans.data.TrafficExpenseItems.length; i++) {
                    var item = ans.data.TrafficExpenseItems[i];
                    var $tr = $(htmlTrafficExpenseItemDetail);
                    $tr.find(".b_StartDate").html(item.b_StartDate);
                    $tr.find(".b_EndDate").html(item.b_EndDate);
                    $tr.find(".b_ProjectName").html(item.b_ProjectName);
                    $tr.find(".b_Type").html(item.b_Type);
                    $tr.find(".b_StartPoint").html(item.b_StartPoint);
                    $tr.find(".b_EndPoint").html(item.b_EndPoint);
                    $tr.find(".b_Currency").html(item.b_Currency);
                    $tr.find(".b_Rate").html(item.b_Rate);
                    $tr.find(".b_OriginalCurrency").html(item.b_OriginalCurrency);
                    $tr.find(".b_Count").html(item.b_Count);
                    $tr.find(".b_TaxRate").html(item.b_TaxRate);
                    $tr.find(".b_Tax").html(item.b_Tax);
                    $tr.find(".b_TaxFreeAmount").html(item.b_TaxFreeAmount);
                    $tr.find(".b_CNYSubtotal").html(item.b_CNYSubtotal);
                    $TrafficExpenseItemTrDetail.append($tr);
                }
                $("#TrafDetailID").show();
            }
            else
            {
                $("#TrafDetailID").hide();
            }

            //餐补详情
            $MealsSubsidiesItemTrDetail.empty();
            if (ans.data != null && ans.data.MealsSubsidiesItems != null && ans.data.MealsSubsidiesItems.length > 0) {
                for (var i = 0; i < ans.data.MealsSubsidiesItems.length; i++) {
                    var item = ans.data.MealsSubsidiesItems[i];
                    var $tr = $(htmlMealsSubsidiesItemDetail);
                    $tr.find(".b_StartDate").html(item.b_StartDate);
                    $tr.find(".b_EndDate").html(item.b_EndDate);
                    $tr.find(".b_ProjectName").html(item.b_ProjectName);
                    $tr.find(".b_Place").html(item.b_Place);
                    $tr.find(".b_CompanionAmount").html(item.b_CompanionAmount);
                    $tr.find(".b_CompanionName").html(item.b_CompanionName);
                    $tr.find(".b_Currency").html(item.b_Currency);
                    $tr.find(".b_Rate").html(item.b_Rate);
                    $tr.find(".b_FixedSubsidy").html(item.b_FixedSubsidy);
                    $tr.find(".b_TaxRate").html(item.b_TaxRate);
                    $tr.find(".b_Tax").html(item.b_Tax);
                    $tr.find(".b_TaxFreeAmount").html(item.b_TaxFreeAmount);
                    $tr.find(".b_CNYSubtotal").html(item.b_CNYSubtotal);
                    $MealsSubsidiesItemTrDetail.append($tr);                    
                }
                $("#MealDetailID").show();
            }
            else
            {

                 $("#MealDetailID").hide();
            }

            //其他详情
            $OthersItemTrDetail.empty();
            if (ans.data != null && ans.data.OthersItems != null && ans.data.OthersItems.length > 0) {
                for (var i = 0; i < ans.data.OthersItems.length; i++) {
                    var item = ans.data.OthersItems[i];
                    var $tr = $(htmlOthersItemDetail);
                    $tr.find(".b_StartDate").html(item.b_StartDate);
                    $tr.find(".b_EndDate").html(item.b_EndDate);
                    $tr.find(".b_ProjectName").html(item.b_ProjectName);
                    $tr.find(".b_Place").html(item.b_Place);
                    $tr.find(".b_Type").html(item.b_Type);
                    $tr.find(".b_Reason").html(item.b_Reason);
                    $tr.find(".b_Currency").html(item.b_Currency);
                    $tr.find(".b_Rate").html(item.b_Rate);
                    $tr.find(".b_OriginalCurrency").html(item.b_OriginalCurrency);
                    $tr.find(".b_Count").html(item.b_Count);
                    $tr.find(".b_TaxRate").html(item.b_TaxRate);
                    $tr.find(".b_Tax").html(item.b_Tax);
                    $tr.find(".b_TaxFreeAmount").html(item.b_TaxFreeAmount);
                    $tr.find(".b_CNYSubtotal").html(item.b_CNYSubtotal);
                    $OthersItemTrDetail.append($tr);                    
                }
                $("#OtherDetailID").show();
            }
            else
            {
                $("#OtherDetailID").hide();
            }

            //借款明细详情
            $LoanItemsTrDetail.empty();
            if (ans.data != null && ans.data.LoanItems != null && ans.data.LoanItems.length > 0) {
                for (var i = 0; i < ans.data.LoanItems.length; i++) {
                    var item = ans.data.LoanItems[i];
                    var $tr = $(htmlLoanItemsDetail);
                    $tr.find(".b_LoanOrderNo").html(item.b_LoanOrderNo);
                    $tr.find(".b_Date").html(item.b_Date);
                    $tr.find(".b_Borrower").html(item.b_Borrower);
                    $tr.find(".b_LoanAmount").html(item.b_LoanAmount);
                    $tr.find(".b_LoanReason").html(item.b_LoanReason);
                    $LoanItemsTrDetail.append($tr);
                }
                $("#LoanDetailID").show();
            }
            else
            {
                 $("#LoanDetailID").hide();
            }

            //附件信息
            $("#fileListDetail").empty();
            if (ans.data != null && ans.data.Files != null && ans.data.Files.length > 0)
            {
                for (var i = 0; i < ans.data.Files.length; i++) {
                    var item = ans.data.Files[i];
                    $("#fileListDetail").append("<div><label><a href='#' id='" + item.id + "' onclick='DownloadAttachment(this)'>" + item.fileName + "</a></label></div>");
                }
            }

            $TripReimbursementDetailModel.modal("show");
        }
    });
}

//下载附件
function DownloadAttachment(obj) {
    location.href = 'FileManage/DownloadAttachment' + "?id=" + $(obj).attr("id");
}