//页面标签大小改变事件扩展插件，JQuery内建只支持window大小改变
;(function ($, window, undefined) {
    var elems = $([]),
        jqResize = $.resize = $.extend($.resize, {}),
        timeoutId,
        strSetTimeout = 'setTimeout',
        strResize = 'resize',
        strData = strResize + '-special-event',
        strDelay = 'delay',
        strThrottle = 'throttleWindow';
    jqResize[strDelay] = 250;
    jqResize[strThrottle] = true;
    $.event.special[strResize] = {
        setup: function () {
            if (!jqResize[strThrottle] && this[strSetTimeout]) {
                return false;
            }
            var elem = $(this);
            elems = elems.add(elem);
            $.data(this, strData, {
                w: elem.width(),
                h: elem.height()
            });
            if (elems.length === 1) {
                loopy();
            }

            return null;
        },
        teardown: function () {
            if (!jqResize[strThrottle] && this[strSetTimeout]) {
                return false;
            }
            var elem = $(this);
            elems = elems.not(elem);
            elem.removeData(strData);
            if (!elems.length) {
                clearTimeout(timeoutId);
            }

            return null;
        },
        add: function (handleObj) {
            if (!jqResize[strThrottle] && this[strSetTimeout]) {
                return false;
            }
            var oldHandler;

            function newHandler(e, w, h) {
                var elem = $(this),
                    data = $.data(this, strData);
                data.w = w !== undefined ? w : elem.width();
                data.h = h !== undefined ? h : elem.height();
                oldHandler.apply(this, arguments);
            }

            if ($.isFunction(handleObj)) {
                oldHandler = handleObj;
                return newHandler;
            } else {
                oldHandler = handleObj.handler;
                handleObj.handler = newHandler;
            }

            return null;
        }
    };

    function loopy() {
        timeoutId = window[strSetTimeout](function () {
            elems.each(function () {
                var elem = $(this),
                    width = elem.width(),
                    height = elem.height(),
                    data = $.data(this, strData);
                if (width !== data.w || height !== data.h) {
                    elem.trigger(strResize, [data.w = width, data.h = height]);
                }
            });
            loopy();
        }, jqResize[strDelay]);
    }
})(jQuery, window);