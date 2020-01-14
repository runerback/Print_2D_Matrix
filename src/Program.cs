using System;

namespace Print2dMatrix
{
    sealed class Program
    {
        static void Main()
        {
            try
            {
                var driver = new ConsolePrinterDriver();
                var printer = new Printer();

                var simpleMatrix = new SimpleMatrixMock(3, 5);
                printer.Print(simpleMatrix, driver);
            }
            catch (Exception ex)
            {
                var fg = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                
                Console.WriteLine(ex);

                Console.ForegroundColor = fg;
            }
        }
    }
}
