using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace AOC
{
    class main
    {
        static void Main(string[] args)
        {
            if (args[0] == "all")
            {
                foreach (int day in Enumerable.Range(1, PuzzleSolvers.numberOfSolvers))
                {
                    SolveDayAndPrintSolution(day);
                }
            }
            else
            {
                try
                {
                    int day = Convert.ToInt32(args[0]);
                    if (day < 1 || day > PuzzleSolvers.numberOfSolvers) 
                    {
                        Console.WriteLine(
                            "Only valid arguments are a number in the range 1 to " +
                            PuzzleSolvers.numberOfSolvers +
                            " or 'all'");
                        return;
                    }

                    SolveDayAndPrintSolution(day);
                }
                catch (FormatException)
                {
                    Console.WriteLine(
                        "Only valid arguments are a number in the range 1 to " +
                        PuzzleSolvers.numberOfSolvers +
                        " or 'all'");
                }
            }
        }

        private static void SolveDayAndPrintSolution(int day)
        {
            string dayAsString = day.ToString();
            if (dayAsString.Length == 1) { dayAsString = "0" + dayAsString; }

            string relativePath =
                PuzzleSolvers.puzzleInputPathRelativeToSolvers +
                PuzzleSolvers.puzzleInputFileStem +
                dayAsString +
                PuzzleSolvers.puzzleInputExtension;

            // I'm only interested in the runtime of the solution itself, so do the file IO outside the timed
            // block.
            string[] puzzleInputLines = File.ReadAllLines(relativePath);

            Stopwatch stopwatch = Stopwatch.StartNew();

            // Remember to convert 1-indexed day number to 0-index for array indexing
            PuzzleSolution solution = PuzzleSolvers.puzzleSolvers[day - 1].SolvePuzzle(puzzleInputLines);

            stopwatch.Stop();
            TimeSpan executionTime = stopwatch.Elapsed;

            Console.WriteLine($"Day {dayAsString}: {solution.part_one}, {solution.part_two} ({executionTime.Seconds}s {executionTime.Milliseconds}ms)");
        }
    }
}
