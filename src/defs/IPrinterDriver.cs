namespace Print2dMatrix
{
    public interface IPrinterDriver
    {
        void Print(string? content);
        
        void PrintLine();
        void PrintLine(string? content);
    }
}
