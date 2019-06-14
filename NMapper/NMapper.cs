using System;
using System.Collections.Generic;
using System.Text;
using NMapper.Cache;

namespace NMapper
{
    public class NMapper<TDest>
    {
        public static TDest SingleFrom<TSrc>(TSrc srcInstance)
        {
            return SingleCache<TDest, TSrc>.MapperDelegate(srcInstance);
        }

        public static TDest SingleConvertFrom<TSrc>(TSrc srcInstance)
        {
            return SingleConvertCache<TDest, TSrc>.MapperDelegate(srcInstance);
        }
    }
}
