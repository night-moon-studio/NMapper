using Natasha;
using System;

namespace NMapper
{
    public class DefaultStrategy : AStrategy
    {

        public override (ScriptConnectionType Type, string Script) Handler(NBuildInfo srcMember, NBuildInfo destMember)
        {
            return (ScriptConnectionType.NeedAssignment, $"arg.{srcMember.MemberName}");
        }

    }
}
