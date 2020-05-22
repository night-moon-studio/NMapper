using Natasha;
using Natasha.CSharp;
using NMapper.Strategy;
using NMapper.Template;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;

namespace NMapper
{

    public static class NMapperConfiger
    {

        public static readonly ConcurrentBag<Action<AStrategy>> NameStrategies;
        public static readonly ConcurrentBag<Action<AStrategy>> TypeStrategies;

        static NMapperConfiger()
        {

            NameStrategies = new ConcurrentBag<Action<AStrategy>>();
            TypeStrategies = new ConcurrentBag<Action<AStrategy>>();

        }




        public static void GlobalNameStrategy(Action<AStrategy> action)
        {
            NameStrategies.Add(action);
        }
        public static void GlobalTypeStrategy(Action<AStrategy> action)
        {
            TypeStrategies.Add(action);
        }

    }




    public static class NMapperConfiger<TSrc, TDest>
    {

        private static readonly ConcurrentBag<Action> _configs;
        public static readonly AStrategy Strategy;
        public static readonly AStrategy DefaultNameStrategy;
        public static readonly AStrategy DefaultTypeStrategy;

        static NMapperConfiger()
        {

            _configs = new ConcurrentBag<Action>();
            Strategy = new DefaultStrategy();
            DefaultNameStrategy = new DefaultNameStrategy();
            DefaultTypeStrategy = new DefaultTypeStrategy();
            Strategy.Or(DefaultNameStrategy).And(DefaultTypeStrategy);
            foreach (var item in NMapperConfiger.NameStrategies)
            {
                item(DefaultNameStrategy);
            }
            foreach (var item in NMapperConfiger.TypeStrategies)
            {
                item(DefaultTypeStrategy);
            }


            NMapperConfiger.GlobalNameStrategy(strategy =>
            {

                strategy.Or<CaseStrategy>();

            });
            NMapperConfiger.GlobalTypeStrategy(strategy =>
            {

                strategy
                .Or<SubClassStrategy>()
                .Or<PrimitiveTypeConverterStrategy>()
                .Or<CollectionStrategy>();

            });

        }




        ///// <summary>
        ///// 装载或策略
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        //public static AStrategy EquipOr<T>() where T : AStrategy, new()
        //{

        //    Strategy.Or<T>();
        //    return Strategy;

        //}
        //public static AStrategy EquipOr(AStrategy strategy)
        //{

        //    Strategy.Or(strategy);
        //    return Strategy;

        //}
        ///// <summary>
        ///// 装载与策略
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        //public static AStrategy EquipAnd<T>() where T : AStrategy, new()
        //{

        //    Strategy.And<T>();
        //    return Strategy;

        //}
        //public static AStrategy EquipAnd(AStrategy strategy)
        //{

        //    Strategy.And(strategy);
        //    return Strategy;

        //}




        /// <summary>
        /// 直接赋值一个初始化表达式，比如: var dest = new Test();
        /// </summary>
        /// <param name="init">初始化表达式的字符串</param>
        internal static void SetInit(string init)
        {

            _configs.Add(() =>
            {

                NMapperTemplate<TSrc, TDest>.SetInit(init);

            });

        }
        /// <summary>
        /// var dest = {initFunc([script - destType])}; Func参数为目标初始化的类型。
        /// </summary>
        /// <param name="initFunc">Func参数为目标初始化的类型。</param>
        public static void SetInit(Func<string, string> initFunc)
        {

            _configs.Add(() =>
            {

                NMapperTemplate<TSrc, TDest>.SetInit(initFunc);

            });

        }




        /// <summary>
        /// 在映射时忽略某个成员
        /// </summary>
        /// <param name="destMember">目标成员名</param>
        public static void Ignore(string destMember)
        {

            _configs.Add(() =>
            {

                NMapperTemplate<TSrc, TDest>.DirectMapping(destMember, default);
                NMapperTemplate<TSrc, TDest>.AssignmentMapping(destMember, default);

            });

        }
        public static void Ignore(Expression<Func<TDest, object>> expression)
        {

            var body = expression.Body.ToString();
            Ignore(body.Split('.')[1]);

        }




        /// <summary>
        /// 将目标成员设置为初始值
        /// </summary>
        /// <param name="destMember">目标成员名</param>
        public static void Default(string destMember)
        {

            _configs.Add(() =>
            {

                NMapperTemplate<TSrc, TDest>.AssignmentMapping(destMember, "default");
                NMapperTemplate<TSrc, TDest>.DirectMapping(destMember, default);

            });

        }
        public static void Default(Expression<Func<TDest, object>> expression)
        {

            var body = expression.Body.ToString();
            Default(body.Split('.')[1]);

        }




        /// <summary>
        /// dest.{destMember} = {srcScript};
        /// </summary>
        /// <param name="destMember">目标成员</param>
        /// <param name="srcScript">赋值表达式</param>
        public static void AssignmentMapping(string destMember, string srcScript)
        {

            _configs.Add(() =>
            {

                NMapperTemplate<TSrc, TDest>.AssignmentMapping(destMember, srcScript);

            });

        }
        public static void AsseignmentMapping(Expression<Func<TDest, object>> destMemberExpression, string srcScript)
        {

            var body = destMemberExpression.Body.ToString();
            AssignmentMapping(body.Split('.')[1], srcScript);

        }
        public static void AsseignmentMapping(string destMember, Expression<Func<TSrc, object>> srcScriptExpression)
        {

            var body = srcScriptExpression.Body.ToString();
            AssignmentMapping(destMember, body.Split('.')[1]);

        }
        public static void AsseignmentMapping(Expression<Func<TDest, object>> destMemberExpression, Expression<Func<TSrc, object>> srcScriptExpression)
        {

            var destBody = destMemberExpression.Body.ToString();
            var srcBody = srcScriptExpression.Body.ToString();
            AssignmentMapping(destBody.Split('.')[1], srcBody.Split('.')[1]);

        }




        /// <summary>
        /// {destScript};
        /// </summary>
        /// <param name="destMember">目标成员</param>
        /// <param name="destScript">段代码</param>
        public static void DirectMapping(string destMember, string destScript)
        {

            _configs.Add(() =>
            {

                NMapperTemplate<TSrc, TDest>.DirectMapping(destMember, destScript);

            });

        }
        public static void DirectMapping(Expression<Func<TDest, object>> destMemberExpression, string srcScript)
        {

            var body = destMemberExpression.Body.ToString();
            DirectMapping(body.Split('.')[1], srcScript);

        }




        private static void Handler()
        {

            var srcMembers = NBuildInfo.GetInfos<TSrc>();
            var destMembers = NBuildInfo.GetInfos<TDest>();
            var destList = destMembers.Values.Where(item => item != null && item.CanRead);
            foreach (var itemDest in destList)
            {

                foreach (var item in srcMembers)
                {

                    var itemSrc = item.Value;
                    if (itemSrc != null && itemSrc.CanWrite)
                    {

                        (ScriptConnectionType Type, string Script) result = default;
                        if (Strategy.Check(itemSrc, itemDest, ref result))
                        {

                            if (result != default)
                            {

                                if (result.Type == ScriptConnectionType.NoAssignment)
                                {
                                    NMapperTemplate<TSrc, TDest>.DirectMapping(itemDest.MemberName, result.Script);
                                }
                                else if (result.Type == ScriptConnectionType.NeedAssignment)
                                {
                                    NMapperTemplate<TSrc, TDest>.AssignmentMapping(itemDest.MemberName, result.Script);
                                }

                            }

                        }

                    }

                }

            }
            foreach (var item in _configs)
            {
                item();
            }

        }




        public static Func<TSrc, TDest> Complie()
        {

            Handler();
            var func = NDelegate.RandomDomain().Func<TSrc, TDest>(NMapperTemplate<TSrc, TDest>.Script, typeof(TSrc), typeof(TDest));
            MapperOperator<TSrc, TDest>.Map = func;
            return func;

        }

    }

}
