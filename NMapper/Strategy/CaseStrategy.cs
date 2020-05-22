using Natasha;

namespace NMapper.Strategy
{

    public class CaseStrategy : AStrategy
    {

        public override bool Condition(NBuildInfo srcMember, NBuildInfo destMember)
        {
            return srcMember.MemberName.ToLower() == destMember.MemberName.ToLower();
        }

    }

}
