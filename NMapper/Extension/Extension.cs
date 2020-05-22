namespace NMapper
{
    public static class NMapperExtension
    {

        public static TDest Map<TSrc,TDest>(this TSrc src)
        {
            return MapperOperator<TDest>.MapFrom(src);
        }
    }
}
