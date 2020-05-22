using System;
using NMapper;
using SelfTest.Model;

namespace SelfTest
{
    class Program
    {
        static void Main(string[] args)
        {
            SingleSrcModel model = new SingleSrcModel();
            model.Age = 100;
            model.LongLongAgo = 1111111111;
            model.Money = 1000;
            model.Name = "小明";
            model.Time = DateTime.Now;
            model.Title = "gagagag";
            var result = MapperOperator<SingleDestModel>.MapFrom(model);
            var result1 = MapperOperator<SingleConvertDestModel>.MapFrom(result);
            var result2 = MapperOperator<SingleConvertSrcModel>.MapFrom(result1);

            SingleConvertSrcModel a = new SingleConvertSrcModel();
            a.Age = Convert.ToInt32(result1.Age);
            Console.ReadKey();
        }
    }
}
