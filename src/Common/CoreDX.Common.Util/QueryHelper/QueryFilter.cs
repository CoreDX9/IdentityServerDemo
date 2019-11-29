using System.Text.Json;
using CoreDX.Common.Util.TypeExtensions;

namespace CoreDX.Common.Util.QueryHelper
{
    public enum FilterRuleGroupOprator : byte
    {
        AndAlso = 1,
        OrElse = 2
    }

    public enum FilterRuleOprator : byte
    {
        Equal = 1,
        NotEqual = 2,
        LessThan = 3,
        LessThanOrEqual = 4,
        GreaterThan = 5,
        GreaterThanOrEqual = 6,
        StringStartsWith = 7,
        StringNotStartsWith = 8,
        StringEndsWith = 9,
        StringNotEndsWith = 10,
        StringContains = 11,
        StringNotContains = 12,
        Include = 13,
        NotInclude = 14,
        IsNull = 15,
        IsNotNull = 16,
    }
    public class QueryFilter
    {
        /// <summary>
        /// 是否搜索，本来应该是bool，true
        /// </summary>
        public string _search { get; set; }
        /// <summary>
        /// 请求发送次数，方面服务器处理重复请求
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
        public FilterRuleGroup FilterObject => Filters.IsNullOrWhiteSpace()
            ? new FilterRuleGroup { Rules = new[] { new FilterRule() } }
            : JsonSerializer.Deserialize<FilterRuleGroup>(Filters ?? string.Empty);
    }

    /// <summary>
    /// 高级搜索条件组
    /// </summary>
    public class FilterRuleGroup
    {
        /// <summary>
        /// 条件组合方式：and、or
        /// </summary>
        public FilterRuleGroupOprator GroupOprator { get; set; }
        /// <summary>
        /// 搜索条件集合
        /// </summary>
        public FilterRule[] Rules { get; set; }
        /// <summary>
        /// 搜索条件组集合
        /// </summary>
        public FilterRuleGroup[] Groups { get; set; }
    }

    /// <summary>
    /// 高级搜索条件
    /// </summary>
    public class FilterRule
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
        public FilterRuleOprator Oprator { get; set; }
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string Data { get; set; }
    }
}