using DepsOfPlugin1;
using PluginBase;

namespace Plugin1
{
    public class Demo1 : IPlugin
    {
        public string Run(object content)
        {
            return new Worker().GetJsonText(content);
        }
    }
}
