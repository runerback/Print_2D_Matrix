using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace Print2dMatrix
{
    sealed class ComplexMatrixMock : IMatrix
    {
        private readonly IMatrix matrixMock = Mock.Of<IMatrix>();

        public ComplexMatrixMock(int rowCount, int columnCount, int subRowCount, int subColumnCount)
        {
            Assert.That(rowCount, Is.GreaterThan(0), nameof(rowCount));
            Assert.That(columnCount, Is.GreaterThan(0), nameof(columnCount));
            Assert.That(subRowCount, Is.GreaterThan(0), nameof(subRowCount));
            Assert.That(subColumnCount, Is.GreaterThan(0), nameof(subColumnCount));

            Mock.Get(matrixMock)
                .SetupGet(it => it.Value)
                .Returns(CreateCellsMock(rowCount, columnCount, subRowCount, subColumnCount));
        }

        public ICell<int>[,] Value => matrixMock.Value;

        private ICell[,] CreateCellsMock(int rowCount, int columnCount, int subRowCount, int subColumnCount)
        {
            var totalRowCount = rowCount + subRowCount;
            var totalColumnCount = columnCount + subColumnCount;

            var rowDistribution = new RandomDistribution()
                .Generate(rowCount, subRowCount)
                .ToArray();
            var columnDistribution = new RandomDistribution()
                .Generate(columnCount, subColumnCount)
                .ToArray();

            var fullsizeCells = CreateFullSizeMatrix(
                totalRowCount,
                totalColumnCount,
                rowDistribution,
                columnDistribution
            );

            return Shrink(fullsizeCells, rowCount, columnCount, rowDistribution, columnDistribution);
        }

        private ICell[,] CreateFullSizeMatrix(
            int totalRowCount,
            int totalColumnCount,
            IEnumerable<IRange> rowDistribution,
            IEnumerable<IRange> columnDistribution
        )
        {
            var cells = new ICell[totalRowCount, totalColumnCount];

            using (var columnIterator = columnDistribution.GetEnumerator())
            {
                var columnMoved = true;
                var columnCanMove = columnIterator.MoveNext();

                for (int i = 0; i < totalColumnCount; i++)
                {
                    if (
                        columnCanMove &&
                        i >= columnIterator.Current.Start &&
                        i < columnIterator.Current.End
                    )
                    {
                        if (columnMoved)
                            columnMoved = false;

                        SetupComplexColumn(ref cells, i, rowDistribution);
                    }
                    else
                    {
                        if (!columnMoved)
                        {
                            if (columnCanMove)
                                columnCanMove = columnIterator.MoveNext();

                            columnMoved = true;
                        }

                        SetupSimpleColumn(ref cells, i, rowDistribution);
                    }
                }
            }

            return cells;
        }

        private void SetupSimpleColumn(ref ICell[,] cells, int columnIndex, IEnumerable<IRange> rowDistribution)
        {
            var rnd = new Random();

            using var rangeIterator = rowDistribution.GetEnumerator();

            var moved = true;
            var canMove = rangeIterator.MoveNext();

            for (int i = 0, j = cells.GetLength(0); i < j; i++)
            {
                if (
                        canMove &&
                        i >= rangeIterator.Current.Start &&
                        i < rangeIterator.Current.End
                    )
                {
                    if (moved)
                        moved = false;

                    //skip inner row
                    if (i != rangeIterator.Current.Start)
                    {
                        continue;
                    }
                }
                else
                {
                    if (!moved)
                    {
                        if (canMove)
                            canMove = rangeIterator.MoveNext();

                        moved = true;
                    }
                }

                var mock = Mock.Of<ICell>();

                Mock.Get(mock)
                    .SetupGet(it => it.Value)
                    .Returns(rnd.Next(100, 1000));

                cells[i, columnIndex] = mock;
            }
        }

        private void SetupComplexColumn(ref ICell[,] cells, int columnIndex, IEnumerable<IRange> rowDistribution)
        {
            var rnd = new Random();

            using var rangeIterator = rowDistribution.GetEnumerator();

            var moved = true;
            var canMove = rangeIterator.MoveNext();

            for (int i = 0, j = cells.GetLength(0); i < j; i++)
            {
                if (
                        canMove &&
                        i >= rangeIterator.Current.Start &&
                        i < rangeIterator.Current.End
                    )
                {
                    if (moved)
                        moved = false;
                }
                else
                {
                    if (!moved)
                    {
                        if (canMove)
                            canMove = rangeIterator.MoveNext();

                        moved = true;
                    }
                }

                var mock = Mock.Of<ICell>();

                Mock.Get(mock)
                    .SetupGet(it => it.Value)
                    .Returns(rnd.Next(100, 1000));

                cells[i, columnIndex] = mock;
            }
        }

        private ICell[,] Shrink(
            ICell[,] fullsizeCells,
            int rowCount,
            int columnCount,
            IEnumerable<IRange> rowDistribution,
            IEnumerable<IRange> columnDistribution
        )
        {
            var fullsizeRowCount = fullsizeCells.GetLength(0);
            var fullsizeColumnCount = fullsizeCells.GetLength(1);

            throw new NotImplementedException();
        }

        /// <summary>
        /// column based shrink.
        /// which means right column collapse into left base column in each (fullsize) row
        /// </summary>
        private ICell[,] ShrinkOnColumn(
            ICell[,] fullsizeCells,
            int columnCount,
            IEnumerable<IRange> rowDistribution,
            IEnumerable<IRange> columnDistribution
        )
        {
            var fullsizeRowCount = fullsizeCells.GetLength(0);
            var fullsizeColumnCount = fullsizeCells.GetLength(1);

            var cells = new ICell[fullsizeRowCount, columnCount];

            using (var columnIterator = columnDistribution.GetEnumerator())
            {
                var columnMoved = true;
                var columnCanMove = columnIterator.MoveNext();

                int columnIndex = 0;
                for (int i = 0; i < fullsizeColumnCount; i++)
                {
                    if (
                        columnCanMove &&
                        i >= columnIterator.Current.Start &&
                        i < columnIterator.Current.End
                    )
                    {
                        if (columnMoved)
                            columnMoved = false;

                        //shrink
                        if (i == columnIterator.Current.Start)
                        {
                            ShrinkIntoBaseColumn(
                                ref cells,
                                fullsizeCells,
                                rowDistribution,
                                columnIterator.Current,
                                columnIndex
                            );
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (!columnMoved)
                        {
                            if (columnCanMove)
                                columnCanMove = columnIterator.MoveNext();

                            columnMoved = true;
                        }

                        for (int x = 0; x < fullsizeRowCount; x++)
                        {
                            cells[x, columnIndex] = fullsizeCells[x, i];
                        }
                    }

                    columnIndex++;
                }
            }

            return cells;
        }

        private void ShrinkIntoBaseColumn(
            ref ICell[,] shrinkingCells,
            ICell[,] fullsizeCells,
            IEnumerable<IRange> rowDistribution,
            IRange columnRange,
            int columnIndex
        )
        {
            var fullsizeRowCount = fullsizeCells.GetLength(0);

            using var rangeIterator = rowDistribution.GetEnumerator();

            var moved = true;
            var canMove = rangeIterator.MoveNext();

            for (int i = 0; i < fullsizeRowCount; i++)
            {
                if (
                        canMove &&
                        i >= rangeIterator.Current.Start &&
                        i < rangeIterator.Current.End
                    )
                {
                    if (moved)
                        moved = false;

                    //shrink range
                    var cells = new ICell[1, columnRange.End - columnRange.Start];
                    for (int x = 0; x < cells.Length; x++)
                    {
                        cells[0, x] = fullsizeCells[i, x + columnRange.Start];
                    }

                    var shrinkedCell = Mock.Of<ICell>();

                    Mock.Get(shrinkedCell)
                        .SetupGet(it => it.InnerMatrix)
                        .Returns(cells);

                    shrinkingCells[i, columnIndex] = shrinkedCell;
                }
                else
                {
                    if (!moved)
                    {
                        if (canMove)
                            canMove = rangeIterator.MoveNext();

                        moved = true;
                    }
                }
            }
        }

        private ICell[,] ShrinkOnRow(
            ICell[,] columnShrinkedCells,
            int rowCount,
            int fullsizeColumnCount,
            IEnumerable<IRange> rowDistribution,
            IEnumerable<IRange> columnDistribution
        )
        {
            var fullsizeRowCount = columnShrinkedCells.GetLength(0);
            var columnCount = columnShrinkedCells.GetLength(1);

            var cells = new ICell[rowCount, columnCount];

            using (var rangeIterator = rowDistribution.GetEnumerator())
            {
                var moved = true;
                var canMove = rangeIterator.MoveNext();

                int rowIndex = 0;
                for (int i = 0; i < fullsizeRowCount; i++)
                {
                    if (
                            canMove &&
                            i >= rangeIterator.Current.Start &&
                            i < rangeIterator.Current.End
                        )
                    {
                        if (moved)
                            moved = false;

                        //shrink on first row
                        if (i == rangeIterator.Current.Start)
                        {
                            ShirnkIntoBaseRow(
                                ref cells,
                                columnShrinkedCells,
                                columnDistribution,
                                rangeIterator.Current,
                                rowIndex,
                                fullsizeColumnCount
                            );
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (!moved)
                        {
                            if (canMove)
                                canMove = rangeIterator.MoveNext();

                            moved = true;

                            //copy
                            for (int x = 0; x < columnCount; x++)
                            {
                                cells[rowIndex, x] = columnShrinkedCells[i, x];
                            }
                        }
                    }
                }

                rowIndex++;
            }

            return cells;
        }

        private void ShirnkIntoBaseRow(
            ref ICell[,] shrinkingCells,
            ICell[,] columnShrinkedCells,
            IEnumerable<IRange> columnDistribution,
            IRange rowRange,
            int rowIndex,
            int fullsizeColumnCount
        )
        {
            var columnCount = columnShrinkedCells.GetLength(1);
            var shrinkingColumnIndexes = new HashSet<int>(
                ShrinkingColumnIndexIterator(columnDistribution, columnCount)
            );

            for (int i = 0; i < columnCount; i++)
            {
                if (shrinkingColumnIndexes.Contains(i))
                {
                    //shrink to top
                    throw new NotImplementedException();
                }
                else
                {
                    //normal copy
                    throw new NotImplementedException();
                }
            }
        }

        private IEnumerable<int> ShrinkingColumnIndexIterator(
            IEnumerable<IRange> columnDistribution,
            int fullsizeColumnCount
        )
        {
            using (var rangeIterator = columnDistribution.GetEnumerator())
            {
                var moved = true;
                var canMove = rangeIterator.MoveNext();

                int columnIndex = 0;
                for (int i = 0; i < fullsizeColumnCount; i++)
                {
                    if (
                            canMove &&
                            i >= rangeIterator.Current.Start &&
                            i < rangeIterator.Current.End
                        )
                    {
                        if (moved)
                            moved = false;

                        if (i != rangeIterator.Current.Start)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (!moved)
                        {
                            if (canMove)
                                canMove = rangeIterator.MoveNext();

                            moved = true;
                        }
                    }

                    yield return columnIndex++;
                }
            }
        }
    }
}
