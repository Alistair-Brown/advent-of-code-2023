using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AOC.DayTwentyThree;

namespace AOC
{
    internal class DayTwentyThreeSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] puzzleInputLines)
        {
            HikingTrails hikingTrails = new HikingTrails(puzzleInputLines);

            int partOneLongestRoute = hikingTrails.LongestRoute(true);
            int partTwoLongestRoute = hikingTrails.LongestRoute(false);

            return new PuzzleSolution(partOneLongestRoute.ToString(), partTwoLongestRoute.ToString());
        }
    }
}
