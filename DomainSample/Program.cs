using System;
using System.Linq;
using System.Linq.Expressions;
using Domain;
using Entity;
using Util.TypeExtensions;

namespace DomainSample
{
    class Program
    {
        static void Main(string[] args)
        {
            #region 与泛型相关的静态字段共享性测试，各种静态objCount字段

            S1 s = new S1();
            S2 ss = new S2();

            ST1 st1 = new ST1();
            ST1 st2 = new ST1();
            ST1 st3 = new ST1();

            ST2 s1t = new ST2();
            ST2 s2t = new ST2();
            ST2 s3t = new ST2();
            ST2 s4t = new ST2();

            STT1 stt1 = new STT1();
            STT1 stt2 = new STT1();
            STT1 stt3 = new STT1();
            STT1 stt4 = new STT1();
            STT1 stt5 = new STT1();

            STT2 s1tt2 = new STT2();
            STT2 s2tt2 = new STT2();
            STT2 s3tt2 = new STT2();
            STT2 s4tt2 = new STT2();
            STT2 s5tt2 = new STT2();
            STT2 s6tt2 = new STT2();

            #endregion

            P2 pp = new P2();
            P3 ppp = new P3();

            //var pr1 = Expression.Parameter(typeof(int?), "i1");
            //var pr2 = Expression.Parameter(typeof(int), "i2");
            //var l = Expression.Constant(pr1);
            //var r = Expression.Constant(pr2);
            //var e = Expression.Equal(l, r);
            //var f = Expression.Lambda(e, pr1, pr2);

            Expression<Func<int?, int, bool >> ff = (a1, a2) => a1 == a2;

            Person.PublicPropertyChangedEventHandler +=
                (sender, arg) => Console.WriteLine($"Person {sender.GetHashCode()}:{arg.PropertyName} 发生更改！");

            var p1 = new Person {Id = 1, Age = 1, Name = "P-Bob"};
            var p2 = new Person {Id = 2, Age = 2, Name = "Bob" };
            var p3 = new Person {Id = 3, Age = 3, Name = "Bob2" };
            var p4 = new Person {Id = 4, Age = 4, Name = "Bob-C1"};
            var p5 = new Person {Id = 5, Age = 5, Name = "Bob-C2" };
            
            p1.Children.Add(p2);
            p2.Parent = p1;
            p2.ParentId = p1.Id;

            p1.Children.Add(p3);
            p3.Parent = p1;
            p3.ParentId = p1.Id;

            p2.Children.Add(p4);
            p4.Parent = p2;
            p4.ParentId = p2.Id;

            p2.Children.Add(p5);
            p5.Parent = p2;
            p5.ParentId = p2.Id;

            TreeNode[] nodes = {
                new TreeNode("@babel", null),
                new TreeNode("code-frame", "@babel"),
                new TreeNode("lib\r\n(code-frame)", "code-frame"),
                new TreeNode("core", "@babel"),
                new TreeNode("lib", "core"),
                new TreeNode("config", "lib"),
                new TreeNode("files", "config"),
                new TreeNode("helpers", "config"),
                new TreeNode("validation", "config"),
                new TreeNode("tools", "lib"),
                new TreeNode("transformation", "lib"),
                new TreeNode("file", "transformation"),
                new TreeNode("util", "transformation"),
                new TreeNode("node_modules", "core"),
                new TreeNode(".bin", "node_modules"),
                new TreeNode("debug", "node_modules"),
                new TreeNode("dist", "debug"),
                new TreeNode("src", "debug"),
                new TreeNode("ms", "node_modules"),
                new TreeNode("generator", "@babel"),
            };

            var hNodes = nodes[0].AsHierarchical(tNode => nodes.Where(t => t.ParentValue == tNode.Value));
            var strNodes = hNodes.ToString(tn => tn.Value, true);
            Console.WriteLine("-------------------------------------------------------------");
            Console.WriteLine(strNodes);
            Console.WriteLine("-------------------------------------------------------------");

            var test = hNodes.AsEnumerable(c => c.Children);
            var testC = test.Count();

            var start = test.ElementAt(new Random().Next(0, testC));
            var stst = start.ToString(tn => tn.Value, true);
            var end = test.ElementAt(new Random().Next(0, testC));
            var enst = end.ToString(tn => tn.Value, true);

            var path = start.GetPathToNode(end);

            var strPath = string.Join(" -> ", path.Select(p => p.Current.Value));

            Console.WriteLine("节点路径查找");
            Console.WriteLine($"起点：{start.Current.Value}");
            Console.WriteLine($"终点：{end.Current.Value}");
            Console.WriteLine($"路径：{strPath}");
            Console.WriteLine("-------------------------------------------------------------");
            Console.WriteLine($"起点的子树：\r\n{stst}");
            Console.WriteLine($"终点的子树：\r\n{enst}");
            Console.WriteLine("-------------------------------------------------------------");

            var hi = p1.AsHierarchical();
            var b = hi.Children?.First() == hi?.Children?.First();

            foreach (var p in p1.AsEnumerable())
            {
                Console.WriteLine($@"person id:{p.Id} name:{p.Name} depth:{p.Depth} pId:{p.ParentId} children:{string.Join(',', p.Children.Select(x => x.Id))}");
            }

            Console.WriteLine("-------------------------------------------------------------");

            foreach (var p in p1.AsEnumerable(EnumerateType.Bfs))
            {
                Console.WriteLine($@"person id:{p.Id} name:{p.Name} depth:{p.Depth} pId:{p.ParentId} children:{string.Join(',', p.Children.Select(x => x.Id))}");
            }

            Console.WriteLine();
            Console.WriteLine("按任意键退出……");
            Console.ReadKey();
        }
    }

    public class Person : DomainTreeEntityBase<int, Person, int>
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class P2 : DomainTreeEntityBase<int, P2, int>
    {
        public int i { get; set; }
    }

    public class P3 : P2
    {
        public int j { get; set; }
    }

    public class S1 : ChangeTrackableSample
    {
        public int i { get; set; }
    }

    public class S2 : ChangeTrackableSample
    {
        public int j { get; set; }
    }

    public class ST1 : ChangeTrackableSample<int>
    {
        public int i { get; set; }
    }

    public class ST2 : ChangeTrackableSample<double>
    {
        public int j { get; set; }
    }

    public class STT1 : ChangeTrackableSample2<int>
    {
        public int i { get; set; }
    }

    public class STT2 : ChangeTrackableSample2<double>
    {
        public int j { get; set; }
    }

    public class TreeNode
    {
        public TreeNode(string value, string parentValue)
        {
            ParentValue = parentValue;
            Value = value;
        }
        public string ParentValue { get; set; }

        public string Value { get; set; }

        public override string ToString()
        {
            return $"{nameof(Value)}: {Value}; {nameof(ParentValue)}: {ParentValue}";
        }
    }
}
