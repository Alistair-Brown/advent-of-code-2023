using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class DaySixSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] puzzleInputLines)
        {
            string[] rawTimes = puzzleInputLines[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] rawDistances = puzzleInputLines[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Debug.Assert(rawTimes[0] == "Time:");
            Debug.Assert(rawDistances[0] == "Distance:");
            Debug.Assert(rawTimes.Length == rawDistances.Length);

            int partOneWaysToWin = 1;
            string rawPartTwoTime = "";
            string rawPartTwoDistance = "";
            for (int ii = 1; ii < rawTimes.Length; ++ii)
            {
                partOneWaysToWin *= WaysToWinRace(Convert.ToUInt64(rawTimes[ii]), Convert.ToUInt64(rawDistances[ii]));
                rawPartTwoTime += rawTimes[ii];
                rawPartTwoDistance += rawDistances[ii];
            }

            int partTwoWaysToWin = WaysToWinRace(Convert.ToUInt64(rawPartTwoTime), Convert.ToUInt64(rawPartTwoDistance));

            return new PuzzleSolution(partOneWaysToWin.ToString(), partTwoWaysToWin.ToString());
        }

        // To solve with O(n) complexity, we can solve a quadratic equation.
        // The optimal amount of time to hold the button down for is half the
        // total time. Hold down a ms longer you gain 1ms of speed to use in
        // ((time / 2) - 1) time, but lose 1ms of time to use the (time / 2) speed
        // you've already gain, which is a net loss. Equivalent applies for holding
        // 1ms shorter. The optimum time held gives you (time/2) speed to use in
        // (time/2) time, giving you (time/2)^2 distance.
        // As you move away from this optimum by holding down for longer, the formula
        // for distance as speed * time becomes:
        // ((time / 2) + extraTimeHeld) * ((time / 2) - extraTimeHeld) = distance
        // We need to rearrange this formula and find all integer values of total time
        // held that exceed the required distance.
        private int WaysToWinRace(ulong time, ulong previousWinningDistance)
        {
            int waysToWin;
            ulong requiredDistance = previousWinningDistance + 1;

            // Rearranged from formula in header comment, this finds how many extra ms we
            // can hold down the button for and still reach the required distance.
            double extraMsHeld = (Math.Sqrt((((double)time / 2) * (double)time / 2) - requiredDistance));

            // The above didn't account for the fact that only integer hold times are valid.
            // If our total time was even, then the optimum hold (half of that total) is an
            // integer number of seconds. So we just need to floor the extraMsHeld to an integer
            // double it to account the possibility of holding it for *less* than the optimum
            // and then add 1 to make sure our total includes that central optimum value.
            if (time % 2 == 0) { waysToWin = (int)Math.Floor(extraMsHeld) * 2 + 1; }
            // Odd cases have their optimum position halfways between two values.
            // So first valid step is 0.5ms from optimum, next at 1.5ms etc. To get the number
            // of valid integer hold times, we can therefore subtract 0.5ms from our hold time, to
            // instead get the extra hold time from the first of the two optimum integer values
            // in the middle of the range, then floor and double this to get the integer values
            // on either side of these midpoints, then +2 to add those optimum two integer midpoints
            // back into our total.
            else { waysToWin = (int)Math.Floor((extraMsHeld - 0.5f)) * 2 + 2; }

            return waysToWin;
        }
    }
}
