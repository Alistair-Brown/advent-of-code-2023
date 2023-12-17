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
            CrucibleMap crucibleMapOne = new CrucibleMap(puzzleInputLines);
            CrucibleMap crucibleMapTwo = new CrucibleMap(puzzleInputLines);

            int partOneCheapestRoute = crucibleMapOne.CheapestRoute();

            return new PuzzleSolution(partOneCheapestRoute.ToString(), "world");
        }
    }
}
