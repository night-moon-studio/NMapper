using System;
using System.Collections.Concurrent;
using System.Text;

namespace NMapper.Template
{
    public static class NMapperTemplate<TSrc, TDest>
    {

        private static string _init;
        private static readonly ConcurrentDictionary<string, string> _needAssignmentMemberMappings;
        private static readonly ConcurrentDictionary<string, string> _noAssignmentMemberMappings;
        static NMapperTemplate()
        {

            SetInit(item => $"new {item}();");
            _needAssignmentMemberMappings = new ConcurrentDictionary<string, string>();
            _noAssignmentMemberMappings = new ConcurrentDictionary<string, string>();

        }




        /// <summary>
        /// 直接赋值一个初始化表达式，比如: var dest = new Test();
        /// </summary>
        /// <param name="init">初始化表达式的字符串</param>
        internal static void SetInit(string init)
        {

            _init = init;

        }
        /// <summary>
        /// var dest = {initFunc([script - destType])}; Func参数为目标初始化的类型。
        /// </summary>
        /// <param name="initFunc">Func参数为目标初始化的类型。</param>
        internal static void SetInit(Func<string, string> initFunc)
        {
            
            _init = "var dest = " + initFunc(typeof(TDest).GetDevelopName());
            StringBuilder builder = new StringBuilder();
            if (!typeof(TSrc).IsValueType && !(typeof(TSrc) == typeof(string)))
            {
                builder.AppendLine(@"if(arg == default){return dest;}");
            }

        }




        /// <summary>
        /// dest.{destMember} = {srcScript};
        /// </summary>
        /// <param name="destMember">目标成员</param>
        /// <param name="srcScript">赋值表达式</param>
        internal static void AssignmentMapping(string destMember, string srcScript)
        {

            _needAssignmentMemberMappings[destMember] = srcScript;

        }
        /// <summary>
        /// {destScript};
        /// </summary>
        /// <param name="destMember">目标成员</param>
        /// <param name="destScript">段代码</param>
        internal static void DirectMapping(string destMember, string destScript)
        {

            _noAssignmentMemberMappings[destMember] = destScript;

        }




        /// <summary>
        /// 获取脚本
        /// </summary>
        public static string Script
        {

            get
            {

                StringBuilder builder = new StringBuilder(_init);
                foreach (var item in _needAssignmentMemberMappings)
                {

                    if (item.Value != default)
                    {
                        builder.AppendLine($"dest.{item.Key} = {item.Value};");
                    }

                }
                foreach (var item in _noAssignmentMemberMappings)
                {

                    if (item.Value != default)
                    {
                        builder.Append(item.Value);
                        if (!item.Value.EndsWith(";"))
                        {
                            builder.Append(';');
                        }
                    }

                }
                builder.AppendLine("return dest;");
                return builder.ToString();

            }

        }

    }

}
