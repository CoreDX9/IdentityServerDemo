本文章默认读者了解Openssl,CA,网站证书相关知识，直接实战！配置完成后，浏览器会显示“安全的HTTPS”连接。不会像其他文章那样，是红色警告的证书提示。

准备环境
笔者使用的是Ubuntu16 ，其实什么系统都无所谓，请不要使用旧版Openssl就好，因为里面的漏洞实在太致命。

先创建一个文件夹
```
mkdir openssl.Test

cd openssl.Test

mkdir -p ./demoCA/{private,newcerts} 

touch demoCA/index.txt 

touch demoCA/serial

echo 01 > demoCA/serial

复制一份配置文件，可能会因系统不一致

cp /etc/ssl/openssl.cnf .
```

自建CA

1 设置私钥
```
openssl genrsa -des3 -out ./demoCA/private/cakey.pem 2048
```
2 申请自签名证书

根据提示一步步填写就好，但要记清楚填了什么，后面会用到
```
openssl req -new -x509 -days 365 -key ./demoCA/private/cakey.pem -out ./demoCA/cacert.pem
```
调整openssl.cnf

调整openssl.cnf 用来支持v3扩展，以实现多域名及扩展用途（EKU）

调整原因有二，一是让服务端证书支持多个二级域名，二是符合Chrome的证书检查，否则使用Chrome浏览时会提示不信任此证书

用自己喜欢的编辑器打开openssl.cnf

确保req下存在以下2行（默认第一行是有的，第2行被注释了）
```
[ req ]
distinguished_name = req_distinguished_name
req_extensions = v3_req
```
新增最后一行内容 subjectAltName = @alt_names（前2行默认存在）
```
[ v3_req ]
# Extensions to add to a certificate request
basicConstraints = CA:FALSE
keyUsage = nonRepudiation, digitalSignature, keyEncipherment
#设置扩展用途 （EKU） 可选
#extendedKeyUsage = serverAuth,clientAuth
subjectAltName = @alt_names
```
新增 alt_names,注意括号前后的空格，DNS.x 的数量可以自己加,别忘了把申请证书时填写的域名(Common Name)加上
```
[ alt_names ]
IP.1 = 127.0.0.1
DNS.1 = abc.example.com
DNS.2 = dfe.example.org
DNS.3 = ex.abcexpale.net
```
创建服务端证书
1 设置私钥
```
openssl genrsa -out userkey.pem 2048
```
2 创建证书申请 
提示 Common Name 时 请填写域名，不要设置密码，否则Nginx部分会有麻烦，国家，省份，城市，公司，部门要和创建CA填写的资料一致，当然在配置文件里调整策略能绕过此检查。本文尽量使用默认的配置来完成~~~，所以请保持一致。
```
openssl req -new -days 365 -key userkey.pem -out userreq.pem
```
3 签发
```
openssl ca -in userreq.pem -out usercert.pem -extensions v3_req -config openssl.cnf
```
4 查看证书
```
openssl x509 -in usercert.pem -text -noout
```
创建客户端证书
与服务端类似
```
openssl genrsa -out clientkey.pem 2048
# Common name 随意写，设不设置密码随意
openssl req -new -days 365 -key clientkey.pem -out clientreq.pem
openssl ca -in clientreq.pem -out clientcert.pem -extensions v3_req -config openssl.cnf
```
转换下格式，方便windows下导入
```
openssl pkcs12 -export -inkey clientkey.pem -in clientcert.pem -out client.pfx
```

在浏览器端导入证书即可，忽略这句话的朋友，通常会遇到400的问题，无法自拔！

--------------------- 
作者：生活在底层的低级码农 
来源：CSDN 
原文：https://blog.csdn.net/hatmen2/article/details/81265593 
版权声明：本文为博主原创文章，转载请附上博文链接！