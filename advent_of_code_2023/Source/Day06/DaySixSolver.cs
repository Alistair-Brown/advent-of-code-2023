using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        // There's definitely a maths-y way of working this out with O(n), should
        // find that.
        private int WaysToWinRace(ulong time, ulong distance)
        {
            int waysToWin = 0;
            for (ulong timeHeld = 1; timeHeld < time; ++timeHeld)
            {
                if (IsAWayToWin(time, timeHeld, distance))
                {
                    ++waysToWin;
                }
            }

            return waysToWin;
        }

        private bool IsAWayToWin(ulong totalTime, ulong timeHeld, ulong distanceNeeded)
        {
            ulong speed = timeHeld;
            ulong distanceTravelled = speed * (totalTime - timeHeld);
            return distanceTravelled > distanceNeeded;
        }
    }
}
