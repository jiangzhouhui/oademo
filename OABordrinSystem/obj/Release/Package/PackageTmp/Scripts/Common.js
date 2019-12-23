// 封装一个对话框对象
var dialog = {

    // 提示信息
    showMsg: function (title, content) {
        if (document.getElementById('dialogModal') != null) {
            $('#dialogModal').remove();
        }

        htmlString = '<div class="modal fade MicroYaHei" id="dialogModal">' +
                        '<div class="modal-dialog">' +
                            '<div class="modal-content">' +
                                '<div class="modal-header">' +
                                    '<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
                                    '<h1 class="modal-title">' + title + '</h1>' +
                                '</div>' +
                            '<div class="modal-body">' +
                                '<h2>' + content + '</h2>' +
                            '</div>' +
                            '<div class="modal-footer">' +
                                '<button type="button" class="btn btn-success" data-dismiss="modal">Close</button>' +
                            '</div>' +
                        '</div>' +
                     '</div>';
        $('body').append(htmlString);
        $('#dialogModal').modal('show');
    },
    confirm: function (title, content, callback) {
        if (document.getElementById('confirmDialogModal') != null) {
            $('#confirmDialogModal').remove();
        }

        htmlString = '<div class="modal fade MicroYaHei" id="confirmDialogModal">' +
                        '<div class="modal-dialog">' +
                            '<div class="modal-content">' +
                                '<div class="modal-header">' +
                                    '<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
                                    '<h1 class="modal-title">' + title + '</h1>' +
                                '</div>' +
                            '<div class="modal-body">' +
                                '<h2>' + content + '</h2>' +
                            '</div>' +
                            '<div class="modal-footer">' +
                                '<button type="button" id="btndialogModalConfirm" class="btn btn-danger" data-dismiss="modal" >确定</button>' +
                                '<button type="button" class="btn btn-success" data-dismiss="modal">取消</button>' +
                            '</div>' +
                        '</div>' +
                     '</div>';

        $('body').append(htmlString);
        $('#confirmDialogModal').modal('show');

        $("#btndialogModalConfirm").click(function () { callback(); });

    }
}



//公共方法
var app = (function () {
    var a = {};
    var b = [];
    a.inc = function (d, c) {
        return true
    };
    a.register = function (e, c) {
        var g = e.split(".");
        var f = a;
        var d = null;
        while (d = g.shift()) {
            if (g.length) {
                if (f[d] === undefined) {
                    f[d] = {}
                }
                f = f[d]
            } else {
                if (f[d] === undefined) {
                    try {
                        f[d] = c(a)
                    } catch (h) {
                        b.push(h)
                    }
                }
            }
        }
    };
    a.IE = /msie/i.test(navigator.userAgent);
    return a
})();

app.register("util", function () {
    var a = {
        //为对象下的控件元素进行封装
        getEditParam: function ($edittable) {
            var ans = {};
            $edittable.find("select,input[type=text],input[type=hidden],input[type=file],input[type=checkbox],textarea,label").each(function () {
                var $this = $(this);
                if ($this.attr("name") != undefined) {
                    ans[$this.attr("name")] = $this;
                }
            })
            return ans;
        },
        //验证输入是否为数字
        ValidNumber: function (obj) {

            var re = new RegExp("^[0-9]*$");
            var bool = re.exec(obj.val());
            if (!bool) {
                obj.closest("div").addClass("has-error");
            }
            return bool;
        },
        //正浮点数
        ValidJustFloat: function (obj) {
            var re = new RegExp("^\\d+(\\.\\d+)?$");
            var bool = re.exec(obj.val());
            if (!bool) {
                obj.closest("div").addClass("has-error");
            }
            return bool;
        },
        //验证输入是否为时间
        ValidDate: function (obj) {
            var re = new RegExp("(([0-9]{3}[1-9]|[0-9]{2}[1-9][0-9]{1}|[0-9]{1}[1-9][0-9]{2}|[1-9][0-9]{3})-(((0[13578]|1[02])-(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)-(0[1-9]|[12][0-9]|30))|(02-(0[1-9]|[1][0-9]|2[0-8]))))|((([0-9]{2})(0[48]|[2468][048]|[13579][26])|((0[48]|[2468][048]|[3579][26])00))-02-29)");
            var bool = re.exec(obj.val());
            if (!bool) {
                obj.closest("div").addClass("has-error");
            }
            return bool;
        },
        ValidEmail: function (obj) {
            var pattern = /^([\.a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+(\.[a-zA-Z0-9_-])+/;
            if (!pattern.test(obj.val())) {
                obj.closest("div").addClass("has-error");
                return false;
            }
            return true;
        },
        ValidMobilePhone: function (obj) {
            //电信手机号码正则        
            var dianxin = /^1[3578][01379]\d{8}$/;
            //联通手机号正则        
            var liantong = /^1[34578][01256]\d{8}$/;
            //移动手机号正则        
            var yidong = /^(134[012345678]\d{7}|1[34578][012356789]\d{8})$/;
            if (dianxin.test(obj.val()) || liantong.test(obj.val()) || yidong.test(obj.val())) {
                return true;
            }
            else {
                obj.closest("div").addClass("has-error");
                return false;
            }
        },
        //验证必填信息
        ValidationFromData: function ($edittable) {
            var isCanSubmit = true;
            $edittable.find("select,input[type=text],input[type=hidden],input[type=file],input[type=checkbox],textarea").each(function () {
                var $this = $(this);
                if ($this.attr("IsNotNull") == "true" && $.trim($this.val()) == "") {
                    $this.closest("div").addClass("has-error");
                    isCanSubmit = false;
                }
                else {
                    $this.closest("div").removeClass("has-error");
                }
            });
            return isCanSubmit;
        },
        //序列化参数
        serializeParamArray: function (param) {
            var ans = {};
            for (var key in param) {
                if (param[key].is(":checkbox")) {
                    ans[key] = param[key].is(":checked");
                } else {
                    ans[key] = $.trim(param[key].val());
                }
            }
            return ans;
        },
        //为对象成员赋值
        bindValue: function (editParams, ans) {
            if (typeof (editParams) != "undefined" && editParams) {
                if (editParams.selector) {
                    editParams = app.util.getNameParam(editParams);
                }
                var hasData = typeof (ans) != "undefined" && typeof (ans.data) != "undefined" && ans.data;
                for (var key in editParams) {
                    var a;
                    if (ans === "") {
                        a = "";
                    } else if (hasData) {
                        a = ans.data[key];
                    }
                    var editparam = editParams[key];
                    editparam.each(function () {
                        var $thisEditParam = $(this);
                        switch ($thisEditParam[0].nodeName) {
                            case "SELECT":
                                if ($thisEditParam.find("option[value='" + a + "']").length == 0) {
                                    //$thisEditParam.val('0');
                                    $thisEditParam.find("option:eq(0)").attr("selected", "selected");
                                } else {
                                    $thisEditParam.val(a);
                                }
                                break;
                            case "INPUT":
                            case "TEXTAREA":
                                if ($thisEditParam.is(":checkbox")) {
                                    if (a == "1" || a == true) {
                                        $thisEditParam.closest("div").addClass("checked");
                                    } else {
                                        $thisEditParam.closest("div").removeClass("checked");
                                    }
                                }
                                else if (!$thisEditParam.is(":file")) {
                                    $thisEditParam.val(a);
                                }

                                break;
                            case "SPAN":
                                $thisEditParam.html(a);
                                break;
                            case "DIV":
                            case "TD":
                            case "TR":
                            case "LABEL":
                                $thisEditParam.html(a);
                                break;
                            case "FONT":
                                $thisEditParam.html(a);
                                break;
                            default:
                                $thisEditParam.val(a);
                        }
                    })
                }
            }
        },
        clearError: function ($model) {
            if ($model) {
                $model.find(".has-error").removeClass("has-error");
                $model.find("div.errorMessage").css("display", "none");
                //$model.find("input[type=text],textarea,select").attr("data-placement", "bottom").each(function () {
                //    $(this).tooltip('hide');
                //});
            }
        },
        onError: function (errors, $model) {
            $model.find("div.errorMessage").css("display", "none");
            for (var i = 0; i < errors.length; i++) {
                var errorItem = errors[i];
                if (errorItem.key == "errorMessage") {
                    $model.find("div.errorMessage").find("span").text(errorItem.value);
                    $model.find("div.errorMessage").css("display", "");
                    break;
                }
                else {
                    var options = { "trigger": "manual" };
                    $obj = $model.find("[name='" + errorItem.key + "']");
                    $obj.tooltip(options);
                    $obj.attr("data-original-title", errorItem.value);
                    $obj.tooltip('show');
                    break;
                }
            }
        },
        ModelToDetails: function ($edittable) {
            $edittable.find("input[type=text],textarea,select").hide().each(function () {
                var $this = $(this);
                var value = $this.val();
                if ($this[0].nodeName == "SELECT") {
                    value = $this.find("option:selected").text();
                }
                $this.next(".theValue").remove();
                $this.after("<label class='theValue control-label' style='font-weight:normal;text-align: left;'>" + value + "</label>");
            });
            $edittable.find("input[type=radio],input[type=checkbox]").each(function () {
                $(this).attr("disabled", "disabled");
            });

            $edittable.find("div.modal-footer").find(".btn-danger").hide();

        },
        ModelToEdit: function ($edittable) {
            $edittable.find("input[type=text],textarea,select").show().next(".theValue").remove();
            $edittable.find("input[type=radio],input[type=checkbox]").each(function () {
                $(this).removeAttr("disabled");
            });
            //$edittable.find("div.modal-footer").find(".btn-danger").show();
        },
        //得到当前浏览的datatables页码
        DTCurrentPage: function (table) {
            var tableSetings = table.fnSettings();
            var paging_length = tableSetings._iDisplayLength;//当前每页显示多少  
            var page_start = tableSetings._iDisplayStart;//当前页开始  
            var rslt = page_start / paging_length; //除    
            if (rslt >= 0) {
                rslt = Math.floor(rslt); //返回小于等于原rslt的最大整数。     
            }
            else {
                rslt = Math.ceil(rslt); //返回大于等于原rslt的最小整数。     
            }
            return rslt;
        }
    }
    return a;
});

//jquery 扩展
(function ($) {
    $.fn.extend({
        //限制只能输入数字
        number: function () {
            return $(this).bind("keypress", function (e) {
                var keyCode = e.which;
                return keyCode == 59 || keyCode == 190 || (keyCode > 47 && keyCode < 58);
            })
        },
        //限制只能输入浮点数
        digital: function () {
            return $(this).bind("keypress", function (e) {
                var keyCode = e.which;
                return keyCode == 46 || keyCode == 45 || keyCode == 59 || keyCode == 190 || (keyCode > 47 && keyCode < 58);
            })
        }
    })
})(jQuery);


// Extend the default Number object with a formatMoney() method:
// usage: someVar.formatMoney(decimalPlaces, symbol, thousandsSeparator, decimalSeparator)
// defaults: (2, "$", ",", ".")
Number.prototype.formatMoney = function (places, symbol, thousand, decimal) {
    places = !isNaN(places = Math.abs(places)) ? places : 2;
    symbol = symbol !== undefined ? symbol : "";
    thousand = thousand || ",";
    decimal = decimal || ".";
    var number = this,
        negative = number < 0 ? "-" : "",
        i = parseInt(number = Math.abs(+number || 0).toFixed(places), 10) + "",
        j = (j = i.length) > 3 ? j % 3 : 0;
    return symbol + negative + (j ? i.substr(0, j) + thousand : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thousand) + (places ? decimal + Math.abs(number - i).toFixed(places).slice(2) : "");
};

//去除千分位
function delcommafy(num) {
    if ($.trim((num + "")) == "") {
        return "";
    }
    num = num.replace(/,/gi, '');
    return num;
}


//Ajax基本设置
$.ajaxSetup({
    dataType: 'json',
    type: "POST",
    cache: false
})

//获取当前的日期时间 格式“yyyy-MM-dd HH:MM:SS”
function GetNowFormatDate() {
    var date = new Date();
    var seperator1 = "-";
    var seperator2 = ":";
    var month = date.getMonth() + 1;
    var strDate = date.getDate();
    if (month >= 1 && month <= 9) {
        month = "0" + month;
    }
    if (strDate >= 0 && strDate <= 9) {
        strDate = "0" + strDate;
    }
    //var currentdate = date.getFullYear() + seperator1 + month + seperator1 + strDate
    //+ " " + date.getHours() + seperator2 + date.getMinutes()
    //+ seperator2 + date.getSeconds();
    var currentdate = date.getFullYear() + seperator1 + month + seperator1 + strDate;
    return currentdate;
}

//获取时间差
function GetDateDiff(startDate, endDate) {
    if ($.trim(startDate) != "" && $.trim(endDate) != "") {
        var startTime = new Date(Date.parse(startDate.replace(/-/g, "/"))).getTime();
        var endTime = new Date(Date.parse(endDate.replace(/-/g, "/"))).getTime();
        var dates = Math.abs((startTime - endTime)) / (1000 * 60 * 60 * 24);
        return dates;
    }
    else
    {
        return 0;
    }
}

//$(document).keypress(function (e) {
//    // 回车键事件  
//    if (e.which == 13) {

//    }
//});

$(function () {
    $("#SearchPara").delegate("input", "keydown", function (e) {
        var ev = document.all ? window.event : e;
        if (ev.keyCode == 13) {
            ev.preventDefault();
            $("#BtnSearch").trigger("click");

        }
    });
});


//制保留2位小数，如：2，会在2后面补上00.即2.00 
function toDecimal2(x) {
    var f = parseFloat(x);
    if (isNaN(f)) {
        return false;
    }
    var f = Math.round(x * 100) / 100;
    var s = f.toString();
    var rs = s.indexOf('.');
    if (rs < 0) {
        rs = s.length;
        s += '.';
    }
    while (s.length <= rs + 2) {
        s += '0';
    }
    return s;
}



//将数字金额转换为大写
function convertCurrency(money) {
    //汉字的数字
    var cnNums = new Array('零', '壹', '贰', '叁', '肆', '伍', '陆', '柒', '捌', '玖');
    //基本单位
    var cnIntRadice = new Array('', '拾', '佰', '仟');
    //对应整数部分扩展单位
    var cnIntUnits = new Array('', '万', '亿', '兆');
    //对应小数部分单位
    var cnDecUnits = new Array('角', '分', '毫', '厘');
    //整数金额时后面跟的字符
    var cnInteger = '整';
    //整型完以后的单位
    var cnIntLast = '元';
    //最大处理的数字
    var maxNum = 999999999999999.9999;
    //金额整数部分
    var integerNum;
    //金额小数部分
    var decimalNum;
    //输出的中文金额字符串
    var chineseStr = '';
    //分离金额后用的数组，预定义
    var parts;
    if (money == '') { return ''; }
    money = parseFloat(money);
    if (money >= maxNum) {
        //超出最大处理数字
        return '';
    }
    if (money == 0) {
        chineseStr = cnNums[0] + cnIntLast + cnInteger;
        return chineseStr;
    }
    //转换为字符串
    money = money.toString();
    if (money.indexOf('.') == -1) {
        integerNum = money;
        decimalNum = '';
    } else {
        parts = money.split('.');
        integerNum = parts[0];
        decimalNum = parts[1].substr(0, 4);
    }
    //获取整型部分转换
    if (parseInt(integerNum, 10) > 0) {
        var zeroCount = 0;
        var IntLen = integerNum.length;
        for (var i = 0; i < IntLen; i++) {
            var n = integerNum.substr(i, 1);
            var p = IntLen - i - 1;
            var q = p / 4;
            var m = p % 4;
            if (n == '0') {
                zeroCount++;
            } else {
                if (zeroCount > 0) {
                    chineseStr += cnNums[0];
                }
                //归零
                zeroCount = 0;
                chineseStr += cnNums[parseInt(n)] + cnIntRadice[m];
            }
            if (m == 0 && zeroCount < 4) {
                chineseStr += cnIntUnits[q];
            }
        }
        chineseStr += cnIntLast;
    }
    //小数部分
    if (decimalNum != '') {
        var decLen = decimalNum.length;
        for (var i = 0; i < decLen; i++) {
            var n = decimalNum.substr(i, 1);
            if (n != '0') {
                chineseStr += cnNums[Number(n)] + cnDecUnits[i];
            }
        }
    }
    if (chineseStr == '') {
        chineseStr += cnNums[0] + cnIntLast + cnInteger;
    } else if (decimalNum == '') {
        chineseStr += cnInteger;
    }
    return chineseStr;
}


//中文转换为英文
function GetLanguageValueByParam(chineseValue, languageCategory, fileName) {
    var value = "";
    $.ajax({
        url: '/Common/GetLanguageValueByParam',
        data: { "chineseValue": chineseValue, "languageCategory": languageCategory, "fileName": fileName },
        dataType: "json",
        async: false,
        cache: true,
        success: function (ans) {
            if (ans.error.length > 0) {
                return false;
            }
            value = ans.data;
        }
    });
    return value;
}

//根据版本编号获取   流程版本数据
function GetWorkFlowVersionByVersionNo(b_VersionNo, $div, itemStatus) {
    $.ajax({
        url: '/Common/GetWorkFlowVersionByVersionNo',
        data: { "versionNo": b_VersionNo },
        dataType: "json",
        async: false,
        cache: true,
        success: function (ans) {
            if (ans.error.length > 0) {
                return false;
            }
            $div.empty();
            $div.append(ans.data.B_WORKFLOWCHARTCODE);
            $div.find("text").each(function () {
                var aimRect = $(this).closest("g").find("rect").first();
                if (aimRect.attr("fill") == "rgb(238, 238, 0)") {
                    aimRect.attr("fill", "none");
                }
            });
            $div.find("text").each(function () {
                var textValue = $(this).attr("status");
                if (textValue == itemStatus) {
                    var aimRect = $(this).closest("g").find("rect").first();
                    aimRect.attr("fill", "rgb(238, 238, 0)");
                    aimRect.attr("fill-opacity", "1");
                    aimRect.attr("fill-rule", "evenodd");
                }
            });


        }
    });
}