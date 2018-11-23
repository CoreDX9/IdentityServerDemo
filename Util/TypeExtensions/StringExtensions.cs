using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Util.TypeExtensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static byte[] ToBytesFromBase64String(this string base64String)
        {
            return Convert.FromBase64String(base64String);
        }

        public static byte[] ToBytesFromHexString(this string hexString)
        {
            if (hexString.IsNullOrWhiteSpace())
            {
                return null;
            }

            string str = hexString.ToLower();
            Regex r = new Regex(@"^(0x)?([0-9a-f]{2})+");
            if (!r.IsMatch(str))
            {
                throw new InvalidOperationException("数字格式错误");
            }

            var spidx = str.IndexOf('x');
            spidx = spidx < 0 ? 0 : spidx + 1;
            str = str.Substring(spidx);

            byte[] bytes = new byte[str.Length / 2];

            for (int i = 0; i < str.Length / 2; i += 2)
            {
                bytes[i / 2] = Byte.Parse(str.Substring(i,2), NumberStyles.HexNumber);
            }

            return bytes;
        }

        public static object ToEnumObject(this string stringOrNumber, Type @enum)
        {
            if(!@enum.IsEnum) throw new ArgumentException($"{@enum.FullName}不是枚举类型。");

            object enumObject;
            var exception = new InvalidOperationException($"待转换的值不在{@enum.FullName}的定义中。");

            switch (Enum.GetUnderlyingType(@enum))
            {
                #region 有符号整数

                case var t when t == typeof(sbyte):
                    {
                        enumObject = sbyte.TryParse(stringOrNumber, out var rei)
                            ? Enum.GetValues(@enum).Cast<sbyte>().Any(en => en == rei)
                                ?
                                Enum.ToObject(@enum, rei)
                                :
                                throw exception
                            : Enum.Parse(@enum, stringOrNumber);
                    }
                    break;
                case var t when t == typeof(short):
                    {
                        enumObject = short.TryParse(stringOrNumber, out var rei)
                            ? Enum.GetValues(@enum).Cast<short>().Any(en => en == rei)
                                ?
                                Enum.ToObject(@enum, rei)
                                :
                                throw exception
                            : Enum.Parse(@enum, stringOrNumber);
                    }
                    break;
                case var t when t == typeof(int):
                    {
                        enumObject = int.TryParse(stringOrNumber, out var rei)
                            ? Enum.GetValues(@enum).Cast<int>().Any(en => en == rei)
                                ?
                                Enum.ToObject(@enum, rei)
                                :
                                throw exception
                            : Enum.Parse(@enum, stringOrNumber);
                    }
                    break;
                case var t when t == typeof(long):
                    {
                        enumObject = long.TryParse(stringOrNumber, out var rei)
                            ? Enum.GetValues(@enum).Cast<long>().Any(en => en == rei)
                                ?
                                Enum.ToObject(@enum, rei)
                                :
                                throw exception
                            : Enum.Parse(@enum, stringOrNumber);
                    }
                    break;

                #endregion

                #region 无符号整数

                case var t when t == typeof(byte):
                    {
                        enumObject = byte.TryParse(stringOrNumber, out var rei)
                            ? Enum.GetValues(@enum).Cast<byte>().Any(en => en == rei)
                                ?
                                Enum.ToObject(@enum, rei)
                                :
                                throw exception
                            : Enum.Parse(@enum, stringOrNumber);
                    }
                    break;
                case var t when t == typeof(ushort):
                    {
                        enumObject = ushort.TryParse(stringOrNumber, out var rei)
                            ? Enum.GetValues(@enum).Cast<ushort>().Any(en => en == rei)
                                ?
                                Enum.ToObject(@enum, rei)
                                :
                                throw exception
                            : Enum.Parse(@enum, stringOrNumber);
                    }
                    break;
                case var t when t == typeof(uint):
                    {
                        enumObject = uint.TryParse(stringOrNumber, out var rei)
                            ? Enum.GetValues(@enum).Cast<uint>().Any(en => en == rei)
                                ?
                                Enum.ToObject(@enum, rei)
                                :
                                throw exception
                            : Enum.Parse(@enum, stringOrNumber);
                    }
                    break;
                case var t when t == typeof(ulong):
                    {
                        enumObject = ulong.TryParse(stringOrNumber, out var rei)
                            ? Enum.GetValues(@enum).Cast<ulong>().Any(en => en == rei)
                                ?
                                Enum.ToObject(@enum, rei)
                                :
                                throw exception
                            : Enum.Parse(@enum, stringOrNumber);
                    }
                    break;

                #endregion

                default:
                    throw new InvalidOperationException($"无法将值转换为{@enum.FullName}");
            }
            return enumObject;
        }

        public static T ToEnum<T>(this string stringOrNumber) 
            where T : Enum
        {
            return (T)stringOrNumber.ToEnumObject(typeof(T));
        }
    }
}
