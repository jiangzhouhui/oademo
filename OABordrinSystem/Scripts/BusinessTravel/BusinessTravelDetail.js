
var $BusinessTravelDetailModel = $("#BusinessTravelDetailModel");
var detailParam = app.util.getEditParam($BusinessTravelDetailModel);

var $FlightBookingDetail = $("#FlightBookingDetail");
var $FlightBookingItemTrDetail = $("#FlightBookingItemTrDetail");
var FlightBookingDetailhtml = $FlightBookingItemTrDetail.html();

var $HotelBookingDetail = $("#HotelBookingDetail");
var $HotelBookingTrDetail = $("#HotelBookingTrDetail");
var HotelBookingDetailhtml = $HotelBookingTrDetail.html();

$(document).ready(function () {

    //$BusinessTravelDetailModel.find("input[name='nb_FlightBooking']").first().iCheck('check');
   //$("#b_Didi").iCheck('check');
    //$("#b_HotelBooking").iCheck('check');
});


function LoadDetail(id){
    $.ajax({
        url: '/BusinessTravel/GetBusinessTravelById',
        data: { "id": id },
        dataType: "json",
        success: function (ans) {
            if (ans.error.length > 0) {
                app.util.onError(ans.error, $BusinessTravelDetailModel);
                return;
            }
            if (ans.data.b_TripType == "Domestic") {
                ans.data.b_TripType = "国内";
            } else {
                ans.data.b_TripType = "国际";
            }       

            if (ans.data.b_Type == "Non Project") {
                ans.data.b_Type = "非项目";
                $("#departmentLeaderDetail").show();
                $("#b_ProjectNameDetail").hide();
            } else {
                ans.data.b_Type = "项目";
                $("#departmentLeaderDetail").hide();
                $("#b_ProjectNameDetail").show();
            }
            
            if (ans.data.b_IDType == "HuZhao") {
                ans.data.b_IDType = "护照";
                $("#radioInfo").show();
            } else {
                ans.data.b_IDType = "身份证号码";
                $("#radioInfo").hide();
            }
            if (ans.data.b_FlightIsssue == "FlightIsssue") {
                $BusinessTravelDetailModel.find("#b_FlightIsssue").first().iCheck('check');
            }
            else
            {
                $BusinessTravelDetailModel.find("#b_FlightIsssue").first().iCheck('uncheck');
            }
            if (ans.data.b_FlightBooking == "FlightBooking") {
                $BusinessTravelDetailModel.find("#b_FlightBooking").first().iCheck('check');
                $("#FlightBookingInfoDetails").show();
            } else {
                $BusinessTravelDetailModel.find("#b_FlightBooking").first().iCheck('uncheck');
                $("#FlightBookingInfoDetails").hide();
            }
            if (ans.data.b_HotelBooking == "HotelBooking") {
                $BusinessTravelDetailModel.find("#b_HotelBooking").first().iCheck('check');
                $("#HotelBookingDetails").show();
            } else {
                $BusinessTravelDetailModel.find("#b_HotelBooking").first().iCheck('uncheck');
                $("#HotelBookingDetails").hide();
            }
            if (ans.data.b_Didi == "Didi") {
                $BusinessTravelDetailModel.find("#b_Didi").first().iCheck('check');
                $("#DidiItemsDetail").show();
            }
            else
            {
                $BusinessTravelDetailModel.find("#b_Didi").first().iCheck('uncheck');
                $("#DidiItemsDetail").hide();
            }
            if (ans.data.b_Others == "Others") {
                $BusinessTravelDetailModel.find("#b_Others").first().iCheck('check');
                detailParam.b_OtherContent.show();
            } else {
                $BusinessTravelDetailModel.find("#b_Others").first().iCheck('uncheck');
                detailParam.b_OtherContent.hide();
            }
          

            $BusinessTravelDetailModel.find("h4.modal-title").empty().append("详情 <label style='color:rgb(88,136,193)'>" + ans.data.b_DocumentNo + "</label>");
            app.util.clearError($BusinessTravelDetailModel);
            app.util.bindValue(detailParam, ans);

            $FlightBookingItemTrDetail.empty();
            if (ans.data != null && ans.data.FlightBookingItems != null && ans.data.FlightBookingItems.length > 0) {
                for (var i = 0; i < ans.data.FlightBookingItems.length; i++) {
                    var item = ans.data.FlightBookingItems[i];
                    var $tr = $(FlightBookingDetailhtml);
                    $tr.find(".b_FirstName").html(item.b_FirstName);
                    $tr.find(".b_LastName").html(item.b_LastName);
                    $tr.find(".b_IDType").html(item.b_IDType == "ShenFen"?"身份证":"护照");
                    $tr.find(".b_IDCardNo").html(item.b_IDCardNo);
                    $tr.find(".b_Nationality").html(item.b_Nationality);
                    $tr.find(".b_PassportNumber").html(item.b_PassportNumber);
                    $tr.find(".b_Dateofexpiration").html(item.b_Dateofexpiration);
                    $tr.find(".b_Dateofbirth").html(item.b_Dateofbirth);
                    $tr.find(".b_Address").html(item.b_Address);
                    $tr.find(".b_Gooff").html(item.b_Gooff);
                    $tr.find(".b_Goplace").html(item.b_Goplace);
                    $tr.find(".b_Flightnumber").html(item.b_Flightnumber);
                    $FlightBookingItemTrDetail.append($tr);
                }
                $("#FlightBookingInfoDetails").show();
            } else {
                $("#FlightBookingInfoDetails").hide();
            }

            $HotelBookingTrDetail.empty();
            if (ans.data != null && ans.data.HotelBookingItems != null && ans.data.HotelBookingItems.length > 0) {
                for (var i = 0; i < ans.data.HotelBookingItems.length; i++) {
                    var item = ans.data.HotelBookingItems[i];
                    var $tr = $(HotelBookingDetailhtml);
                    $tr.find(".b_Checkindate").html(item.b_Checkindate);
                    $tr.find(".b_Leavedate").html(item.b_Leavedate);
                    $tr.find(".b_Specificaddress").html(item.b_Specificaddress);
                    $HotelBookingTrDetail.append($tr);
                }
                $("#HotelBookingDetails").show();
            } else {
                $("#HotelBookingDetails").hide();
            }

            //附件信息
            $("#fileListDetail").empty();
            if (ans.data != null && ans.data.Files != null && ans.data.Files.length > 0) {
                for (var i = 0; i < ans.data.Files.length; i++) {
                    var item = ans.data.Files[i];
                    $("#fileListDetail").append("<div><label><a href='#' id='" + item.id + "' onclick='DownloadAttachment(this)'>" + item.fileName + "</a></label></div>");
                }
            }

            GetBookingStaffingDetail(detailParam.b_Employee.html());
            $BusinessTravelDetailModel.modal("show");
        }
    });
}


//下载附件
function DownloadAttachment(obj) {
    location.href = 'FileManage/DownloadAttachment' + "?id=" + $(obj).attr("id");
}

//获取行政代订
function GetBookingStaffingDetail(userName)
{
    $.ajax({
        url: '/BusinessTravel/GetBookingStaffingByUserName',
        dataType: "json",
        data: { "userName": userName },
        success: function (ans) {
            if (ans.error.length > 0) {
                return false;
            }
            if(ans.data!=null)
            {
                $("label.BookingStaffing").show();
            }
            else
            {
                $("label.BookingStaffing").hide();
            }

        }
    });
}