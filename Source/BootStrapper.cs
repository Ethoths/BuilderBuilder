using System;

namespace BuilderBuilder
{
    public class BootStrapper
    {
        public static void Main(string[] args)
        {
            new Program().Run(@"C:\Projects\BuilderBuilder\BuilderBuilder\Test\TestModels\bin\TestModels.dll");
            Console.ReadKey();
        }
    }
}
