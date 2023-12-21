using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using AOC.DaySeventeen;

namespace AOC
{
    // Pretty sure I could get a good speed up on this day by not storing the intermediate steps on the way to the minimum
    // steps, and jumping straight up to the minimum when we change direction. Would reduce the number of different approach
    // cases each node has to store, and reduce the total number of iterations through the algorithm.
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
