namespace Print2dMatrix
{
    public interface ICell<T>
    {
        T Value {get;}
        ICell<T>[,]? InnerMatrix { get; }
    }

    public interface ICell : ICell<int>
    {
    }
}
