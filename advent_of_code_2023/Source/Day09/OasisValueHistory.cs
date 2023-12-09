using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.DayNine
{
    internal class OasisValueHistory
    {
        // Contains the original sequence on the top row, with the sequence of diffs between
        // initial sequence values in the row below, then the diffs between those etc, with
        // the final row being the point at which all diffs are 0.
        List<List<int>> allSequences;

        public readonly int newFinalSequenceValue;
        public readonly int newFirstSequenceValue;
        public OasisValueHistory(string inputLine)
        {
            allSequences = new List<List<int>>();
            allSequences.Add(new List<int>());

            foreach(string value in inputLine.Split(' '))
            {
                allSequences[0].Add(Convert.ToInt32(value));
            }

            FillOutSequences();
            newFinalSequenceValue = GetNextSequenceValue();
            newFirstSequenceValue = GetInitialSequenceValue();
        }

        private void FillOutSequences()
        {
            while (true)
            {
                List<int> bottomSequence = allSequences[allSequences.Count - 1];

                // Once our bottom sequence is all zeroes, there are no more values differences
                // to be found.
                bool allZero = true;
                foreach (int value in bottomSequence)
                {
                    if (value != 0)
                    {
                        allZero = false;
                        break;
                    }
                }
                if (allZero)
                {
                    break;
                }

                // Create the next sequence of value differences from the differences in the
                // current bottom sequence in the list, and then add that on as our new 
                // bottom sequence.
                List<int> newBottomSequence = new List<int>();
                for (int ii = 0; ii < bottomSequence.Count - 1; ++ii)
                {
                    newBottomSequence.Add(bottomSequence[ii + 1] - bottomSequence[ii]);
                }
                allSequences.Add(newBottomSequence);
            }
        }

        private int GetNextSequenceValue()
        {
            // The next value in the stop sequence is just the sum of all the right hand
            // values in the sequences produced in the previous step.
            int nextSequenceValue = 0;
            foreach (List<int> sequence in allSequences)
            {
                nextSequenceValue += sequence[sequence.Count - 1];
            }
            return nextSequenceValue;
        }

        // To get the new value on the left hand side of the initial sequence, we just need
        // to work bottom-up along the 0th element of each other sequence, subtracting the
        // newly found initial value from the sequence below, since that value is the difference
        // between the 0th element in the sequence above and the imaginary preceding value.
        private int GetInitialSequenceValue()
        {
            int initialiSequenceValue = 0;
            for (int ii = allSequences.Count - 1; ii >= 0; --ii)
            { 
                initialiSequenceValue = allSequences[ii][0] - initialiSequenceValue;
            }
            return initialiSequenceValue;
        }
    }
}
