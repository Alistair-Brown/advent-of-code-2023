using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal interface IPuzzleSolver
    {
        PuzzleSolution SolvePuzzle(string[] puzzle_input_lines);
    }
}
