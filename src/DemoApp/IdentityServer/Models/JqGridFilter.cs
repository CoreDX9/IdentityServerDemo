using CoreDX.Common.Util.TypeExtensions;
using System.Linq;
using System.Text.Json;

namespace IdentityServer.Models
{
    public class JqGridParameter
    {
        /// <summary>
        /// 是否搜索，本来应该是bool，true
        /// </summary>
        public string _search { get; set; }
        /// <summary>
        /// 请求发送次数，方便服务器处理重复请求
        /// </summary>
        public long Nd { get; set; }
        /// <summary>
        /// 当页数据条数
        /// </summary>
        public int Rows { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// 排序列，多列排序时为排序列名+空格+排序方式，多个列之间用逗号隔开。例：id asc,name desc
        /// </summary>
        public string Sidx { get; set; }
        /// <summary>
        /// 分离后的排序列
        /// </summary>
        public string[][] SIdx => Sidx.Split(", ").Select(s => s.Split(" ")).ToArray();
        /// <summary>
        /// 排序方式：asc、desc
        /// </summary>
        public string Sord { get; set; }
        /// <summary>
        /// 高级搜索条件json
        /// </summary>
        public string Filters { get; set; }

        /// <summary>
        /// 序列化的高级搜索对象
        /// </summary>
        public JqGridSearchRuleGroup FilterObject => Filters.IsNullOrWhiteSpace()
            ? new JqGridSearchRuleGroup { Rules = new[] { new JqGridSearchRule { Op = SearchOper, Data = SearchString, Field = SearchField } } }
            : JsonSerializer.Deserialize<JqGridSearchRuleGroup>(Filters, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        /// <summary>
        /// 简单搜索字段
        /// </summary>
        public string SearchField { get; set; }
        /// <summary>
        /// 简单搜索关键字
        /// </summary>
        public string SearchString { get; set; }
        /// <summary>
        /// 简单搜索操作
        /// </summary>
        public string SearchOper { get; set; }

    }

    /// <summary>
    /// 高级搜索条件组
    /// </summary>
    public class JqGridSearchRuleGroup
    {
        /// <summary>
        /// 条件组合方式：and、or
        /// </summary>
        public string GroupOp { get; set; }
        /// <summary>
        /// 搜索条件集合
        /// </summary>
        public JqGridSearchRule[] Rules { get; set; }
        /// <summary>
        /// 搜索条件组集合
        /// </summary>
        public JqGridSearchRuleGroup[] Groups { get; set; }
    }

    /// <summary>
    /// 高级搜索条件
    /// </summary>
    public class JqGridSearchRule
    {
        /// <summary>
        /// 搜索字段
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// 搜索字段的大驼峰命名
        /// </summary>
        public string PascalField => Field?.Length > 0 ? Field.Substring(0, 1).ToUpper() + Field.Substring(1) : Field;
        /// <summary>
        /// 搜索操作
        /// </summary>
        public string Op { get; set; }
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string Data { get; set; }
    }
}