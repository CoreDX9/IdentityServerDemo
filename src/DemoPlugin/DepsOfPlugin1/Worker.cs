using System;
using System.Text.Json;

namespace DepsOfPlugin1
{
    public class Worker
    {
        public string GetJsonText(object content)
        {
            return JsonSerializer.Serialize(new { Content = content, About = "This is Plugin No.1", Time = DateTime.Now });
        }
    }
}
