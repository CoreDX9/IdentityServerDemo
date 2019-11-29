using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Linq.Expressions;
using System.Reflection;
using CoreDX.Common.Util.QueryHelper;

namespace CoreDX.Common.Util.TypeExtensions
{
    /// <summary>
    /// JqGrid搜索表达式扩展
    /// </summary>
    public static class QueryFilterExtensions
    {
        //前端的（不）属于条件搜索需要传递一个json数组的字符串作为参数
        //为了避免在搜索字符串的时候分隔符是搜索内容的一部分导致搜索关键字出错
        //无论定义什么分隔符都不能完全避免这种尴尬的情况，所以使用标准的json以绝后患
        /// <summary>
        /// 根据搜索条件构造where表达式，支持JqGrid高级搜索
        /// </summary>
        /// <typeparam name="T">搜索的对象类型</typeparam>
        /// <param name="ruleGroup">JqGrid搜索条件组</param>
        /// <param name="propertyMap">属性映射，把搜索规则的名称映射到属性名称，如果属性是复杂类型，使用点号可以继续访问内部属性</param>
        /// <returns>where表达式</returns>
        public static Expression<Func<T, bool>> BuildWhere<T>(this FilterRuleGroup ruleGroup, IDictionary<string, string> propertyMap = null)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "searchObject");

            return Expression.Lambda<Func<T, bool>>(BuildGroupExpression<T>(ruleGroup, parameter, propertyMap), parameter);
        }

        /// <summary>
        /// 构造搜索条件组的表达式（一个组中可能包含若干子条件组）
        /// </summary>
        /// <typeparam name="T">搜索的对象类型</typeparam>
        /// <param name="group">条件组</param>
        /// <param name="parameter">参数表达式</param>
        /// <param name="propertyMap">属性映射</param>
        /// <returns>返回bool的条件组的表达式</returns>
        private static Expression BuildGroupExpression<T>(FilterRuleGroup group, ParameterExpression parameter, IDictionary<string, string> propertyMap)
        {
            List<Expression> expressions = new List<Expression>();
            foreach (var rule in group.Rules ?? new FilterRule[0])
            {
                expressions.Add(BuildRuleExpression<T>(rule, parameter, propertyMap));
            }

            foreach (var subGroup in group.Groups ?? new FilterRuleGroup[0])
            {
                expressions.Add(BuildGroupExpression<T>(subGroup, parameter, propertyMap));
            }

            if (expressions.Count == 0)
            {
                throw new InvalidOperationException("构造where子句异常，生成了0个比较条件表达式。");
            }

            if (expressions.Count == 1)
            {
                return expressions[0];
            }

            var expression = expressions[0];
            switch (group.GroupOprator)
            {
                case FilterRuleGroupOprator.AndAlso:
                    foreach (var exp in expressions.Skip(1))
                    {
                        expression = Expression.AndAlso(expression, exp);
                    }
                    break;
                case FilterRuleGroupOprator.OrElse:
                    foreach (var exp in expressions.Skip(1))
                    {
                        expression = Expression.OrElse(expression, exp);
                    }
                    break;
                default:
                    throw new InvalidOperationException($"不支持创建{group.GroupOprator}类型的逻辑运算表达式");
            }

            return expression;
        }

        private static readonly FilterRuleOprator[] SpecialRuleOps = { FilterRuleOprator.Include, FilterRuleOprator.NotInclude, FilterRuleOprator.IsNull, FilterRuleOprator.IsNotNull };

        /// <summary>
        /// 构造条件表达式
        /// </summary>
        /// <typeparam name="T">搜索的对象类型</typeparam>
        /// <param name="rule">条件</param>
        /// <param name="parameter">参数</param>
        /// <param name="propertyMap">属性映射</param>
        /// <returns>返回bool的条件表达式</returns>
        private static Expression BuildRuleExpression<T>(FilterRule rule, ParameterExpression parameter,
            IDictionary<string, string> propertyMap)
        {
            Expression l;

            //如果实体属性名称和前端名称不一致，或者属性是一个自定义类型，需要继续访问其内部属性，使用点号分隔
            if (propertyMap?.ContainsKey(rule.Field) == true)
            {
                var names = propertyMap[rule.Field].Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                l = Expression.Property(parameter, names[0]);
                foreach (var name in propertyMap[rule.Field].Split('.').Skip(1))
                {
                    l = Expression.Property(l, name);
                }
            }
            else
            {
                l = Expression.Property(parameter, rule.PascalField);
            }

            Expression r = null; //值表达式
            Expression e; //返回bool的各种比较表达式

            //属于和不属于比较是多值比较，需要调用Contains方法，而不是调用比较操作符
            //为空和不为空的右值为常量null，不需要构造
            var specialRuleOps = SpecialRuleOps;

            var isGen = false;
            var pt = typeof(T).GetProperty(rule.PascalField).PropertyType;
            //如果属性类型是可空值类型，取出内部类型
            if (pt.IsDerivedFrom(typeof(Nullable<>)))
            {
                isGen = true;
                pt = pt.GenericTypeArguments[0];
            }

            //根据属性类型创建要比较的常量值表达式（也就是r）
            if (!specialRuleOps.Contains(rule.Oprator))
            {
                switch (pt)
                {
                    case Type ct when ct == typeof(bool):
                        r = BuildConstantExpression(rule, bool.Parse);
                        break;

                    #region 文字

                    case Type ct when ct == typeof(char):
                        r = BuildConstantExpression(rule, str => str[0]);
                        break;
                    case Type ct when ct == typeof(string):
                        r = BuildConstantExpression(rule, str => str);
                        break;

                    #endregion

                    #region 有符号整数

                    case Type ct when ct == typeof(sbyte):
                        r = BuildConstantExpression(rule, sbyte.Parse);
                        break;
                    case Type ct when ct == typeof(short):
                        r = BuildConstantExpression(rule, short.Parse);
                        break;
                    case Type ct when ct == typeof(int):
                        r = BuildConstantExpression(rule, int.Parse);
                        break;
                    case Type ct when ct == typeof(long):
                        r = BuildConstantExpression(rule, long.Parse);
                        break;

                    #endregion

                    #region 无符号整数

                    case Type ct when ct == typeof(byte):
                        r = BuildConstantExpression(rule, byte.Parse);
                        break;
                    case Type ct when ct == typeof(ushort):
                        r = BuildConstantExpression(rule, ushort.Parse);
                        break;
                    case Type ct when ct == typeof(uint):
                        r = BuildConstantExpression(rule, uint.Parse);
                        break;
                    case Type ct when ct == typeof(ulong):
                        r = BuildConstantExpression(rule, ulong.Parse);
                        break;

                    #endregion

                    #region 小数

                    case Type ct when ct == typeof(float):
                        r = BuildConstantExpression(rule, float.Parse);
                        break;
                    case Type ct when ct == typeof(double):
                        r = BuildConstantExpression(rule, double.Parse);
                        break;
                    case Type ct when ct == typeof(decimal):
                        r = BuildConstantExpression(rule, decimal.Parse);
                        break;

                    #endregion

                    #region 其它常用类型

                    case Type ct when ct == typeof(DateTime):
                        r = BuildConstantExpression(rule, DateTime.Parse);
                        break;
                    case Type ct when ct == typeof(DateTimeOffset):
                        r = BuildConstantExpression(rule, DateTimeOffset.Parse);
                        break;
                    case Type ct when ct == typeof(Guid):
                        r = BuildConstantExpression(rule, Guid.Parse);
                        break;
                    case Type ct when ct.IsEnum:
                        r = Expression.Constant(rule.Data.ToEnumObject(ct));
                        break;

                    #endregion

                    default:
                        throw new InvalidOperationException($"不支持创建{pt.FullName}类型的数据表达式");
                }
            }

            if (r != null && pt.IsValueType && isGen)
            {
                var gt = typeof(Nullable<>).MakeGenericType(pt);
                r = Expression.Convert(r, gt);
            }

            switch (rule.Oprator)
            {
                case FilterRuleOprator.Equal: //等于
                    e = Expression.Equal(l, r);
                    break;
                case FilterRuleOprator.NotEqual: //不等于
                    e = Expression.NotEqual(l, r);
                    break;
                case FilterRuleOprator.LessThan: //小于
                    e = Expression.LessThan(l, r);
                    break;
                case FilterRuleOprator.LessThanOrEqual: //小于等于
                    e = Expression.LessThanOrEqual(l, r);
                    break;
                case FilterRuleOprator.GreaterThan: //大于
                    e = Expression.GreaterThan(l, r);
                    break;
                case FilterRuleOprator.GreaterThanOrEqual: //大于等于
                    e = Expression.GreaterThanOrEqual(l, r);
                    break;
                case FilterRuleOprator.StringStartsWith: //开头是（字符串）
                    if (pt == typeof(string))
                    {
                        e = Expression.Call(l, pt.GetMethod(nameof(string.StartsWith), new[] { typeof(string) }), r);
                    }
                    else
                    {
                        throw new InvalidOperationException($"不支持创建{pt.FullName}类型的开始于表达式");
                    }

                    break;
                case FilterRuleOprator.StringNotStartsWith: //开头不是（字符串）
                    if (pt == typeof(string))
                    {
                        e = Expression.Not(Expression.Call(l, pt.GetMethod(nameof(string.StartsWith), new[] { typeof(string) }), r));
                    }
                    else
                    {
                        throw new InvalidOperationException($"不支持创建{pt.FullName}类型的不开始于表达式");
                    }

                    break;
                case FilterRuleOprator.StringEndsWith: //结尾是（字符串）
                    if (pt == typeof(string))
                    {
                        e = Expression.Call(l, pt.GetMethod(nameof(string.EndsWith), new[] { typeof(string) }), r);
                    }
                    else
                    {
                        throw new InvalidOperationException($"不支持创建{pt.FullName}类型的结束于表达式");
                    }

                    break;
                case FilterRuleOprator.StringNotEndsWith: //结尾不是（字符串）
                    if (pt == typeof(string))
                    {
                        e = Expression.Not(Expression.Call(l, pt.GetMethod(nameof(string.EndsWith), new[] { typeof(string) }), r));
                    }
                    else
                    {
                        throw new InvalidOperationException($"不支持创建{pt.FullName}类型的不结束于表达式");
                    }

                    break;
                case FilterRuleOprator.StringContains: //包含（字符串）
                    if (pt == typeof(string))
                    {
                        e = Expression.Call(l, pt.GetMethod(nameof(string.Contains), new[] { typeof(string) }), r);
                    }
                    else
                    {
                        throw new InvalidOperationException($"不支持创建{pt.FullName}类型的包含表达式");
                    }

                    break;
                case FilterRuleOprator.StringNotContains: //不包含（字符串）
                    if (pt == typeof(string))
                    {
                        e = Expression.Not(Expression.Call(l, pt.GetMethod(nameof(string.Contains), new[] { typeof(string) }), r));
                    }
                    else
                    {
                        throw new InvalidOperationException($"不支持创建{pt.FullName}类型的包含表达式");
                    }

                    break;
                case FilterRuleOprator.Include: //属于（是候选值列表之一）
                    e = BuildContainsExpression(rule, l, pt);
                    break;
                case FilterRuleOprator.NotInclude: //不属于（不是候选值列表之一）
                    e = Expression.Not(BuildContainsExpression(rule, l, pt));
                    break;
                case FilterRuleOprator.IsNull: //为空
                    r = Expression.Constant(null);
                    e = Expression.Equal(l, r);
                    break;
                case FilterRuleOprator.IsNotNull: //不为空
                    r = Expression.Constant(null);
                    e = Expression.Not(Expression.Equal(l, r));
                    break;
                //case "bt": //区间
                //    throw new NotImplementedException($"尚未实现创建{rule.Oprator}类型的比较表达式");
                default:
                    throw new InvalidOperationException($"不支持创建{rule.Oprator}类型的比较表达式");
            }

            return e;

            Expression BuildConstantExpression<TValue>(FilterRule jRule, Func<string, TValue> valueConvertor)
            {
                var rv = valueConvertor(jRule.Data);
                return Expression.Constant(rv);
            }
        }

        /// <summary>
        /// 构造Contains调用表达式
        /// </summary>
        /// <param name="rule">条件</param>
        /// <param name="parameter">参数</param>
        /// <param name="parameterType">参数类型</param>
        /// <returns>Contains调用表达式</returns>
        private static Expression BuildContainsExpression(FilterRule rule, Expression parameter, Type parameterType)
        {
            Expression e = null;

            var genMethod = typeof(Queryable).GetMethods()
                .Single(m => m.Name == nameof(Queryable.Contains) && m.GetParameters().Length == 2);

            var jsonArray = JsonSerializer.Deserialize<string[]>(rule.Data);

            switch (parameterType)
            {
                #region 文字

                case Type ct when ct == typeof(char):
                    if (jsonArray.Any(o => o.Length != 1)) { throw new InvalidOperationException("字符型的候选列表中存在错误的候选项"); }
                    e = CallContains(parameter, jsonArray, str => str[0], genMethod, ct);
                    break;
                case Type ct when ct == typeof(string):
                    e = CallContains(parameter, jsonArray, str => str, genMethod, ct);
                    break;

                #endregion

                #region 有符号整数

                case Type ct when ct == typeof(sbyte):
                    e = CallContains(parameter, jsonArray, sbyte.Parse, genMethod, ct);
                    break;
                case Type ct when ct == typeof(short):
                    e = CallContains(parameter, jsonArray, short.Parse, genMethod, ct);
                    break;
                case Type ct when ct == typeof(int):
                    e = CallContains(parameter, jsonArray, int.Parse, genMethod, ct);
                    break;
                case Type ct when ct == typeof(long):
                    e = CallContains(parameter, jsonArray, long.Parse, genMethod, ct);
                    break;

                #endregion

                #region 无符号整数

                case Type ct when ct == typeof(byte):
                    e = CallContains(parameter, jsonArray, byte.Parse, genMethod, ct);
                    break;
                case Type ct when ct == typeof(ushort):
                    e = CallContains(parameter, jsonArray, ushort.Parse, genMethod, ct);
                    break;
                case Type ct when ct == typeof(uint):
                    e = CallContains(parameter, jsonArray, uint.Parse, genMethod, ct);
                    break;
                case Type ct when ct == typeof(ulong):
                    e = CallContains(parameter, jsonArray, ulong.Parse, genMethod, ct);
                    break;

                #endregion

                #region 小数

                case Type ct when ct == typeof(float):
                    e = CallContains(parameter, jsonArray, float.Parse, genMethod, ct);
                    break;
                case Type ct when ct == typeof(double):
                    e = CallContains(parameter, jsonArray, double.Parse, genMethod, ct);
                    break;
                case Type ct when ct == typeof(decimal):
                    e = CallContains(parameter, jsonArray, decimal.Parse, genMethod, ct);
                    break;

                #endregion

                #region 其它常用类型

                case Type ct when ct == typeof(DateTime):
                    e = CallContains(parameter, jsonArray, DateTime.Parse, genMethod, ct);
                    break;
                case Type ct when ct == typeof(DateTimeOffset):
                    e = CallContains(parameter, jsonArray, DateTimeOffset.Parse, genMethod, ct);
                    break;
                case Type ct when ct == typeof(Guid):
                    e = CallContains(parameter, jsonArray, Guid.Parse, genMethod, ct);
                    break;
                case Type ct when ct.IsEnum:
                    e = CallContains(Expression.Convert(parameter, typeof(object)), jsonArray, enumString => enumString.ToEnumObject(ct), genMethod, ct);
                    break;

                    #endregion
            }

            return e;

            static MethodCallExpression CallContains<T>(Expression pa, string[] jArray, Func<string, T> selector, MethodInfo genericMethod, Type type)
            {
                var data = jArray.Select(selector).ToArray().AsQueryable();
                var method = genericMethod.MakeGenericMethod(type);

                return Expression.Call(null, method, new[] { Expression.Constant(data), pa });
            }
        }
    }
}
