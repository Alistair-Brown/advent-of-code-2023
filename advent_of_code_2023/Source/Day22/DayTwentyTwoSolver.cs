using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AOC.DayTwentyTwo;

namespace AOC
{
    internal class DayTwentyTwoSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] puzzleInputLines)
        {
            SandJenga sandJenga = new SandJenga(puzzleInputLines);

            int partOneDisintegratableBricks = sandJenga.DisintegratableBricks();
            int partTwoChainReactionSum = sandJenga.SumOfChainReactions();

            return new PuzzleSolution(partOneDisintegratableBricks.ToString(), partTwoChainReactionSum.ToString());
        }
    }
}
