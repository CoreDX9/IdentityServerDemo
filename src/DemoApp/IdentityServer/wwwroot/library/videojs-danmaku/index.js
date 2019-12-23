; (function (window, jQuery) {
    var $ = jQuery;
    var videojs = window.videojs;

    var Component = videojs.getComponent('Component');
    var Danmaku = videojs.extend(Component, {
        constructor: function (player, options) {
            Component.apply(this, arguments);

            player.on('ready', function () {
                options.commentManager.init();
                for (var provider of options.commentProviders) {
                    provider.addTarget(options.commentManager);
                    provider.load()
                        .then(function () {
                            videojs.log('弹幕已载入');
                        })
                        .catch(function (e) {
                            videojs.log('弹幕载入出错');
                            console.error(e);
                        });
                }
            });

            player.on('play', function () {
                options.commentManager.start();
            });

            player.on('pause', function () {
                options.commentManager.stop();
            });

            player.on('timeupdate', function () {
                //这里是关键，此事件会随着播放持续发生，在此处更新弹幕管理器当前时间，让后续弹幕能正常进入
                options.commentManager.time(player.currentTime() * 1000);
            });

            player.on('playerresize', function () {
                $(options.commentManager.stage).width(player.currentWidth());
                $(options.commentManager.stage).height(player.currentHeight() - 30);
                options.commentManager.setBounds();
            });
        },

        createEl: function () {
            var el = $(
                `<div class="vjs-danmaku abp" style="display:contents;">
                       <div class="vjs-danmaku-stage abp-container" style="pointer-events: none;"></div>
                     </div>`);
            this.options().commentManager.stage = $('div', el)[0];
            return el[0];
        },
    });

    // Register the component with Video.js, so it can be used in players.
    videojs.registerComponent('Danmaku', Danmaku);
})(window, jQuery);