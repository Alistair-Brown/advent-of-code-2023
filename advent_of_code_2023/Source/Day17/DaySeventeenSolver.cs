using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using AOC.DaySeventeen;

namespace AOC
{
    internal class DaySeventeenSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] puzzleInputLines)
        {
            CrucibleMap crucibleMapOne = new CrucibleMap(puzzleInputLines, 1, 3);
            CrucibleMap crucibleMapTwo = new CrucibleMap(puzzleInputLines, 4, 10);

            int partOneCheapestRoute = crucibleMapOne.CheapestRoute();
            int partTwoCheapestRoute = crucibleMapTwo.CheapestRoute();

            return new PuzzleSolution(partOneCheapestRoute.ToString(), partTwoCheapestRoute.ToString());
        }
    }
}
