using AOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AOC.DayTwentyFive;

namespace AOC
{
    internal class DayTwentyFiveSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] puzzleInputLines)
        {
            ComponentJumble jumble = new ComponentJumble(puzzleInputLines);
            int partOneProduct = jumble.ProductOfCutConnections();

            return new PuzzleSolution(partOneProduct.ToString(), "World");
        }
    }
}
