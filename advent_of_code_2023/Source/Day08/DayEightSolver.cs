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
            Dictionary<string, MapNode> leftRightMap = new Dictionary<string, MapNode>();

            string leftRightInstructions = puzzleInputLines[0];

            for (int ii = 2; ii < puzzleInputLines.Length; ++ii)
            {
                MapNode node = new MapNode(puzzleInputLines[ii]);
                leftRightMap.Add(node.ownNodeName, node);
            }

            // PartOne
            int currentLeftRightIndex = 0;
            int partOneStepsTaken = 0;
            string currentMapNodeName = "AAA";
            while (true)
            {
                if (leftRightInstructions[currentLeftRightIndex] == 'L')
                {
                    currentMapNodeName = leftRightMap[currentMapNodeName].leftNodeName;
                }
                else
                {
                    currentMapNodeName = leftRightMap[currentMapNodeName].rightNodeName;
                }
                ++partOneStepsTaken;

                if (currentMapNodeName == "ZZZ")
                {
                    break;
                }

                ++currentLeftRightIndex;
                if (currentLeftRightIndex == leftRightInstructions.Length)
                {
                    currentLeftRightIndex = 0;
                }
            }

            // PartTwo
            List<string> partTwoStartingNodes = new List<string>();
            foreach (string nodeName in leftRightMap.Keys)
            {
                if (nodeName[2] == 'A')
                {
                    partTwoStartingNodes.Add(nodeName);
                }
            }

            // The A node isn't in the loop, so let's just make our LR instructions start at the second instruction,
            // since that's the first one that's part of the actual loop.
            leftRightInstructions += leftRightInstructions[0];
            leftRightInstructions = leftRightInstructions.Remove(0, 1);

            List<List<string>> eachStartingNodesSubseqentLoopStarts = new List<List<string>>();

            foreach (string startingNode in partTwoStartingNodes)
            {
                List<string> startingNodes = new List<string>();

                int index = leftRightInstructions.Length - 1;
                Debug.Assert(leftRightMap.ContainsKey(startingNode));
                string currentNode = leftRightInstructions[index] == 'L' ?
                    leftRightMap[startingNode].leftNodeName :
                    leftRightMap[startingNode].rightNodeName;

                startingNodes.Add(currentNode);

                while (true)
                {
                    foreach (char instruction in leftRightInstructions)
                    {
                        currentNode = instruction == 'L' ?
                            leftRightMap[currentNode].leftNodeName :
                            leftRightMap[currentNode].rightNodeName;
                    }

                    if (startingNodes.Contains(currentNode))
                    {
                        eachStartingNodesSubseqentLoopStarts.Add(startingNodes);
                        break;
                    }
                    else
                    {
                        startingNodes.Add(currentNode);
                    }
                }
            }

            Dictionary<string, MapRoute> mapRoutes = new Dictionary<string, MapRoute>();
            List<string> currentLoopStartNodes = new List<string>();
            foreach (List<string> loopStarts in eachStartingNodesSubseqentLoopStarts)
            {
                foreach (string loopStart in loopStarts)
                {
                    mapRoutes.Add(loopStart, new MapRoute(loopStart, leftRightInstructions, leftRightMap));
                }
            }

            // Can just note that it's a condition of the input that a-z loops are closed and of the same
            // length, so just LCM.

            //currentLeftRightIndex = 0;
            //int partTwoStepsTaken = 0;
            //while (true)
            //{
            //    for (int ii = 0; ii < currentMapNodeNames.Count; ++ii)
            //    {
            //        if (leftRightInstructions[currentLeftRightIndex] == 'L')
            //        {
            //            currentMapNodeNames[ii] = leftRightMap[currentMapNodeNames[ii]].leftNodeName;
            //        }
            //        else
            //        {
            //            currentMapNodeNames[ii] = leftRightMap[currentMapNodeNames[ii]].rightNodeName;
            //        }
            //    }

            //    ++partTwoStepsTaken;
            //    ++currentLeftRightIndex;
            //    if (currentLeftRightIndex == leftRightInstructions.Length)
            //    {
            //        currentLeftRightIndex = 0;
            //    }

            //    bool allEndWithZ = true;
            //    foreach (string nodeName in currentMapNodeNames)
            //    {
            //        if (nodeName[2] != 'Z')
            //        {
            //            allEndWithZ = false;
            //            break;
            //        }
            //    }

            //    if (allEndWithZ)
            //    {
            //        break;
            //    }
            //}


            // Part two again.
            // If you just run the full LR input from each node, you've got a complete description
            // of all possible routes, just need to mark how many indices from each start reaches a
            // node that ends in Z.





            ulong loopLength = (ulong)leftRightInstructions.Length;
            ulong loopsTaken = 0;
            ulong stepsIntoLoop = 0;
            bool foundSolution = false;
            //HashSet<string> goodNodes = new HashSet<string> { "bbk",
            //                                                    "dkd",
            //                                                    "jgt",
            //                                                    "qtm",
            //                                                    "qjn",
            //                                                    "kfx",
            //                                                    "hjm",
            //                                                    "spc",
            //                                                    "gkh",
            //                                                    "bpt",
            //                                                    "lbj",
            //                                                    "dfj"};

            //List<ulong> loopsLengths = new List<ulong>();
            //foreach (string nodeName in currentLoopStartNodes)
            //{
            //    HashSet<string> visited = new HashSet<string>();
            //    string currentNode = nodeName;
            //    Console.WriteLine("NEW!!!!!!!!!!!!!!!!!!!");
            //    Console.WriteLine(currentNode);
            //    visited.Add(currentNode);
            //    int loops = 0;
            //    while (true)
            //    {
            //        loops++;
            //        currentNode = mapRoutes[currentNode].finalNode;
            //        Console.WriteLine(currentNode);
            //        if (visited.Contains(currentNode))
            //        {
            //            loopsLengths.Add((ulong)loops - 1);
            //            break;
            //        }
            //        else
            //        {
            //            visited.Add(currentNode);
            //        }
            //        //Console.WriteLine(currentNode);
            //    }
            //}

            //// All of them take one step and then enter a loop of a known length
            //// which ends in a z node.
            //ulong total = loopsLengths[0];
            //ulong loopsTwo = 0;
            //while (true)
            //{
            //    ++loopsTwo;
            //    total += loopsLengths[0];

            //    if ((total % loopsLengths[1] == 0) &&
            //        (total % loopsLengths[2] == 0) &&
            //        (total % loopsLengths[3] == 0) &&
            //        (total % loopsLengths[4] == 0) &&
            //        (total % loopsLengths[5] == 0))
            //    {
            //        break;
            //    }

            //}

            //// I'm on the right lines, but actually it's not a true loop until
            //// we're back to the same node and LR index
            //// Need to build a complete loop for each start position, noting:
            //// - How many steps to reach the start of the loop
            //// - How many steps long is the loop
            //// - What the indices of z nodes within the loop
            //Console.WriteLine(total);

            while (true)
            {
                ++loopsTaken;
                //int numGoodNodes = 0;
                List<List<int>> zPositionsThisLoop = new List<List<int>>();
                for (int ii = 0; ii < currentLoopStartNodes.Count; ++ii)
                {
                    //if (goodNodes.Contains(currentLoopStartNodes[ii]))
                    //{
                    //    numGoodNodes++;
                    //}

                    //Reset the start node names as we go, once we have their z positions for this

                    //loop that's all we need.

                   zPositionsThisLoop.Add(mapRoutes[currentLoopStartNodes[ii]].stepsToReachANodeEndingInZ);
                   currentLoopStartNodes[ii] = mapRoutes[currentLoopStartNodes[ii]].finalNode;
                }

                //if (numGoodNodes == currentLoopStartNodes.Count)
                //{
                //    break;
                //}

                foreach (int zSteps in zPositionsThisLoop[0])
                {
                    foundSolution = true;
                    for (int ii = 1; ii < zPositionsThisLoop.Count; ++ii)
                    {
                        if (!zPositionsThisLoop[ii].Contains(zSteps))
                        {
                            foundSolution = false;
                            break;
                        }
                    }

                    if (foundSolution)
                    {
                        stepsIntoLoop = (ulong)zSteps;
                        break;
                    }
                }

                if (foundSolution)
                {
                    break;
                }
                else
                {
                    ++loopsTaken;
                }
            }

            ulong partTwoStepsTaken = (loopsTaken * loopLength) + stepsIntoLoop;

            return new PuzzleSolution(partOneStepsTaken.ToString(), partTwoStepsTaken.ToString());
        }
    }
}
