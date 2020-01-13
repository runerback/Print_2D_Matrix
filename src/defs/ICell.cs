namespace Print2dMatrix
{
    public interface ICell
    {
        int Value { get; }
        ICell[,]? InnerMatrix { get; }
    }
}
