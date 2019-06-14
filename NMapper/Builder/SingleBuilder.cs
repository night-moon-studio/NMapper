using System;
using System.Text;
using Natasha;

namespace NMapper.Builder
{
    public class SingleBuilder<TDest,TSrc>: TypeIterator
    {
        private const string SRC = "s";
        private const string DEST = "d";
        private readonly StringBuilder _script;
        private readonly Type _destType;
        private readonly FastMethodOperator _handler;
        public SingleBuilder()
        {
            _script = new StringBuilder();
            _destType = typeof(TDest);
            _handler = FastMethodOperator.New;
        }

        private void SetField(BuilderInfo info)
        {
            _handler.Using(info.Type);
            var destInfo = _destType.GetField(info.MemberName);
            if (destInfo != null)
            {
                if (destInfo.FieldType == info.Type)
                {
                    _script.AppendLine($"{DEST}.{destInfo.Name}={SRC}.{destInfo.Name};");
                }
            }
        }
        private void SetProperty(BuilderInfo info)
        {
            _handler.Using(info.Type);
            var destInfo = _destType.GetProperty(info.MemberName);
            if (destInfo != null)
            {
                if (destInfo.PropertyType == info.Type && destInfo.CanWrite)
                {
                    _script.AppendLine($"{DEST}.{destInfo.Name}={SRC}.{destInfo.Name};");
                }
            }
        }


        #region MyRegion
        public override void FieldOnceTypeHandler(BuilderInfo info)
        {
            SetField(info);
        }
        public override void FieldArrayOnceTypeHandler(BuilderInfo info)
        {
            SetField(info);
        }
        public override void FieldEntityHandler(BuilderInfo info)
        {
            SetField(info);
        }

        public override void FieldArrayEntityHandler(BuilderInfo info)
        {
            SetField(info);
        }
        public override void FieldCollectionHandler(BuilderInfo info)
        {
            SetField(info);
        }
        public override void FieldDictionaryHandler(BuilderInfo info)
        {
            SetField(info);
        }
        public override void FieldICollectionHandler(BuilderInfo info)
        {
            SetField(info);
        }

        public override void FieldIDictionaryHandler(BuilderInfo info)
        {
            SetField(info);
        }
        #endregion


        #region Property
        public override void PropertyOnceTypeHandler(BuilderInfo info)
        {
            SetProperty(info);
        }
        public override void PropertyArrayOnceTypeHandler(BuilderInfo info)
        {
            SetProperty(info);
        }
        public override void PropertyEntityHandler(BuilderInfo info)
        {
            SetProperty(info);
        }

        public override void PropertyArrayEntityHandler(BuilderInfo info)
        {
            SetProperty(info);
        }
        public override void PropertyCollectionHandler(BuilderInfo info)
        {
            SetProperty(info);
        }
        public override void PropertyDictionaryHandler(BuilderInfo info)
        {
            SetProperty(info);
        }
        public override void PropertyICollectionHandler(BuilderInfo info)
        {
            SetProperty(info);
        }

        public override void PropertyIDictionaryHandler(BuilderInfo info)
        {
            SetProperty(info);
        }

        #endregion


        public override void EntityStartHandler(BuilderInfo info)
        {
            _script.Append($@"if({SRC}==null){{return null;}}");
            _script.Append($"var {DEST} = new {NameReverser.GetName<TDest>()}();");
        }

        public override void EntityReturnHandler(BuilderInfo info)
        {
            _script.Append($"return {DEST};");
        }

        public Delegate Create()
        {
            if (TypeRouter(typeof(TSrc)))
            {
                //创建委托
                _handler.ComplierOption.UseFileComplie();
                return _handler
                            .ClassName($"NMapperSingle{AvailableNameReverser.GetName(typeof(TSrc))}To{AvailableNameReverser.GetName(typeof(TDest))}")
                            .MethodName("Mapper")
                            .Param<TSrc>(SRC)                
                            .MethodBody(_script.ToString())        
                            .Return<TDest>()                    
                           .Complie();
            }
            return null;
        }
    }
}
