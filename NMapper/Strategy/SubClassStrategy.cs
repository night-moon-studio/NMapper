using Natasha;
using System;
using System.Collections.Generic;
using System.Text;

namespace NMapper.Strategy
{
    public class SubClassStrategy:AStrategy
    {
        public override bool Condition(NBuildInfo srcMember, NBuildInfo destMember)
        { 
            return srcMember.MemberType.IsImplementFrom(destMember.MemberType) || srcMember.MemberType.IsSubclassOf(destMember.MemberType);
        }
    }
}
