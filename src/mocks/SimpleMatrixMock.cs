using Moq;
using NUnit.Framework;

namespace Print2dMatrix
{
    sealed class SimpleMatrixMock : IMatrix
    {
        private readonly IMatrix matrixMock = Mock.Of<IMatrix>();

        public SimpleMatrixMock(int rowCount, int columnCount)
        {
            Assert.That(rowCount, Is.GreaterThan(0), nameof(rowCount));
            Assert.That(columnCount, Is.GreaterThan(0), nameof(columnCount));

            Mock.Get(matrixMock)
                .SetupGet(it => it.Value)
                .Returns(CreateCellsMock(rowCount, columnCount));
        }

        public ICell<int>[,] Value => matrixMock.Value;

        private ICell[,] CreateCellsMock(int rowCount, int columnCount)
        {
            var cells = new ICell[rowCount, columnCount];
            for (int i = 0, j = rowCount * columnCount; i < j; i++)
            {
                var cellMock = Mock.Of<ICell>();

                Mock.Get(cellMock)
                    .SetupGet(it => it.Value)
                    .Returns(i);

                cells[i / columnCount, i % columnCount] = cellMock;
            }

            return cells;
        }
    }
}
