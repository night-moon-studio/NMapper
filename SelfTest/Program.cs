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
            var result = NMapper<SingleDestModel>.SingleFrom(model);
            var result1 = NMapper<SingleSrcModel>.SingleFrom(result);

            var result2 = NMapper<SingleConvertSrcModel>.SingleFrom(model);
            var result3 = NMapper<SingleConvertDestModel>.SingleConvertFrom(model);
            Console.ReadKey();
        }
    }
}
