using Natasha;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.Concurrent;

namespace NMapper.Strategy
{
    public class DictionaryStrategy : AStrategy
    {

        public readonly ConcurrentDictionary<string, string> MappingCache;
        public DictionaryStrategy():base()
        {
            MappingCache = new ConcurrentDictionary<string, string>();
        }




        public override bool Condition(NBuildInfo srcMember, NBuildInfo destMember)
        {
            return (!srcMember.MemberType.IsImplementFrom(destMember.MemberType)) &&
                destMember.MemberType != srcMember.MemberType &&
                srcMember.MemberType.IsImplementFrom(typeof(IDictionary<,>)) &&
                destMember.MemberType.IsImplementFrom<IEnumerable>();
        }




        public override (ScriptConnectionType Type, string Script) Handler(NBuildInfo srcMember, NBuildInfo destMember)
        {

            var srcMemberScript = $"arg.{srcMember.MemberName}";
            var destMemberScrtpt = $"dest.{destMember.MemberName}";


            var destCacheKey = $"-dict-{destMember.MemberName}-key";
            var destCacheValue = $"-dict-{destMember.MemberName}-value";


            StringBuilder builder = new StringBuilder();
            builder.AppendLine($@"if({srcMemberScript} != default){{");
            if (destMember.MemberType.IsImplementFrom<IDictionary>())
            {

                if (destMember.MemberType.IsInterface)
                {

                    var destTypes = destMember.MemberType.GetGenericArguments();
                    var destKeyType = destTypes[0];
                    var destValueType = destTypes[1];
                    if (srcMember.MemberType == typeof(IDictionary))
                    {

                        var tempVarName = $"@temp_dict_{destMember.MemberName}";
                        if (!destMember.MemberType.IsInterface)
                        {

                            //IDictionary -> Dictionary<key,value>
                            builder.AppendLine($@"var {tempVarName}= {srcMemberScript}.Keys;
                                                {destMemberScrtpt} = new {destMember.MemberTypeName}();
                                                foreach(var item in {tempVarName}){{");
                            var key = MappingCache.ContainsKey(destCacheKey) ? default : $"({ destKeyType.GetDevelopName()})item";
                            var value = MappingCache.ContainsKey(destCacheValue) ? default : $"({destValueType.GetDevelopName()})({srcMemberScript}[item])";
                            builder.AppendLine($@"{destMemberScrtpt}[{key}] = {value};");
                            builder.AppendLine("}");

                        }
                        else
                        {

                            //IDictionary -> IDictionary<key,value>
                            builder.AppendLine($@"var {tempVarName} = {srcMemberScript}.Keys;
                                                {destMemberScrtpt} = new Dictionary<{destKeyType.GetDevelopName()},{destValueType.GetDevelopName()}>();
                                                foreach(var item in {tempVarName}){{");
                            var key = MappingCache.ContainsKey(destCacheKey) ? default : $"({ destKeyType.GetDevelopName()})item";
                            var value = MappingCache.ContainsKey(destCacheValue) ? default : $"({destValueType.GetDevelopName()})({srcMemberScript}[item])";
                            builder.AppendLine($@"{destMemberScrtpt}[{key}] = {value};");
                            builder.AppendLine("}");

                        }
                       
                    }
                    else
                    {

                        //IDictionary<key1,value1> -> IDictionary<key,value>
                        builder.Append(destMemberScrtpt + " = ");
                        if (!destMember.MemberType.IsInterface)
                        {
                            builder.Append($"new {destMember.MemberTypeName}");
                        }
                        builder.Append($"({srcMemberScript}.Select(item=>KeyValuePair.Create(");


                        var srcTypes = srcMember.MemberType.GetGenericArguments();
                        var srcKeyType = srcTypes[0];
                        var srctValueType = srcTypes[1];


                        if (destKeyType != srcKeyType)
                        {

                            if (MappingCache.ContainsKey(destCacheKey))
                            {

                                builder.Append(MappingCache[destCacheKey]);

                            }
                            else if (
                                (destKeyType.IsPrimitive || destKeyType == typeof(string)) &&
                                (srcKeyType.IsPrimitive || srcKeyType == typeof(string)))
                            {

                                builder.Append($"Convert.To{destKeyType.Name}(item.Key)");

                            }
                            else
                            {

                                builder.Append($"MapperOperator<{destKeyType.GetDevelopName()}>.MapFrom(item.Key)");

                            }

                        }
                        else
                        {

                            builder.Append($"item.Key");

                        }


                        if (destValueType != srctValueType)
                        {

                            if (MappingCache.ContainsKey(destCacheValue))
                            {

                                // key = Key.Name
                                builder.Append(MappingCache[destCacheValue]);

                            }
                            else if (
                                (destValueType.IsPrimitive || destValueType == typeof(string)) &&
                                (srctValueType.IsPrimitive || srctValueType == typeof(string)))
                            {

                                // key = Convert.Int32(Key)
                                builder.Append($"Convert.To{destValueType.Name}(item.Value)");

                            }
                            else
                            {

                                // Key = MapperOperator<T>.MapFrom(Key);
                                builder.Append($"MapperOperator<{destValueType.GetDevelopName()}>.MapFrom(item.Value)");

                            }

                        }
                        else
                        {

                            builder.Append($"item.Value");

                        }


                        builder.Append(')');
                    }

                }

            }
            else
            {

                var destTypes = destMember.MemberType.GetGenericArguments();
                var destKeyType = destTypes[0];
                var destValueType = destTypes[1];

                if (destMember.MemberType.IsInterface)
                {

                    if (srcMember.MemberType == typeof(IDictionary) || 
                            (destMember.MemberType == typeof(IEnumerable) || 
                            destMember.MemberType == typeof(ICollection)))
                    {

                        //IDictionary -> ICollection / IEnumable
                        if (MappingCache.ContainsKey(destCacheKey))
                        {

                            var tempScript = MappingCache[destCacheKey];
                            if (tempScript == default)
                            {

                                builder.Append($"{destMemberScrtpt} = {srcMemberScript}.Keys;");

                            }
                            else
                            {

                                builder.Append($"{destMemberScrtpt} = {tempScript};");

                            }

                        }
                        else if (MappingCache.ContainsKey(destCacheValue))
                        {

                            var tempScript = MappingCache[destCacheValue];
                            if (tempScript == default)
                            {

                                builder.Append($"{destMemberScrtpt} = {srcMemberScript}.Values;");

                            }
                            else
                            {

                                builder.Append($"{destMemberScrtpt} = {tempScript};");

                            }

                        }
                        else
                        {

                            builder.Append($"{destMemberScrtpt} = {srcMemberScript}.Values;");

                        }

                    }
                    else
                    {

                        //IDictionary<k,v> -> ICollection / IEnumable
                        if (MappingCache.ContainsKey(destCacheKey))
                        {

                            var tempScript = MappingCache[destCacheKey];
                            if (tempScript == default)
                            {

                                builder.Append($"{destMemberScrtpt} = {srcMemberScript}.Keys;");

                            }
                            else
                            {

                                builder.Append($"{destMemberScrtpt} = {tempScript};");

                            }

                        }
                        else if (MappingCache.ContainsKey(destCacheValue))
                        {

                            var tempScript = MappingCache[destCacheValue];
                            if (tempScript == default)
                            {

                                builder.Append($"{destMemberScrtpt} = {srcMemberScript}.Values;");

                            }
                            else
                            {

                                builder.Append($"{destMemberScrtpt} = {tempScript};");

                            }

                        }
                        else
                        {

                            builder.Append($"{destMemberScrtpt} = {srcMemberScript}.Values;");

                        }

                    }

                }


                //Dict -> ICollection

                    if (!destMember.MemberType.IsInterface)
                    {

                    if (MappingCache.ContainsKey(destCacheKey))
                    {
                    }
                    }

                        else if (MappingCache.ContainsKey(destCacheValue))
                        {

                        }
                        //IDictionary -> Dictionary<key,value>
                        builder.AppendLine($@"var temp{destMember.MemberName} = {srcMemberScript}.Keys;
                                                dest.{destMember.MemberName} = new {destMember.MemberTypeName}();
                                                foreach(var item in temp{destMember.MemberName}){{");
                        var key = MappingCache.ContainsKey($"-dict-{destMember.MemberName}-key") ? default : $"({ destKeyType.GetDevelopName()})item";
                        var value = MappingCache.ContainsKey($"-dict-{destMember.MemberName}-value") ? default : $"({destValueType.GetDevelopName()})({srcMemberScript}[item])";
                        builder.AppendLine($@"dest.{destMember.MemberName}[{key}] = {value};");
                        builder.AppendLine("}");

                    }
                    else
                    {

                        //IDictionary -> IDictionary<key,value>
                        builder.AppendLine($@"  var temp{destMember.MemberName} = {srcMemberScript}.Keys;
                                                var tempDict{destMember.MemberName} = new Dictionary<{destKeyType.GetDevelopName()},{destValueType.GetDevelopName()}>();
                                                foreach(var item in temp{destMember.MemberName}){{");
                        var key = MappingCache.ContainsKey($"-dict-{destMember.MemberName}-key") ? default : $"({ destKeyType.GetDevelopName()})item";
                        var value = MappingCache.ContainsKey($"-dict-{destMember.MemberName}-value") ? default : $"({destValueType.GetDevelopName()})({srcMemberScript}[item])";
                        builder.AppendLine($@"tempDict[{key}] = {value};");
                        builder.AppendLine("}");
                        builder.AppendLine($"dest.{destMember.MemberName} = tempDict{destMember.MemberName};");

                    }

                else
                {

                    //IDictionary<key1,value1> -> IDictionary<key,value>
                    if (!destMember.MemberType.IsInterface)
                    {
                        builder.Append($"new {destMember.MemberTypeName}");
                    }
                    builder.Append($"({srcMemberScript}.Select(item=>KeyValuePair.Create(");


                    var srcTypes = srcMember.MemberType.GetGenericArguments();
                    var srcKeyType = srcTypes[0];
                    var srctValueType = srcTypes[1];


                    if (destKeyType != srcKeyType)
                    {

                        var tempKey = $"-dict-{destMember.MemberName}-key";
                        if (MappingCache.ContainsKey(tempKey))
                        {

                            builder.Append(MappingCache[tempKey]);

                        }
                        else if (
                            (destKeyType.IsPrimitive || destKeyType == typeof(string)) &&
                            (srcKeyType.IsPrimitive || srcKeyType == typeof(string)))
                        {

                            builder.Append($"Convert.To{destKeyType.Name}(item.Key)");

                        }
                        else
                        {

                            builder.Append($"MapperOperator<{destKeyType.GetDevelopName()}>.MapFrom(item.Key)");

                        }

                    }
                    else
                    {

                        builder.Append($"item.Key");

                    }


                    if (destValueType != srctValueType)
                    {


                        var tempKey = $"-dict-{destMember.MemberName}-value";
                        if (MappingCache.ContainsKey(tempKey))
                        {

                            // key = Key.Name
                            builder.Append(MappingCache[tempKey]);

                        }
                        else if (
                            (destValueType.IsPrimitive || destValueType == typeof(string)) &&
                            (srctValueType.IsPrimitive || srctValueType == typeof(string)))
                        {

                            // key = Convert.Int32(Key)
                            builder.Append($"Convert.To{destValueType.Name}(item.Value)");

                        }
                        else
                        {

                            // Key = MapperOperator<T>.MapFrom(Key);
                            builder.Append($"MapperOperator<{destValueType.GetDevelopName()}>.MapFrom(item.Value)");

                        }

                    }
                    else
                    {

                        builder.Append($"item.Value");

                    }


                    builder.Append(')');
                }

            return base.Handler(srcMember, destMember);

        }




    }
}
