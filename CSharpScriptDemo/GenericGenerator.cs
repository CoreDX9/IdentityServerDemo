using System;
using System.Linq;

namespace CSharpScriptDemo
{
    public class GenericGenerator
    {
        private static readonly string GeneratedAttribute =
            @"[System.CodeDom.Compiler.GeneratedCode(""walterlv"", ""1.0"")]";

        public string Transform(string originalCode, int genericCount)
        {
            if (genericCount == 1)
            {
                return originalCode;
            }

            var content = originalCode
                // 替换泛型。
                .Replace("<out T>", FromTemplate("<{0}>", "out T{n}", ", ", genericCount))
                .Replace("Task<T>", FromTemplate("Task<({0})>", "T{n}", ", ", genericCount))
                .Replace("Func<T, Task>", FromTemplate("Func<{0}, Task>", "T{n}", ", ", genericCount))
                .Replace(" T, Task>", FromTemplate(" {0}, Task>", "T{n}", ", ", genericCount))
                .Replace("(T, bool", FromTemplate("({0}, bool", "T{n}", ", ", genericCount))
                .Replace("var (t, ", FromTemplate("var ({0}, ", "t{n}", ", ", genericCount))
                .Replace(", t)", FromTemplate(", {0})", "t{n}", ", ", genericCount))
                .Replace("return (t, ", FromTemplate("return ({0}, ", "t{n}", ", ", genericCount))
                .Replace("<T>", FromTemplate("<{0}>", "T{n}", ", ", genericCount))
                .Replace("(T value)", FromTemplate("(({0}) value)", "T{n}", ", ", genericCount))
                .Replace("(T t)", FromTemplate("({0})", "T{n} t{n}", ", ", genericCount))
                .Replace("(t)", FromTemplate("({0})", "t{n}", ", ", genericCount))
                .Replace("var t =", FromTemplate("var ({0}) =", "t{n}", ", ", genericCount))
                .Replace(" T ", FromTemplate(" ({0}) ", "T{n}", ", ", genericCount))
                .Replace(" t;", FromTemplate(" ({0});", "t{n}", ", ", genericCount))
                // 生成 [GeneratedCode]。
                .Replace("    public interface ", $"    {GeneratedAttribute}{Environment.NewLine}    public interface ")
                .Replace("    public class ", $"    {GeneratedAttribute}{Environment.NewLine}    public class ")
                .Replace("    public sealed class ", $"    {GeneratedAttribute}{Environment.NewLine}    public sealed class ");
            return content.Trim();
        }

        private static string FromTemplate(string template, string part, string separator, int count)
        {
            return string.Format(template,
                string.Join(separator, Enumerable.Range(1, count).Select(x => part.Replace("{n}", x.ToString()))));
        }
    }
}