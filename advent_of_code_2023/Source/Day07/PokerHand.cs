using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.DaySeven
{
    enum HandType
    {
        HighCard = 0,
        OnePair = 1,
        TwoPair = 2,
        ThreeOfAKind = 3,
        FullHouse = 4,
        FourOfAKind = 5,
        FiveOfAKind = 6
    }

    internal class PokerHand
    {
        static Dictionary<char, int> partOnePointsPerCard = new Dictionary<char, int>
        {
            { '2', 0 },
            { '3', 1 },
            { '4', 2 },
            { '5', 3 },
            { '6', 4 },
            { '7', 5 },
            { '8', 6 },
            { '9', 7 },
            { 'T', 8 },
            { 'J', 9 },
            { 'Q', 10 },
            { 'K', 11 },
            { 'A', 12 }
        };
        static Dictionary<char, int> partTwoPointsPerCard = new Dictionary<char, int>
        {
            { 'J', 0 },
            { '2', 1 },
            { '3', 2 },
            { '4', 3 },
            { '5', 4 },
            { '6', 5 },
            { '7', 6 },
            { '8', 7 },
            { '9', 8 },
            { 'T', 9 },
            { 'Q', 10 },
            { 'K', 11 },
            { 'A', 12 }
        };
        const int numCardTypes = 13;

        public readonly ulong handScore;
        public readonly string rawHand;
        public readonly HandType partOneHandType;
        public readonly HandType partTwoHandType;

        public PokerHand(string rawInputLine)
        {
            string[] handAndScore = rawInputLine.Split(' ');
            Debug.Assert(handAndScore.Length == 2);

            rawHand = handAndScore[0];
            handScore = Convert.ToUInt64(handAndScore[1]);

            partOneHandType = GetPartOneHandType(rawHand);
            partTwoHandType = GetPartTwoHandType(rawHand);
        }

        private HandType GetPartOneHandType(string rawHand)
        {
            int[] numberOfEachCard = new int[numCardTypes];

            foreach (char card in rawHand)
            {
                ++numberOfEachCard[partOnePointsPerCard[card]];
            }

            bool pairFound = false;
            bool threeFound = false;

            foreach (int numCards in numberOfEachCard)
            {
                if (numCards == 2)
                {
                    if (threeFound)
                    {
                        return HandType.FullHouse;
                    }
                    else if (pairFound)
                    {
                        return HandType.TwoPair;
                    }
                    pairFound = true;
                }
                else if (numCards == 3)
                {
                    if (pairFound)
                    {
                        return HandType.FullHouse;
                    }
                    threeFound = true;
                }
                else if (numCards == 4)
                {
                    return HandType.FourOfAKind;
                }
                else if (numCards == 5)
                {
                    return HandType.FiveOfAKind;
                }
            }

            if (pairFound)
            {
                return HandType.OnePair;
            }
            else if (threeFound)
            {
                return HandType.ThreeOfAKind;
            }
            else
            {
                return HandType.HighCard;
            }
        }

        private HandType GetPartTwoHandType(string rawHand)
        {
            int[] numberOfEachCard = new int[numCardTypes];

            foreach (char card in rawHand)
            {
                ++numberOfEachCard[partTwoPointsPerCard[card]];
            }

            int jokersFound = numberOfEachCard[0];
            int pairsFound = 0;
            int threesFound = 0;
            int foursFound = 0;
            int fivesFound = 0;

            // Joker takes the 0th index, so start looking at the remaining cards
            // from index one.
            for (int ii = 1; ii < numCardTypes; ++ii)
            {
                int numCards = numberOfEachCard[ii];

                if (numCards == 2)
                {
                    ++pairsFound;
                }
                else if (numCards == 3)
                {
                    ++threesFound;
                }
                else if (numCards == 4)
                {
                    ++foursFound;
                }
                else if (numCards == 5)
                {
                    ++fivesFound;
                }
            }

            if ((fivesFound == 1) || 
                ((foursFound == 1) && (jokersFound == 1)) ||
                ((threesFound == 1) && (jokersFound == 2)) ||
                ((pairsFound == 1) && (jokersFound == 3)) ||
                (jokersFound == 4) ||
                (jokersFound == 5))
            {
                return HandType.FiveOfAKind;
            }
            else if ((foursFound == 1) ||
                ((threesFound == 1) && (jokersFound == 1)) ||
                ((pairsFound == 1) && (jokersFound == 2)) ||
                (jokersFound == 3))
            {
                return HandType.FourOfAKind;
            }
            else if (((threesFound == 1) && (pairsFound == 1)) ||
                ((threesFound == 1) && (jokersFound == 1)) ||
                ((pairsFound == 2) && (jokersFound == 1)))
            {
                return HandType.FullHouse;
            }
            else if ((threesFound == 1) ||
                ((pairsFound == 1) && (jokersFound == 1)) ||
                (jokersFound == 2))
            {
                return HandType.ThreeOfAKind;
            }
            else if (pairsFound == 2)
            {
                return HandType.TwoPair;
            }
            else if ((pairsFound == 1) || (jokersFound == 1))
            {
                return HandType.OnePair;
            }
            else
            {
                return HandType.HighCard;
            }
        }

        public int PartOneCompareTo(PokerHand otherHand)
        {
            if (partOneHandType != otherHand.partOneHandType)
            {
                return partOneHandType > otherHand.partOneHandType ? 1 : -1;
            }

            for (int ii = 0; ii < rawHand.Length; ++ii)
            {
                if (rawHand[ii] != otherHand.rawHand[ii])
                {
                    return partOnePointsPerCard[rawHand[ii]] > partOnePointsPerCard[otherHand.rawHand[ii]] ? 1 : -1;
                }
            }

            return 0;
        }

        public int PartTwoCompareTo(PokerHand otherHand)
        {
            if (partTwoHandType != otherHand.partTwoHandType)
            {
                return partTwoHandType > otherHand.partTwoHandType ? 1 : -1;
            }

            for (int ii = 0; ii < rawHand.Length; ++ii)
            {
                if (rawHand[ii] != otherHand.rawHand[ii])
                {
                    return partTwoPointsPerCard[rawHand[ii]] > partTwoPointsPerCard[otherHand.rawHand[ii]] ? 1 : -1;
                }
            }

            return 0;
        }
    }
}
