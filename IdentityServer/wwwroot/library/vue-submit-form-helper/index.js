;(function(window) {
    if (!window.Vue || !window.Qs || !window.axios) {
        console.error("必要的依赖不足。需要先引用vuejs、qs和axios");
    } else {
        var Vue = window.Vue;
        var Qs = window.Qs;
        var Axios = window.axios;
        //表单提交助手，提供对 .Net Core 反请求伪造令牌的申请和发送的自动支持
        Vue.prototype.$submitFormHelper = (function () {
            var headerName = null;
            var requestToken = null;
            var axios = Axios.create();

            var getAntiforgeryToken = function () {
                return new Promise(function (resolve, reject) {
                    axios.post('/Manage/Home/GetAntiXsrfRequestToken')
                        .then(function (response) {
                            if (headerName !== response.data.headerName) {
                                headerName = response.data.headerName;
                            }
                            requestToken = response.data.requestToken;

                            resolve({
                                headerName: response.data.headerName,
                                requestToken: response.data.requestToken
                            });
                        })
                        .catch(function (error) {
                            reject(error);
                        });
                });
            };

            var clearAntiforgeryToken = function () {
                headerName = null;
                requestToken = null;
            };

            var submit = function (url, method, data) {
                var headers = {
                    'Content-Type': 'application/x-www-form-urlencoded'
                };
                headers[headerName] = requestToken;
                requestToken = null;

                //请求新的反请求伪造令牌，request token在响应头中提供；cookie token由浏览器自动设置在cookie中，在同一个域共享，可能导致不同页面之间的冲突，所以提交助手支持自动刷新token并重试提交
                headers['X-REQUEST-CSRF-TOKEN'] = 'true';

                return new Promise(function (resolve, reject) {
                    axios({
                        method: method,
                        url: url,
                        data: Qs.stringify(data, { allowDots: true, arrayFormat: 'indices' }),
                        headers: headers
                    })
                        .then(function (response) {
                            if (response.headers['x-response-csrf-token-name']) {
                                headerName = response.headers['x-response-csrf-token-name'];
                            }

                            if (response.headers['x-response-csrf-token-value']) {
                                requestToken = response.headers['x-response-csrf-token-value'];
                            }

                            resolve(response);
                        })
                        .catch(function (error) {
                            reject(error);
                        });
                });
            };

            var trySubmit = function (url, method, data) {
                return new Promise(function (resolve, reject) {
                    submit(url, method, data)
                        .then(function (response) {
                            resolve(response);
                        })
                        .catch(function (error) {
                            if (error.response.status === 400) {
                                getAntiforgeryToken()
                                    .then(function () {
                                        submit(url, method, data)
                                            .then(function (response) {
                                                resolve(response);
                                            })
                                            .catch(function (errorIn) {
                                                reject({ error, errorIn });
                                            });
                                    })
                                    .catch(function (errorIn) {
                                        reject({ error, errorIn });
                                    });
                            } else {
                                reject(error);
                            }
                        });
                });
            };

            var submitForm = function (url, method, data) {
                return new Promise(function (resolve, reject) {
                    if (requestToken === null || headerName === null) {
                        getAntiforgeryToken()
                            .then(function () {
                                trySubmit(url, method, data)
                                    .then(function (response) {
                                        resolve(response);
                                    })
                                    .catch(function (error) {
                                        reject(error);
                                    });
                            })
                            .catch(function (error) {
                                reject(error);
                            });
                    } else {
                        trySubmit(url, method, data)
                            .then(function (response) {
                                resolve(response);
                            })
                            .catch(function (error) {
                                reject(error);
                            });
                    }
                });
            }

            return {
                submitForm: submitForm,
                getAntiforgeryToken: getAntiforgeryToken,
                clearAntiforgeryToken: clearAntiforgeryToken
            };
        })();
    }
})(window);