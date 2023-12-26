using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.DayTwentyFive
{
    internal class ComponentJumble
    {
        private Dictionary<string, List<string>> componentsAndConnections;

        public ComponentJumble(string[] inputLines)
        {
            componentsAndConnections = new Dictionary<string, List<string>>();

            foreach (string line in inputLines)
            {
                string[] splitOnColon = line.Split(new string[] { ": " }, StringSplitOptions.None);
                string lhsComponent = splitOnColon[0];
                string[] rhsComponents = splitOnColon[1].Split(' ');

                if (!componentsAndConnections.ContainsKey(lhsComponent))
                {
                    componentsAndConnections.Add(lhsComponent, new List<string>());
                }

                foreach (string rhs in rhsComponents)
                {
                    if (!componentsAndConnections.ContainsKey(rhs))
                    {
                        componentsAndConnections.Add(rhs, new List<string>());
                    }

                    componentsAndConnections[lhsComponent].Add(rhs);
                    componentsAndConnections[rhs].Add(lhsComponent);
                }
            }
        }

        public int ProductOfCutConnections()
        {
            bool foundConnectionOne = false;
            bool foundConnectionTwo = false;
            bool foundConnectionThree = false;

            (string, string) connectionOne = ("", "");
            (string, string) connectionTwo = ("", "");
            (string, string) connectionThree = ("", "");
            List<List<string>> otherRoutes = new List<List<string>>();

            HashSet<string> invalidConnections = new HashSet<string>();

            bool foundAllThreeConnections = false;

            while (!foundAllThreeConnections)
            {
                foreach (KeyValuePair<string, List<string>> nodeAndConnections in componentsAndConnections)
                {
                    foreach (string connectedNode in nodeAndConnections.Value)
                    {
                        if (!SetContainsConnection(nodeAndConnections.Key, connectedNode, invalidConnections))
                        {
                            foundConnectionOne = IsOneOfNConnections(
                                nodeAndConnections.Key,
                                connectedNode,
                                2,
                                new List<(string, string)>(),
                                out otherRoutes);

                            if (foundConnectionOne)
                            {
                                connectionOne = (nodeAndConnections.Key, connectedNode);
                                break;
                            }
                        }
                    }

                    if (foundConnectionOne)
                    {
                        break;
                    }
                }

                // We've found one of the three conections to cut, and 2 routes each containing one of the other
                // two. Repeating a similar approach will tell us which of the connections in the other 2 routes is
                // the one to cut.
                for (int ii = 0; ii < otherRoutes[0].Count - 1; ++ii)
                {
                    if (!SetContainsConnection(otherRoutes[0][ii], otherRoutes[0][ii + 1], invalidConnections))
                    {
                        foundConnectionTwo = IsOneOfNConnections(
                        otherRoutes[0][ii],
                        otherRoutes[0][ii + 1],
                        1,
                        new List<(string, string)>() { (connectionOne.Item1, connectionOne.Item2) },
                        out List<List<string>> unused);

                        if (foundConnectionTwo)
                        {
                            connectionTwo = (otherRoutes[0][ii], otherRoutes[0][ii + 1]);
                            break;
                        }
                    }
                }

                if (!foundConnectionTwo)
                {
                    InsertConnections(connectionOne.Item1, connectionOne.Item2, invalidConnections);
                    continue;
                }

                for (int ii = 0; ii < otherRoutes[1].Count - 1; ++ii)
                {
                    if (!SetContainsConnection(otherRoutes[1][ii], otherRoutes[1][ii + 1], invalidConnections))
                    {
                        foundConnectionThree = IsOneOfNConnections(
                        otherRoutes[1][ii],
                        otherRoutes[1][ii + 1],
                        0,
                        new List<(string, string)>() {
                        (connectionOne.Item1, connectionOne.Item2),
                        (connectionTwo.Item1, connectionTwo.Item2)
                        },
                        out List<List<string>> unused);

                        if (foundConnectionThree)
                        {
                            connectionThree = (otherRoutes[1][ii], otherRoutes[1][ii + 1]);
                            break;
                        }
                    }
                }

                if (!foundConnectionThree)
                {
                    InsertConnections(connectionOne.Item1, connectionOne.Item2, invalidConnections);
                    continue;
                }
                else
                {
                    foundAllThreeConnections = true;
                }
            }

            HashSet<string> connections = new HashSet<string>();
            InsertConnections(connectionOne.Item1, connectionOne.Item2, connections);
            InsertConnections(connectionTwo.Item1, connectionTwo.Item2, connections);
            InsertConnections(connectionThree.Item1, connectionThree.Item2, connections);

            int subJumbleOneSize = SubJumbleSize(connectionOne.Item1, connections);
            int subJumbleTwoSize = componentsAndConnections.Count - subJumbleOneSize;

            return subJumbleOneSize * subJumbleTwoSize;
        }

        bool IsOneOfNConnections(
            string nodeOne,
            string nodeTwo,
            int numberOfOtherConnectionsAllowed,
            List<(string, string)> disallowedConnectionsList,
            out List<List<string>> otherRoutes)
        {
            otherRoutes = new List<List<string>>();

            HashSet<string> disallowedConnections = new HashSet<string>();
            InsertConnections(nodeOne, nodeTwo, disallowedConnections);

            foreach ((string one, string two) connection in disallowedConnectionsList)
            {
                InsertConnections(connection.one, connection.two, disallowedConnections);
            }

            for (int ii = 0; ii < numberOfOtherConnectionsAllowed; ++ii)
            {
                List<string> routeFound = DFSBetweenNodes(nodeOne, nodeTwo, disallowedConnections);

                if (routeFound.Count > 0)
                {
                    for (int node = 0; node < routeFound.Count - 1; ++node)
                    {
                        InsertConnections(routeFound[node], routeFound[node + 1], disallowedConnections);
                    }
                    otherRoutes.Add(routeFound);
                }
                else
                {
                    return false;
                }
            }

            // Then try one more routes than we are allowed to find. If we *don't* one, then our input
            // connection is one of N routes between the two input nodes.
            List<string> oneMoreRoute = DFSBetweenNodes(nodeOne, nodeTwo, disallowedConnections);

            if (oneMoreRoute.Count > 0)
            {
                return false;
            }

            return true;
        }

        private int SubJumbleSize(string startingNode, HashSet<string> disallowedConnections)
        {
            HashSet<string> visitedNodes = new HashSet<string>() { startingNode };
            Stack<string> nodesToVisitFrom = new Stack<string>();
            nodesToVisitFrom.Push(startingNode);

            int size = 1;

            while (nodesToVisitFrom.Count > 0)
            {
                string currentNode = nodesToVisitFrom.Pop();

                foreach (string connectedNode in componentsAndConnections[currentNode])
                {
                    if (!SetContainsConnection(currentNode, connectedNode, disallowedConnections) &&
                        !visitedNodes.Contains(connectedNode))
                    {
                        ++size;
                        nodesToVisitFrom.Push(connectedNode);
                        visitedNodes.Add(connectedNode);
                    }
                }
            }

            return size;
        }

        private List<string> DFSBetweenNodes(string nodeOne, string nodeTwo, HashSet<string> disallowedConnections)
        {
            HashSet<string> localDisallowedConnections = new HashSet<string>();
            Stack<string> nodesOnRoute = new Stack<string>();
            HashSet<string> visitedNodes = new HashSet<string>();
            nodesOnRoute.Push(nodeOne);
            visitedNodes.Add(nodeOne);

            while (nodesOnRoute.Count > 0)
            {
                bool completedLoop = false;
                bool foundNextStep = false;

                foreach (string connectedNode in componentsAndConnections[nodesOnRoute.Peek()])
                {
                    if ((connectedNode == nodeTwo) &&
                        !SetContainsConnection(nodesOnRoute.Peek(), connectedNode, disallowedConnections))
                    {
                        nodesOnRoute.Push(connectedNode);
                        completedLoop = true;
                        break;
                    }

                    // We can follow any route that doesn't take us straight back to the node we just removed from the
                    // stack, one already in the route, or contains a disallowed connection.
                    if (!visitedNodes.Contains(connectedNode) &&
                        !SetContainsConnection(nodesOnRoute.Peek(), connectedNode, disallowedConnections) &&
                        !SetContainsConnection(nodesOnRoute.Peek() , connectedNode, localDisallowedConnections))
                    {
                        nodesOnRoute.Push(connectedNode);
                        visitedNodes.Add(connectedNode);
                        foundNextStep = true;
                        break;
                    }
                }

                if (completedLoop)
                {
                    break;
                }

                if (!foundNextStep)
                {
                    string mostRecentlyVisitedNode = nodesOnRoute.Pop();
                    if (nodesOnRoute.Count > 0)
                    {
                        InsertConnections(nodesOnRoute.Peek(), mostRecentlyVisitedNode, localDisallowedConnections);
                        visitedNodes.Remove(mostRecentlyVisitedNode);
                    }
                }
            }

            // This will either be the complete stack for the route, or an empty stack, depending on whether we found
            // a route.
            return nodesOnRoute.ToList();
        }

        // Connections are storied with the connected nodes in alphabetical order so that the connection appears
        // the same in the set regardless of direction of traversal.
        private void InsertConnections(string nodeOne, string nodeTwo, HashSet<string> setOfConnections)
        {
            string combinedString = AlphabeticallyCombineNodes(nodeOne, nodeTwo);

            Debug.Assert(!setOfConnections.Contains(combinedString), "Don't think I should ever be entering the same string twice");

            setOfConnections.Add(combinedString);
        }

        private bool SetContainsConnection(string nodeOne, string nodeTwo, HashSet<string> setOfConnections)
        {
            return setOfConnections.Contains(AlphabeticallyCombineNodes(nodeOne, nodeTwo));
        }

        private string AlphabeticallyCombineNodes(string nodeOne, string nodeTwo)
        {
            string alphabeticalOrderString;

            if (string.Compare(nodeOne, nodeTwo) < 0)
            {
                alphabeticalOrderString = nodeOne + nodeTwo;
            }
            else
            {
                alphabeticalOrderString = nodeTwo + nodeOne;
            }

            return alphabeticalOrderString;
        }
    }
}
