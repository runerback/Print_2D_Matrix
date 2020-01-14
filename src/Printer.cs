using System;
using NUnit.Framework;

namespace Print2dMatrix
{
    sealed class Printer : IPrinter
    {
        public void Print<T>(IMatrix<T> matrix, IPrinterDriver driver)
        {
            Assert.That(matrix, Is.Not.Null);
            Assert.That(driver, Is.Not.Null);

            var cells = matrix.Value;

            for (int i0 = 0, i1 = cells.GetLength(0) - 1; i0 <= i1; i0++, i1--)
            {
                for (int j0 = 0, j1 = cells.GetLength(1) - 1; j0 <= j1; j0++, j1--)
                {
                    if (
                        cells[i0, j0].InnerMatrix?.Length > 0 ||
                        cells[i1, j1].InnerMatrix?.Length > 0
                    )
                    {
                        PrintComplexMatrix(cells, driver);
                        return;
                    }
                }
            }

            PrintSimpleMatrix(cells, driver);
        }

        private void PrintSimpleMatrix<T>(ICell<T>[,] cells, IPrinterDriver driver)
        {
            var rowCount = cells.GetLength(0);
            var columnCount = cells.GetLength(1);

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    driver.Print($"{cells[i, j].Value}\t");
                }
                driver.PrintLine();
            }
        }

        private void PrintComplexMatrix<T>(ICell<T>[,] cells, IPrinterDriver driver)
        {
            var rowCount = cells.GetLength(0);
            var columnCount = cells.GetLength(1);

            int totalRowCount = rowCount;
            for (int i = 0; i < rowCount; i++)
            {
                for (int j0 = 0, j1 = columnCount - 1; j0 <= j1; j0++, j1--)
                {
                    var cell0 = cells[i, j0];
                    if (cell0.InnerMatrix?.Length > 0)
                    {
                        totalRowCount += cell0.InnerMatrix!.GetLength(0) - 1;
                        break;
                    }

                    var cell1 = cells[i, j1];
                    if (cell1.InnerMatrix?.Length > 0)
                    {
                        totalRowCount += cell1.InnerMatrix!.GetLength(0) - 1;
                        break;
                    }
                }
            }

            var posTable = new bool[totalRowCount, columnCount];

            int prevInnerRowCountSum = 0;
            int prevSumRowIndex = 0;

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    var cell = cells[i, j];

                    if (cell.InnerMatrix?.Length > 0)
                    {
                        var innerRowCount = cell.InnerMatrix!.GetLength(0);

                        if (prevSumRowIndex != i)
                        {
                            prevInnerRowCountSum += innerRowCount - 1;
                            prevSumRowIndex = i;
                        }

                        for (int x = 0; x < innerRowCount; x++)
                        {
                            posTable[x + i + prevInnerRowCountSum, j] = true;
                        }
                    }
                    else
                    {
                        posTable[i + prevInnerRowCountSum, j] = true;
                    }
                }
            }

            for (int i = 0; i < totalRowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    driver.Print($"{Convert.ToByte(posTable[i, j])}");
                }
            }
        }
    }
}
