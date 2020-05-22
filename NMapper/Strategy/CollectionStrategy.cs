using Natasha;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NMapper.Strategy
{
    public class CollectionStrategy : AStrategy
    {

        public override bool Condition(NBuildInfo srcMember, NBuildInfo destMember)
        {

            return (!srcMember.MemberType.IsImplementFrom(destMember.MemberType)) &&
                (

                    destMember.MemberType != srcMember.MemberType

                ) && (

                    destMember.MemberType.IsImplementFrom(typeof(IEnumerable)) &&
                    !destMember.MemberType.IsImplementFrom(typeof(IDictionary))
                ) && (

                    srcMember.MemberType.IsImplementFrom(typeof(IEnumerable)) &&
                    !srcMember.MemberType.IsImplementFrom(typeof(IDictionary))

               );

        }




        public override (ScriptConnectionType Type, string Script) Handler(NBuildInfo srcMember, NBuildInfo destMember)
        {

            return (ScriptConnectionType.NoAssignment, CollectionScript(srcMember.MemberType,$"arg.{srcMember.MemberName}",destMember.MemberType, $"dest.{destMember.MemberName}"));

        }



        public static string CollectionScript(Type srcType, string srcMember, Type destType, string destMember)
        {

            StringBuilder builder = new StringBuilder();
            builder.AppendLine($@"if({srcMember} != default){{");


            //如果目标成员是泛型集合
            if (destType.IsGenericType)
            {

                builder.AppendLine($"{destMember} =" + CollectionToCollectionScript(srcType, destType, $@"{srcMember}"));

            }
            else
            {

                builder.AppendLine($"{destMember} =" + CollectionToArrayScript(srcType, destType, $@"{srcMember}"));

            }
            builder.Append('}');

            return builder.ToString();
        }




        public static string CollectionToCollectionScript(Type srcType, Type destType, string srcMember)
        {

            //获取集合元素的类型
            Type srcEleType = default;
            if (srcType.IsGenericType)
            {
                srcEleType = srcType.GetGenericArguments()[0];
            }
            else if (srcType.IsArray)
            {
                srcEleType = srcType.GetElementType();
            }


            //如果目标成员是泛型集合
            var destEleType = destType.GetGenericArguments()[0];

            //元素类型相同
            if (destEleType == srcEleType)
            {

                if (destType.IsInterface)
                {

                    return $@"{srcMember};";

                }
                else
                {

                    return $@"new {destType.GetDevelopName()}({srcMember});";

                }

            }
            //元素类型可以相互转换
            else if (
                    (destEleType.IsPrimitive || destEleType == typeof(string)) &&
                    (srcEleType.IsPrimitive || srcEleType == typeof(string))
                    )
            {

                if (destType.IsInterface)
                {

                    return $@"{srcMember}.Select(item=>Convert.To{destEleType.Name}(item));";

                }
                else
                {

                    return $@"new {destType.GetDevelopName()}({srcMember}.Select(item=>Convert.To{destEleType.Name}(item)));";

                }

            }
            //源继承了目标类型
            else if (srcEleType.IsImplementFrom(destEleType))
            {

                if (destType.IsInterface)
                {

                    return $@"{srcMember};";

                }
                else
                {

                    return $@"new {destType.GetDevelopName()}({srcMember});";

                }

            }
            //其他
            else
            {

                if (destType.IsInterface)
                {

                    return $@"{srcMember}.Select(item=>MapperOperator<{destEleType.GetDevelopName()}>.MapFrom(item));";

                }
                else
                {

                    return $@"new {destType.GetDevelopName()}({srcMember}.Select(item=>MapperOperator<{destEleType.GetDevelopName()}>.MapFrom(item)));";

                }

            }


        }




        public static string CollectionToArrayScript(Type srcType, Type destType, string srcMember)
        {

            //获取集合元素的类型
            Type srcEleType = default;
            if (srcType.IsGenericType)
            {
                srcEleType = srcType.GetGenericArguments()[0];
            }
            else if (srcType.IsArray)
            {
                srcEleType = srcType.GetElementType();
            }


            string script = default;
            var destEleType = destType.GetElementType();
            if (destEleType == srcEleType)
            {

                script = $@"{srcMember}.ToArray();";

            }
            else if (srcEleType.IsImplementFrom(destEleType))
            {

                if (srcType.IsArray)
                {

                    script = $@"{srcMember};";

                }
                else
                {
                    script = $@"{srcMember}.Select(item=>MapperOperator<{destEleType.GetDevelopName()}>.MapFrom(item)).ToArray();";
                }

            }
            else if (
                    (destEleType.IsPrimitive || destEleType == typeof(string)) &&
                    (srcEleType.IsPrimitive || srcEleType == typeof(string))
                    )
            {

                script = $@"{srcMember}.Select(item=>Convert.To{destEleType.Name}(item)).ToArray();";

            }
            else
            {

                script = $@"{srcMember}.Select(item=>MapperOperator<{destEleType.GetDevelopName()}>.MapFrom(item)).ToArray();";

            }
            return script;

        }
    }
}
