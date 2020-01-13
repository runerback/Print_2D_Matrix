namespace Print2dMatrix
{
    public interface IPrinter
    {
        void Print(IMatrix matrix, IPrinterDriver driver);
    }
}
