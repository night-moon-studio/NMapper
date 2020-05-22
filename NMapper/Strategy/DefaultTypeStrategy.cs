using Natasha;

namespace NMapper.Strategy
{
    public class DefaultTypeStrategy : AStrategy
    {

        public override bool Condition(NBuildInfo srcMember, NBuildInfo destMember)
        {

            return srcMember.MemberType == destMember.MemberType;

        }

    }
}
