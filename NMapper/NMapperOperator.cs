using Natasha;
using Natasha.CSharp;
using System;
using System.Collections.Concurrent;

namespace NMapper
{
    public static class MapperOperator<TDest>
    {

        public static TDest MapFrom<TSrc>(TSrc src)
        {

            Type srcType = src.GetType();
            if (srcType == MapperOperator<TSrc, TDest>._srcType)
            {

                return MapperOperator<TSrc, TDest>.Map(src);

            }
            else
            {

                return MapperOperator<TSrc, TDest>.SubClassMap(src, srcType.GetDevelopName());

            }
            
        }

    }


    public static class MapperOperator<TSrc, TDest>
    {

        public static Func<TSrc, TDest> Map;
        internal static readonly Type _srcType;
        private static readonly string _destScript;
        private static readonly ConcurrentDictionary<string, Func<TSrc, TDest>> _cache;
        private static PrecisionCache<Func<TSrc, TDest>> _precisionCache;

        
        static MapperOperator()
        {
            _srcType = typeof(TSrc);
            _destScript = typeof(TDest).GetDevelopName();
            _cache = new ConcurrentDictionary<string, Func<TSrc, TDest>>();
            _precisionCache = _cache.PrecisioTree();
            Complie();
        }




        internal static TDest SubClassMap(TSrc src,string realType)
        {

            var func = _precisionCache.GetValue(realType);
            if (func!=default)
            {

                return func(src);

            }
            else
            {

                var result = NDelegate.RandomDomain().Func<TSrc, TDest>($"return MapperOperator<{realType}, {_destScript}>.Handler(({realType})arg);");
                _cache[realType] = result;
                _precisionCache = _cache.PrecisioTree();
                return result(src);

            }
        }




        public static void Complie()
        {
            Map = NMapperConfiger<TSrc, TDest>.Complie();
        }

    }

}
