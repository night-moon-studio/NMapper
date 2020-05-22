using Natasha;

namespace NMapper.Strategy
{

    public class DefaultNameStrategy : AStrategy
    {

        public override bool Condition(NBuildInfo srcMember, NBuildInfo destMember)
        {

            return srcMember.MemberName == destMember.MemberName;

        }

    }

}
