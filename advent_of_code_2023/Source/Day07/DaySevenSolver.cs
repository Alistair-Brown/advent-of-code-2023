using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AOC.DaySeven;

namespace AOC
{
    internal class DaySevenSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] puzzleInputLines)
        {
            List<PokerHand> hands = new List<PokerHand>();

            foreach (string line in puzzleInputLines)
            {
                hands.Add(new PokerHand(line));
            }

            hands.Sort((a, b) => { return a.PartOneCompareTo(b); });

            ulong partOneSum = 0;
            for (int ii = 0; ii < hands.Count; ++ii)
            {
                partOneSum += hands[ii].handScore * ((ulong)ii + 1);
            }

            hands.Sort((a, b) => { return a.PartTwoCompareTo(b); });

            ulong partTwoSum = 0;
            for (int ii = 0; ii < hands.Count; ++ii)
            {
                partTwoSum += hands[ii].handScore * ((ulong)ii + 1);
            }

            return new PuzzleSolution(partOneSum.ToString(), partTwoSum.ToString());
        }
    }
}
