using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AOC.DayFour
{
    internal class Scratchcard
    {
        public readonly int cardNumber;
        private HashSet<int> winningNumbers;
        private List<int> numbersYouHave;


        public Scratchcard(string inputLine) 
        {
            char[] whiteSpace = new char[1] { ' ' };

            string[] splitOnColon = inputLine.Split(':');
            string[] cardAndId = splitOnColon[0].Split(whiteSpace, StringSplitOptions.RemoveEmptyEntries);

            Debug.Assert(cardAndId[0] == "Card", "String parsing broken");
            cardNumber = Convert.ToInt32(cardAndId[1]);

            string[] numbersEitherSideOfLine = splitOnColon[1].Split('|');            
            string[] rawWinningNumbers = numbersEitherSideOfLine[0].Split(whiteSpace, StringSplitOptions.RemoveEmptyEntries);
            string[] rawNumbersYouHave = numbersEitherSideOfLine[1].Split(whiteSpace, StringSplitOptions.RemoveEmptyEntries);

            winningNumbers = new HashSet<int>();
            numbersYouHave = new List<int>();

            foreach (string number in rawWinningNumbers)
            {
                winningNumbers.Add(Convert.ToInt32(number));
            }

            foreach (string number in rawNumbersYouHave)
            {
                numbersYouHave.Add(Convert.ToInt32(number));
            }
        }

        public int NumberOfMatches()
        {
            int numMatches = 0;

            foreach (int number in numbersYouHave)
            {
                if (winningNumbers.Contains(number))
                {
                    ++numMatches;
                }
            }

            return numMatches;
        }

        public static int PointsForMatches(int numMatches)
        {
            int points = 0;

            for (int ii = 0; ii < numMatches; ++ii)
            {
                if (points == 0)
                {
                    points = 1;
                }
                else
                {
                    points *= 2;
                }
            }

            return points;
        }
    }
}
