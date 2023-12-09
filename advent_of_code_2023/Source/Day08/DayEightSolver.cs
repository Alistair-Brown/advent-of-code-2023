using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AOC.DayEight;

namespace AOC
{
    internal class DayEightSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] puzzleInputLines)
        {
            Dictionary<string, MapNode> mapNodesByName = new Dictionary<string, MapNode>();

            string leftRightInstructions = puzzleInputLines[0];

            for (int ii = 2; ii < puzzleInputLines.Length; ++ii)
            {
                MapNode node = new MapNode(puzzleInputLines[ii]);
                mapNodesByName.Add(node.ownNodeName, node);
            }

            // PartOne
            ulong partOneStepsTaken = SolvePartOne(mapNodesByName, leftRightInstructions);
            ulong partTwoStepsTaken = SolvePartTwo(mapNodesByName, leftRightInstructions);

            return new PuzzleSolution(partOneStepsTaken.ToString(), partTwoStepsTaken.ToString());
        }

        // Just follow the LR instructions starting from AAA until we reach ZZZ, and return
        // the number of steps it took to get there.
        ulong SolvePartOne(Dictionary<string, MapNode> mapNodesByName, string leftRightInstructions)
        {
            int currentLeftRightIndex = 0;
            ulong partOneStepsTaken = 0;
            string currentMapNodeName = "AAA";
            while (true)
            {
                if (leftRightInstructions[currentLeftRightIndex] == 'L')
                {
                    currentMapNodeName = mapNodesByName[currentMapNodeName].leftNodeName;
                }
                else
                {
                    currentMapNodeName = mapNodesByName[currentMapNodeName].rightNodeName;
                }
                ++partOneStepsTaken;

                if (currentMapNodeName == "ZZZ")
                {
                    break;
                }

                ++currentLeftRightIndex;

                // If we reach the end of the instructions, wrap back to the start.
                if (currentLeftRightIndex == leftRightInstructions.Length)
                {
                    currentLeftRightIndex = 0;
                }
            }

            return partOneStepsTaken;
        }

        ulong SolvePartTwo(Dictionary<string, MapNode> mapNodesByName, string leftRightInstructions)
        {
            List<string> partTwoStartingNodeNames = new List<string>();
            foreach (string nodeName in mapNodesByName.Keys)
            {
                if (nodeName[2] == 'A')
                {
                    partTwoStartingNodeNames.Add(nodeName);
                }
            }

            // Running some tests and inspecting the output revealed some important things about the
            // way our input is constructed:
            //  - From a given node with a name ending in A, a node ending in Z is only ever reached
            //    after executing a whole number of loops of the full LR instruction set, never halfway
            //    through.
            //  - After reaching the Z node, we perform one more complete loop of LR instructions, and then
            //    end up back where we were AFTER the very first loop. That tells us that we will reach a
            //    Z node every n loops of the LR instructions, where n is the number of loops it took us
            //    us to reach it the first time.
            //  - For my instruction input, the number of loops to go from A node to Z node was prime
            //    in all cases, which must be a deliberate feature of the input.
            //  - To find the first case where all paths converge on a Z node at once, we need the lowest
            //    common multiple of all the n's. Since they're prime, this is just their product.
            List<ulong> loopsToReachZForEachStartingA = new List<ulong>();
            foreach (string startingNode in partTwoStartingNodeNames)
            {
                string currentNodeName = startingNode;
                ulong loopsTaken = 0;
                
                while (true)
                {
                    currentNodeName = new MapRoute(currentNodeName, leftRightInstructions, mapNodesByName).finalNode;
                    ++loopsTaken;

                    if (currentNodeName[2] == 'Z')
                    {
                        break;
                    }
                }

                loopsToReachZForEachStartingA.Add(loopsTaken);
            }

            ulong loopsLCM = 1;
            foreach (ulong loops in loopsToReachZForEachStartingA)
            {
                loopsLCM *= loops;
            }

            // To turn this number of loops into a number of steps, just multiply by the number of
            // steps in a single loop
            return loopsLCM * (ulong)leftRightInstructions.Length;
        }
    }
}
