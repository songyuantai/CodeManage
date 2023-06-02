/**
 * layer封装工具
 * */
window.Popup = (function (_ex) {
    /**
     * 以内嵌iframe的方式打开模态对话框
     * @param {string} title 标题
     * @param {string} src iframe地址
     * @param {string} height 高度
     * @param {Function} callback 
     * @param {string} isLarge 是否使用大窗口，默认为true 
     */
    _ex.open = function (title, src, height, callback, isLarge) {
        var largeClass = 'modal-lg';
        if (isLarge === false) {
            largeClass = '';
        }
        var $dialog = $('<div id="dialog" tabindex="-1" role="dialog" aria-labelledby="modalLabel" aria-hidden="true" class="modal fade text-left">'
            + '<div role="document" class="modal-dialog ' + largeClass + '">'
            + '<div class="modal-content">'
            + '<div class="modal-header">'
            + '<h4 id="modalLabel" class="modal-title">' + title + '</h4>'
            + '<button type="button" data-dismiss="modal" aria-label="Close" class="close"><span aria-hidden="true" class="fa fa-close"></span></button></div>'
            + '<div class="modal-body"><iframe src="' + src + '" style="width:100%;height:' + height + ';border-width:0"></iframe>'
            + '</div></div></div></div>');
        $dialog.appendTo('body');

        $dialog.on('hidden.bs.modal', function () {
            if (callback) callback();
            $dialog.remove();
        })

        $dialog.on('show.bs.modal', vCenterModal);

        $(window).on('resize', vCenterModal);

        $dialog.modal('show');

        function vCenterModal() {
            $('#dialog').each(function (i) {
                var $clone = $(this).clone().css('display', 'block').appendTo('body');
                var top = Math.round(($clone.height() - $clone.find('.modal-content').height()) / 2);
                top = top > 0 ? top : 0;
                $clone.remove();
                $(this).find('.modal-content').css("margin-top", top);
            });
        };
    }

    /**
     * 模态框iframe跳转页面
     * @param {string} url 新页面
     * @param {string} title 新标题
     */
    _ex.fetch = function (url, title) {
        $('#dialog .modal-title').text(title);
        $('#dialog iframe').attr('src', url);
    }

    /**
     * 关闭模态对话框
     */
    _ex.close = function () {
        $('#dialog').modal('hide');
    }

    /**
     * bootstrap弹窗
     * @param {string} selector
     * @param {string} type
     * @param {String} message
     */
    _ex.alert = function (selector, type, message) {
        $(selector).empty();
        $(selector).append($('<div class="alert alert-' + type + '">' + message + '</div>'));
    }

    //正在加载中
    _ex.loading = function () {
        return layer.load(0, { shade: 0.1 });
    }

    //加载成功
    _ex.loaded = function (index) {
        if (!!index)
            layer.close(index);
        else
            layer.closeAll('loading');
    }

    //成功
    _ex.success = function (message, then) {
        layer.msg(message, { icon: 1, time: 2000 }, then);
    }

    //弹窗提示
    _ex.info = function (message, then) {
        layer.alert(message, { icon: 1 }, then);
    }

    //警告
    _ex.warning = function (message, then) {
        layer.alert(message, { icon: 0 }, then);
    }

    //错误
    _ex.error = function (message, then) {
        layer.alert(message, { icon: 2 }, then);
    }

    //打开窗口
    _ex.openWin = function (title, src, width, height, callback) {
        var w = width || ($(window).width() - 100 + 'px');
        var h = height || ($(window).height() - 50 + 'px');
        return layer.open({
            type: 2,
            title: title,
            shade: .5,
            shadeClose: true,
            area: [w, h],
            content: src,
            end: callback
        });
    }

    //显示html
    _ex.display = function (title, html, width, height, callback) {
        var w = width || ($(window).width() - 100 + 'px');
        var h = height || ($(window).height() - 50 + 'px');
        return layer.open({
            type: 1,
            title: title,
            shade: .5,
            shadeClose: true,
            area: [w, h],
            content: html,
            end: callback
        });
    }

    //关闭窗口
    _ex.closeWin = function (index) {
        if (!!index)
            layer.close(index);
        else
            layer.closeAll();
    }

    //确认消息
    _ex.confirm = function (message, callback) {
        parent.layer.confirm(message,{
            btn: ['确定', '取消']
        }, callback);
    }

    return _ex;
})(window.LayerUtil || {});