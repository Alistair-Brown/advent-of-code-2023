using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AOC.DayFourteen
{
    struct RoundBoulderSlide
    {
        public int firstBoulderPosition;
        public int numberOfBoulders;
    }

    internal class BoulderField
    {
        private List<List<RoundBoulderSlide>> boulderSlidesInEachColumn;
        private int numberOfRows;

        private char[][] rawBoulderField;

        public BoulderField(string[] inputLines)
        {
            rawBoulderField = new char[inputLines.Length][];
            for (int ii = 0; ii < inputLines.Length; ++ii)
            {
                rawBoulderField[ii] = new char[inputLines[ii].Length];
                for (int jj = 0; jj < inputLines[ii].Length; ++jj)
                {
                    rawBoulderField[ii][jj] = inputLines[ii][jj];
                }
            }

            numberOfRows = inputLines.Length;

            boulderSlidesInEachColumn = new List<List<RoundBoulderSlide>>();

            for (int columnIndex = 0; columnIndex < inputLines[0].Length; ++columnIndex)
            {
                boulderSlidesInEachColumn.Add(new List<RoundBoulderSlide>());

                RoundBoulderSlide currentPortion = new RoundBoulderSlide() { firstBoulderPosition = 0, numberOfBoulders = 0 };

                for (int rowIndex = 0; rowIndex < inputLines.Length; ++rowIndex)
                {
                    if (inputLines[rowIndex][columnIndex] == 'O')
                    {
                        ++currentPortion.numberOfBoulders;
                    }
                    else if (inputLines[rowIndex][columnIndex] == '#')
                    {
                        boulderSlidesInEachColumn[columnIndex].Add(currentPortion);
                        currentPortion = new RoundBoulderSlide { firstBoulderPosition = rowIndex + 1, numberOfBoulders = 0 };
                    }
                }

                boulderSlidesInEachColumn[columnIndex].Add(currentPortion);
            }
        }

        public void PerformFullCycle()
        {
            PerformNorthSlide();
            PerformWestSlide();
            PerformSouthSlide();
            PerformEastSlide();
        }

        public void PerformNorthSlide()
        {
            for (int columnIndex = 0; columnIndex < rawBoulderField[0].Length; ++columnIndex)
            {
                int currentSlideStartIndex = 0;
                for (int rowIndex = 0; rowIndex < rawBoulderField.Length; ++rowIndex)
                {
                    if (rawBoulderField[rowIndex][columnIndex] == 'O')
                    {
                        rawBoulderField[rowIndex][columnIndex] = '.';
                        rawBoulderField[currentSlideStartIndex][columnIndex] = 'O';
                        ++currentSlideStartIndex;
                    }
                    else if (rawBoulderField[rowIndex][columnIndex] == '#')
                    {
                        currentSlideStartIndex = rowIndex + 1;
                    }
                }
            }
        }

        public void PerformSouthSlide()
        {
            for (int columnIndex = 0; columnIndex < rawBoulderField[0].Length; ++columnIndex)
            {
                int currentSlideStartIndex = rawBoulderField.Length - 1;
                for (int rowIndex = rawBoulderField.Length - 1; rowIndex >= 0; --rowIndex)
                {
                    if (rawBoulderField[rowIndex][columnIndex] == 'O')
                    {
                        rawBoulderField[rowIndex][columnIndex] = '.';
                        rawBoulderField[currentSlideStartIndex][columnIndex] = 'O';
                        --currentSlideStartIndex;
                    }
                    else if (rawBoulderField[rowIndex][columnIndex] == '#')
                    {
                        currentSlideStartIndex = rowIndex - 1;
                    }
                }
            }
        }

        public void PerformEastSlide()
        {
            for (int rowIndex = 0; rowIndex < rawBoulderField.Length; ++rowIndex)
            {
                int currentSlideStartIndex = rawBoulderField[0].Length - 1;
                for (int columnIndex = rawBoulderField[0].Length - 1; columnIndex >= 0; --columnIndex)
                {
                    if (rawBoulderField[rowIndex][columnIndex] == 'O')
                    {
                        rawBoulderField[rowIndex][columnIndex] = '.';
                        rawBoulderField[rowIndex][currentSlideStartIndex] = 'O';
                        --currentSlideStartIndex;
                    }
                    else if (rawBoulderField[rowIndex][columnIndex] == '#')
                    {
                        currentSlideStartIndex = columnIndex - 1;
                    }
                }
            }
        }

        public void PerformWestSlide()
        {
            for (int rowIndex = 0; rowIndex < rawBoulderField.Length; ++rowIndex)
            {
                int currentSlideStartIndex = 0;
                for (int columnIndex = 0; columnIndex < rawBoulderField[0].Length; ++columnIndex)
                {
                    if (rawBoulderField[rowIndex][columnIndex] == 'O')
                    {
                        rawBoulderField[rowIndex][columnIndex] = '.';
                        rawBoulderField[rowIndex][currentSlideStartIndex] = 'O';
                        ++currentSlideStartIndex;
                    }
                    else if (rawBoulderField[rowIndex][columnIndex] == '#')
                    {
                        currentSlideStartIndex = columnIndex + 1;
                    }
                }
            }
        }

        public ulong LoadOnNorthColumnPartTwo()
        {
            ulong totalLoad = 0;

            for (int columnIndex = 0; columnIndex < rawBoulderField[0].Length; ++columnIndex)
            {
                ulong currentRowLoad = (ulong)rawBoulderField.Length;
                for (int rowIndex = 0; rowIndex < rawBoulderField.Length; ++rowIndex)
                {
                    if (rawBoulderField[rowIndex][columnIndex] == 'O')
                    {
                        totalLoad += currentRowLoad;
                    }
                    --currentRowLoad;
                }
            }

            return totalLoad;
        }

        public void PrintBoulderField()
        {
            foreach (char[] line in rawBoulderField)
            {
                Console.WriteLine(line);
            }
        }

        public ulong LoadOnNorthColumns()
        {
            ulong load = 0;

            foreach (List<RoundBoulderSlide> boulderSlidesInColumn in boulderSlidesInEachColumn) 
            {
                foreach (RoundBoulderSlide slide in boulderSlidesInColumn)
                {
                    load += TotalLoadFromSlide(slide);
                }
            }

            return load;
        }

        private ulong TotalLoadFromSlide(RoundBoulderSlide slide)
        {
            ulong totalLoad = 0;
            ulong currentLoad = (ulong)numberOfRows - (ulong)slide.firstBoulderPosition;

            for (int ii = 0; ii < slide.numberOfBoulders; ++ii)
            {
                totalLoad += currentLoad;
                --currentLoad;
            }

            return totalLoad;
        }
    }
}
