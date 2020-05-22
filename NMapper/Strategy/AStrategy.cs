using Natasha;
using NMapper.Strategy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NMapper
{

    public abstract class AStrategy
    {

        private readonly LinkedList<AStrategy> _or_strategies;
        private readonly LinkedList<AStrategy> _and_strategies;


        public AStrategy()
        {

            _or_strategies = new LinkedList<AStrategy>();
            _and_strategies = new LinkedList<AStrategy>();
            
        }




        /// <summary>
        /// 增加或条件
        /// </summary>
        /// <typeparam name="TStrategy">策略</typeparam>
        /// <returns></returns>
        public AStrategy Or<TStrategy>() where TStrategy : AStrategy, new()
        {

            return Or(new TStrategy());

        }
        public AStrategy Or(AStrategy strategy)
        {

            lock (_or_strategies)
            {
                _or_strategies.AddLast(strategy);
            }
            return this;

        }




        /// <summary>
        /// 增加与条件
        /// </summary>
        /// <param name="strategy">策略</param>
        /// <returns></returns>
        public AStrategy And<TStrategy>() where TStrategy : AStrategy, new()
        {

            return And(new TStrategy());

        }
        public AStrategy And(AStrategy strategy)
        {

            lock (_and_strategies)
            {
                _and_strategies.AddLast(strategy);
            }
            return this;

        }




        /// <summary>
        /// 移除或策略
        /// </summary>
        /// <param name="strategy"></param>
        /// <returns></returns>
        public AStrategy RemoveOr(AStrategy strategy)
        {

            return RemoveOr(strategy.GetType());

        }
        public AStrategy RemoveOr<TStrategy>() where TStrategy : AStrategy, new()
        {

            return RemoveOr(typeof(TStrategy));

        }
        public AStrategy RemoveOr(Type strategyType)
        {

            lock (_or_strategies)
            {

                var temp = _or_strategies.First;
                for (int i = 0; i < _or_strategies.Count; i += 1)
                {

                    if (temp.Value.GetType() == strategyType)
                    {
                        _or_strategies.Remove(temp);
                        return this;
                    }
                    temp = temp.Next;

                }

            }
            return this;

        }


        /// <summary>
        /// 移除与策略
        /// </summary>
        /// <param name="strategy"></param>
        /// <returns></returns>
        public AStrategy RemoveAnd(AStrategy strategy)
        {

            return RemoveAnd(strategy.GetType());

        }
        public AStrategy RemoveAnd<TStrategy>() where TStrategy : AStrategy, new()
        {

            return RemoveAnd(typeof(TStrategy));

        }
        public AStrategy RemoveAnd(Type strategyType)
        {

            lock (_and_strategies)
            {

                var temp = _and_strategies.First;
                for (int i = 0; i < _or_strategies.Count; i += 1)
                {

                    if (temp.Value.GetType() == strategyType)
                    {
                        _and_strategies.Remove(temp);
                        return this;
                    }
                    temp = temp.Next;

                }

            }
            return this;

        }




        /// <summary>
        /// 检测条件
        /// </summary>
        /// <param name="srcMember">源对象属性信息</param>
        /// <param name="destMember">生成对象的属性信息</param>
        /// <returns></returns>
        public virtual bool Condition(NBuildInfo srcMember, NBuildInfo destMember) { return false; }
        /// <summary>
        /// 生成脚本
        /// </summary>
        /// <param name="srcMember">源对象属性信息</param>
        /// <param name="destMember">生成对象的属性信息</param>
        /// <returns></returns>
        public virtual (ScriptConnectionType Type, string Script) Handler(NBuildInfo srcMember, NBuildInfo destMember)
        {
            return default;
        }




        /// <summary>
        /// 对成员进行策略检查
        /// </summary>
        /// <param name="srcMember"></param>
        /// <param name="destMember"></param>
        /// <returns></returns>
        internal bool Check(NBuildInfo srcMember, NBuildInfo destMember, ref (ScriptConnectionType Type, string Script) handler)
        {

            //当前策略判断
            bool shut = Condition(srcMember, destMember);


            //或策略链判断
            lock (_or_strategies)
            {

                foreach (var item in _or_strategies)
                {

                    shut = shut || item.Check(srcMember, destMember, ref handler);

                }

            }


            if (shut)
            {

                //与策略链判断
                lock (_and_strategies)
                {

                    foreach (var item in _and_strategies)
                    {

                        shut = shut && item.Check(srcMember, destMember, ref handler);

                    }


                }

            }


            //如果当前与子链策略全通过了，则去获取结果
            if (shut)
            {

                if (handler == default)
                {
                    handler = Handler(srcMember, destMember);
                }

            }

            return shut;

        }




        public static AStrategy operator |(AStrategy pre, AStrategy after)
        {
            return pre.Or(after);
        }
        public static AStrategy operator &(AStrategy pre, AStrategy after)
        {
            return pre.And(after);
        }

    }




    public enum ScriptConnectionType
    {

        None,
        /// <summary>
        /// src.Member = {You need write the Script by yourself}; 
        /// </summary>
        NeedAssignment,
        /// <summary>
        /// {You need write the all code by yourself};
        /// </summary>
        NoAssignment

    }

}
