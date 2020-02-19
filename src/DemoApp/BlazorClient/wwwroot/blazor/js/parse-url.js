; (function (window) {
    window.parseUrl = function (url) {
        if (!url) url = window.location;

        //创建一个a标签
        var a = document.createElement('a');
        //将url赋值给标签的href属性。
        a.href = url;
        var result = {
            source: url,
            protocol: a.protocol.replace(':', ''), //协议
            host: a.hostname, //主机名称
            port: a.port, //端口
            queryString: a.search, //查询字符串
            queryParams: (function() { //查询参数
                var ret = {},
                    seg = a.search.replace(/^\?/, '').split('&'),
                    len = seg.length,
                    i = 0,
                    s;
                for (; i < len; i++) {
                    if (!seg[i]) {
                        continue;
                    }
                    s = seg[i].split('=');
                    ret[s[0]] = s[1];
                }
                return ret;
            })(),
            file: (a.pathname.match(/\/([^\/?#]+)$/i) || [null, ''])[1], //文件名
            hash: a.hash.replace('#', ''), //哈希参数
            path: a.pathname.replace(/^([^\/])/, '/$1'), //路径
            relative: (a.href.match(/tps?:\/\/[^\/]+(.+)/) || [null, ''])[1], //相对路径
            segments: a.pathname.replace(/^\//, '').split('/') //路径片段
        };
        var isDefaultPort = ((result.protocol === 'http' && (result.port === '80' || result.port === '')) ||
            (result.protocol === 'https' && (result.port === '443' || result.port === '')));
        result.isDefaultPort = isDefaultPort;
        result.link =
            result.protocol +
            '://' +
            result.host +
            (isDefaultPort ? '' : ':' + result.port) +
            result.path +
            result.queryString +
            result.hash;

        return result;
    };
})(window);