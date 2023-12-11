using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AOC.DayEleven;

namespace AOC
{
    internal class DayElevenSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] inputPuzzleLines)
        {
            Observatory observatory = new Observatory(inputPuzzleLines);

            ulong partOnePathSum = observatory.SumOfPathsBetweenGalaxies(1);
            ulong partTwoPathSum = observatory.SumOfPathsBetweenGalaxies(999999);

            return new PuzzleSolution(partOnePathSum.ToString(), partTwoPathSum.ToString());
        }
    }
}
