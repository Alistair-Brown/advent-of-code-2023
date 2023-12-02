using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.DayOne
{
    internal class CalibrationValue
    {
        public readonly int leftHandValue;
        public readonly int rightHandValue;

        public CalibrationValue(string stringRepresentation, int value) 
        {
            this.stringRepresentation = stringRepresentation;
            this.stringLength = stringRepresentation.Length;
            this.leftHandValue = value * 10;
            this.rightHandValue = value;
        }

        private readonly string stringRepresentation;
        private readonly int stringLength;

        // The presence of a calibration value (i.e. '3', or 'five' is detected by passing a character at a time into
        // an instance of this class, incrementing the relevant one of these counts (for whether we're currently looking
        // for leftmost or rightmost) if we continue to match the string, or resetting the count if we don't have match.
        // Once we've matched characters for the full length of the string, we've found this calibration's string
        // representation within the line.
        private int charactersMatchedFromLeft = 0;
        private int charactersMatchedFromRight = 0;

        public void Reset()
        {
            charactersMatchedFromLeft = 0;
            charactersMatchedFromRight = 0;
        }

        public bool MatchCompletedFromLeft(char nextCharacter)
        {
            if (nextCharacter == stringRepresentation[charactersMatchedFromLeft])
            {
                ++charactersMatchedFromLeft;
                if (charactersMatchedFromLeft == stringLength)
                {
                    return true;
                }
                return false;
            }
            else if (nextCharacter == stringRepresentation[0])
            {
                charactersMatchedFromLeft = 1;
                return false;
            }
            else
            {
                charactersMatchedFromLeft = 0;
                return false;
            }
        }
        public bool MatchCompletedFromRight(char nextCharacter)
        {
            if (nextCharacter == stringRepresentation[stringLength - 1 - charactersMatchedFromRight])
            {
                ++charactersMatchedFromRight;
                if (charactersMatchedFromRight == stringLength)
                {
                    return true;
                }
                return false;
            }
            else if (nextCharacter == stringRepresentation[stringLength - 1])
            {
                charactersMatchedFromRight = 1;
                return false;
            }
            else
            {
                charactersMatchedFromRight = 0;
                return false;
            }
        }
    }
}
