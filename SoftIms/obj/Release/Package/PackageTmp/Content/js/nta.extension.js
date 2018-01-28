var lazyLoadingHtml = '<div class="text-center text-primary"><span class="fa fa-refresh fa-spin" title="System is loading data, please wait..."></span></div>';

$(window).on("load", function () {
    $('.modal-fixed').modal({ backdrop: 'static', show: false });
});

//showMessage
var showMessage = function (msg) {
    alertify.success(msg);
};

//showErrorMessage
var showErrorMessage = function (msg) {
    alertify.error(msg);
};

//popupError
var popupAlert = function (msg) {
    alertify.defaults.glossary.title = 'Alert';
    alertify.alert(msg).set('closable', true);
};

var hideErrorMessage = function () {
    $('.form-error').hide();
}

//showErrorMessage
var showErrorMessage = function (message) {
    $('.form-error').show();
    $('.form-error .error-body').html(message);
};

//bindPageEvent
(function ($) {
    $.fn.bindPageEvent = function (option) {
        $(this).unbind('click').on('click', function (e) {
            e.preventDefault();
            if (typeof ($(this).attr('href')) == 'undefined') {
                return;
            }

            $(option.refreshButton).refreshData({
                page: $(this).attr("href").match(/page=([0-9]+)/)[1]
            });
            $(option.refreshButton).click();
        });
    };
}(jQuery));

//changePageSize
(function ($) {
    $.fn.changePageSize = function (option) {
        $(this).unbind("change").change(function (e) {
            e.preventDefault();
            var refreshButton = option.refreshButton;
          
            $(refreshButton).refreshData({
                page: 1
            });

            $(refreshButton).trigger('click');
        });
    };
}(jQuery));

//addNewRecord
(function ($) {
    $.fn.addNewRecord = function (option) {
        $(this).unbind('click').click(function (e) {
            e.preventDefault();
            var data = {
                id: $(this).attr('data-id'),
                url: $(this).attr('data-url'),
                title: $(this).attr('data-title'),
                refreshButton: $(this).attr('data-refreshButton') != null ? $(this).attr('data-refreshButton') : null,
                modalWidth: $(this).attr('data-modal-width'),
            };

            $.extend(data, option);
            fnAddNew(data.id, data.url, data.title, data.refreshButton, data.modalWidth);
        });
    }
}(jQuery));

//submitData
(function ($) {
    $.fn.submitData = function (option) {
        $(this).on("submit", function (e) {
            e.preventDefault();
            var data = {
                url: $(this).attr('data-url'),
                type: "Post",
                refreshButton: $(this).attr('data-refreshButton')
            };
            $.extend(data, option);
            var refreshButton = data.refreshButton;

            if ($(this).find('.input-validation-error').length == 0) {
                $(this).find(':submit').attr('disabled', 'disabled');
            }

            var form = $(this);
            var url = $(this).attr("action");

            var isValid = $(this).valid();
            if (!isValid) {
                $(this).find(':submit').removeAttr('disabled');
                return false;
            }
            else {
                $(this).find(':submit').attr('disabled', 'disabled');
            }

            var formData = new FormData();
            var other_data = $(this).serializeArray();
            $.each(other_data, function (key, input) {
                formData.append(input.name, input.value);
            });

            $.ajax({
                url: data.url,
                method: data.type,
                data: formData,
                contentType: false,
                processData: false,
                beforeSend: function () {
                    $(".loader").show();
                },
                complete: function () {
                    $(".loader").hide();
                },
                success: function (data) {
                    $(".form-error .error-body").hide();
                    var message = (data.message == null || data.message == '' || data.message == 'undefined') ? "Data save successed!" : data.message;
                    $("#notice .message").html(message);
                    $("#notice").show('slow', function () {
                        setTimeout(function () {
                            $("#notice").hide('slow');
                        }, 5000);
                    });

                    console.log('Url =' + option.redirectUrl);

                    if (refreshButton != null && $(refreshButton).length > 0 && refreshButton != 'undefined') {
                        $(refreshButton).click();
                    }
                    else if (option.redirectUrl != 'undefined' || option.redirectUrl != null) {
                        setTimeout(function () {
                            window.location.href = option.redirectUrl;
                        }, 5000);

                        return false;
                    };
                },
                error: function (result) {
                    form.find(':submit').removeAttr('disabled');
                    console.log(result);
                    $(".loader").hide();
                    var errmsg = "";
                    $.each(result.responseJSON.errors, function (idx, val) {
                        errmsg += val + "<br />";
                    });
                    $(".form-error .error-body").html(errmsg);
                    $(".success-message").hide('slow');
                    $(".form-error").show('slow');
                }
            });
        });
    }
}(jQuery));

var fnAddNew = function (id, url, title, refreshButton, modalWidth) {
    $("#EntryModal .error").hide();
    $.ajax({
        url: url,
        type: 'Get',
        data: { id: id },
        beforeSend: function () {
            $(".loader").show();
        },
        complete: function () {
            $(".loader").hide();
        },
        success: function (data) {
            if (data.redirectUrl) {
                window.location.href = data.redirectUrl;
                return false;
            };

            $("#EntryModal .modal-body").html(data);
            $("#EntryModal .entry-header").html(title);

            $("#EntryModal .success-message").hide();
            $("#EntryModal .error-message").hide();
            if (typeof (modalWidth) != 'undefined') {
                $("#EntryModal .modal-dialog").css("width", modalWidth);
            }
            else {
                $("#EntryModal .modal-dialog").css("width", '600px');
            }

            $("#EntryModal").modal('show');

            if ($('#EntryModal form').find('.focus-first').length > 0) {
                $('#EntryModal form').find('.focus-first').focus();
            }
            else {
                $('#EntryModal form').find('input:text:visible:first').focus();
            }

            var form = $("#EntryModal .modal-body form");
            $.validator.unobtrusive.parse(form);

            $("#EntryModal .modal-body form").on("submit", function (evt) {
                evt.preventDefault();

                if ($(this).find('.input-validation-error').length == 0) {
                    $(this).find(':submit').attr('disabled', 'disabled');
                }

                var url = $(this).attr("action");

                var isValid = $(this).valid();
                if (!isValid) {
                    $(this).find(':submit').removeAttr('disabled');
                    return false;
                }
                else {
                    $(this).find(':submit').attr('disabled', 'disabled');
                }

                var formData = new FormData();
                var other_data = $('#EntryModal form').serializeArray();
                $.each(other_data, function (key, input) {
                    formData.append(input.name, input.value);
                });

                if ($("#files").length > 0) {
                    var selectedFile = $("#files").get(0).files[0];
                    if (selectedFile != null) {
                        formData.append("files", selectedFile);
                    }
                }
                $.ajax({
                    url: url,
                    method: "post",
                    data: formData,
                    contentType: false,
                    processData: false,
                    beforeSend: function () {
                        $(".loader").show();
                    },
                    complete: function () {
                        $(".loader").hide();
                    },
                    success: function (data) {
                        $("#EntryModal").modal('hide');
                        var message = (data.message == null || data.message == '' || data.message == 'undefined') ? "Data save successed!" : data.message;
                        showMessage(message);
                        if ($(refreshButton).length > 0) {
                            $(refreshButton).click();
                        }
                        else if (refreshButton != null) {
                            setTimeout(function () {
                                location.reload();
                            }, 2000);
                        };
                    },
                    error: function (result) {
                        $(".loader").hide();
                        $('#EntryModal form').find(':submit').removeAttr('disabled');
                        console.log(result);
                        var errmsg = "";
                        $.each(result.responseJSON.errors, function (idx, val) {
                            errmsg += val + "<br />";
                        });
                        showErrorMessage(errmsg);
                    }
                });
            });
        }
    });
};

//deleteRecord
(function ($) {
    $.fn.deleteRecord = function (option) {
        $(this).unbind('click').on('click', function (e) {
            $this = $(this);
            e.preventDefault();
            var data = {
                refreshButton: $(this).attr('data-refreshButton'),
                url: $(this).attr('data-url')
            };
            $.extend(data, option);

            var buttonToBeClick = data.refreshButton;
            $("#DeleteConfirmationModal .modal-body form").attr("action", null);
            $("#DeleteConfirmationModal .success-message").hide();
            $("#DeleteConfirmationModal .error-message").hide();
            //child-item-confirmation
            $("#DeleteConfirmationModal").find('.child-item-confirmation').hide();
            $("#DeleteConfirmationModal").find('#deleteChildItem').iCheck('uncheck');

            $("#DeleteConfirmationModal").modal('show');
            $("#DeleteConfirmationModal .modal-body form").attr("action", data.url);
            $("#DeleteConfirmationModal .modal-body form").unbind('submit').on("submit", function (evt) {
                evt.preventDefault();
                var formData = new FormData();
                var other_data = $('#DeleteConfirmationModal form').serializeArray();
                $.each(other_data, function (key, input) {
                    formData.append(input.name, input.value);
                });

                $.ajax({
                    url: data.url,
                    method: "post",
                    data: formData,
                    contentType: false,
                    processData: false,
                    beforeSend: function () {
                        $(".loader").show();
                    },
                    complete: function () {
                        $(".loader").hide();
                    },
                    success: function (result) {
                        $this.closest('tr').hide();
                        $("#DeleteConfirmationModal").modal('hide');

                        var message = (result.message == null || result.message == '') ? "Record deleted successfully." : result.message;
                        $("#notice .message").html(message);
                        $("#notice").show('slow', function () {
                            setTimeout(function () {
                                $("#notice").hide('slow');
                            }, 5000);
                        });
                        if (typeof (data.onSuccess) != 'function') {
                            if ($(buttonToBeClick).length > 0) {
                                $(buttonToBeClick).refreshData({
                                    page: 1
                                });
                                $(buttonToBeClick).click();
                            }
                            else if (buttonToBeClick != null) {
                                location.reload();
                            }
                        }
                        else {
                            data.onSuccess();
                        }
                    },
                    error: function (result) {
                        console.log(result);
                        $(".loader").hide();
                        var errmsg = '';
                        if (result.responseJSON.displayMessage != null && result.responseJSON.displayMessage != typeof undefined) {
                            errmsg = result.responseJSON.displayMessage;
                        }
                        else {
                            errmsg = "<b>Error! </b>";
                            $.each(result.responseJSON.errors, function (idx, val) {
                                errmsg += val + "<br />";
                            });
                        }
                        $("#DeleteConfirmationModal .error-message").html(errmsg);
                        if (result.responseJSON.confirmChild == true) {
                            $("#DeleteConfirmationModal").find('.child-item-confirmation').show('slow');
                        };

                        $("#DeleteConfirmationModal .success-message").hide('slow');
                        $("#DeleteConfirmationModal .error-message").show('slow');
                    }
                });
            });

        })
    };
}(jQuery));

//refreshData
(function ($) {
    $.fn.refreshData = function (option) {
        $(this).unbind('click').on('click', function (e) {
            e.preventDefault();
            var resultDiv = $(this).attr('data-resultElement') == null ? ".detail" : $(this).attr('data-resultElement');
            var data =
            {
                url: $(this).attr("data-url"),
                showLoader: $(this).attr('data-showLoader'),
                form: $(this).closest('form'),
                pageSize: $($(this).attr('data-pageSizeElement')).val(),
                page: 1,
                resultElement: $(this).attr('data-resultElement') == null ? ".detail" : $(this).attr('data-resultElement'),
                type: "get",
                showProgress: $(resultDiv).attr('data-progress') != null
            };

            $.extend(data, option);
            var paramData;
            if (data.form.length > 0) {
                paramData = data.form.serialize() + "&pagesize=" + data.pageSize + "&page=" + data.page + "";
            }

            $.ajax({
                url: data.url,
                type: data.type,
                data: paramData,
                beforeSend: function () {
                    if (data.showLoader == 'true') {
                        $(".loader").show();
                    }
                    if(data.showProgress)
                    {
                        $(data.resultElement).html(lazyLoadingHtml);
                    }
                },
                complete: function () {
                    $(".loader").hide();
                },

                success: function (result) {
                    if (result.redirectUrl) {
                        window.location.href = result.redirectUrl;
                        return false;
                    };

                    $(data.resultElement).show('slow');
                    $(data.resultElement).html(result);
                    $(".loader").hide();
                },
                error: function (result) {
                    $(data.resultElement).show('slow');
                    $(data.resultElement).html(result);
                    $(".loader").hide();
                    console.log(result);
                }
            });
        });
    };
}(jQuery));

//loadChildList
(function ($) {
    $.fn.loadChildList = function (option) {
        var data =
            {
                showLoader: false,
                valueField: "Value",
                textField: "Text",
                type: "get",
            };
        $.extend(data, option);

        $.ajax({
            url: data.url,
            type: data.type,
            data: data.paramData,
            beforeSend: function () {
                if (data.showLoader) {
                    $(".loader").show();
                }
            },
            complete: function () {
                $(".loader").hide();
            },
            success: function (list) {
                var html = '';
                if (data.optionalLabel != null && data.optionalLabel != '') {
                    html = '<option value="">' + data.optionalLabel + '</option>';
                }
                $(list).each(function (idx, value) {
                    html += '<option value="' + eval("value." + data.valueField) + '">' + eval("value." + data.textField) + '</option>';
                });

                $(data.childElement).html(html);

                if (data.selectedValue > 0) {
                    var optionExists = $(data.childElement).find('option[value=' + data.selectedValue + ']').length > 0;

                    var length = ($(data.childElement) + '> option').length;
                    if (optionExists) {
                        $(data.childElement).val(data.selectedValue);
                    }
                    if (length == 2) {
                        $(data.childElement).val($(data.childElement + ' option:eq(1)').val());
                    }
                }
                $(data.childElement).trigger("chosen:updated");
                $(data.childElement).trigger("change");
            },
            error: function (error) {
                console.log(error);
                $(".loader").hide();
            }
        });
    };
}(jQuery));

var jQueryLoaded = function () {

}

var collapseSidebar = function () {
    $(".sidebar-mini").addClass('sidebar-collapse');
}

var exportToExcel = (function () {
    var uri = 'data:application/vnd.ms-excel;base64,', template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><meta charset="UTF-8" /><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table>{table}</table></body></html>', base64 = function (s) { return window.btoa(unescape(encodeURIComponent(s))) },
        format = function (s, c) { return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; }) }
    return function (table, worksheet) {
        var ctx = { worksheet: worksheet || 'Worksheet', table: table.html() }
        window.location.href = uri + base64(format(template, ctx))
    }
})();

//ExportToExcel
$(function () {
    $(".toexcel").off("click").click(function (e) {
        e.preventDefault();
        var table = $(this).attr("data-table") == null ? ".excel-data" : $(this).attr("data-table");
        var worksheet = $(this).attr("data-sheet");
        var reportTitle = $(this).attr("data-report-title");

        if (!table.nodeType) table = $(table)
        if (table == null) {
            return false;
        }

        var $listTable = $(table).clone();
        $("a", $listTable).removeAttr("href");

        var th = $.grep($listTable.find("tbody").find("th"), function (val) {
            return $(val).attr("data-no-print");
        });

        var td = $.grep($listTable.find("td"), function (val) {
            return $(val).attr("data-no-print");
        });

        var span = $.grep($listTable.find("span"), function (val) {
            return $(val).attr("data-no-print");
        });

        $.each(th, function (idx, val) {
            $(val).remove();
        });

        $.each(td, function (idx, val) {
            $(val).remove();
        });

        $.each(span, function (idx, val) {
            $(val).remove();
        });

        var colspan = $("tr", $listTable).eq(1).children().length;

        var $trReportTitle = $("<tr><td style='text-align:center; font-weight:bold; font-size:18px;' colspan='" + colspan + "'>" + reportTitle + "</td></tr>");

        if ($listTable.find("thead").length > 0) {
            $listTable.find("thead").prepend($trReportTitle);
        }
        else {
            $listTable.prepend($trReportTitle);
        }

        exportToExcel($listTable, worksheet);
    });
});

//PrintData
$(function () {
    $(".btnPrint").off("click").click(function (e) {
        e.preventDefault();
        var reportTitle = $(this).attr("data-report-title");
        var subTitle = $(this).attr("data-subtitle");
        var printElement = $(this).attr('data-print-element');
        var staticTitle = $(this).attr('data-static-title');
        var mywindow = window.open('', '_blank');
        mywindow.document.write('<html><head><title> Print ' + reportTitle + '</title>');
        mywindow.document.write('<link rel="stylesheet" href="/Content/bootstrap/dist/css/bootstrap.min.css" type="text/css" />');
        mywindow.document.write('<link rel="stylesheet" href="/Content/css/AdminLTE.css" type="text/css"/>');
        mywindow.document.write('<link rel="stylesheet" href="/Content/css/custom.css" type="text/css" />');
        mywindow.document.write("</head><body>");
        if (staticTitle == null) {
            mywindow.document.write("<table class='table' border='0' style='margin-bottom:10px;'>");
            mywindow.document.write("<tr><td style='text-align:center;font-size:18px;font-weight:bold; border-top:0px;line-height:1.1;'>Inventory Management SYstem</td></tr>");
            mywindow.document.write("<tr><td style='text-align:center;font-size:16px;font-weight: bold; border-top:0px;line-height:1.1;'>Mid Baneshor, Kathmandu</td></tr>");
            mywindow.document.write("<tr><td style='text-align:center; font-weight:bold; font-size:21px;text-decoration: underline; border-top:0px;line-height:1.1;'>" + reportTitle + "</td></tr>");
            if (subTitle != null) {
                mywindow.document.write("<tr><td style='text-align:center; font-weight:bold; font-size:16px; border-top:0px;line-height:1.1;'>" + subTitle + "</td></tr>");
            }
            mywindow.document.write('</table>');
        }
        var $div = $('<div />');
        $div.html($(printElement).html());
        $("a", $div).removeAttr("href");
        $("[data-no-print]", $div).addClass("hide");
        $('th[data-no-print]', $div).addClass('hide');
        $('td[data-no-print]', $div).addClass('hide');
        $('span[data-no-print]', $div).addClass('hide');

        mywindow.document.write($div.html());
        mywindow.document.write('<script type="text/javascript">window.document.close(); window.focus();window.print();window.close();<' + '/script>');
        mywindow.document.write('</body></html>');
        $("[data-no-print]", $div).removeClass("hide");
    });
});

//lazyLoading
(function ($) {
    $.fn.lazyLoading = function (option) {
        $this = $(this);
        var data = {
            url: $this.attr('data-url'),
            type: $this.attr('data-type') == null ? "get" : $this.attr('data-type')
        };
        $.extend(data, option);

        $.ajax({
            url: data.url,
            type: data.type,
            success: function (result) {
                $this.html(result);
                $this.show();
            },

            error: function (result) {
                console.log(result);
            }
        });
    };
}(jQuery));


//searchTableData
function searchTableData(input, table) {
    var filter, tr, td, i;
    filter = $(input).val().toUpperCase();
    if (table.find('tbody').length > 0) {
        tr = table.find('tbody').find("tr");
    }
    else {
        tr = table.find("tr");
    }
    for (i = 0; i < tr.length; i++) {
        td = $('td', tr[i]);
        if (td) {
            if ($(td).text().toUpperCase().indexOf(filter) > -1) {
                tr[i].style.display = "";
            } else {
                tr[i].style.display = "none";
            }
        }
    }
};