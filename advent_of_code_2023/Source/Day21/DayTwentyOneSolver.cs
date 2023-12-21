using AOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AOC.DayTwentyOne;

namespace AOC
{
    internal class DayTwentyOneSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] puzzleInputLines)
        {
            GardenMap gardenMap = new GardenMap(puzzleInputLines);

            int partOneLocations = gardenMap.FiniteGardensVisitedAfterNSteps(64);
            ulong partTwoLocations = gardenMap.InfiniteGardensVisitedAfterNSteps(26501365);

            return new PuzzleSolution(partOneLocations.ToString(), partTwoLocations.ToString());
        }
    }
}
