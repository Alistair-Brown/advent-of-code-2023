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
        public int numberOfRows;

        public BoulderField(string[] inputLines)
        {
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
