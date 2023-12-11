using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AOC.DayOne;
using AOC.DayTwo;
using AOC.DayThree;

namespace AOC
{
    internal static class PuzzleSolvers
    {
        public const string puzzleInputPathRelativeToSolvers = "../../puzzle_inputs/";
        public const string puzzleInputFileStem = "puzzle_input_";
        public const string puzzleInputExtension = ".txt";

        public const int numberOfSolvers = 11;
        static public IPuzzleSolver[] puzzleSolvers = new IPuzzleSolver[numberOfSolvers] { 
            new DayOneSolver(),
            new DayTwoSolver(),
            new DayThreeSolver(),
            new DayFourSolver(),
            new DayFiveSolver(),
            new DaySixSolver(),
            new DaySevenSolver(),
            new DayEightSolver(),
            new DayNineSolver(),
            new DayTenSolver(),
            new DayElevenSolver()};
    }
}
