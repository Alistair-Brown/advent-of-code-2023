using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AOC.DayTwelve
{
    internal class HotSpring
    {
        private string springString;
        private List<int> damagedSpringGroups;

        // This is the minimum length the springString need to be in order to accomodate
        // all of the damaged spring groups, with a single operational spring between each
        // group.
        private int minimumSpringStringLength;

        List<List<int>> springGroupPositions;

        Dictionary<(int stringIndex, int groupIndex), ulong> possibleArrangementsCache;

        public HotSpring(string inputLine, bool partTwo)
        {
            string[] splitOnSpace = inputLine.Split(' ');

            springString = splitOnSpace[0];
            if (partTwo)
            {
                for (int ii = 0; ii < 4; ++ii)
                {
                    springString += '?';
                    springString += splitOnSpace[0];
                }
            }

            string[] rawDamagedSpringGroups = splitOnSpace[1].Split(',');

            damagedSpringGroups = new List<int>();
            minimumSpringStringLength = 0;

            foreach (string rawSpringGroup in rawDamagedSpringGroups)
            {
                int springGroupSize = Convert.ToInt32(rawSpringGroup);
                damagedSpringGroups.Add(springGroupSize);
                minimumSpringStringLength += springGroupSize;
            }

            if (partTwo)
            {
                for (int ii = 0; ii < 4; ++ii)
                {
                    foreach (string rawSpringGroup in rawDamagedSpringGroups)
                    {
                        int springGroupSize = Convert.ToInt32(rawSpringGroup);
                        damagedSpringGroups.Add(springGroupSize);
                        minimumSpringStringLength += springGroupSize;
                    }
                }
            }

            minimumSpringStringLength += damagedSpringGroups.Count - 1;

            springGroupPositions = new List<List<int>>();
            possibleArrangementsCache = new Dictionary<(int stringIndex, int groupIndex), ulong>();
        }

        public ulong NumberOfPossibleStringArrangements()
        {
            ulong possibleArrangements = RecursiveNumberOfPossibleArrangements(
                0, 0, minimumSpringStringLength, new List<int>());

            return possibleArrangements;
        }

        private ulong RecursiveNumberOfPossibleArrangements(
            int currentStringIndex,
            int currentSpringGroupIndex,
            int requiredRemainingStringLength,
            List<int> springGroupIndices) 
        {
            if (possibleArrangementsCache.ContainsKey((currentStringIndex, currentSpringGroupIndex)))
            {
                return possibleArrangementsCache[(currentStringIndex, currentSpringGroupIndex)];
            }

            // If there are no damaged spring groups left to arrange, then whatever arrangement
            // was found on the way to this point was a valid one.
            if (currentSpringGroupIndex == damagedSpringGroups.Count)
            {
                // Must check there were no mandatory positions left
                for (int ii = currentStringIndex; ii < springString.Length; ++ii)
                {
                    if (springString[ii] == '#')
                    {
                        return 0;
                    }
                }

                springGroupPositions.Add(springGroupIndices);
                return 1;
            }

            if (requiredRemainingStringLength > springString.Length - currentStringIndex)
            {
                return 0;
            }

            ulong numberOfValidArrangements = 0;
            for (int stringIndex = currentStringIndex; stringIndex < springString.Length; ++stringIndex)
            {
                int springGroupLength = damagedSpringGroups[currentSpringGroupIndex];
                if (CanInsertSpringGroup(stringIndex, springGroupLength))
                {
                    List<int> copiedIndices = new List<int>(springGroupIndices);
                    copiedIndices.Add(stringIndex);

                    // Need to start the next recursion from one space further on than where this group ends,
                    // to leave a gap between the groups.
                    numberOfValidArrangements += RecursiveNumberOfPossibleArrangements(
                        stringIndex + springGroupLength + 1,
                        currentSpringGroupIndex + 1,
                        requiredRemainingStringLength - springGroupLength - 1,
                        copiedIndices);
                }

                // Can't move beyond a point where a spring MUST be damaged.
                if (springString[stringIndex] == '#')
                {
                    break;
                }
            }

            possibleArrangementsCache.Add((currentStringIndex, currentSpringGroupIndex), numberOfValidArrangements);

            return numberOfValidArrangements;
        }

        private bool CanInsertSpringGroup(int startingStringIndex, int groupSize)
        { 
            if ((startingStringIndex + groupSize) > springString.Length)
            {
                return false;
            }

            for (int stringIndex = startingStringIndex; stringIndex < startingStringIndex + groupSize; ++stringIndex)
            {
                if (springString[stringIndex] == '.')
                {
                    return false;
                }
            }

            // Not valid if we're already halfway through a group
            if (startingStringIndex > 0 && springString[startingStringIndex - 1] == '#')
            {
                return false;
            }

            // If we've inserted the whole group without hitting an operational spring, then this
            // insertion is possible as long as it doesn't leave us halfway through a damaged group.
            if (((startingStringIndex + groupSize) < springString.Length) &&
                (springString[startingStringIndex + groupSize] == '#'))
            {
                return false;
            }

            string insertedGroup = "";
            for (int index = 0; index < startingStringIndex; ++index)
            {
                insertedGroup += '.';
            }
            for (int index = 0; index < groupSize; ++index)
            {
                insertedGroup += '#';
            }
            for (int index = startingStringIndex + groupSize; index < springString.Length; ++index)
            {
                insertedGroup += '.';
            }

            return true;
        }

        private void PrintSpringArrangement(List<int> springGroupPositions)
        {
            int stringIndex = 0;
            int groupIndex = 0;
            string debugViewPositions = "";
            while (groupIndex < springGroupPositions.Count)
            {
                while (stringIndex < springGroupPositions[groupIndex])
                {
                    debugViewPositions += '.';
                    Debug.Assert(springString[stringIndex] == '.' || springString[stringIndex] == '?');
                    ++stringIndex;
                }

                for (int ii = 0; ii < damagedSpringGroups[groupIndex]; ++ii)
                {
                    debugViewPositions += '#';
                    Debug.Assert(springString[stringIndex] == '#' || springString[stringIndex] == '?');
                    ++stringIndex;
                }

                ++groupIndex;
            }

            while (stringIndex < springString.Length)
            {
                debugViewPositions += '.';
                ++stringIndex;
            }

            Console.WriteLine(debugViewPositions);
        }
    }
}
