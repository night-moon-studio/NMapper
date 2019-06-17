using System;
using NMapper.Builder;

namespace NMapper.Cache
{
    public static class SimpleCache<TDest, TSrc>
    {
        public static readonly Func<TSrc, TDest> MapperDelegate;

        static SimpleCache()
        {
            MapperDelegate = (Func<TSrc, TDest>)(new SimpleBuilder<TDest, TSrc>()).Create();
        }
    }
}
