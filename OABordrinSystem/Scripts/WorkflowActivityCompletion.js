var $WorkflowActivityModel = $("#WorkflowActivityModel");

WorkflowActivityParam = app.util.getEditParam($WorkflowActivityModel);

//加载传入的活动路线
function LoadActivityList(itemId,activityId, activityAssignmentId, list)
{
    app.util.bindValue(WorkflowActivityParam, "");
    WorkflowActivityParam.activityId.val(activityId);
    WorkflowActivityParam.activityAssignmentId.val(activityAssignmentId);
    WorkflowActivityParam.itemId.val(itemId);
    $("select[name='workflowPath']").empty();
    if (list.length > 0) {
        //$("select[name='workflowPath']").append("<option value='' selected></option>");
        for (var i = 0; i < list.length; i++) {
            var item = list[i];
            var strHtml = "<option value='" + item.ID + "'>" + item.NAME + "</option>";
            $("select[name='workflowPath']").append(strHtml);
        }
        //$("select[name='workflowPath']").append("<option value='0'>Delegate</option>");
    }
}


function GetWorkFlowActivityParam()
{
    var param = app.util.serializeParamArray(WorkflowActivityParam);
    param.pathId = param.workflowPath;
    var name = $("select[name='workflowPath']").find("option:selected").text();
    param.pathName = name;
    return param;

}












