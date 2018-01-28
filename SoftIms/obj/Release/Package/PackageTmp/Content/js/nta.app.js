$(window).on('load', function () {
    $('.tab-header').unbind("click").on('click', function () {
        $this = $(this);
        if (!$this.attr('data-loaded')) {
            var container = $($this.attr('href'));
            $('.lazy-loading-once', container).each(function () {
                $(this).html(lazyLoadingHtml)
                $(this).show();
                var url = $(this).attr("data-url");
                $.ajax({
                    url: url,
                    success: function (html) {
                        $('.lazy-loading-once', container).html(html);
                    }
                });
            });
            $this.attr('data-loaded', true);
        }
    });

    $(".lazy-loading").each(function () {
        var $this = $(this);
        if (!$this.attr("data-show-loader") || $this.attr("data-show-loader") == "true") {
            var lazyLoadingHtml = '<div class="text-center text-primary"><span class="fa fa-refresh fa-spin" title="System is loading data, please wait..."></span></div>';
            $this.html(lazyLoadingHtml);
        }
        $this.show();
        $.ajax({
            url: $this.attr("data-url"),
            success: function (html) {
                $this.html(html);
                $this.show('slow');
            }
        });
    });

    $(".mask-miti").nepaliDatePicker({
        npdMonth: true,
        npdYear: true,
        npdYearCount: 100
    });
    $(".mask-miti").inputmask("9999-99-99", { placeholder: "yyyy-MM-dd" });
});

var actionData = function (ctx) {
    $this = $(ctx);
    var reloadButton = $this.attr('data-reloadButton') != null ? $this.attr('data-reloadButton') : '.btnReload';
    $.ajax({
        url: $this.attr('data-url'),
        method: "post",
        beforeSend: function () {
            $(".loader").show();
            $this.attr('disabled', 'disabled');
        },
        complete: function () {
            $(".loader").hide();
            $this.removeAttr('disabled');
        },
        success: function (result) {
            $(".loader").hide();
            if (result.redirectUrl) {
                window.location.href = result.redirectUrl;
                return false;
            };
            var message = (result.message == null || result.message == '') ? "Record deleted successfully." : result.message;
            $("#notice .message").html(message);

            $("#notice").show('slow', function () {
                setTimeout(function () {
                    $("#notice").hide('slow');
                }, 3000);
            });
            if ($(reloadButton).length > 0) {
                $(reloadButton).reloadData({
                    page: 1
                });
                $(reloadButton).trigger("click");
            }
            if ($this.attr('data-calback') != '' || $this.attr('data-calback') != null) {
                $(".loader").hide();
                eval($this.attr('data-calback'));
            }
            $this.closest('tr').remove();
        },
        error: function (result) {
            $(".loader").hide();
            $this.removeAttr('disabled');
            var errmsg = "<b>Error! </b>";
            $.each(result.responseJSON.error, function (idx, val) {
                errmsg += val + "<br />";
            });
            errmsg += '<p style="display:none;">' + result.responseJSON.fkMessage + '</p>';
            $this.colsest('.popover').find(".error-message").html(errmsg);
            $this.colsest('.popover').find('.success-message').hide('slow');
            $this.colsest('.popover').find('.error-messag').show('slow');
        }
    });
};

var initialization = function () {
    $('.detail').closest('.box-body').css('min-height', '200px');
    $('.keyword').attr('placeholder', 'Type to filter...');
    $('.fa-search').closest('button').attr('title', 'Click to search');
    $('.fa-search').closest('button').attr('data-toggle', 'tooltip');

    initPopover();
    $('body').on('click', function (e) {
        //did not click a popover toggle or popover
        if ($(e.target).data('toggle') !== 'popover' && $(e.target).parents('.popover.in').length === 0) {
            //$('.popover').hide();
        }
    });

    $('.icheck').iCheck({
        checkboxClass: 'icheckbox_square-blue'
    })
    $('.keyword').on('keyup', function (e) {
        var target = $(this).attr('data-target') == null ? '.table-filter' : $(this).attr('table-filter');
        if ($(target).length == 0) {
            target = $(this).closest('.box').find('.table');
        }
        searchTableData($(this), $(target));
    });

    $('.fancybox').fancybox({
        href: $(this).attr('src'),
        afterClose: function () {
            $(".fancybox").css("display", "");
        }
    });
    $(".btnShowDetail").addNewRecord();
    $('.tablesorter').tablesorter({});
   
    $(".datepicker").datepicker({
        todayHighlight: true,
        format: "yyyy-mm-dd"
    });

    $(".generateCertificateForm #ValidityDateFrom").datepicker({
        todayHighlight: true,
        format: "yyyy-mm-dd",
        changeDate: function (date) {
            $.ajax({
                url: rootPath + "common/GetDefaultValidityDateTo?date=" + date,
                success: function (resp) {
                    $(".generateCertificateForm #ValidityDateTo").val(resp.dateto);
                }
            });
            
        }
    });

    $(".btnNew").addNewRecord();
    $(".btnDelete").deleteRecord();
    $('.btn-delete').deleteRecord();
    $('.btnRefresh').refreshData();

    $(".mask-miti").on("keyup", function (e) {
        if (e.keyCode == 46) {
            $(this).val(null);
        }
    });

    $(".chosen-select").chosen({ width: '100%' });

    $(".btnUpload").on("click", function () {
        console.log('test');
        var $this = $(this);
        $.extend(uploadModelContext, { masterId: $this.data("masterid"), dataId: $this.data("id"), documentTypeId: $this.data("documenttype"), module: $this.data("module") });
        $("#UploadModal").modal("show");
    });

    $(".close-error").on("click", function () {
        $(".msg.error-top").hide('slow');
    });

    $(".close-success").on("click", function () {
        $(".msg.success-top").hide('slow');
    });


    $('input[type=text][data-val-required],select[data-val-required],textarea[data-val-required]').each(function () {
        var req = $(this).attr('data-val-required');
        var ctx = $(this).closest('.form-group');
        var label = $('label[for="' + $(this).attr('id') + '"]', ctx);
        var text = label.text();
        if (text.length > 0 && $('span', label).length == 0) {
            label.append('<span style="color:red"> *</span>');
        }
    });

    $('[data-required]').each(function () {
        if ($(this).text().indexOf('*') == -1) {
            $(this).append('<span style="color:red"> *</span>');
        }
    });

    $('.input-group-addon').unbind("focus").unbind("click").on('click', function () {
        var element = $(this).parent('.input-group').find('input');
        element.focus();
    });
};

$(function () {
    if ($('form').not('[class="no-focus"]').find('.focus-first').length > 0) {
        $('form').find('.focus-first').addClass('gotfocus');
        $('form').find('.focus-first').focus();
    }
    else {
        $('form').not('.no-focus').find('input:text:visible:first').addClass('gotfocus');
        $('form').not('.no-focus').find('input:text:visible:first').focus();
    }
});

function initializeCommon() {
    initPopover();
    initPopoverB();
    initialization();
    $('.frm-search input,select').on('keypress', function (e) {
        var key = e.charCode ? e.charCode : e.keyCode ? e.keyCode : 0;
        if (key == 13) {
            $($(this).closest('.frm-search').find('[data-resultElement]')).trigger('click');
            e.preventDefault();
        }
    });

    $('.frm-search select').on('change', function (e) {
        $($(this).closest('.frm-search').attr('data-trigger')).trigger('click');
    });

    $(".disable-me").find('a').removeAttr('href');
    $('.disabled').each(function () {
        $(this).attr('disabled', 'disabled');
    });

    $('.destroy-me').each(function () {
        $(this).remove();
    });

    $(".disable-me").find('a').removeAttr('href');

    $('.disabled').each(function () {
        $(this).attr('disabled', 'disabled');
    });

    initPopover();
};

var initPopover = function () {
    $('.tp-popover, .btnDelete, .btn-delete').on('click', function (e) {
        e.stopPropagation();
    });

    var hasRepositioned = false;
    $('.tp-popover').popover({
        placement: "bottom",
        animation: 'false',
        html: 'true',
        content: function () {
            $this = $(this);
            var contentId = "content-id-" + $.now();
            if ($this.attr('data-url') == undefined) {
                var html = "<span class='text-danger'>Error!!!</span>";
                return '<div id="' + contentId + '">' + html + '</div>';
            };
            var param = $this.attr('data-param');
            $.ajax({
                type: 'post',
                url: $this.attr('data-url'),
                cache: true,
                beforeSend: function () {
                    $('.popover').hide();
                },
            }).done(function (d) {
                $container = $('#' + contentId);
                $container.html(d);
                $title = $container.closest('.popover').find('.popover-title').html($this.attr('data-title'));
            }).error(function () {
                $container = $('#' + contentId);
                var html = "<span class='text-danger'>Error!!!</span>";
                $container.html(html);
            });
            return '<div id="' + contentId + '"> Loading...</div>';
        }
    }).on('shown.bs.popover', function () {
        var $popup = $(this);
        $(this).next('.popover').find('.cancel-popover').click(function (e) {
            $popup.next('.popover').hide();
        });
    });

    var deletePopover = function (ctx) {
        $(ctx).popover({
            placement: 'bottom',
            title: function () {
                return $(this).attr('data-title') == null ? "Confirm to delete..." : $(this).attr('data-title');
            },
            html: 'true',
            content: function () {
                var btnText = $(this).attr('btn-text') == null ? "Delete" : $(this).attr('btn-text');
                var action = $(this).attr('data-url');
                var alertMsg = $(this).attr('data-alert') == null ? 'Deleting an item is forever. There is no undo.' : $(this).attr('data-alert');
                var contentId = "content-id-" + $.now();
                var calback = $(this).attr('data-calback') == null ? '' : $(this).attr('data-calback');
                var html = '<div>';
                html += '<div class="alert alert-danger error-message no-margin" style="display:none;"></div>';
                html += '<div class="alert alert-success success-message no-margin" style="display:none;"></div>';
                html += '<div>';
                html += '<p>' + alertMsg + '</p>';
                html += '<button type="button" data-calback="' + calback + '" onclick="actionData(this)" data-url="' + action + '" class="btn btn-primary btn-sm btn-danger btn-block btn-delete-data">' + btnText + '</button>';
                html += '</div>';
                html += '</div>';
                return '<div id="' + contentId + '">' + html + '</div>';
            }
        });
    };
    //deletePopover('.btn-delete');
    //deletePopover('.btnDelete');

};

var initPopoverB = function () {
    $('.popover-b').popover({
        placement: 'bottom',
        animation: 'false',
        container: "body",
        html: 'true',
        content: function () {
            var contentId = "content-id-" + $.now();
            $this = $(this);
            var param = $this.attr('data-param');
            $.ajax({
                type: 'post',
                url: $this.attr('data-url'),
                cache: true
            }).done(function (d) {
                $container = $('#' + contentId);
                $container.html(d);
                $title = $container.closest('.popover').find('.popover-title').html($this.attr('data-title'));
            }).error(function () {
                $container = $('#' + contentId);
                var html = "<span class='text-danger'>Error!!!</span>";
                $container.html(html);
            });;
            return '<div id="' + contentId + '"> Loading...</div>';
        }
    });
};

var intitializeWorkflow = function () {
    if ($("#workflow-detail").length > 0) {
        refreshWorkflowView();
    }
};

var workflowSubmitTriggered = false;
var refreshWorkflowView = function () {
    var $workflowDetail = $("#workflow-detail");
    $.ajax({
        url: $workflowDetail.attr("data-url"),
        beforeSend: function () {
            var lazyLoadingHtml = '<div class="text-center text-primary"><span class="fa fa-refresh fa-spin" title="System is loading data, please wait..."></span></div>';
            $workflowDetail.html(lazyLoadingHtml);
        },
        success: function (resp) {
            $workflowDetail.html(resp);

            $("#workflowForm").unbind("submit").on("submit", function (e) {
                e.preventDefault();

                if (!workflowSubmitTriggered) {
                    workflowSubmitTriggered = true;
                    var formData = $(this).serialize();
                    var $this = $(this);
                    $.ajax({
                        url: $this.attr("action"),
                        type: "POST",
                        data: formData,
                        beforeSend: function () {
                            $(".loader").show();
                        },
                        complete: function () {
                            $(".loader").hide();
                            workflowSubmitTriggered = false;
                        },
                        success: function (resp) {
                            refreshWorkflowView();
                        }
                    });
                }
            });
        }
    });
};

function setActiveTab(container) {
  var activeTab = $.cookie(container);
    if (activeTab) {
        $('.' + activeTab).parent('li').addClass('active');
        $('#' + activeTab).addClass('active');
    } else {
        var activeElement = $('#tab-container li:first a').attr('class');
        $('#tab-container li:first').addClass('active');
        $('#' + activeElement).addClass('active');
    }
    $('#tab-container').find('li a').click(function () {
        $.cookie(container, $(this).attr('class'));
    });
}
 



