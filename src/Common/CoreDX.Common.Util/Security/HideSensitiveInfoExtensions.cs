using System.Text;
using CoreDX.Common.Util.TypeExtensions;

namespace CoreDX.Common.Util.Security
{
    public static class HideSensitiveInfoExtensions
    {
        /// <summary>
        /// 隐藏敏感信息
        /// </summary>
        /// <param name="info">信息实体</param>
        /// <param name="left">左边保留的字符数</param>
        /// <param name="right">右边保留的字符数</param>
        /// <param name="substituteCharacterCount">替代字符的个数，非正数表示个数与被隐藏字数一致</param>
        /// <param name="substituteCharacter">替代字符</param>
        /// <param name="basedOnLeft">当长度异常时，是否显示左边 </param>
        /// <param name="hideEmptyOrWhiteSpace">是否隐藏空白字符串</param>
        /// <returns></returns>
        public static string HideSensitiveInfo(this string info, int left, int right, bool basedOnLeft = true, int substituteCharacterCount = 0, char substituteCharacter = '*', bool hideEmptyOrWhiteSpace = false)
        {
            if (info.IsNullOrWhiteSpace())
            {
                if (hideEmptyOrWhiteSpace)
                {
                    return new StringBuilder().Append(substituteCharacter,
                            substituteCharacterCount > 0
                                ? substituteCharacterCount
                                : info.Length > 0
                                    ? info.Length
                                    : 1)
                        .ToString();
                }
                return info;
            }
            StringBuilder sbText = new StringBuilder();
            int hiddenCharCount = info.Length - left - right;
            substituteCharacterCount = substituteCharacterCount > 0 ? substituteCharacterCount : hiddenCharCount;
            if (substituteCharacterCount > 0)
            {
                string prefix = info.Substring(0, left), suffix = info.Substring(info.Length - right);
                sbText.Append(prefix)
                    .Append(substituteCharacter, substituteCharacterCount)
                    .Append(suffix);
            }
            else
            {
                if (basedOnLeft)
                {
                    if (info.Length > left && left > 0)
                    {
                        sbText.Append(info.Substring(0, left))
                            .Append(substituteCharacter, substituteCharacterCount);
                    }
                    else
                    {
                        sbText.Append(info.Substring(0, 1))
                            .Append(substituteCharacter, substituteCharacterCount);
                    }
                }
                else
                {
                    if (info.Length > right && right > 0)
                    {
                        sbText.Append(substituteCharacter, substituteCharacterCount)
                            .Append(info.Substring(info.Length - right));
                    }
                    else
                    {
                        sbText.Append(substituteCharacter, substituteCharacterCount)
                            .Append(info.Substring(info.Length - 1));
                    }
                }
            }
            return sbText.ToString();
        }

        /// <summary>
        /// 隐藏敏感信息
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="subLen">信息总长与左子串（或右子串）的比例</param>
        /// <param name="basedOnLeft">当长度异常时，是否显示左边，默认true，默认显示左边</param>
        /// <code>true</code>显示左边，<code>false</code>显示右边
        /// <param name="substituteCharacterCount">替代字符的个数，非正数表示个数与被隐藏字数一致</param>
        /// <param name="substituteCharacter">替代字符</param>
        /// <param name="hideEmptyOrWhiteSpace">是否隐藏空白字符串</param>
        /// <returns></returns>
        public static string HideSensitiveInfo(this string info, int subLen = 3, bool basedOnLeft = true, int substituteCharacterCount = 0, char substituteCharacter = '*', bool hideEmptyOrWhiteSpace = false)
        {
            if (info.IsNullOrWhiteSpace())
            {
                if (hideEmptyOrWhiteSpace)
                {
                    return new StringBuilder().Append(substituteCharacter,
                            substituteCharacterCount > 0
                                ? substituteCharacterCount
                                : info.Length > 0
                                    ? info.Length
                                    : 1)
                        .ToString();
                }
                return info;
            }
            if (subLen <= 1)
            {
                subLen = 3;
            }
            int subLength = info.Length / subLen;
            StringBuilder sbText = new StringBuilder();
            if (subLength > 0 && info.Length > (subLength * 2))
            {
                string prefix = info.Substring(0, subLength), suffix = info.Substring(info.Length - subLength);
                substituteCharacterCount = substituteCharacterCount > 0 ? substituteCharacterCount : info.Length - prefix.Length - suffix.Length;
                return sbText.Append(prefix)
                    .Append(substituteCharacter, substituteCharacterCount)
                    .Append(suffix)
                    .ToString();
            }
            else
            {
                if (basedOnLeft)
                {
                    string prefix = subLength > 0 ? info.Substring(0, subLength) : info.Substring(0, 1);
                    substituteCharacterCount = substituteCharacterCount > 0 ? substituteCharacterCount : info.Length - prefix.Length;
                    return sbText.Append(prefix)
                        .Append(substituteCharacter, substituteCharacterCount)
                        .ToString();
                }
                else
                {
                    string suffix = subLength > 0 ? info.Substring(info.Length - subLength) : info.Substring(info.Length - 1);
                    substituteCharacterCount = substituteCharacterCount > 0 ? substituteCharacterCount : info.Length - suffix.Length;
                    return sbText.Append(substituteCharacter, substituteCharacterCount)
                        .Append(suffix)
                        .ToString();
                }
            }
        }

        /// <summary>
        /// 隐藏邮件详情
        /// </summary>
        /// <param name="email">邮件地址</param>
        /// <param name="left">邮件头保留字符个数，默认值设置为3</param>
        /// <param name="substituteCharacterCount">替代字符的个数，非正数表示个数与被隐藏字数一致</param>
        /// <param name="substituteCharacter">替代字符</param>
        /// <param name="hideEmptyOrWhiteSpace">是否隐藏空白字符串</param>
        /// <returns></returns>
        public static string HideEmailDetails(this string email, int left = 3, int substituteCharacterCount = 0, char substituteCharacter = '*', bool hideEmptyOrWhiteSpace = false)
        {
            if (email.IsNullOrEmpty())
            {
                return "";
            }
            if (System.Text.RegularExpressions.Regex.IsMatch(email, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))//如果是邮件地址
            {
                int suffixLen = email.Length - email.LastIndexOf('@');
                return HideSensitiveInfo(email, left, suffixLen, false, substituteCharacterCount, substituteCharacter, hideEmptyOrWhiteSpace);
            }
            else
            {
                return HideSensitiveInfo(email, 3, true, substituteCharacterCount, substituteCharacter, hideEmptyOrWhiteSpace);
            }
        }
    }
}
