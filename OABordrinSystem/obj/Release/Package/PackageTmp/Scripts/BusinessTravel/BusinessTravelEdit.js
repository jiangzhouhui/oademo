var $BusinessTravelEdit = $("#BusinessTravelEdit")
//机票代订
var $FlightBookingItem = $("#FlightBookingItem"), $FlightBookingItemTr = $("#FlightBookingItemTr"), FlightBookingItemHtml = $FlightBookingItemTr.html();
//酒店代订
var $HotelBookingItem = $("#HotelBookingItem"), $HotelBookingItemTr = $("#HotelBookingItemTr"), HotelBookingItemHtml = $HotelBookingItemTr.html();

BusinessTravelEditParam = app.util.getEditParam($BusinessTravelEdit);

$(document).ready(function () {
    BusinessTravelEditParam.b_TravelBudget.digital();
    BusinessTravelEditParam.b_TrafficExpense.digital();
    BusinessTravelEditParam.b_HotelExpense.digital();
    BusinessTravelEditParam.b_FixedSubsidy.digital();
    BusinessTravelEditParam.b_OtherExpenses.digital();
    GetAutocompleteChoiceUserData();

    LoadAutocompleteData();
    SetAutocomplete();

    //加载公司代码列表
    GetStructureCompanyCode();

    //加载归属地
    GetRegionList();
})

function GetSaveParam(operation) {
    //获取基础信息
    var param = app.util.serializeParamArray(BusinessTravelEditParam);
    param.operation = operation;

    //获取单选按钮的值

    var b_TripType = "";
    $BusinessTravelEdit.find("input[name='b_TripType']").each(function () {
        var $this = $(this);
        if ($this.closest("div").hasClass("checked")) {
            b_TripType = $this.val();
        }
    });
    param.b_TripType = b_TripType;

    //获取单选按钮的值
    var b_Type = "";
    $BusinessTravelEdit.find("input[name='b_Type']").each(function () {
        var $this = $(this);
        if ($this.closest("div").hasClass("checked")) {
            b_Type = $this.val();
        }
    });
    param.b_Type = b_Type;

    //获取单选按钮的值
    var b_IDType = "";
    $BusinessTravelEdit.find("input[name='b_IDType']").each(function () {
        var $this = $(this);
        if ($this.closest("div").hasClass("checked")) {
            b_IDType = $this.val();
        }
    });
    param.b_IDType = b_IDType;

    if (BusinessTravelEditParam.b_FlightIsssue.closest("div").hasClass("checked")) {
        param.b_FlightIsssue = "FlightIsssue";
    }

    if (BusinessTravelEditParam.b_FlightBooking.closest("div").hasClass("checked")) {
        param.b_FlightBooking = "FlightBooking";
    }

    if (BusinessTravelEditParam.b_HotelBooking.closest("div").hasClass("checked")) {
        param.b_HotelBooking = "HotelBooking";
    }

    if (BusinessTravelEditParam.b_Didi.closest("div").hasClass("checked")) {
        param.b_Didi = "Didi";
    }

    if (BusinessTravelEditParam.b_Others.closest("div").hasClass("checked")) {
        param.b_Others = "Others";
    }

    //获取机票代订
    var items = [];
    $FlightBookingItemTr.find("tr").each(function () {
        var $this = $(this);
        var Id = $.trim($this.find("input.Id").val());
        var b_FirstName = $.trim($this.find("input.b_FirstName").val());
        var b_LastName = $.trim($this.find("input.b_LastName").val());
        var b_IDType = $.trim($this.find("select.b_IDType").val());
        var b_IDCardNo = $.trim($this.find("input.b_IDCardNo").val());
        var b_Nationality = $.trim($this.find("input.b_Nationality").val());
        var b_PassportNumber = $.trim($this.find("input.b_PassportNumber").val());
        var b_Dateofexpiration = $.trim($this.find("input.b_Dateofexpiration").val());
        var b_Dateofbirth = $.trim($this.find("input.b_Dateofbirth").val());
        var b_Address = $.trim($this.find("input.b_Address").val());
        var b_Gooff = $.trim($this.find("input.b_Gooff").val());
        var b_Goplace = $.trim($this.find("input.b_Goplace").val());
        var b_Flightnumber = $.trim($this.find("input.b_Flightnumber").val());
        if (b_FirstName != "" && b_LastName != "" && b_Address != "" && b_Gooff != "" && b_Goplace != "" && b_Flightnumber != "") {
            items.push({ "Id": Id, "b_FirstName": b_FirstName, "b_LastName": b_LastName, "b_IDType": b_IDType, "b_IDCardNo": b_IDCardNo, "b_Nationality": b_Nationality, "b_PassportNumber": b_PassportNumber, "b_Dateofexpiration": b_Dateofexpiration, "b_Dateofbirth": b_Dateofbirth, "b_Address": b_Address, "b_Gooff": b_Gooff, "b_Goplace": b_Goplace, "b_Flightnumber": b_Flightnumber });
        }
    });
    param.FlightBookingItems = items;


    var HotelItems = [];
    $HotelBookingItemTr.find("tr").each(function () {
        var $this = $(this);
        var Id = $.trim($this.find("input.Id").val());
        var b_Checkindate = $.trim($this.find("input.b_Checkindate").val());
        var b_Leavedate = $.trim($this.find("input.b_Leavedate").val());
        var b_Specificaddress = $.trim($this.find("input.b_Specificaddress").val());
        if (b_Checkindate != "" && b_Leavedate != "" && b_Specificaddress != "") {
            HotelItems.push({ "Id": Id, "b_Checkindate": b_Checkindate, "b_Leavedate": b_Leavedate, "b_Specificaddress": b_Specificaddress });
        }
    });
    param.HotelBookingItems = HotelItems;

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
function GetAuditBusinessTravel() {
    //获取基础信息
    var param = app.util.serializeParamArray(BusinessTravelEditParam);

    //获取单选按钮的值
    var b_TripType = "";
    $BusinessTravelEdit.find("input[name='b_TripType']").each(function () {
        var $this = $(this);
        if ($this.closest("div").hasClass("checked")) {
            b_TripType = $this.val();
        }
    });
    param.b_TripType = b_TripType;

    //获取单选按钮值
    var b_Type = "";
    $BusinessTravelEdit.find("input[name='b_Type']").each(function () {
        var $this = $(this);
        if ($this.closest("div").hasClass("checked")) {
            b_Type = $this.val();
        }
    });
    param.b_Type = b_Type;

    if (BusinessTravelEditParam.b_FlightIsssue.closest("div").hasClass("checked")) {
        param.b_FlightIsssue = "FlightIsssue";
    }

    if (BusinessTravelEditParam.b_FlightBooking.closest("div").hasClass("checked")) {
        param.b_FlightBooking = "FlightBooking";
    }

    if (BusinessTravelEditParam.b_HotelBooking.closest("div").hasClass("checked")) {
        param.b_HotelBooking = "HotelBooking";
    }

    if (BusinessTravelEditParam.b_Didi.closest("div").hasClass("checked")) {
        param.b_Didi = "Didi";
    }

    if (BusinessTravelEditParam.b_Others.closest("div").hasClass("checked")) {
        param.b_Others = "Others";
    }

    if (param.status == "Administrative approval") {
        //获取机票代订
        var items = [];
        $FlightBookingItemTr.find("tr").each(function () {
            var $this = $(this);
            var Id = $.trim($this.find("input.Id").val());
            var b_FirstName = $.trim($this.find("input.b_FirstName").val());
            var b_LastName = $.trim($this.find("input.b_LastName").val());
            var b_IDType = $.trim($this.find("select.b_IDType").val());
            var b_IDCardNo = $.trim($this.find("input.b_IDCardNo").val());
            var b_Nationality = $.trim($this.find("input.b_Nationality").val());
            var b_PassportNumber = $.trim($this.find("input.b_PassportNumber").val());
            var b_Dateofexpiration = $.trim($this.find("input.b_Dateofexpiration").val());
            var b_Dateofbirth = $.trim($this.find("input.b_Dateofbirth").val());
            var b_Address = $.trim($this.find("input.b_Address").val());
            var b_Gooff = $.trim($this.find("input.b_Gooff").val());
            var b_Goplace = $.trim($this.find("input.b_Goplace").val());
            var b_Flightnumber = $.trim($this.find("input.b_Flightnumber").val());
            if (b_FirstName != "" && b_LastName != "" && b_Address != "" && b_Gooff != "" && b_Goplace != "" && b_Flightnumber != "") {
                items.push({ "Id": Id, "b_FirstName": b_FirstName, "b_LastName": b_LastName, "b_IDType": b_IDType, "b_IDCardNo": b_IDCardNo, "b_Nationality": b_Nationality, "b_PassportNumber": b_PassportNumber, "b_Dateofexpiration": b_Dateofexpiration, "b_Dateofbirth": b_Dateofbirth, "b_Address": b_Address, "b_Gooff": b_Gooff, "b_Goplace": b_Goplace, "b_Flightnumber": b_Flightnumber });
            }
        });
        param.FlightBookingItems = items;

        var HotelItems = [];
        $HotelBookingItemTr.find("tr").each(function () {
            var $this = $(this);
            var Id = $.trim($this.find("input.Id").val());
            var b_Checkindate = $.trim($this.find("input.b_Checkindate").val());
            var b_Leavedate = $.trim($this.find("input.b_Leavedate").val());
            var b_Specificaddress = $.trim($this.find("input.b_Specificaddress").val());
            if (b_Checkindate != "" && b_Leavedate != "" && b_Specificaddress != "") {
                HotelItems.push({ "Id": Id, "b_Checkindate": b_Checkindate, "b_Leavedate": b_Leavedate, "b_Specificaddress": b_Specificaddress });
            }
        });
        param.HotelBookingItems = HotelItems;
    }

    return param;
}

//小窗口显示样式
function displayModelSetting() {
    var status = BusinessTravelEditParam.status.val();
    $BusinessTravelEdit.find("div.modal-footer").find("button").each(function () {
        $(this).hide();
    });
    $("#btnClose").show();
    if (status == "" || status == "Start") {
        app.util.ModelToEdit($BusinessTravelEdit);
        if (BusinessTravelEditParam.b_IsHangUp.val() == "true") {
            app.util.ModelToDetails(BusinessTravelEditParam.b_Location.closest("div"));
            app.util.ModelToDetails(BusinessTravelEditParam.b_CompanyCode.closest("div"));
            app.util.ModelToDetails($BusinessTravelEdit.find("input[name='b_TripType']").closest("div"));
            app.util.ModelToDetails($BusinessTravelEdit.find("input[name='b_Type']").closest("div"));
        }
        //显示操作按钮
        $FlightBookingItemTr.find(".AddFlightBookingItem,.deleteFlightBookingItem").show();
        $HotelBookingItemTr.find(".AddHotelBookingItem,.deleteHotelBookingItem").show();
        $("#SaveBusinessTravel").show();
        $("#SubmitBusinessTravel").show();

    }
    else if (status == "Dept.Manager" || status == "Dept.Director" || status == "Division VP" || status == "Project Manager" || status == "Project Director" || status == "Project VP" || status == "CEO") {
        app.util.ModelToDetails($BusinessTravelEdit);

        //显示操作按钮
        $FlightBookingItemTr.find(".AddFlightBookingItem,.deleteFlightBookingItem").hide();
        $HotelBookingItemTr.find(".AddHotelBookingItem,.deleteHotelBookingItem").hide();
        $("#AuditApprove").show();
        $("#SendRefuse").show();
    }
    else if (status == "Administrative approval") {
        app.util.ModelToDetails($BusinessTravelEdit);
        //显示操作按钮
        $FlightBookingItemTr.find(".AddFlightBookingItem,.deleteFlightBookingItem").show();
        $HotelBookingItemTr.find(".AddHotelBookingItem,.deleteHotelBookingItem").show();
        app.util.ModelToEdit($("#FlightBookingInfo"));
        app.util.ModelToEdit($("#HotelBookingEdit"));
        $("#AuditApprove").show();
        $("#SendRefuse").show();
        $("#SendHangUp").show();
    }
    else {
        app.util.ModelToDetails($BusinessTravelEdit);
        //显示操作按钮
        $FlightBookingItemTr.find(".AddFlightBookingItem,.deleteFlightBookingItem").hide();
        $HotelBookingItemTr.find(".AddHotelBookingItem,.deleteHotelBookingItem").hide();
        $("#AuditApprove").show();
        $("#SendRefuse").show();
        $("#SendHangUp").show();
    }
    GetBookingStaffingByUserName(BusinessTravelEditParam.b_Employee.val());
    app.util.ModelToEdit(BusinessTravelEditParam.b_Remark.closest("div"));
}

//获取所有员工姓名
BusinessTravelEditParam.b_Employee.autocomplete({
    source: function (request, response) {
        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
        response($.grep(choiceUserList, function (value) {
            return matcher.test(value);
        }));
    }
});

BusinessTravelEditParam.b_SeniorManager.autocomplete({
    source: function (request, response) {
        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
        response($.grep(choiceUserList, function (value) {
            return matcher.test(value);
        }));
    }
});

BusinessTravelEditParam.b_Director.autocomplete({
    source: function (request, response) {
        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
        response($.grep(choiceUserList, function (value) {
            return matcher.test(value);
        }));
    }
});

BusinessTravelEditParam.b_VP.autocomplete({
    source: function (request, response) {
        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
        response($.grep(choiceUserList, function (value) {
            return matcher.test(value);
        }));
    }
});


//员工列表AutoComplete
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

//获取所有项目
BusinessTravelEditParam.b_ProjectName.autocomplete({
    source: function (request, response) {
        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
        response($.grep(projectList, function (value) {
            return matcher.test(value);
        }));
    }
})

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


var availableTags = [];
function SetAutocomplete() {
    BusinessTravelEditParam.b_ProjectName.on("keydown", function (event) {
        if (event.keyCode === $.ui.keyCode.TAB &&
            $(this).autocomplete("instance").menu.active) {
            event.preventDefault();
        }
    }).autocomplete({
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
    $.ajax({
        url: '/AutoComplete/GetProjectList',
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


//机票代订委托事件
$FlightBookingItem.delegate("a", "click", function () {
    var $this = $(this);
    if ($this.hasClass("AddFlightBookingItem")) {
        $this.closest("tr").after(AddNewFlightBookingTr());
        return false;
    }
    if ($this.hasClass("deleteFlightBookingItem")) {
        $this.closest("tr").remove();
        if ($FlightBookingItemTr.find("tr").length <= 0) {
            $FlightBookingItemTr.prepend(AddNewFlightBookingTr());
        }
        return false;
    }
});


function AddNewFlightBookingTr(userName) {
    var html = $(FlightBookingItemHtml);
    html.find(".b_Dateofexpiration").datepicker({ autoclose: true, format: 'yyyy-mm-dd' });
    html.find(".b_Dateofbirth").datepicker({ autoclose: true, format: 'yyyy-mm-dd' });
    html.find(".b_Gooff").datepicker({ autoclose: true, format: 'yyyy-mm-dd' });
    return html;
}

//酒店代订委托事件
$HotelBookingItem.delegate("a", "click", function () {
    var $this = $(this);
    if ($this.hasClass("AddHotelBookingItem")) {
        $this.closest("tr").after(AddNewBookingItemTr());
        return false;
    }
    if ($this.hasClass("deleteHotelBookingItem")) {
        $this.closest("tr").remove();
        if ($HotelBookingItemTr.find("tr").length <= 0) {
            $HotelBookingItemTr.prepend(AddNewBookingItemTr());
        }
        return false;
    }
})


function AddNewBookingItemTr() {
    var html = $(HotelBookingItemHtml);
    html.find('.b_Checkindate').datepicker({
        format: "yyyy-mm-dd",
        autoclose: true,
        todayHighlight: true,
        // endDate: new Date()
    }).on('changeDate', function (e) {
        var startTime = e.date;
        html.find('.b_Leavedate').datepicker('setStartDate', startTime);
    });

    html.find('.b_Leavedate').datepicker({
        format: "yyyy-mm-dd",
        autoclose: true,
        todayHighlight: true,
        //startDate: new Date()
    }).on('changeDate', function (e) {
        var endTime = e.date;
        html.find('.b_Checkindate').datepicker('setEndDate', endTime);
    });
    return html;
}

BusinessTravelEditParam.b_TravelDate.datepicker({
    format: "yyyy-mm-dd",
    autoclose: true,
    todayHighlight: true,
}).on('changeDate', function (e) {
    var startTime = e.date;
    BusinessTravelEditParam.b_EstimatedReturnDate.datepicker('setStartDate', startTime);
    var days = GetDateDiff(BusinessTravelEditParam.b_TravelDate.val(), BusinessTravelEditParam.b_EstimatedReturnDate.val());
    BusinessTravelEditParam.b_DidiMoney.val((days + 1) * 100);
});

BusinessTravelEditParam.b_EstimatedReturnDate.datepicker({
    format: "yyyy-mm-dd",
    autoclose: true,
    todayHighlight: true,
    startDate: new Date()
}).on('changeDate', function (e) {
    var endTime = e.date;
    BusinessTravelEditParam.b_TravelDate.datepicker('setEndDate', endTime);
    var days = GetDateDiff(BusinessTravelEditParam.b_TravelDate.val(), BusinessTravelEditParam.b_EstimatedReturnDate.val());
    BusinessTravelEditParam.b_DidiMoney.val((days + 1) * 100);
});


//上传附件
$("#upload").click(function () {
    var formData = new FormData();
    formData.append("myfile", document.getElementById("Btfile").files[0]);
    formData.append("id", BusinessTravelEditParam.Id.val());
    formData.append("status", BusinessTravelEditParam.status.val());
    formData.append("relationTableName", "R_File");
    $.ajax({
        url: '/FileManage/UploadFile',
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (ans) {
            if (ans.error.length > 0) {
                app.util.onError(ans.error, $BusinessTravelEdit);
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
            data: { "id": $(obj).attr("Id"), "relationId": $(obj).attr("relationId"), "relationTableName": "R_File" },
            dataType: "json",
            success: function (ans) {
                if (ans.error.length > 0) {
                    app.util.onError(ans.error, $BusinessTravelEdit);
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
            BusinessTravelEditParam.b_Location.empty();
            BusinessTravelEditParam.b_Location.append("<option value=''>选择地区</option>")
            if (ans.data != null && ans.data.length > 0) {
                for (var i = 0; i < ans.data.length; i++) {
                    var value = ans.data[i];
                    BusinessTravelEditParam.b_Location.append("<option value='" + value + "'>" + value + "</option>");
                }
            }
        }
    });
}

//申请人信息联动
$BusinessTravelEdit.delegate("input[name='b_Employee']", "blur", function () {
    LoadEmployeeInfo($(this).val());
});

//获取申请人信息
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
                BusinessTravelEditParam.b_StaffNo.val(item.b_JobNumber);
                BusinessTravelEditParam.b_Dept.val(item.b_Department);
                BusinessTravelEditParam.b_SeniorManager.val(item.b_SeniorManager);
                BusinessTravelEditParam.b_Director.val(item.b_Director);
                BusinessTravelEditParam.b_VP.val(item.b_VP);
                if (item.b_Department != null && item.b_Department != "") {
                    BusinessTravelEditParam.b_Dept.val(item.b_Department);
                }
                else if (item.b_Centre != null && item.b_Dept != "") {
                    BusinessTravelEditParam.b_Dept.val(item.b_Centre);
                }
                BusinessTravelEditParam.b_CostCenter.val(item.b_CostCenter);
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
            BusinessTravelEditParam.b_CompanyCode.empty();
            BusinessTravelEditParam.b_CompanyCode.append("<option value=''>选择公司代码</option>")
            if (ans.data != null && ans.data.length > 0) {
                for (var i = 0; i < ans.data.length; i++) {
                    var value = ans.data[i];
                    BusinessTravelEditParam.b_CompanyCode.append("<option value='" + value + "'>" + value + "</option>");
                }
            }
        }
    });
}


//判断机票代订
$("input[name='b_FlightBooking']").on('ifChanged', function (event) {
    var $this = $(this);
    if ($this.is(':checked')) {
        $("#FlightBookingInfo").show();
    }
    else {
        $("#FlightBookingInfo").hide();
    }
});

//判断酒店代订
$("input[name='b_HotelBooking']").on('ifChanged', function (event) {
    var $this = $(this);
    if ($this.is(':checked')) {
        $("#HotelBookingEdit").show();
    }
    else {
        $("#HotelBookingEdit").hide();
    }
});

//判断滴滴
$("input[name='b_Didi']").on('ifChanged', function (event) {
    var $this = $(this);
    if ($this.is(':checked')) {
        $("#DidiItemsEdit").show();
    }
    else {
        $("#DidiItemsEdit").hide();
    }
});


//判断其他
$("input[name='b_Others']").on('ifChanged', function (event) {
    var $this = $(this);
    if ($this.is(':checked')) {
        BusinessTravelEditParam.b_OtherContent.show();
    }
    else {
        BusinessTravelEditParam.b_OtherContent.hide();
    }
})

//判断项目和非项目
$("input[name='b_Type']").on('ifChecked', function (event) {
    var $this = $(this);
    if ($this.val() == "Project") {
        $("#nb_ProjectName").show();
        $("#departmentLeaderInfo").hide();
    }
    else {
        $("#nb_ProjectName").hide();
        BusinessTravelEditParam.b_ProjectName.val("");
        $("#departmentLeaderInfo").show();
    }
});

//判断证件类型
$("input[name='b_IDType']").on('ifChecked', function (event) {
    var $this = $(this);
    if ($this.val() == "ShenFen") {
        BusinessTravelEditParam.b_IDType.each(function () {
            $(this).removeAttr("disabled");
        })
        $("#radioInfo").hide();
    } else {
        BusinessTravelEditParam.b_IDType.each(function () {
            $(this).removeAttr("disabled");
            $(this).val("");
        })
        $("#radioInfo").show();
    }
});


//判断出差类型
$("input[name='b_TripType']").on('ifChecked', function (event) {
    var $this = $(this);
    if ($this.val() == "Domestic") {

    }
    else {

    }
});

$BusinessTravelEdit.delegate("input[name='b_TrafficExpense'],input[name='b_HotelExpense'],input[name='b_FixedSubsidy'],input[name='b_OtherExpenses']", "blur", function () {
    CalculationTotalAmount();
});


//计算总金额
function CalculationTotalAmount() {
    var b_TrafficExpense = $.trim(BusinessTravelEditParam.b_TrafficExpense.val());
    var b_HotelExpense = $.trim(BusinessTravelEditParam.b_HotelExpense.val());
    var b_FixedSubsidy = $.trim(BusinessTravelEditParam.b_FixedSubsidy.val());
    var b_OtherExpenses = $.trim(BusinessTravelEditParam.b_OtherExpenses.val());
    b_TrafficExpense = b_TrafficExpense == "" ? 0 : b_TrafficExpense;
    b_HotelExpense = b_HotelExpense == "" ? 0 : b_HotelExpense;
    b_FixedSubsidy = b_FixedSubsidy == "" ? 0 : b_FixedSubsidy;
    b_OtherExpenses = b_OtherExpenses == "" ? 0 : b_OtherExpenses;

    if (b_TrafficExpense != "" && b_HotelExpense != "" && b_FixedSubsidy != "" && b_OtherExpenses != "") {
        b_TrafficExpense = b_TrafficExpense == "NaN" ? 0 : b_TrafficExpense;
        b_HotelExpense = b_HotelExpense == "NaN" ? 0 : b_HotelExpense;
        b_FixedSubsidy = b_FixedSubsidy == "NaN" ? 0 : b_FixedSubsidy;
        b_OtherExpenses = b_OtherExpenses == "NaN" ? 0 : b_OtherExpenses;
        var b_TravelBudget = parseFloat(b_TrafficExpense) + parseFloat(b_HotelExpense) + parseFloat(b_FixedSubsidy) + parseFloat(b_OtherExpenses);
        BusinessTravelEditParam.b_TravelBudget.val(b_TravelBudget);
    }
}

//获取行政代订
function GetBookingStaffingByUserName(userName)
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