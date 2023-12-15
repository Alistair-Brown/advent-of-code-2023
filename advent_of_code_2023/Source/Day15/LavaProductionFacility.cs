using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.DayFifteen
{
    internal class LavaProductionFacility
    {
        List<ulong> initialisationStepValues;
        List<(string label, int focalLength)>[] hashBoxes;

        public LavaProductionFacility(string inputLine)
        {
            initialisationStepValues = new List<ulong>();

            hashBoxes = new List<(string label, int focalLength)>[256];
            foreach (int ii in Enumerable.Range(0, 256))
            {
                hashBoxes[ii] = new List<(string label, int focalLength)>();
            }

            string[] rawInitSteps = inputLine.Split(',');

            foreach (string step in rawInitSteps)
            {
                initialisationStepValues.Add(HashInitialisationStep(step));
                PerformLensMovingStep(step);
            }
        }

        public ulong SumOfInitialisationHashes()
        {
            ulong sum = 0;
            foreach (ulong value in initialisationStepValues)
            {
                sum += value;
            }

            return sum;
        }

        public ulong TotalFocusingPower()
        {
            ulong totalFocusingPower = 0;
            for (int ii = 0; ii < hashBoxes.Length; ++ii)
            {
                totalFocusingPower += BoxFocusingPower(hashBoxes[ii], ii);
            }
            return totalFocusingPower;
        }

        private ulong BoxFocusingPower(List<(string label, int focalLength)> box, int boxNumber)
        {
            ulong totalBoxPower = 0;

            for (int ii = 0; ii < box.Count; ++ii)
            {
                totalBoxPower += (ulong)((boxNumber + 1) * (ii + 1) * box[ii].focalLength);
            }

            return totalBoxPower;
        }

        private void PerformLensMovingStep(string rawInitStep)
        {
            string[] splitOnEquals = rawInitStep.Split('=');

            if (splitOnEquals.Length == 2)
            {
                string label = splitOnEquals[0];
                ulong boxNumber = HashInitialisationStep(label);
                int boxPosition = BoxContainsLabel(hashBoxes[boxNumber], label);

                if (boxPosition == -1)
                {
                    hashBoxes[boxNumber].Add((label, Convert.ToInt32(splitOnEquals[1])));
                }
                else
                {
                    hashBoxes[boxNumber][boxPosition] = (label, Convert.ToInt32(splitOnEquals[1]));
                }
            }
            else
            {
                Debug.Assert(rawInitStep[rawInitStep.Length - 1] == '-');
                string label = rawInitStep.Substring(0, rawInitStep.Length - 1);
                ulong boxNumber = HashInitialisationStep(label);
                int boxPosition = BoxContainsLabel(hashBoxes[boxNumber], label);

                if (boxPosition >= 0)
                {
                    hashBoxes[boxNumber].RemoveAt(boxPosition);
                }
            }

        }

        private ulong HashInitialisationStep(string rawStep)
        {
            int currentValue = 0;
            foreach (char character in rawStep)
            {
                int asciiVal = (int)character;
                currentValue += asciiVal;
                currentValue *= 17;
                currentValue %= 256;
            }

            return (ulong)currentValue;
        }

        // Returns -1 if not present, otherwise position in box
        private int BoxContainsLabel(List<(string label, int focalLength)> box, string labelToMatch)
        {
            int position = -1;

            int ii = 0;
            foreach ((string label, int focalLength) entry in box)
            {
                if (entry.label == labelToMatch)
                {
                    position = ii;
                    break;
                }
                ++ii;
            }

            return position;
        }
    }
}
