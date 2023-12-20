using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.DaySeventeen
{
    internal class CrucibleMap
    {
        enum Direction
        {
            North = 0,
            South = 1,
            East = 2,
            West = 3
        }

        // Making a class rather than struct since I need to store this by reference in
        // the sorted list while Djikstra-ing.
        class CrucibleMapNode
        {
            public int rowPos;
            public int columnPos;

            public int value;

            // Has an index for each way we could approach: 1 step north, 2 steps north,
            // 3 steps north, 1 step south, 2 steps south etc.
            public int[] lowestCostToReachFromDifferentDirectionsAndSteps;
        }

        CrucibleMapNode[][] mapToTraverse;
        int minSteps;
        int maxSteps;

        public CrucibleMap(string[] rawMap, int minSteps, int maxSteps)
        {
            mapToTraverse = new CrucibleMapNode[rawMap.Length][];

            foreach (int row in Enumerable.Range(0, rawMap.Length))
            {
                mapToTraverse[row] = new CrucibleMapNode[rawMap[0].Length];

                foreach (int column in Enumerable.Range(0, rawMap[0].Length))
                {
                    mapToTraverse[row][column] = new CrucibleMapNode()
                    {
                        rowPos = row,
                        columnPos = column,
                        value = (int)rawMap[row][column] - (int)'0',
                        lowestCostToReachFromDifferentDirectionsAndSteps = new int[4 * maxSteps],
                    };
                    foreach (int ii in Enumerable.Range(0, 4 * maxSteps))
                    {
                        mapToTraverse[row][column].lowestCostToReachFromDifferentDirectionsAndSteps[ii] = int.MaxValue;
                    }
                }
            }

            this.minSteps = minSteps;
            this.maxSteps = maxSteps;
        }

        public int CheapestRoute()
        {
            SortedList<(int cost, int row, int column, int nSteps, int sSteps, int eSteps, int wSteps), CrucibleMapNode> nodesToExploreFrom =
                new SortedList<(int cost, int row, int column, int nSteps, int sSteps, int eSteps, int wSteps), CrucibleMapNode>();

            CrucibleMapNode startingNode = mapToTraverse[0][0];
            nodesToExploreFrom.Add((0, 0, 0, 0, 0, 0, 0), startingNode);

            bool foundExit = false;
            int cheapestRoute = 0;
            while (nodesToExploreFrom.Count > 0)
            {
                CrucibleMapNode currentNode = nodesToExploreFrom.Values[0];
                (int cost, int row, int column, int nSteps, int sSteps, int eSteps, int wSteps) currentKey = nodesToExploreFrom.Keys[0];
                nodesToExploreFrom.RemoveAt(0);

                if ((currentNode.rowPos == mapToTraverse.Length - 1) &&
                    (currentNode.columnPos == mapToTraverse[0].Length - 1))
                {
                    cheapestRoute = currentKey.cost;
                    foundExit = true;
                    break;
                }

                // Move North if allowed
                if (currentNode.rowPos > 0 &&
                    currentKey.nSteps < maxSteps &&
                    currentKey.sSteps == 0 &&
                    !(currentKey.eSteps > 0 && currentKey.eSteps < minSteps) &&
                    !(currentKey.wSteps > 0 && currentKey.wSteps < minSteps))
                {
                    CrucibleMapNode nextNode = mapToTraverse[currentNode.rowPos - 1][currentNode.columnPos];
                    int nextCost = currentKey.cost + nextNode.value;
                    int index = GetNextDirectionAndStepIndex(
                                currentKey.nSteps,
                                currentKey.sSteps,
                                currentKey.eSteps,
                                currentKey.wSteps,
                                Direction.North);

                    if (nextCost < nextNode.lowestCostToReachFromDifferentDirectionsAndSteps[index])
                    {
                        nextNode.lowestCostToReachFromDifferentDirectionsAndSteps[index] = nextCost;

                        nodesToExploreFrom.Add(
                            (nextCost, nextNode.rowPos, nextNode.columnPos,
                            currentKey.nSteps + 1,
                            0,
                            0,
                            0),
                            nextNode);
                    }
                }

                // Move South if allowed
                if (currentNode.rowPos < mapToTraverse.Length - 1 &&
                    currentKey.sSteps < maxSteps &&
                    currentKey.nSteps == 0 &&
                    !(currentKey.eSteps > 0 && currentKey.eSteps < minSteps) &&
                    !(currentKey.wSteps > 0 && currentKey.wSteps < minSteps))
                {
                    CrucibleMapNode nextNode = mapToTraverse[currentNode.rowPos + 1][currentNode.columnPos];
                    int nextCost = currentKey.cost + nextNode.value;
                    int index = GetNextDirectionAndStepIndex(
                                currentKey.nSteps,
                                currentKey.sSteps,
                                currentKey.eSteps,
                                currentKey.wSteps,
                                Direction.South);

                    if (nextCost < nextNode.lowestCostToReachFromDifferentDirectionsAndSteps[index])
                    {
                        nextNode.lowestCostToReachFromDifferentDirectionsAndSteps[index] = nextCost;

                        nodesToExploreFrom.Add(
                            (nextCost, nextNode.rowPos, nextNode.columnPos,
                            0,
                            currentKey.sSteps + 1,
                            0,
                            0),
                            nextNode);
                    }
                }

                // Move East if allowed
                if (currentNode.columnPos < mapToTraverse[0].Length - 1 &&
                    currentKey.eSteps < maxSteps &&
                    currentKey.wSteps == 0 &&
                    !(currentKey.nSteps > 0 && currentKey.nSteps < minSteps) &&
                    !(currentKey.sSteps > 0 && currentKey.sSteps < minSteps))
                {
                    CrucibleMapNode nextNode = mapToTraverse[currentNode.rowPos][currentNode.columnPos + 1];
                    int nextCost = currentKey.cost + nextNode.value;
                    int index = GetNextDirectionAndStepIndex(
                                currentKey.nSteps,
                                currentKey.sSteps,
                                currentKey.eSteps,
                                currentKey.wSteps,
                                Direction.East);

                    if (nextCost < nextNode.lowestCostToReachFromDifferentDirectionsAndSteps[index])
                    {
                        nextNode.lowestCostToReachFromDifferentDirectionsAndSteps[index] = nextCost;

                        nodesToExploreFrom.Add(
                            (nextCost, nextNode.rowPos, nextNode.columnPos,
                            0,
                            0,
                            currentKey.eSteps + 1,
                            0),
                            nextNode);
                    }
                }

                // Move West if allowed
                if (currentNode.columnPos > 0 &&
                    currentKey.wSteps < maxSteps &&
                    currentKey.eSteps == 0 &&
                    !(currentKey.nSteps > 0 && currentKey.nSteps < minSteps) &&
                    !(currentKey.sSteps > 0 && currentKey.sSteps < minSteps))
                {
                    CrucibleMapNode nextNode = mapToTraverse[currentNode.rowPos][currentNode.columnPos - 1];
                    int nextCost = currentKey.cost + nextNode.value;
                    int index = GetNextDirectionAndStepIndex(
                                currentKey.nSteps,
                                currentKey.sSteps,
                                currentKey.eSteps,
                                currentKey.wSteps,
                                Direction.West);

                    if (nextCost < nextNode.lowestCostToReachFromDifferentDirectionsAndSteps[index])
                    {
                        nextNode.lowestCostToReachFromDifferentDirectionsAndSteps[index] = nextCost;

                        nodesToExploreFrom.Add(
                            (nextCost, nextNode.rowPos, nextNode.columnPos,
                            0,
                            0,
                            0,
                            currentKey.wSteps + 1),
                            nextNode);
                    }
                }
            }

            Debug.Assert(foundExit, "Never found an exit");
            return cheapestRoute;
        }

        private int GetNextDirectionAndStepIndex(int nSteps, int sSteps, int eSteps, int wSteps, Direction nextStep)
        {
            int index;

            switch (nextStep)
            {
                case Direction.North:
                    index = ((int)nextStep * maxSteps) + nSteps;
                    break;
                case Direction.South:
                    index = ((int)nextStep * maxSteps) + sSteps;
                    break;
                case Direction.East:
                    index = ((int)nextStep * maxSteps) + eSteps;
                    break;
                case Direction.West:
                    index = ((int)nextStep * maxSteps) + wSteps;
                    break;
                default:
                    Debug.Assert(false);
                    index = 0;
                    break;
            }
            return index;
        }
    }
}
