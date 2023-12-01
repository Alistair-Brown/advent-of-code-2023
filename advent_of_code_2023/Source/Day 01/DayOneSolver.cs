using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using AOC.DayOne;

namespace AOC
{
    internal class DayOneSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] puzzleInputLines)
        {
            int partOneSum = 0;
            int partTwoSum = 0;

            CalibrationValue[] partOneCalibrationValues = new CalibrationValue[]{
                new CalibrationValue("0", 0),
                new CalibrationValue("1", 1),
                new CalibrationValue("2", 2),
                new CalibrationValue("3", 3),
                new CalibrationValue("4", 4),
                new CalibrationValue("5", 5),
                new CalibrationValue("6", 6),
                new CalibrationValue("7", 7),
                new CalibrationValue("8", 8),
                new CalibrationValue("9", 9)
            };

            CalibrationValue[] partTwoAdditionalCalibrationValues = new CalibrationValue[]{
                new CalibrationValue("one", 1),
                new CalibrationValue("two", 2),
                new CalibrationValue("three", 3),
                new CalibrationValue("four", 4),
                new CalibrationValue("five", 5),
                new CalibrationValue("six", 6),
                new CalibrationValue("seven", 7),
                new CalibrationValue("eight", 8),
                new CalibrationValue("nine", 9)
            };

            foreach (string line in puzzleInputLines)
            {
                bool partOneFound = false;
                bool partTwoFound = false;
                int partOneValue = 0;
                int partTwoValue = 0;

                for (int ii = 0; ii < line.Length; ++ii)
                {
                    char charFromLeft = line[ii];

                    // Start by checking for a match from the part one values, since this will also count as
                    // a match for part two if we haven't already found one.
                    foreach(CalibrationValue calibrationValue in partOneCalibrationValues)
                    {
                        if (calibrationValue.MatchCompletedFromLeft(charFromLeft))
                        {
                            partOneFound = true;
                            partOneValue += calibrationValue.leftHandValue;

                            if (!partTwoFound)
                            {
                                partTwoFound = true;
                                partTwoValue += calibrationValue.leftHandValue;
                            }
                            break;
                        }
                    }

                    if (!partTwoFound)
                    {
                        foreach (CalibrationValue calibrationValue in partTwoAdditionalCalibrationValues)
                        {
                            if (calibrationValue.MatchCompletedFromLeft(charFromLeft))
                            {
                                partTwoFound = true;
                                partTwoValue += calibrationValue.leftHandValue;
                                break;
                            }
                        }
                    }
                    
                    if (partOneFound && partTwoFound)
                    {
                        foreach (CalibrationValue calibrationValue in partOneCalibrationValues)
                        {
                            calibrationValue.Reset();
                        }
                        foreach (CalibrationValue calibrationValue in partTwoAdditionalCalibrationValues)
                        {
                            calibrationValue.Reset();
                        }

                        break;
                    }
                }

                partOneFound = false;
                partTwoFound = false;

                for (int ii = line.Length - 1; ii >= 0; --ii)
                {
                    char charFromRight = line[ii];

                    foreach (CalibrationValue calibrationValue in partOneCalibrationValues)
                    {
                        if (calibrationValue.MatchCompletedFromRight(charFromRight))
                        {
                            partOneFound = true;
                            partOneValue += calibrationValue.rightHandValue;

                            if (!partTwoFound)
                            {
                                partTwoFound = true;
                                partTwoValue += calibrationValue.rightHandValue;
                            }

                            break;
                        }
                    }

                    if (!partTwoFound)
                    {
                        foreach (CalibrationValue calibrationValue in partTwoAdditionalCalibrationValues)
                        {
                            if (calibrationValue.MatchCompletedFromRight(charFromRight))
                            {
                                partTwoFound = true;
                                partTwoValue += calibrationValue.rightHandValue;
                                break;
                            }
                        }
                    }

                    if (partOneFound && partTwoFound)
                    {
                        foreach (CalibrationValue calibrationValue in partOneCalibrationValues)
                        {
                            calibrationValue.Reset();
                        }
                        foreach (CalibrationValue calibrationValue in partTwoAdditionalCalibrationValues)
                        {
                            calibrationValue.Reset();
                        }

                        break;
                    }
                }

                partOneSum += partOneValue;
                partTwoSum += partTwoValue;
            }

            return new PuzzleSolution(Convert.ToString(partOneSum), Convert.ToString(partTwoSum));
        }
    }
}
