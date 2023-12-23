using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace AOC.DayTwentyThree
{
    enum LocationType
    {
        Start,
        End,
        Junction,
    }

    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    class ConnectedLocation
    {
        public HikingLocation location;
        public int stepsToReach;
        public bool isReachableInIce;
    }

    class HikingLocation
    {
        public (int row, int column) position;
        public LocationType locationType;

        public SortedList<(int row, int column), ConnectedLocation> connectedLocations;
    }

    class SubRouteBetweenLocations
    {
        public HikingLocation subRouteStart;
        public Direction firstStepDirection;
    }

    internal class HikingTrails
    {
        string[] rawHikingMap;

        (int row, int column) startPosition;
        (int row, int column) endPosition;
        Dictionary<(int row, int column), HikingLocation> hikingLocations;

        public HikingTrails(string[] inputLines)
        {
            rawHikingMap = inputLines;

            startPosition = (0, 1);
            endPosition = (inputLines.Length - 1, inputLines[0].Length - 2);

            hikingLocations = new Dictionary<(int row, int column), HikingLocation>();
            hikingLocations.Add(startPosition, new HikingLocation()
            {
                position = startPosition,
                locationType = LocationType.Start,
                connectedLocations = new SortedList<(int row, int column), ConnectedLocation>()
            });

            BFSHiking();
        }

        // Use a BFS to connect all of the junctions in the map.
        private void BFSHiking()
        {
            Stack<SubRouteBetweenLocations> subRoutes = new Stack<SubRouteBetweenLocations>();
            subRoutes.Push(new SubRouteBetweenLocations()
            {
                subRouteStart = hikingLocations[startPosition],
                firstStepDirection = Direction.Down,
            });

            while (subRoutes.Count > 0)
            {
                SubRouteBetweenLocations subRouteToWalk = subRoutes.Pop();

                (int row,
                 int column,
                 int steps,
                 bool walkableInIce,
                 bool reverseWalkableInIce,
                 List<Direction> nextDirections) nextLocation =
                    WalkToNextLocation(subRouteToWalk);

                // If we haven't already found this location, we need to set it up in the dictionary
                // and spawn some new subroutes from it.
                if (!hikingLocations.ContainsKey((nextLocation.row, nextLocation.column)))
                {
                    HikingLocation newLocation = new HikingLocation()
                    {
                        position = (nextLocation.row, nextLocation.column),
                        locationType = (nextLocation.row, nextLocation.column) == endPosition ?
                            LocationType.End : LocationType.Junction,
                        connectedLocations = new SortedList<(int row, int column), ConnectedLocation>()
                    };
                    hikingLocations.Add(newLocation.position, newLocation);

                    foreach (Direction direction in nextLocation.nextDirections)
                    {
                        subRoutes.Push(new SubRouteBetweenLocations()
                        {
                            subRouteStart = newLocation,
                            firstStepDirection = direction
                        });
                    }
                }

                HikingLocation subRouteStart = subRouteToWalk.subRouteStart;
                HikingLocation subRouteEnd = hikingLocations[(nextLocation.row, nextLocation.column)];

                // It's possible that these locations have already found each other in the opposite
                // direction, don't double link.
                if (!subRouteStart.connectedLocations.ContainsKey(subRouteEnd.position))
                {
                    subRouteStart.connectedLocations.Add(subRouteEnd.position, new ConnectedLocation()
                    {
                        location = subRouteEnd,
                        stepsToReach = nextLocation.steps,
                        isReachableInIce = nextLocation.walkableInIce
                    });
                    subRouteEnd.connectedLocations.Add(subRouteStart.position, new ConnectedLocation()
                    {
                        location = subRouteStart,
                        stepsToReach = nextLocation.steps,
                        isReachableInIce = nextLocation.reverseWalkableInIce
                    });
                }
            }
        }

        private (int row,
                 int column,
                 int steps,
                 bool walkableInIce,
                 bool reverseWalkableInIce,
                 List<Direction> nextDirections) WalkToNextLocation(SubRouteBetweenLocations subRoute)
        {
            (int row, int column) subStartPos = subRoute.subRouteStart.position;

            // Take the first step
            (int row, int column) position =
                subRoute.firstStepDirection == Direction.Up ? (subStartPos.row - 1, subStartPos.column) :
                subRoute.firstStepDirection == Direction.Down ? (subStartPos.row + 1, subStartPos.column) :
                subRoute.firstStepDirection == Direction.Left ? (subStartPos.row, subStartPos.column - 1) :
                (subStartPos.row, subStartPos.column + 1);
            int stepsTaken = 1;
            Direction lastStepDirection = subRoute.firstStepDirection;
            bool walkableInIce = true;
            bool reverseWalkableInIce = true;
            List<Direction> nextDirections = new List<Direction>();

            while (true)
            {
                if (walkableInIce)
                {
                    walkableInIce = StepWasWalkable(position, lastStepDirection);
                }
                if (reverseWalkableInIce)
                {
                    reverseWalkableInIce = StepWasWalkable(position, Reverse(lastStepDirection));
                }

                if (position == endPosition)
                {
                    break;
                }

                int numOptions = 0;
                (bool up, bool down, bool left, bool right) possibleDirections = (false, false, false, false);
                if ((lastStepDirection != Direction.Down) &&
                    (rawHikingMap[position.row - 1][position.column] != '#'))
                {
                    possibleDirections.up = true;
                    ++numOptions;
                }
                if ((lastStepDirection != Direction.Up) &&
                    (rawHikingMap[position.row + 1][position.column] != '#'))
                {
                    possibleDirections.down = true;
                    ++numOptions;
                }
                if ((lastStepDirection != Direction.Left) &&
                    (rawHikingMap[position.row][position.column + 1] != '#'))
                {
                    possibleDirections.right = true;
                    ++numOptions;
                }
                if ((lastStepDirection != Direction.Right) &&
                    (rawHikingMap[position.row][position.column - 1] != '#'))
                {
                    possibleDirections.left = true;
                    ++numOptions;
                }

                Debug.Assert(numOptions > 0, "I didn't think the map had any dead ends");
                
                if (numOptions > 1)
                {
                    // Found a junction
                    if (possibleDirections.up)
                        nextDirections.Add(Direction.Up);
                    if (possibleDirections.down)
                        nextDirections.Add(Direction.Down);
                    if (possibleDirections.left)
                        nextDirections.Add(Direction.Left);
                    if (possibleDirections.right)
                        nextDirections.Add(Direction.Right);
                    break;
                }

                if (possibleDirections.up)
                {
                    position = (position.row - 1, position.column);
                    ++stepsTaken;
                    lastStepDirection = Direction.Up;
                }
                else if (possibleDirections.down)
                {
                    position = (position.row + 1, position.column);
                    ++stepsTaken;
                    lastStepDirection = Direction.Down;
                }
                else if (possibleDirections.left)
                {
                    position = (position.row, position.column - 1);
                    ++stepsTaken;
                    lastStepDirection = Direction.Left;
                }
                else if (possibleDirections.right)
                {
                    position = (position.row, position.column + 1);
                    ++stepsTaken;
                    lastStepDirection = Direction.Right;
                }
            }

            return (position.row, position.column, stepsTaken, walkableInIce, reverseWalkableInIce, nextDirections);
        }

        private bool StepWasWalkable((int row, int column) position, Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return rawHikingMap[position.row][position.column] != 'v';
                case Direction.Down:
                    return rawHikingMap[position.row][position.column] != '^';
                case Direction.Left:
                    return rawHikingMap[position.row][position.column] != '>';
                case Direction.Right:
                default:
                    Debug.Assert(direction == Direction.Right);
                    return rawHikingMap[position.row][position.column] != '<';
            }
        }

        private Direction Reverse(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Up;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                default:
                    Debug.Assert(direction == Direction.Right);
                    return Direction.Left;
            }
        }

        struct Route
        {
            public int stepsTaken;
            public HashSet<(int row, int column)> locationsVisited;
            public HikingLocation currentLocation;
        }

        public int LongestRoute(bool isIcy)
        {
            int longestRoute = 0;

            Stack<Route> routesInProgress = new Stack<Route>();
            routesInProgress.Push(new Route()
            {
                stepsTaken = 0,
                locationsVisited = new HashSet<(int row, int column)>(),
                currentLocation = hikingLocations[startPosition]
            });

            while (routesInProgress.Count > 0) 
            {
                Route route = routesInProgress.Pop();
                route.locationsVisited.Add(route.currentLocation.position);

                if (route.currentLocation.locationType == LocationType.End)
                {
                    if (route.stepsTaken > longestRoute)
                    {
                        longestRoute = route.stepsTaken;
                        continue;
                    }
                }

                foreach (KeyValuePair<(int row, int column), ConnectedLocation> location in route.currentLocation.connectedLocations)
                {
                    if (!route.locationsVisited.Contains(location.Key) && (!isIcy || location.Value.isReachableInIce))
                    {
                        Route newRoute = new Route()
                        {
                            stepsTaken = route.stepsTaken + location.Value.stepsToReach,
                            locationsVisited = new HashSet<(int row, int column)>(route.locationsVisited),
                            currentLocation = location.Value.location
                        };
                        routesInProgress.Push(newRoute);
                    }
                }
            }

            return longestRoute;
        }
    }
}
