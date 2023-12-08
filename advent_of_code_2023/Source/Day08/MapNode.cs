using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.DayEight
{
    internal struct MapNode
    {
        public readonly string ownNodeName;
        public readonly string leftNodeName;
        public readonly string rightNodeName;

        public MapNode(string rawInputLine)
        {
            Debug.Assert(rawInputLine.Substring(3, 4) == " = (");
            Debug.Assert(rawInputLine.Substring(10, 2) == ", ");
            Debug.Assert(rawInputLine.Substring(15, 1) == ")");

            ownNodeName = rawInputLine.Substring(0, 3);
            leftNodeName = rawInputLine.Substring(7, 3);
            rightNodeName = rawInputLine.Substring(12, 3);
        }
    }

    internal class MapRoute
    {
        public readonly List<int> stepsToReachANodeEndingInZ;
        public readonly string startNode;
        public readonly string finalNode;

        public MapRoute(string startNode, string leftRightInstructions, Dictionary<string, MapNode> leftRightMap)
        {
            this.startNode = startNode;
            stepsToReachANodeEndingInZ = new List<int>();

            string currentNode = startNode;
            int stepsTaken = 0;
            foreach (char instruction in leftRightInstructions)
            {
                currentNode = instruction == 'L' ?
                    leftRightMap[currentNode].leftNodeName :
                    leftRightMap[currentNode].rightNodeName;

                ++stepsTaken;

                if (currentNode[2] == 'Z')
                {
                    stepsToReachANodeEndingInZ.Add(stepsTaken);
                }
            }

            finalNode = currentNode;
        }
    }
}
