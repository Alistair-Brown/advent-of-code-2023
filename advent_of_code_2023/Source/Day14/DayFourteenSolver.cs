using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

using AOC.DayFourteen;

namespace AOC
{
    internal class DayFourteenSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] inputLines)
        {
            BoulderField partOneboulderField = new BoulderField(inputLines);
            partOneboulderField.PerformNorthSlide();
            ulong partOneLoad = partOneboulderField.LoadOnNorthColumns();

            BoulderField partTwoboulderField = new BoulderField(inputLines);
            ulong partTwoLoad = 0;

            for (int ii = 0; ii < 10000000; ++ii)
            {
                partTwoboulderField.PerformFullCycle();

                // We just need to run until we detect that we're into a repeating pattern.
                // At that point, just find out how many steps into that pattern 1 billion
                // cycles would be, and get the load for the equivalent point in the very
                // first cycle of the pattern.
                int cycleStart = 0;
                int cycleLength = 0;
                if (partTwoboulderField.CheckForDuplicateState(ref cycleStart, ref cycleLength))
                {
                    int numberIntoPattern = (1000000000 - cycleStart) % cycleLength;
                    partTwoLoad = partTwoboulderField.LoadOnNorthColumnsAfterCycles(cycleStart + numberIntoPattern);
                    break;
                }
            }

            return new PuzzleSolution(partOneLoad.ToString(), partTwoLoad.ToString());
        }
    }
}
