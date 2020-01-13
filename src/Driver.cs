using System;

namespace Print2dMatrix
{
    sealed class Driver : IPrinterDriver
    {
        void IPrinterDriver.Print(string? content)
        {
            Console.Write(content);
        }

        void IPrinterDriver.PrintLine(string? content)
        {
            Console.WriteLine(content);
        }
    }
}
