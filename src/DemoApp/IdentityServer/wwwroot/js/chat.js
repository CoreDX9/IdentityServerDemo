"use strict";

$(document).ready(function() {
    var hubConnection = new window.signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .withAutomaticReconnect({
            nextRetryDelayInMilliseconds: retryContext => {
                //retryContext.previousRetryCount
                //retryContext.elapsedMilliseconds
                if (retryContext.previousRetryCount <= 3) {
                    var sen = 5;
                    if (retryContext.previousRetryCount === 0) {
                        $('#connState').text('连接中断，将在' + sen + '秒后重连……');
                    }
                    else {
                        $('#connState').text('第' + retryContext.previousRetryCount + '次重新连接失败，将在' + sen + '秒后重试……');
                    }
                    console.error(retryContext.retryReason);
                    return sen * 1000;
                } else {
                    $('#connState').text('已' + retryContext.previousRetryCount + '次重新连接失败，请检查网络状况。点击上线按钮重新连接');
                    console.error(retryContext.retryReason);
                    return null;
                }
            }
        })
        .configureLogging(window.signalR.LogLevel.Information)
        .build();

    hubConnection.on("ReceiveMessage",
        function(user, message) {
            var msg;
            if (user === $('#user').val()) {
                msg = $('#messageBubbleTemplate .chat-self').eq(0).clone();
            } else {
                msg = $('#messageBubbleTemplate .chat-other').eq(0).clone();
            }
            $('.name', msg).text(user);
            $('.logo img', msg).attr('src', '/images/userLogo/' + user + '.png');
            $('.content', msg).text(message);

            var el = $('.chat-board');
            var nsc = isAtBottom(el);

            //追加消息并重新计算消息框大小
            el.append(msg);
            chatContentResize($('.content', msg));

            //如果需要滚动，重新计算滚动位置并设置滚动动画
            if (nsc) {
                scrollToBottom(el);
            }
        });

    hubConnection.on("ReceiveOnlineNotice",
        function(user) {
            var msg = $('#messageBubbleTemplate .chat-notice').eq(0).clone();
            $('span', msg).text('用户 ' + user + ' 上线');

            var el = $('.chat-board');
            var nsc = isAtBottom(el);

            el.append(msg);
            chatContentResize($('.content', msg));

            if (nsc) {
                scrollToBottom(el);
            }
        });

    hubConnection.on("ReceiveOfflineNotice",
        function (user) {
            var msg = $('#messageBubbleTemplate .chat-notice').eq(0).clone();
            $('span', msg).text('用户 ' + user + ' 下线');

            var el = $('.chat-board');
            var nsc = isAtBottom(el);

            el.append(msg);
            chatContentResize($('.content', msg));

            if (nsc) {
                scrollToBottom(el);
            }
        });

    hubConnection.on("ReceiveClearChatBoardCommand",
        function(user) {
            $('.chat-board').empty();

            var msg = $('#messageBubbleTemplate .chat-notice').eq(0).clone();
            $('span', msg).text('用户 ' + user + ' 清空了所有人的聊天版');

            var el = $('.chat-board');
            var nsc = isAtBottom(el);

            el.append(msg);
            chatContentResize($('.content', msg));

            if (nsc) {
                scrollToBottom(el);
            }
        });

    connect(hubConnection);

    hubConnection.onreconnecting((error) => {
        $('#connState').text('正在重新连接……');
    });

    hubConnection.onreconnected((connectionId) => {
        $('#connState').text('已连接');
    });

    hubConnection.onclose(function (err) {
        $('#connState').text('连接已关闭');
        console.log('连接已关闭');
        if (err) {
            console.log(err);
        }
    });

    function sendEvent(event) {
        var message = document.getElementById("messageInput").value;
        if ((/^\s*$/).test(message)) {
            return;
        }
        hubConnection.invoke("SendMessage", message)
            .then(function() {
                $('#messageInput').val('');
            })
            .catch(function (err) {
            return console.log(err.toString());
        });
        event.preventDefault();
    }

    document.getElementById("send").addEventListener("click", sendEvent);

    $('#offline').click(function () {
        if (hubConnection.connection.connectionState !== window.signalR.HubConnectionState.Disconnected) {
            hubConnection.stop();
        }
    });

    $('#online').click(function () {
        if (hubConnection.connection.connectionState === window.signalR.HubConnectionState.Disconnected) {
            connect(hubConnection);
        }
    });

    $('#clearChatBoard').click(function() {
        $.ajax({
            url: '/SignalR/Chat/Index?handler=ClearOnlineUsersChatBoard',
            error: function(xhr, err, exception) {
                alert('清空失败');
            }
        });
    });

    $(document).keypress(function(e) {
        if (e.ctrlKey && e.keyCode === 10)
            sendEvent(e);
    });

    $(window).resize(function () { elementResize('.chat-board');  });

    $('.chat-board').resize(function () {
        var el = $('.chat-self .content, .chat-other .content');
        el.each(function () {
            chatContentResize($(this));
        });
    });

    elementResize('.chat-board');
});

function connect(hubConnection) {
    $('#connState').text('正在连接……');
    hubConnection.start()
        .then(function () {
            $('#connState').text('已连接');
            console.log('连接成功');
        })
        .catch(function (err) {
            $('#connState').text('连接失败，请检查网络状况');
            console.log(err);
        });
}

function chatContentResize(jq) {
    jq.css('display', 'inline');
    jq.parent().css('width', 'auto');
    if ($('.chat-board').width() * 0.75 > jq.width()) {
        jq.parent().width(jq.width() + 15);
    }
    jq.css('display', 'block');
}

function isAtBottom(jqEl) {
    var el0 = jqEl[0];
    var scrollTop = el0.scrollTop;//页面上卷的高度
    var wholeHeight = el0.scrollHeight;//页面底部到顶部的距离
    var pHeight = el0.clientHeight;//页面可视区域的高度

    //检查如果滚到底了，新消息到来的时候自动滚到底（此处设置检查结果，是否需要滚动）
    if (scrollTop + pHeight >= wholeHeight) {
        return  true;
    }

    return false;
    //if (scrollTop === 0) {
    //    alert('我到顶部了');
    //}
}

function scrollToBottom(jqEl) {
    var el0 = jqEl[0];
    var scrollTop = el0.scrollTop;
    var pHeight = el0.clientHeight;
    jqEl.animate({ scrollTop: scrollTop + pHeight }, 500);
}