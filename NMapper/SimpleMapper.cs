using System;
using System.Collections.Generic;
using System.Text;
using NMapper.Cache;

namespace NMapper
{
    public class SimpleMapper<TDest>
    {

        public static TDest From<TSrc>(TSrc srcInstance)
        {
            return SimpleCache<TDest, TSrc>.MapperDelegate(srcInstance);
        }
    }
}
