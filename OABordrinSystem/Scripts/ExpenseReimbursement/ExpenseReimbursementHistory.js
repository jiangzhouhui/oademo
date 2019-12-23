﻿var $ExpenseReimbursementHistoryModel = $("#ExpenseReimbursementHistoryModel");
var $ExpenseReimbursementHistory = $("#ExpenseReimbursementHistory");
var ExpenseReimbursementHistory;
ExpenseReimbursementHistoryParam = app.util.getEditParam($ExpenseReimbursementHistoryModel);

$(document).ready(function () {
    ExpenseReimbursementHistory = $ExpenseReimbursementHistory.dataTable({
        "bServerSide": true,
        "bProcessing": false,
        "bFilter": false,
        "bAutoWidth": true,
        "aaSorting": [2, "desc"],
        "sAjaxSource": '/ExpenseReimbursement/GetExpenseReimbursementHistoryList',
        "fnServerData": RetrieveExpenseReimbursementHistoryData,
        "aoColumns": [
                 { "data": "ItemStatus" },
                 { "data": "OperateStr" },
                 { "data": "Created_on" },
                 { "data": "CreateName" },
                 { "data": "Comments", "sWidth": 200 },

        ],
        "oLanguage": {
            "sLengthMenu": "_MENU_",
            "sInfo": "从 _START_ 到 _END_ /共 _TOTAL_ 条数据"
        },
        "aLengthMenu": [15, 25, 50, 100]
    });
});

function RetrieveExpenseReimbursementHistoryData(sSource, aoData, fnCallback) {
    if (aoData[0].value != 1) {
        $.ajax({
            url: '/ExpenseReimbursement/GetExpenseReimbursementHistoryList',//这个就是请求地址对应sAjaxSource
            data: GetExpenseReimbursementHistoryParam(aoData),
            async: false,
            dataType: 'json',
            success: function (result) {
                fnCallback(result);//把返回的数据传给这个方法就可以了,datatable会自动绑定数据的
            },
            error: function (msg) {
            }
        });
    }
}

function GetExpenseReimbursementHistoryParam(aoData) {
    //DataTable固定传入参数
    var sEcho = aoData[0].value;
    var iDisplayStart = aoData[3].value;
    var iDisplayLength = aoData[4].value;
    //排序数组中的索引值
    var index = aoData[aoData.length - 3].value;
    //根据索引获取排序名称
    var iSortTitle = aoData[5 + index * 2].value;
    if (iSortTitle == "Created_on") {
        iSortTitle = "Create_onStr";
    }
    //获取排序的方式
    var sSortType = aoData[aoData.length - 2].value;
    var Item_id = ExpenseReimbursementHistoryParam.Item_id.val();
    return { "sEcho": sEcho, "iDisplayStart": iDisplayStart, "iDisplayLength": iDisplayLength, "iSortTitle": iSortTitle, "sSortType": sSortType, "Item_id": Item_id };
}