using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AOC.DayTwenty;

namespace AOC
{
    internal class DayTwentySolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] puzzleInputLines)
        {
            CommunicationNetwork communicationNetwork = new CommunicationNetwork(puzzleInputLines);
            CommunicationNetwork partTwoCommunicationsNetwork = new CommunicationNetwork(puzzleInputLines);

            ulong partOneProduct = communicationNetwork.PressButtonNTimes(1000);
            ulong partTwoPresses = partTwoCommunicationsNetwork.PressesUntilRxReceivesLowPulse();

            return new PuzzleSolution(partOneProduct.ToString(), partTwoPresses.ToString());
        }
    }
}
