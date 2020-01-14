using System;
using System.Collections.Generic;
using System.Linq;

namespace Print2dMatrix
{
    sealed class RandomDistribution
    {
        public IEnumerable<IRange> Generate(int largerCount, int smallerCount)
        {
            var rnd = new Random();

            var smallerCountInBlocks = new List<int>();
            var remainedSamllerCount = smallerCount;
            while (remainedSamllerCount > 0)
            {
                var count = NextInt(rnd, remainedSamllerCount);
                smallerCountInBlocks.Add(count);
                remainedSamllerCount -= count;
            }

            var smallerBlockCount = smallerCountInBlocks.Count;

            var smallerBlockStartIndexSource = Enumerable.Range(0, largerCount).ToList();
            var smallerBlockStartIndexes = new List<int>(smallerBlockCount);

            for (int i = 0; i < smallerBlockCount; i++)
            {
                var nextIndexIndex = NextInt(rnd, smallerBlockStartIndexSource.Count);
                smallerBlockStartIndexes.Add(smallerBlockStartIndexSource[nextIndexIndex]);
                smallerBlockStartIndexSource.RemoveAt(nextIndexIndex);
            }

            using var startIndexIterator = smallerBlockStartIndexes.OrderBy(it => it).GetEnumerator();
            using var largerCountIterator = smallerCountInBlocks.GetEnumerator();
            var prevCountSum = 0;
            while (startIndexIterator.MoveNext())
            {
                var startIndex = startIndexIterator.Current;

                largerCountIterator.MoveNext();
                var largerCountInBlock = largerCountIterator.Current;

                yield return new RangeImpl(
                    startIndex + prevCountSum, 
                    startIndex + prevCountSum + largerCountInBlock
                );

                prevCountSum += largerCountInBlock;
            }
        }

        private int NextInt(Random random, int max)
        {
            return (int)Math.Floor(
                Math.Pow(
                    random.Next(
                        (int)Math.Floor(
                            Math.Pow(max, 2)
                        )
                    ) + 1,
                    0.5
                )
            );
        }

        sealed class RangeImpl : IRange
        {
            public RangeImpl(int start, int end)
            {
                Start = start;
                End = end;
            }

            public int Start { get; set; }

            public int End { get; set; }
        }
    }
}
