using CoreDX.Common.Util.TypeExtensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CoreDX.Common.Util.Security
{
    public class AesProtector
    {
        private readonly SymmetricAlgorithm _aes;

        public AesProtector()
        {
            _aes = Rijndael.Create();
        }

        public AesProtector(byte[] key, byte[] iv, int blockSize, int keySize, int feedbackSize,
            PaddingMode paddingMode, CipherMode mode)
        {
            _aes = Rijndael.Create();
            _aes.BlockSize = blockSize;
            _aes.KeySize = keySize;
            _aes.FeedbackSize = feedbackSize;
            _aes.Padding = paddingMode;
            _aes.Mode = mode;

            //Key和IV要在上面的参数设置完成完成后在设置，否则可能会被内部重新生成的值覆盖
            _aes.Key = key;
            _aes.IV = iv;
        }

        public byte[] Protect(byte[] normal)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                CryptoStream cs = new CryptoStream(ms, _aes.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(normal, 0, normal.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
        }

        public byte[] Unprotect(byte[] secret)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                CryptoStream cs = new CryptoStream(ms, _aes.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(secret, 0, secret.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
        }

        public byte[] GenerateKey()
        {
            //备份信息
            var key = _aes.Key;
            var iv = _aes.IV;
            //生成
            _aes.GenerateKey();
            //获取结果
            var res = _aes.Key;
            //还原信息
            _aes.Key = key;
            _aes.IV = iv;

            return res;
        }

        public byte[] GenerateIV()
        {
            //备份信息
            var key = _aes.Key;
            var iv = _aes.IV;
            //生成
            _aes.GenerateIV();
            //获取结果
            var res = _aes.IV;
            //还原信息
            _aes.Key = key;
            _aes.IV = iv;

            return res;
        }
    }

    public class MD5
    {
        public string Md5Encrypt(string normalTxt)
        {
            var bytes = Encoding.Default.GetBytes(normalTxt);//求Byte[]数组
            var md5 = new MD5CryptoServiceProvider().ComputeHash(bytes);//求哈希值
            return Convert.ToBase64String(md5);//将Byte[]数组转为净荷明文(其实就是字符串)
        }
    }


    public static class EncryptUtil
    {
        #region MD5加密

        ///// <summary>
        ///// MD5加密
        ///// </summary>
        //public static string Md532(this string value)
        //{
        //    value = value ?? throw new ArgumentNullException();

        //    var encoding = Encoding.UTF8;
        //    MD5 md5 = MD5.Create();
        //    return HashAlgorithmBase(md5, value, encoding);
        //}

        ///// <summary>
        ///// 加权MD5加密
        ///// </summary>
        //public static string Md532(this string value, string salt)
        //{
        //    return salt == null ? value.Md532() : (value + "『" + salt + "』").Md532();
        //}

        #endregion

        #region SHA 加密

        /// <summary>
        /// SHA1 加密
        /// </summary>
        public static string Sha1(this string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("未将对象引用设置到对象的实例。");
            }

            var encoding = Encoding.UTF8;
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            return HashAlgorithmBase(sha1, value, encoding);
        }

        //
        // 摘要:
        //     /// Creates a SHA256 hash of the specified input. ///
        //
        // 参数:
        //   input:
        //     The input.
        //
        // 返回结果:
        //     A hash
        public static string Sha256(this string input)
        {
            if (input.IsNullOrEmpty())
            {
                return string.Empty;
            }
            using (SHA256 sHA = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                return Convert.ToBase64String(sHA.ComputeHash(bytes));
            }
        }

        //
        // 摘要:
        //     /// Creates a SHA256 hash of the specified input. ///
        //
        // 参数:
        //   input:
        //     The input.
        //
        // 返回结果:
        //     A hash.
        public static byte[] Sha256(this byte[] input)
        {
            if (input == null)
            {
                return null;
            }
            using (SHA256 sHA = SHA256.Create())
            {
                return sHA.ComputeHash(input);
            }
        }

        //
        // 摘要:
        //     /// Creates a SHA512 hash of the specified input. ///
        //
        // 参数:
        //   input:
        //     The input.
        //
        // 返回结果:
        //     A hash
        public static string Sha512(this string input)
        {
            if (input.IsNullOrEmpty())
            {
                return string.Empty;
            }
            using (SHA512 sHA = SHA512.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                return Convert.ToBase64String(sHA.ComputeHash(bytes));
            }
        }

        #endregion

        #region HMAC 加密

        /// <summary>
        /// HmacSha1 加密
        /// </summary>
        public static string HmacSha1(this string value, string keyVal)
        {
            if (value == null)
            {
                throw new ArgumentNullException("未将对象引用设置到对象的实例。");
            }
            var encoding = Encoding.UTF8;
            byte[] keyStr = encoding.GetBytes(keyVal);
            HMACSHA1 hmacSha1 = new HMACSHA1(keyStr);
            return HashAlgorithmBase(hmacSha1, value, encoding);
        }

        /// <summary>
        /// HmacSha256 加密
        /// </summary>
        public static string HmacSha256(this string value, string keyVal)
        {
            if (value == null)
            {
                throw new ArgumentNullException("未将对象引用设置到对象的实例。");
            }
            var encoding = Encoding.UTF8;
            byte[] keyStr = encoding.GetBytes(keyVal);
            HMACSHA256 hmacSha256 = new HMACSHA256(keyStr);
            return HashAlgorithmBase(hmacSha256, value, encoding);
        }

        /// <summary>
        /// HmacSha384 加密
        /// </summary>
        public static string HmacSha384(this string value, string keyVal)
        {
            if (value == null)
            {
                throw new ArgumentNullException("未将对象引用设置到对象的实例。");
            }
            var encoding = Encoding.UTF8;
            byte[] keyStr = encoding.GetBytes(keyVal);
            HMACSHA384 hmacSha384 = new HMACSHA384(keyStr);
            return HashAlgorithmBase(hmacSha384, value, encoding);
        }

        /// <summary>
        /// HmacSha512 加密
        /// </summary>
        public static string HmacSha512(this string value, string keyVal)
        {
            if (value == null)
            {
                throw new ArgumentNullException("未将对象引用设置到对象的实例。");
            }
            var encoding = Encoding.UTF8;
            byte[] keyStr = encoding.GetBytes(keyVal);
            HMACSHA512 hmacSha512 = new HMACSHA512(keyStr);
            return HashAlgorithmBase(hmacSha512, value, encoding);
        }

        /// <summary>
        /// HmacMd5 加密
        /// </summary>
        public static string HmacMd5(this string value, string keyVal)
        {
            if (value == null)
            {
                throw new ArgumentNullException("未将对象引用设置到对象的实例。");
            }
            var encoding = Encoding.UTF8;
            byte[] keyStr = encoding.GetBytes(keyVal);
            HMACMD5 hmacMd5 = new HMACMD5(keyStr);
            return HashAlgorithmBase(hmacMd5, value, encoding);
        }

        ///// <summary>
        ///// HmacRipeMd160 加密
        ///// </summary>
        //public static string HmacRipeMd160(this string value, string keyVal)
        //{
        //    if (value == null)
        //    {
        //        throw new ArgumentNullException("未将对象引用设置到对象的实例。");
        //    }
        //    var encoding = Encoding.UTF8;
        //    byte[] keyStr = encoding.GetBytes(keyVal);
        //    HMACRIPEMD160 hmacRipeMd160 = new HMACRIPEMD160(keyStr);
        //    return HashAlgorithmBase(hmacRipeMd160, value, encoding);
        //}

        #endregion

        #region DES 加密解密

        /// <summary>
        /// DES 加密
        /// </summary>
        public static string Des(this string value, string keyVal, string ivVal)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(value);
                var des = new DESCryptoServiceProvider { Key = Encoding.ASCII.GetBytes(keyVal.Length > 8 ? keyVal.Substring(0, 8) : keyVal), IV = Encoding.ASCII.GetBytes(ivVal.Length > 8 ? ivVal.Substring(0, 8) : ivVal) };
                var desencrypt = des.CreateEncryptor();
                byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
                return BitConverter.ToString(result);
            }
            catch { return "转换出错！"; }
        }

        /// <summary>
        /// DES 解密
        /// </summary>
        public static string UnDes(this string value, string keyVal, string ivVal)
        {
            try
            {
                string[] sInput = value.Split("-".ToCharArray());
                byte[] data = new byte[sInput.Length];
                for (int i = 0; i < sInput.Length; i++)
                {
                    data[i] = byte.Parse(sInput[i], NumberStyles.HexNumber);
                }
                var des = new DESCryptoServiceProvider { Key = Encoding.ASCII.GetBytes(keyVal.Length > 8 ? keyVal.Substring(0, 8) : keyVal), IV = Encoding.ASCII.GetBytes(ivVal.Length > 8 ? ivVal.Substring(0, 8) : ivVal) };
                var desencrypt = des.CreateDecryptor();
                byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
                return Encoding.UTF8.GetString(result);
            }
            catch { return "解密出错！"; }
        }

        #endregion

        #region BASE64 加密解密

        /// <summary>
        /// BASE64 加密
        /// </summary>
        /// <param name="value">待加密字段</param>
        /// <returns></returns>
        public static string Base64(this string value)
        {
            var btArray = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(btArray, 0, btArray.Length);
        }

        /// <summary>
        /// BASE64 解密
        /// </summary>
        /// <param name="value">待解密字段</param>
        /// <returns></returns>
        public static string UnBase64(this string value)
        {
            var btArray = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(btArray);
        }

        #endregion

        #region Base64加密解密
        /// <summary>
        /// Base64加密 可逆
        /// </summary>
        /// <param name="value">待加密文本</param>
        /// <returns></returns>
        public static string Base64Encrypt(string value)
        {
            return Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(value));
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="ciphervalue">密文</param>
        /// <returns></returns>
        public static string Base64Decrypt(string ciphervalue)
        {
            return System.Text.Encoding.Default.GetString(System.Convert.FromBase64String(ciphervalue));
        }
        #endregion

        #region 内部方法

        /// <summary>
        /// 转成数组
        /// </summary>
        private static byte[] Str2Bytes(this string source)
        {
            source = source.Replace(" ", "");
            byte[] buffer = new byte[source.Length / 2];
            for (int i = 0; i < source.Length; i += 2) buffer[i / 2] = Convert.ToByte(source.Substring(i, 2), 16);
            return buffer;
        }

        /// <summary>
        /// 转换成字符串
        /// </summary>
        private static string Bytes2Str(this IEnumerable<byte> source, string formatStr = "{0:X2}")
        {
            StringBuilder pwd = new StringBuilder();
            foreach (byte btStr in source) { pwd.AppendFormat(formatStr, btStr); }
            return pwd.ToString();
        }

        private static byte[] FormatByte(this string strVal, Encoding encoding)
        {
            return encoding.GetBytes(strVal.Base64().Substring(0, 16).ToUpper());
        }

        /// <summary>
        /// HashAlgorithm 加密统一方法
        /// </summary>
        private static string HashAlgorithmBase(HashAlgorithm hashAlgorithmObj, string source, Encoding encoding)
        {
            byte[] btStr = encoding.GetBytes(source);
            byte[] hashStr = hashAlgorithmObj.ComputeHash(btStr);
            return hashStr.Bytes2Str();
        }

        #endregion

        /// <summary>
        /// RSA加解密 使用OpenSSL的公钥加密/私钥解密
        /// 
        /// 公私钥请使用openssl生成  ssh-keygen -t rsa 命令生成的公钥私钥是不行的
        /// 
        /// 作者：李志强
        /// 时间：2017年10月30日15:50:14
        /// QQ:501232752
        /// </summary>
        public class RSAHelper
        {
            private readonly RSA _privateKeyRsaProvider;
            private readonly RSA _publicKeyRsaProvider;
            private readonly HashAlgorithmName _hashAlgorithmName;
            private readonly Encoding _encoding;

            /// <summary>
            /// 实例化RSAHelper
            /// </summary>
            /// <param name="rsaType">加密算法类型 RSA SHA1;RSA2 SHA256 密钥长度至少为2048</param>
            /// <param name="encoding">编码类型</param>
            /// <param name="privateKey">私钥</param>
            /// <param name="publicKey">公钥</param>
            public RSAHelper(RSAType rsaType, Encoding encoding, string privateKey, string publicKey = null)
            {
                _encoding = encoding;
                if (!string.IsNullOrEmpty(privateKey))
                {
                    _privateKeyRsaProvider = CreateRsaProviderFromPrivateKey(privateKey);
                }

                if (!string.IsNullOrEmpty(publicKey))
                {
                    _publicKeyRsaProvider = CreateRsaProviderFromPublicKey(publicKey);
                }

                _hashAlgorithmName = rsaType == RSAType.RSA ? HashAlgorithmName.SHA1 : HashAlgorithmName.SHA256;
            }

            #region 使用私钥签名

            /// <summary>
            /// 使用私钥签名
            /// </summary>
            /// <param name="data">原始数据</param>
            /// <returns></returns>
            public string Sign(string data)
            {
                byte[] dataBytes = _encoding.GetBytes(data);

                var signatureBytes = _privateKeyRsaProvider.SignData(dataBytes, _hashAlgorithmName, RSASignaturePadding.Pkcs1);

                return Convert.ToBase64String(signatureBytes);
            }

            #endregion

            #region 使用公钥验证签名

            /// <summary>
            /// 使用公钥验证签名
            /// </summary>
            /// <param name="data">原始数据</param>
            /// <param name="sign">签名</param>
            /// <returns></returns>
            public bool Verify(string data, string sign)
            {
                byte[] dataBytes = _encoding.GetBytes(data);
                byte[] signBytes = Convert.FromBase64String(sign);

                var verify = _publicKeyRsaProvider.VerifyData(dataBytes, signBytes, _hashAlgorithmName, RSASignaturePadding.Pkcs1);

                return verify;
            }

            #endregion

            #region 解密

            public string Decrypt(string cipherText)
            {
                if (_privateKeyRsaProvider == null)
                {
                    throw new Exception("_privateKeyRsaProvider is null");
                }
                return Encoding.UTF8.GetString(_privateKeyRsaProvider.Decrypt(Convert.FromBase64String(cipherText), RSAEncryptionPadding.Pkcs1));
            }

            #endregion

            #region 加密

            public string Encrypt(string text)
            {
                if (_publicKeyRsaProvider == null)
                {
                    throw new Exception("_publicKeyRsaProvider is null");
                }
                return Convert.ToBase64String(_publicKeyRsaProvider.Encrypt(Encoding.UTF8.GetBytes(text), RSAEncryptionPadding.Pkcs1));
            }

            #endregion

            #region 使用私钥创建RSA实例

            public RSA CreateRsaProviderFromPrivateKey(string privateKey)
            {
                var privateKeyBits = Convert.FromBase64String(privateKey);

                var rsa = RSA.Create();
                var rsaParameters = new RSAParameters();

                using (BinaryReader binr = new BinaryReader(new MemoryStream(privateKeyBits)))
                {
                    byte bt = 0;
                    ushort twobytes = 0;
                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130)
                        binr.ReadByte();
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();
                    else
                        throw new Exception("Unexpected value read binr.ReadUInt16()");

                    twobytes = binr.ReadUInt16();
                    if (twobytes != 0x0102)
                        throw new Exception("Unexpected version");

                    bt = binr.ReadByte();
                    if (bt != 0x00)
                        throw new Exception("Unexpected value read binr.ReadByte()");

                    rsaParameters.Modulus = binr.ReadBytes(GetIntegerSize(binr));
                    rsaParameters.Exponent = binr.ReadBytes(GetIntegerSize(binr));
                    rsaParameters.D = binr.ReadBytes(GetIntegerSize(binr));
                    rsaParameters.P = binr.ReadBytes(GetIntegerSize(binr));
                    rsaParameters.Q = binr.ReadBytes(GetIntegerSize(binr));
                    rsaParameters.DP = binr.ReadBytes(GetIntegerSize(binr));
                    rsaParameters.DQ = binr.ReadBytes(GetIntegerSize(binr));
                    rsaParameters.InverseQ = binr.ReadBytes(GetIntegerSize(binr));
                }

                rsa.ImportParameters(rsaParameters);
                return rsa;
            }

            #endregion

            #region 使用公钥创建RSA实例

            public RSA CreateRsaProviderFromPublicKey(string publicKeyString)
            {
                // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
                byte[] seqOid = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
                byte[] seq = new byte[15];

                var x509Key = Convert.FromBase64String(publicKeyString);

                // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
                using (MemoryStream mem = new MemoryStream(x509Key))
                {
                    using (BinaryReader binr = new BinaryReader(mem))  //wrap Memory Stream with BinaryReader for easy reading
                    {
                        byte bt = 0;
                        ushort twobytes = 0;

                        twobytes = binr.ReadUInt16();
                        if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                            binr.ReadByte();    //advance 1 byte
                        else if (twobytes == 0x8230)
                            binr.ReadInt16();   //advance 2 bytes
                        else
                            return null;

                        seq = binr.ReadBytes(15);       //read the Sequence OID
                        if (!CompareBytearrays(seq, seqOid))    //make sure Sequence for OID is correct
                            return null;

                        twobytes = binr.ReadUInt16();
                        if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
                            binr.ReadByte();    //advance 1 byte
                        else if (twobytes == 0x8203)
                            binr.ReadInt16();   //advance 2 bytes
                        else
                            return null;

                        bt = binr.ReadByte();
                        if (bt != 0x00)     //expect null byte next
                            return null;

                        twobytes = binr.ReadUInt16();
                        if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                            binr.ReadByte();    //advance 1 byte
                        else if (twobytes == 0x8230)
                            binr.ReadInt16();   //advance 2 bytes
                        else
                            return null;

                        twobytes = binr.ReadUInt16();
                        byte lowbyte = 0x00;
                        byte highbyte = 0x00;

                        if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
                            lowbyte = binr.ReadByte();  // read next bytes which is bytes in modulus
                        else if (twobytes == 0x8202)
                        {
                            highbyte = binr.ReadByte(); //advance 2 bytes
                            lowbyte = binr.ReadByte();
                        }
                        else
                            return null;
                        byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
                        int modsize = BitConverter.ToInt32(modint, 0);

                        int firstbyte = binr.PeekChar();
                        if (firstbyte == 0x00)
                        {   //if first byte (highest order) of modulus is zero, don't include it
                            binr.ReadByte();    //skip this null byte
                            modsize -= 1;   //reduce modulus buffer size by 1
                        }

                        byte[] modulus = binr.ReadBytes(modsize);   //read the modulus bytes

                        if (binr.ReadByte() != 0x02)            //expect an Integer for the exponent data
                            return null;
                        int expbytes = (int)binr.ReadByte();        // should only need one byte for actual exponent data (for all useful values)
                        byte[] exponent = binr.ReadBytes(expbytes);

                        // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                        var rsa = RSA.Create();
                        RSAParameters rsaKeyInfo = new RSAParameters
                        {
                            Modulus = modulus,
                            Exponent = exponent
                        };
                        rsa.ImportParameters(rsaKeyInfo);

                        return rsa;
                    }

                }
            }

            #endregion

            #region 导入密钥算法

            private int GetIntegerSize(BinaryReader binr)
            {
                byte bt = 0;
                int count = 0;
                bt = binr.ReadByte();
                if (bt != 0x02)
                    return 0;
                bt = binr.ReadByte();

                if (bt == 0x81)
                    count = binr.ReadByte();
                else
                if (bt == 0x82)
                {
                    var highbyte = binr.ReadByte();
                    var lowbyte = binr.ReadByte();
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                    count = BitConverter.ToInt32(modint, 0);
                }
                else
                {
                    count = bt;
                }

                while (binr.ReadByte() == 0x00)
                {
                    count -= 1;
                }
                binr.BaseStream.Seek(-1, SeekOrigin.Current);
                return count;
            }

            private bool CompareBytearrays(byte[] a, byte[] b)
            {
                if (a.Length != b.Length)
                    return false;
                int i = 0;
                foreach (byte c in a)
                {
                    if (c != b[i])
                        return false;
                    i++;
                }
                return true;
            }

            #endregion

        }

        /// <summary>
        /// RSA算法类型
        /// </summary>
        public enum RSAType
        {
            /// <summary>
            /// SHA1
            /// </summary>
            RSA = 0,
            /// <summary>
            /// RSA2 密钥长度至少为2048
            /// SHA256
            /// </summary>
            RSA2
        }

    }

    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        //2048 公钥
    //        string publicKey =
    //            "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoQh0wEqx/R2H1v00IU12Oc30fosRC/frhH89L6G+fzeaqI19MYQhEPMU13wpeqRONCUta+2iC1sgCNQ9qGGf19yGdZUfueaB1Nu9rdueQKXgVurGHJ+5N71UFm+OP1XcnFUCK4wT5d7ZIifXxuqLehP9Ts6sNjhVfa+yU+VjF5HoIe69OJEPo7OxRZcRTe17khc93Ic+PfyqswQJJlY/bgpcLJQnM+QuHmxNtF7/FpAx9YEQsShsGpVo7JaKgLo+s6AFoJ4QldQKir2vbN9vcKRbG3piElPilWDpjXQkOJZhUloh/jd7QrKFimZFldJ1r6Q59QYUyGKZARUe0KZpMQIDAQAB";
    //        //2048 私钥
    //        string privateKey =
    //            "MIIEpAIBAAKCAQEAoQh0wEqx/R2H1v00IU12Oc30fosRC/frhH89L6G+fzeaqI19MYQhEPMU13wpeqRONCUta+2iC1sgCNQ9qGGf19yGdZUfueaB1Nu9rdueQKXgVurGHJ+5N71UFm+OP1XcnFUCK4wT5d7ZIifXxuqLehP9Ts6sNjhVfa+yU+VjF5HoIe69OJEPo7OxRZcRTe17khc93Ic+PfyqswQJJlY/bgpcLJQnM+QuHmxNtF7/FpAx9YEQsShsGpVo7JaKgLo+s6AFoJ4QldQKir2vbN9vcKRbG3piElPilWDpjXQkOJZhUloh/jd7QrKFimZFldJ1r6Q59QYUyGKZARUe0KZpMQIDAQABAoIBAQCRZLUlOUvjIVqYvhznRK1OG6p45s8JY1r+UnPIId2Bt46oSLeUkZvZVeCnfq9k0Bzb8AVGwVPhtPEDh73z3dEYcT/lwjLXAkyPB6gG5ZfI/vvC/k7JYV01+neFmktw2/FIJWjEMMF2dvLNZ/Pm4bX1Dz9SfD/45Hwr8wqrvRzvFZsj5qqOxv9RPAudOYwCwZskKp/GF+L+3Ycod1Wu98imzMZUH+L5dQuDGg3kvf3ljIAegTPoqYBg0imNPYY/EGoFKnbxlK5S5/5uAFb16dGJqAz3XQCz9Is/IWrOTu0etteqV2Ncs8uqPdjed+b0j8CMsr4U1xjwPQ8WwdaJtTkRAoGBANAndgiGZkCVcc9975/AYdgFp35W6D+hGQAZlL6DmnucUFdXbWa/x2rTSEXlkvgk9X/PxOptUYsLJkzysTgfDywZwuIXLm9B3oNmv3bVgPXsgDsvDfaHYCgz0nHK6NSrX2AeX3yO/dFuoZsuk+J+UyRigMqYj0wjmxUlqj183hinAoGBAMYMOBgF77OXRII7GAuEut/nBeh2sBrgyzR7FmJMs5kvRh6Ck8wp3ysgMvX4lxh1ep8iCw1R2cguqNATr1klOdsCTOE9RrhuvOp3JrYzuIAK6MpH/uBICy4w1rW2+gQySsHcH40r+tNaTFQ7dQ1tef//iy/IW8v8i0t+csztE1JnAoGABdtWYt8FOYP688+jUmdjWWSvVcq0NjYeMfaGTOX/DsNTL2HyXhW/Uq4nNnBDNmAz2CjMbZwt0y+5ICkj+2REVQVUinAEinTcAe5+LKXNPx4sbX3hcrJUbk0m+rSu4G0B/f5cyXBsi9wFCAzDdHgBduCepxSr04Sc9Hde1uQQi7kCgYB0U20HP0Vh+TG2RLuE2HtjVDD2L/CUeQEiXEHzjxXWnhvTg+MIAnggvpLwQwmMxkQ2ACr5sd/3YuCpB0bxV5o594nsqq9FWVYBaecFEjAGlWHSnqMoXWijwu/6X/VOTbP3VjH6G6ECT4GR4DKKpokIQrMgZ9DzaezvdOA9WesFdQKBgQCWfeOQTitRJ0NZACFUn3Fs3Rvgc9eN9YSWj4RtqkmGPMPvguWo+SKhlk3IbYjrRBc5WVOdoX8JXb2/+nAGhPCuUZckWVmZe5pMSr4EkNQdYeY8kOXGSjoTOUH34ZdKeS+e399BkBWIiXUejX/Srln0H4KoHnTWgxwNpTsBCgXu8Q==";

    //        var rsa = new EncryptUtil.RSAHelper(EncryptUtil.RSAType.RSA2, Encoding.UTF8, privateKey, publicKey);

    //        string str = "博客园 http://www.cnblogs.com/";

    //        Console.WriteLine("原始字符串：" + str);

    //        //加密
    //        string enStr = rsa.Encrypt(str);

    //        Console.WriteLine("加密字符串：" + enStr);

    //        //解密
    //        string deStr = rsa.Decrypt(enStr);

    //        Console.WriteLine("解密字符串：" + deStr);

    //        //私钥签名
    //        string signStr = rsa.Sign(str);

    //        Console.WriteLine("字符串签名：" + signStr);

    //        //公钥验证签名
    //        bool signVerify = rsa.Verify(str, signStr);

    //        Console.WriteLine("验证签名：" + signVerify);

    //        Console.ReadKey();
    //    }

    //}
}
