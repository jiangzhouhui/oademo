//function SetCurrentStatus(itemStatus)
//{
//    $("#workflow").find("text").each(function () {
//        var textValue = $(this).text();
//        if (textValue == itemStatus) {
//            var aimRect = $(this).closest("g").find("rect").first();
//            aimRect.attr("fill", "rgb(238, 238, 0)");
//            aimRect.attr("fill-opacity", "1");
//            aimRect.attr("fill-rule", "evenodd");
//        }
//    });
//}

function GetWorkFlowByDataId(dataId) {

    $.ajax({
        url: '/WorkFlow/GetWorkFlowByDataId',
        data: { "dataId": dataId },
        dataType: "json",
        success: function (ans) {
            if (ans.error.length > 0) {
                return;
            }
            DrawSvg(ans);
        }
    });
}


function GetWorkFlowMapByKeyedName(KeyedName)
{
    $.ajax({
        url: '/WorkFlow/GetWorkFlowMapByKeyedName',
        data: { "keyedName": KeyedName },
        dataType: "json",
        success: function (ans) {
            if (ans.error.length > 0) {
                return;
            }
            DrawSvg(ans);
        }
    });
}

function DrawSvg(ans)
{
    var strHtml = "<svg xmlns='http://www.w3.org/2000/svg' overflow='hidden' style='-ms-touch-action: none;' width='1330' height='400'><defs />";
    if (ans.data != null && ans.data.length > 0) {
        //画主要的节点
        for (var index = 0; index < ans.data.length; index++) {
            var dataItem = ans.data[index];
            strHtml = strHtml + "<g>";
            //创建Rect对象
            var rectOne = "<rect fill='none' fill-opacity='0' stroke='none' stroke-linecap='butt' stroke-linejoin='miter' stroke-miterlimit='4' stroke-opacity='0' stroke-width='1' x='" + dataItem.X + "' y='" + dataItem.Y + "' width='20' height='20' rx='0' ry='0' />";
            strHtml = strHtml + rectOne;
            var rectTwo = "<rect fill='none' fill-opacity='0' stroke='none' stroke-linecap='butt' stroke-linejoin='miter' stroke-miterlimit='4' stroke-opacity='0' stroke-width='1' x='" + parseInt(dataItem.X - 3) + "' y='" + parseInt(dataItem.Y - 3) + "' width='3' height='3' rx='0' ry='0' />";
            strHtml = strHtml + rectTwo;
            var rectThree = "<rect fill='none' fill-opacity='0' stroke='none' stroke-linecap='butt' stroke-linejoin='miter' stroke-miterlimit='4' stroke-opacity='0' stroke-width='1' x='" + parseInt(dataItem.X + 20) + "' y='" + parseInt(dataItem.Y - 3) + "' width='3' height='3' rx='0' ry='0' />";
            strHtml = strHtml + rectThree;
            var rectFour = "<rect fill='none' fill-opacity='0' stroke='none' stroke-linecap='butt' stroke-linejoin='miter' stroke-miterlimit='4' stroke-opacity='0' stroke-width='1' x='" + parseInt(dataItem.X + 20) + "' y='" + parseInt(dataItem.Y + 20) + "' width='3' height='3' rx='0' ry='0' />";
            strHtml = strHtml + rectFour;
            var rectFive = "<rect fill='none' fill-opacity='0' stroke='none' stroke-linecap='butt' stroke-linejoin='miter' stroke-miterlimit='4' stroke-opacity='0' stroke-width='1' x='" + parseInt(dataItem.X - 3) + "' y='" + parseInt(dataItem.Y + 20) + "' width='3' height='3' rx='0' ry='0' />";
            strHtml = strHtml + rectFive;

            //创建Text对象
            var textOne = "<text font-family='Arial' font-size='9pt' font-style='normal' font-variant='normal' font-weight='normal' text-decoration='none' fill='rgb(0, 0, 255)' fill-opacity='1' fill-rule='evenodd' kerning='auto' stroke='none' stroke-linecap='butt' stroke-linejoin='miter' stroke-miterlimit='4' stroke-opacity='0' stroke-width='1' text-anchor='start' x='" + parseInt(dataItem.X - 2) + "' y='" + parseInt(dataItem.Y + 34) + "' rotate='0' text-rendering='optimizeLegibility'>" + dataItem.Keyed_Name + "</text>";
            strHtml = strHtml + textOne;
            var textTwo = "<text font-family='Arial' font-size='9pt' font-style='normal' font-variant='normal' font-weight='normal' text-decoration='none' fill='rgb(0, 0, 255)' fill-opacity='1' fill-rule='evenodd' kerning='auto' stroke='none' stroke-linecap='butt' stroke-linejoin='miter' stroke-miterlimit='4' stroke-opacity='0' stroke-width='1' text-anchor='start' x='" + parseInt(dataItem.X) + "' y='" + parseInt(dataItem.Y - 4) + "' rotate='0' text-rendering='optimizeLegibility'></text>";
            strHtml = strHtml + textTwo;
            var textThree = "<text font-family='Arial' font-size='9pt' font-style='normal' font-variant='normal' font-weight='normal' text-decoration='none' fill='rgb(0, 0, 255)' fill-opacity='1' fill-rule='evenodd' kerning='auto' stroke='none' stroke-linecap='butt' stroke-linejoin='miter' stroke-miterlimit='4' stroke-opacity='0' stroke-width='1' text-anchor='start' x='" + parseInt(dataItem.X) + "' y='" + parseInt(dataItem.Y + 50) + "' rotate='0' text-rendering='optimizeLegibility'></text>";
            strHtml = strHtml + textThree;

            var imageHtml = "";
            //创建image对象
            if (dataItem.Keyed_Name == "End") {
                imageHtml = "<image fill-opacity='0' stroke='none' stroke-linecap='butt' stroke-linejoin='miter' stroke-miterlimit='4' stroke-opacity='0' stroke-width='1' transform='matrix(0.3125 0 0 0.3125 0 0)' preserveAspectRatio='none meet' x='" + parseFloat(dataItem.X * 3.2) + "' y='" + parseFloat(dataItem.Y * 3.2) + "' width='64' height='64' xmlns:xlink='http://www.w3.org/1999/xlink' xlink:href='http://localhost/InnovatorServer/Client/X-salt=std_11.0.0.6549-X/images/Delete.svg?recipient=workflow' />";
            }
            else if (dataItem.Keyed_Name == "Start") {
                imageHtml = "<image fill-opacity='0' stroke='none' stroke-linecap='butt' stroke-linejoin='miter' stroke-miterlimit='4' stroke-opacity='0' stroke-width='1' transform='matrix(0.3125 0 0 0.3125 0 0)' preserveAspectRatio='none meet' x='" + parseFloat(dataItem.X * 3.2) + "' y='" + parseFloat(dataItem.Y * 3.2) + "' width='64' height='64' xmlns:xlink='http://www.w3.org/1999/xlink' xlink:href='http://localhost/InnovatorServer/Client/X-salt=std_11.0.0.6549-X/images/WorkflowStart.svg?recipient=workflow' />";
            }
            else {
                imageHtml = "<image fill-opacity='0' stroke='none' stroke-linecap='butt' stroke-linejoin='miter' stroke-miterlimit='4' stroke-opacity='0' stroke-width='1' transform='matrix(0.3125 0 0 0.3125 0 0)' preserveAspectRatio='none meet' x='" + parseFloat(dataItem.X * 3.2) + "' y='" + parseFloat(dataItem.Y * 3.2) + "' width='64' height='64' xmlns:xlink='http://www.w3.org/1999/xlink' xlink:href='http://localhost/InnovatorServer/Client/X-salt=std_11.0.0.6549-X/images/WorkflowNode.svg?recipient=workflow' />";
            }
            strHtml = strHtml + imageHtml;
            strHtml = strHtml + "</g>";
        }
        var ncount = 0;

        //画流程图的线条
        for (var i = 0; i < ans.data.length; i++) {
            var dataItem = ans.data[i];
            if (dataItem.ProcessPaths != null) {
                for (var j = 0; j < dataItem.ProcessPaths.length; j++) {
                    var pathItem = dataItem.ProcessPaths[j];
                    strHtml = strHtml + "<g>";
                    if (pathItem.segments == "" || pathItem.segments==null) {
                        //var AX = parseFloat(dataItem.X + 24.1);
                        //var AY = parseFloat(dataItem.Y + 9.2);
                        //var BX = parseFloat(pathItem.RelatedX - 4.2);
                        //var BY = parseFloat(pathItem.RelatedY + 9.1);
                        var AX = parseFloat(dataItem.X + 12);
                        var AY = parseFloat(dataItem.Y + 9.2);
                        var BX = parseFloat(pathItem.RelatedX+12);
                        var BY = parseFloat(pathItem.RelatedY + 9.2);
                        var pointsXY = getArrowKeypoints(AX, AY, BX, BY);
                        var k = (BY - AY) * (BX - AX);
                        var hudu = Math.atan(k);
                        //获取正弦值
                        var sinValue = Math.sin(hudu);
                        //求亮点之间的距离
                        var value = Math.sqrt(Math.abs(dataItem.X - pathItem.RelatedX) * Math.abs(dataItem.X - pathItem.RelatedX) + Math.abs(dataItem.Y - pathItem.Y) * Math.abs(dataItem.Y - pathItem.Y));
                        //线条
                        var pathLine = "<path fill='none' fill-opacity='0' stroke='rgb(128, 128, 128)' stroke-dasharray='none' stroke-linecap='butt' stroke-linejoin='miter' stroke-miterlimit='4' stroke-opacity='1' stroke-width='1' d='M " + AX + "  " + AY + " L " + BX + " " + BY + "' dojoGfxStrokeStyle='solid' />";
                        //文本
                        var textObj = "<text font-family='Arial' font-size='9pt' font-style='normal' font-variant='normal' font-weight='normal' text-decoration='none' fill='rgb(0, 0, 0)' fill-opacity='1' fill-rule='evenodd' kerning='auto' stroke='none' stroke-linecap='butt' stroke-linejoin='miter' stroke-miterlimit='4' stroke-opacity='0' stroke-width='1' text-anchor='start' x='" + parseInt(((pathItem.RelatedX - dataItem.X) / 2) * 0.7 + dataItem.X) + "' y='" + parseInt(dataItem.Y + 22) + "' rotate='0' text-rendering='optimizeLegibility'>" + pathItem.Lable + "</text>";
                        //箭头
                        //var polyline = "<polyline fill='rgb(128, 128, 128)' fill-opacity='1' fill-rule='evenodd' stroke='rgb(128, 128, 128)' stroke-dasharray='none' stroke-linecap='butt' stroke-linejoin='miter' stroke-miterlimit='4' stroke-opacity='1' stroke-width='1' points='"+parseFloat(pathItem.RelatedX-5) +","+parseFloat(pathItem.RelatedY+9) +" "+parseFloat(pathItem.RelatedX-14) +","+ parseFloat(pathItem.RelatedY+9-3) +" "+parseFloat(pathItem.RelatedX-14) +","+parseFloat(pathItem.RelatedY+11) +" "+parseFloat(pathItem.RelatedX-5)+","+ parseFloat(pathItem.RelatedY+9)+"' dojoGfxStrokeStyle='solid' />";
                        var polyline = "<polyline fill='rgb(128, 128, 128)' fill-opacity='1' fill-rule='evenodd' stroke='rgb(128, 128, 128)' stroke-dasharray='none' stroke-linecap='butt' stroke-linejoin='miter' stroke-miterlimit='4' stroke-opacity='1' stroke-width='1' points='" + BX + "," + BY + " " + pointsXY[0] + "," + pointsXY[1] + " " + pointsXY[2] + "," + pointsXY[3] + " " + BX + "," + BY + "' dojoGfxStrokeStyle='solid' />";
                        strHtml = strHtml + pathLine + textObj + polyline;

                    }
                    else {
                        //if (ncount > 0) {
                        //    continue;
                        //}
                        //线条
                        var dataArray = pathItem.segments.split(',');
                        //var X1 = dataArray[0];
                        //var Y1 = dataArray[1];
                        //var X2 = parseFloat(pathItem.RelatedX + 20);
                        //var Y2 = parseFloat(pathItem.RelatedY + 20);
                        //var pointsXY= getArrowKeypoints(X1, Y1, X2, Y2);
                        //计算出箭头的亮点坐标
                        //var pointAX = X2 - ((X2 - X1) - (Y2 - Y1)) / Math.sqrt(Math.pow(X2 - X1, 2) + Math.pow(Y2 - Y1, 2)) * 4;
                        //var pointAY = Y2 - ((X2 - X1) + (Y2 - Y1)) / Math.sqrt(Math.pow(X2 - X1, 2) + Math.pow(Y2 - Y1, 2)) * 4;
                        //var pointBX = X2 - ((X2 - X1) + (Y2 - Y1)) / Math.sqrt(Math.pow(X2 - X1, 2) + Math.pow(Y2 - Y1, 2)) * 4;
                        //var pointBY = Y2 + ((X2 - X1) - (Y2 - Y1)) / Math.sqrt(Math.pow(X2 - X1, 2) + Math.pow(Y2 - Y1, 2)) * 4;

                        var startX = dataArray[0];
                        var startY = dataArray[1];
                        var endX = parseFloat(pathItem.RelatedX + 20);
                        var endY = parseFloat(pathItem.RelatedY + 20);
                        var pointsXY = getArrowKeypoints(startX, startY, endX, endY);
                        //线条
                        var pathLine = "<path fill='none' fill-opacity='0' stroke='rgb(128, 128, 128)' stroke-dasharray='none' stroke-linecap='butt' stroke-linejoin='miter' stroke-miterlimit='4' stroke-opacity='1' stroke-width='1' d='M " + parseFloat(dataItem.X - 1) + " " + parseFloat(dataItem.Y + 20 - 1) + " L " + dataArray[0] + " " + dataArray[1] + " L " + parseFloat(pathItem.RelatedX + 20) + " " + parseFloat(pathItem.RelatedY + 20) + "' dojoGfxStrokeStyle='solid' />";
                        //箭头
                        //var polyline= "<polyline fill='rgb(128, 128, 128)' fill-opacity='1' fill-rule='evenodd' stroke='rgb(128, 128, 128)' stroke-dasharray='none' stroke-linecap='butt' stroke-linejoin='miter' stroke-miterlimit='4' stroke-opacity='1' stroke-width='1' points='"+parseFloat(pathItem.RelatedX + 20)  +","+ parseFloat(pathItem.RelatedY + 20) +" "+ parseFloat(pathItem.RelatedX + 20 +5) +","+ parseFloat(pathItem.RelatedY + 20 +8) +" "+ parseFloat(pathItem.RelatedX + 20 +8) +","+parseFloat(pathItem.RelatedY + 20 +5)+" "+ parseFloat(pathItem.RelatedX + 20)+","+parseFloat(pathItem.RelatedY + 20) +"' dojoGfxStrokeStyle='solid' />";
                        var polyline = "<polyline fill='rgb(128, 128, 128)' fill-opacity='1' fill-rule='evenodd' stroke='rgb(128, 128, 128)' stroke-dasharray='none' stroke-linecap='butt' stroke-linejoin='miter' stroke-miterlimit='4' stroke-opacity='1' stroke-width='1' points='" + parseFloat(pathItem.RelatedX + 20) + "," + parseFloat(pathItem.RelatedY + 20) + " " + pointsXY[0] + "," + pointsXY[1] + " " + pointsXY[2] + "," + pointsXY[3] + " " + parseFloat(pathItem.RelatedX + 20) + "," + parseFloat(pathItem.RelatedY + 20) + "' dojoGfxStrokeStyle='solid' />";
                        //文本
                        var textObj = "<text font-family='Arial' font-size='9pt' font-style='normal' font-variant='normal' font-weight='normal' text-decoration='none' fill='rgb(0, 0, 0)' fill-opacity='1' fill-rule='evenodd' kerning='auto' stroke='none' stroke-linecap='butt' stroke-linejoin='miter' stroke-miterlimit='4' stroke-opacity='0' stroke-width='1' text-anchor='start' transform='matrix(1 0 0 1 " + pathItem.X + " " + pathItem.Y + ")' x='" + parseFloat(dataItem.X + 10) + "' y='" + parseFloat(dataItem.Y + 10) + "' rotate='0' text-rendering='optimizeLegibility'>" + pathItem.Lable + "</text>";
                        strHtml = strHtml + pathLine + polyline + textObj;
                        ncount++;

                    }
                    strHtml = strHtml + "</g>";
                }
            }
        }
        strHtml = strHtml + "</svg>";
        $("#workflow").empty();
        $("#workflow").append(strHtml);
    }
}


function getArrowKeypoints(startX, startY, endX, endY) {
    //    L/_C_\R
    var arrowRadius = 4;   //箭头尺寸大小就靠它了
    var pointsXY = [0, 0, 0, 0];
    //根据关系直线起至坐标获得线的夹角
    var tmpx = endX - startX;
    var tmpy = startY - endY;
    var angle = Math.atan2(tmpy, tmpx) * (180 / Math.PI);

    var centerX = endX - arrowRadius * Math.cos(angle * (Math.PI / 180));
    var centerY = endY + arrowRadius * Math.sin(angle * (Math.PI / 180));
    var topX = endX;
    var topY = endY;
    var leftX = centerX + arrowRadius * Math.cos((angle + 120) * (Math.PI / 180));
    var leftY = centerY - arrowRadius * Math.sin((angle + 120) * (Math.PI / 180));
    var rightX = centerX + arrowRadius * Math.cos((angle + 240) * (Math.PI / 180));
    var rightY = centerY - arrowRadius * Math.sin((angle + 240) * (Math.PI / 180));

    pointsXY[0] = leftX;
    pointsXY[1] = leftY;
    //pointsXY[2] = centerX;   在箭身靠终点的一个点，这样画箭头更漂亮些
    //pointsXY[3] = centerY;
    pointsXY[2] = rightX;
    pointsXY[3] = rightY;
    //pointsXY[6] = ;
    //pointsXY[7] = ;
    return pointsXY;
}




