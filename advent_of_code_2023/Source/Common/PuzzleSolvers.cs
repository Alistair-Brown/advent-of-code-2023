using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal static class PuzzleSolvers
    {
        public const string puzzleInputPathRelativeToSolvers = "../../puzzle_inputs/";
        public const string puzzleInputFileStem = "puzzle_input_";
        public const string puzzleInputExtension = ".txt";

        public const int numberOfSolvers = 1;
        static public IPuzzleSolver[] puzzleSolvers = new IPuzzleSolver[numberOfSolvers] { new DayOneSolver() };
    }
}
