namespace Print2dMatrix
{
    public interface IPrinter
    {
        void Print<T>(IMatrix<T> matrix, IPrinterDriver driver);
    }
}
