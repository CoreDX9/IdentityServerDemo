using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace CSharpScriptDemo
{
    public class Globals
    {
        public int X;
        public int Y;
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //执行表达式，返回强类型结果
            int result = await CSharpScript.EvaluateAsync<int>("1 + 2");
            Console.WriteLine("1 + 2 : " + result);

            //处理编译错误
            try
            {
                Console.WriteLine(await CSharpScript.EvaluateAsync("2+"));
            }
            catch (CompilationErrorException e)
            {
                Console.WriteLine("2+ : " + string.Join(Environment.NewLine, e.Diagnostics));
            }

            //添加程序集引用
            var result1 = await CSharpScript.EvaluateAsync("System.Net.Dns.GetHostName()",
                ScriptOptions.Default.WithReferences(typeof(System.Net.Dns).Assembly));
            Console.WriteLine("System.Net.Dns.GetHostName() : " + result1);

            //导入命名空间 using
            var result2 = await CSharpScript.EvaluateAsync("Directory.GetCurrentDirectory()",
                ScriptOptions.Default.WithImports("System.IO"));
            Console.WriteLine("Directory.GetCurrentDirectory() : " + result2);

            //导入静态类型 using static
            var result3 = await CSharpScript.EvaluateAsync("Sqrt(2)",
                ScriptOptions.Default.WithImports("System.Math"));
            Console.WriteLine("Sqrt(2) : " + result3);

            //参数化脚本
            var globals = new Globals {X = 1, Y = 2};
            Console.WriteLine("X + Y : " + await CSharpScript.EvaluateAsync<int>("X+Y", globals: globals));

            //编译缓存并多次执行脚本
            var script = CSharpScript.Create<int>("X*Y", globalsType: typeof(Globals));
            script.Compile();
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("No " + (i + 1) + " X*Y : " + (await script.RunAsync(new Globals { X = i, Y = i })).ReturnValue);
            }

            //编译脚本为委托
            script = CSharpScript.Create<int>("X/Y", globalsType: typeof(Globals));
            ScriptRunner<int> runner = script.CreateDelegate();
            for (int i = 1; i < 11; i++)
            {
                Console.WriteLine("No " + (i + 1) + " X/Y : " + await runner(new Globals { X = new Random().Next(1,i), Y = new Random().Next(1, i) }));
            }

            //运行脚本片段并检查已定义的变量
            var state = await CSharpScript.RunAsync<int>("int answer = 42;");
            foreach (var variable in state.Variables)
                Console.WriteLine($"{variable.Name} = {variable.Value} of type {variable.Type}");

            //连接多个片段为一个脚本
            var script1 = CSharpScript.
                Create<int>("int x = 1;").
                ContinueWith("int y = 2;").
                ContinueWith("x + y");
            Console.WriteLine("x + y : " + (await script1.RunAsync()).ReturnValue);
            //获取编译对象以访问所有Roslyn API
            //var compilation = script1.GetCompilation();

            //从之前的状态继续执行脚本
            var state1 = await CSharpScript.RunAsync("int x = 1;");
            state1 = await state1.ContinueWithAsync("int y = 2;");
            state1 = await state1.ContinueWithAsync("x+y");
            Console.WriteLine("x + y : " + state1.ReturnValue);

            //读取代码文件并执行编译
            var file = @"C:\Users\Administrator\source\repos\ConsoleApp2\ConsoleApp2/GenericGenerator.cs";
            var originalText = File.ReadAllText(file);
            var syntaxTree = CSharpSyntaxTree.ParseText(originalText);//获取语法树
            var type = CompileType("GenericGenerator", syntaxTree);//执行编译并获取类型

            var transformer = Activator.CreateInstance(type);
            var newContent = (string)type.GetMethod("Transform").Invoke(transformer,
                new object[] { "某个泛型类的全文，假装我是泛型类 Walterlv<T> is a sb.", 2 });

            var aa = new GenericGenerator();
            var str = aa.Transform("某个泛型类的全文，假装我是泛型类 Walterlv<T> is a sb.", 25);
            Console.ReadKey();
        }

        private static Type CompileType(string originalClassName, SyntaxTree syntaxTree)
        {
            // 指定编译选项。
            var assemblyName = $"{originalClassName}.g";
            var compilation = CSharpCompilation.Create(assemblyName, new[] { syntaxTree },
                    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(
                    // 这算是偷懒了吗？我把 .NET Core 运行时用到的那些引用都加入到引用了。
                    // 加入引用是必要的，不然连 object 类型都是没有的，肯定编译不通过。
                    AppDomain.CurrentDomain.GetAssemblies().Where(x=>!string.IsNullOrEmpty(x.Location)).Select(x => MetadataReference.CreateFromFile(x.Location)));

            // 编译到内存流中。
            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (result.Success)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    var assembly = Assembly.Load(ms.ToArray());
                    return assembly.GetTypes().First(x => x.Name == originalClassName);
                }
                throw new Exception(string.Join(Environment.NewLine, result.Diagnostics));
            }
        }
    }
}
