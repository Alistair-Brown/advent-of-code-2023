using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AOC.DayFour;

namespace AOC
{
    internal class DayFourSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] puzzleInputLines)
        {
            int partOnePointsSum = 0;
            ulong partTwoNumScratchcards = 0;

            List<(Scratchcard scratchcard, int copies)> scratchcards = new List<(Scratchcard, int)>();

            foreach (string line in puzzleInputLines)
            {
                scratchcards.Add((new Scratchcard(line), 1));
                ++partTwoNumScratchcards;
            }

            for (int ii = 0; ii < scratchcards.Count; ++ii)
            {
                int numMatches = scratchcards[ii].scratchcard.NumberOfMatches();
                int copiesOfCurrentCard = scratchcards[ii].copies;
                partOnePointsSum += Scratchcard.PointsForMatches(numMatches);
                partTwoNumScratchcards += (ulong)(numMatches * copiesOfCurrentCard);

                // We start adding the extra copies at the index matching the current card's ID, since the card
                // IDs are 1-indexed but the list is 0-indexed.
                int cardIdToGainCopiesOf = scratchcards[ii].scratchcard.cardNumber;
                for (int jj = 0; jj < numMatches; ++jj)
                {
                    scratchcards[cardIdToGainCopiesOf] = (scratchcards[cardIdToGainCopiesOf].scratchcard, scratchcards[cardIdToGainCopiesOf].copies + copiesOfCurrentCard);
                    ++cardIdToGainCopiesOf;
                }
            }

            return new PuzzleSolution(partOnePointsSum.ToString(), partTwoNumScratchcards.ToString());
        }
    }
}
