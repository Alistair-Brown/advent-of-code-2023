using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AOC.DayNine;

namespace AOC
{
    internal class DayNineSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] puzzleInputLines)
        {
            List<OasisValueHistory> oasisHistories = new List<OasisValueHistory>();

            foreach (string inputLine in puzzleInputLines)
            {
                oasisHistories.Add(new OasisValueHistory(inputLine));
            }

            long partOneSum = 0;
            foreach (OasisValueHistory history in oasisHistories)
            {
                partOneSum += (long)history.newFinalSequenceValue;
            }

            long partTwoSum = 0;
            foreach (OasisValueHistory history in oasisHistories)
            {
                partTwoSum += (long)history.newFirstSequenceValue;
            }

            return new PuzzleSolution(partOneSum.ToString(), partTwoSum.ToString());
        }
    }
}
