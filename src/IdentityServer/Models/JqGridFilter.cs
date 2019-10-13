using System.Linq;
using Newtonsoft.Json;

namespace IdentityServer.Models
{
    public class JqGridParameter
    {
        public string _search { get; set; }
        public long Nd { get; set; }
        public int Rows { get; set; }
        public int Page { get; set; }
        public string Sidx { get; set; }
        public string[][] SIdx => Sidx.Split(", ").Select(s=>s.Split(" ")).ToArray(); 
        public string Sord { get; set; }
        public string Filters { get; set; }

        public JqGridSearchRuleGroup FilterObject => Filters.IsNullOrWhiteSpace()
            ? new JqGridSearchRuleGroup {Rules = new[] {new JqGridSearchRule {Op = SearchOper, Data = SearchString, Field = SearchField}}}
            : JsonConvert.DeserializeObject<JqGridSearchRuleGroup>(Filters ?? string.Empty);

        public string SearchField { get; set; }
        public string SearchString { get; set; }
        public string SearchOper { get; set; }

    }

    public class JqGridSearchRuleGroup
    {
        public string GroupOp { get; set; }
        public JqGridSearchRule[] Rules { get; set; }
        public JqGridSearchRuleGroup[] Groups { get; set; }
    }

    public class JqGridSearchRule
    {
        public string Field { get; set; }
        public string PascalField => Field?.Length > 0 ? Field.Substring(0, 1).ToUpper() + Field.Substring(1) : Field;
        public string Op { get; set; }
        public string Data { get; set; }
    }
}