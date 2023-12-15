using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AOC.DayFifteen;

namespace AOC
{
    internal class DayFifteenSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] inputLines)
        {
            Debug.Assert(inputLines.Length == 1);

            LavaProductionFacility lavaProductionFacility = new LavaProductionFacility(inputLines[0]);
            ulong partOneSum = lavaProductionFacility.SumOfInitialisationHashes();
            ulong partTwoSum = lavaProductionFacility.TotalFocusingPower();

            return new PuzzleSolution(partOneSum.ToString(), partTwoSum.ToString());
        }
    }
}
