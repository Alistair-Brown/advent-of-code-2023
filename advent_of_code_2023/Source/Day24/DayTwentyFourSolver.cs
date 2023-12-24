using AOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AOC.DayTwentyFour;

namespace AOC
{
    internal class DayTwentyFourSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] puzzleInputLines)
        {
            Hailstorm hailstorm = new Hailstorm(puzzleInputLines);

            int partOneIntersections = hailstorm.NumberOfIntersections(200000000000000, 400000000000000);

            return new PuzzleSolution(partOneIntersections.ToString(), "World");
        }
    }
}
