namespace Print2dMatrix
{
    public interface IMatrix<T>
    {
        ICell<T>[,] Value { get; }
    }

    public interface IMatrix : IMatrix<int>
    {
    }
}
