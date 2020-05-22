using Natasha;
using System;
using System.Collections.Generic;
using System.Text;

namespace NMapper.Strategy
{
    public class PrimitiveTypeConverterStrategy : AStrategy
    {

        public override bool Condition(NBuildInfo srcMember, NBuildInfo destMember)
        {

            return (srcMember.MemberType != destMember.MemberType &&
                (srcMember.MemberType.IsPrimitive || srcMember.MemberType == typeof(string)) &&
                (destMember.MemberType.IsPrimitive || destMember.MemberType == typeof(string)));

        }

        public override (ScriptConnectionType Type, string Script) Handler(NBuildInfo srcMember, NBuildInfo destMember)
        {
            return (ScriptConnectionType.NeedAssignment, $"Convert.To{destMember.MemberType.Name}(arg.{srcMember.MemberName})");
        }
    }
}
