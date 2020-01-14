using System;

namespace Print2dMatrix
{
    sealed class ConsolePrinterDriver : IPrinterDriver
    {
        void IPrinterDriver.Print(string? content)
        {
            Console.Write(content);
        }

        void IPrinterDriver.PrintLine()
        {
            Console.WriteLine();
        }

        void IPrinterDriver.PrintLine(string? content)
        {
            Console.WriteLine(content);
        }
    }
}
