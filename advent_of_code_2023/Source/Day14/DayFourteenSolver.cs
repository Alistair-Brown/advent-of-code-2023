using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AOC.DayFourteen;

namespace AOC
{
    internal class DayFourteenSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] inputLines)
        {
            BoulderField boulderField = new BoulderField(inputLines);

            ulong partOneLoad = boulderField.LoadOnNorthColumns();

            for (int ii = 0; ii < 1000; ++ii)
            {
                boulderField.PerformFullCycle();
                Console.WriteLine(boulderField.LoadOnNorthColumnPartTwo());
            }

            return new PuzzleSolution(partOneLoad.ToString(), "abc");
        }
    }
}
