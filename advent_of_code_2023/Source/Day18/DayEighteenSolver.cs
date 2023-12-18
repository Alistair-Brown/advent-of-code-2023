using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AOC.DayEighteen;

namespace AOC
{
    internal class DayEighteenSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] puzzleInputLines)
        {
            LagoonDigger lagoonDigger = new LagoonDigger(puzzleInputLines, false);

            ulong partOneTrenchSize = lagoonDigger.TrenchSize();

            LagoonDigger lagoonDiggerTwo = new LagoonDigger(puzzleInputLines, true);

            ulong partTwoTrenchSize = lagoonDiggerTwo.TrenchSize();

            return new PuzzleSolution(partOneTrenchSize.ToString(), partTwoTrenchSize.ToString());
        }
    }
}
