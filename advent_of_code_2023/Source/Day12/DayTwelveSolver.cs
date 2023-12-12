using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AOC.DayTwelve;

namespace AOC
{
    internal class DayTwelveSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] puzzleInputLines)
        {
            List<HotSpring> hotSprings = new List<HotSpring>();

            foreach (string inputLine in puzzleInputLines)
            {
                hotSprings.Add(new HotSpring(inputLine, false));
            }

            ulong partOnePossibleArrangments = 0;
            foreach (HotSpring hotSpring in hotSprings)
            {
                partOnePossibleArrangments += hotSpring.NumberOfPossibleStringArrangements();
            }

            List<HotSpring> partTwoHotSprings = new List<HotSpring>();
            foreach (string inputLine in puzzleInputLines)
            {
                partTwoHotSprings.Add(new HotSpring(inputLine, true));
            }

            ulong partTwoPossibleArrangments = 0;
            foreach (HotSpring hotSpring in partTwoHotSprings)
            {
                partTwoPossibleArrangments += hotSpring.NumberOfPossibleStringArrangements();
            }

            return new PuzzleSolution(partOnePossibleArrangments.ToString(), partTwoPossibleArrangments.ToString());
        }
    }
}
