using System;
using NMapper.Builder;

namespace NMapper.Cache
{
    public static class SingleCache<TDest, TSrc>
    {
        public static readonly Func<TSrc, TDest> MapperDelegate;

        static SingleCache()
        {
            MapperDelegate = (Func<TSrc, TDest>)(new SingleBuilder<TDest, TSrc>()).Create();
        }
    }
}
