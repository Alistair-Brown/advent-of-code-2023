using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AOC
{
    internal class DayOneSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] puzzleInputLines)
        {
            ulong partOneSum = 0;
            ulong partTwoSum = 0;

            foreach(string line in puzzleInputLines)
            {
                string digitOne = "0";
                string digitTwo = "0";
                foreach (int ii in Enumerable.Range(0, line.Length))
                {
                    if (line[ii] >= '0' && line[ii] <= '9')
                    {
                        digitOne = Char.ToString(line[ii]);
                        break;
                    }
                }

                foreach (int ii in Enumerable.Range(0, line.Length).Reverse())
                {
                    if (line[ii] >= '0' && line[ii] <= '9')
                    {
                        digitTwo = Char.ToString(line[ii]);
                        break;
                    }
                }

                partOneSum += Convert.ToUInt64(digitOne + digitTwo);
            }

            foreach (string line in puzzleInputLines)
            {
                string digitOne = "0";
                string digitTwo = "0";
                int digitPos = 0;

                foreach (int ii in Enumerable.Range(0, line.Length))
                {
                    if (line[ii] >= '0' && line[ii] <= '9')
                    {
                        digitOne = Char.ToString(line[ii]);
                        digitPos = ii;
                        break;
                    }
                }

                if (line.IndexOf("one") <= digitPos && line.IndexOf("one") >= 0)
                {
                    digitOne = "1";
                    digitPos = line.IndexOf("one");
                }
                if (line.IndexOf("two") <= digitPos && line.IndexOf("two") >= 0)
                {
                    digitOne = "2";
                    digitPos = line.IndexOf("two");
                }
                if (line.IndexOf("three") <= digitPos && line.IndexOf("three") >= 0)
                {
                    digitOne = "3";
                    digitPos = line.IndexOf("three");
                }
                if (line.IndexOf("four") <= digitPos && line.IndexOf("four") >= 0)
                {
                    digitOne = "4";
                    digitPos = line.IndexOf("four");
                }
                if (line.IndexOf("five") <= digitPos && line.IndexOf("five") >= 0)
                {
                    digitOne = "5";
                    digitPos = line.IndexOf("five");
                }
                if (line.IndexOf("six") <= digitPos && line.IndexOf("six") >= 0)
                {
                    digitOne = "6";
                    digitPos = line.IndexOf("six");
                }
                if (line.IndexOf("seven") <= digitPos && line.IndexOf("seven") >= 0)
                {
                    digitOne = "7";
                    digitPos = line.IndexOf("seven");
                }
                if (line.IndexOf("eight") <= digitPos && line.IndexOf("eight") >= 0)
                {
                    digitOne = "8";
                    digitPos = line.IndexOf("eight");
                }
                if (line.IndexOf("nine") <= digitPos && line.IndexOf("nine") >= 0)
                {
                    digitOne = "9";
                    digitPos = line.IndexOf("nine");
                }

                foreach (int ii in Enumerable.Range(0, line.Length).Reverse())
                {
                    if (line[ii] >= '0' && line[ii] <= '9')
                    {
                        digitTwo = Char.ToString(line[ii]);
                        digitPos = ii;
                        break;
                    }
                }

                if (line.LastIndexOf("one") > digitPos)
                {
                    digitTwo = "1";
                    digitPos = line.LastIndexOf("one");
                }
                if (line.LastIndexOf("two") > digitPos)
                {
                    digitTwo = "2";
                    digitPos = line.LastIndexOf("two");
                }
                if (line.LastIndexOf("three") > digitPos)
                {
                    digitTwo = "3";
                    digitPos = line.LastIndexOf("three");
                }
                if (line.LastIndexOf("four") > digitPos)
                {
                    digitTwo = "4";
                    digitPos = line.LastIndexOf("four");
                }
                if (line.LastIndexOf("five") > digitPos)
                {
                    digitTwo = "5";
                    digitPos = line.LastIndexOf("five");
                }
                if (line.LastIndexOf("six") > digitPos)
                {
                    digitTwo = "6";
                    digitPos = line.LastIndexOf("six");
                }
                if (line.LastIndexOf("seven") > digitPos)
                {
                    digitTwo = "7";
                    digitPos = line.LastIndexOf("seven");
                }
                if (line.LastIndexOf("eight") > digitPos)
                {
                    digitTwo = "8";
                    digitPos = line.LastIndexOf("eight");
                }
                if (line.LastIndexOf("nine") > digitPos)
                {
                    digitTwo = "9";
                    digitPos = line.LastIndexOf("nine");
                }

                partTwoSum += Convert.ToUInt64(digitOne + digitTwo);
            }

            return new PuzzleSolution(Convert.ToString(partOneSum), Convert.ToString(partTwoSum));
        }
    }
}
