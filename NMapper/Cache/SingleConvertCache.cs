using System;
using NMapper.Builder;

namespace NMapper.Cache
{
    public static class SingleConvertCache<TDest, TSrc>
    {
        public static readonly Func<TSrc, TDest> MapperDelegate;

        static SingleConvertCache()
        {
            MapperDelegate = (Func<TSrc, TDest>)(new SingleConvertBuilder<TDest, TSrc>()).Create();
        }
    }
}
