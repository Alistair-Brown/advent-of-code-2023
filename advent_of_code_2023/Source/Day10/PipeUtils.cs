using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.DayTen
{
    internal enum Direction
    {
        North,
        South,
        East,
        West
    }

    public struct GridPos
    {
        public int row;
        public int column;

        public GridPos(int row, int column)
        {
            this.row = row;
            this.column = column;
        }
    }

    internal struct JourneyAroundLoop
    {
        public GridPos currentPosition;
        public int stepsTaken;
        public Direction lastDirectionMovedFrom;

        public int leftTurnsTaken;
        public int rightTurnsTaken;

        public Stack<GridPos> positionsToLeftOfLoop;
        public Stack<GridPos> positionsToRightOfLoop;

        public JourneyAroundLoop(GridPos currentPos, Direction lastDirection)
        {
            this.currentPosition = currentPos;
            this.lastDirectionMovedFrom = lastDirection;
            this.stepsTaken = 0;
            this.leftTurnsTaken = 0;
            this.rightTurnsTaken = 0;

            positionsToLeftOfLoop = new Stack<GridPos>();
            positionsToRightOfLoop = new Stack<GridPos>();
        }

    }

    internal static class PipeUtils
    {
        public const char StartPipe = 'S';
        public const char NorthSouthPipe = '|';
        public const char EastWestPipe = '-';
        public const char NorthEastPipe = 'L';
        public const char NorthWestPipe = 'J';
        public const char SouthWestPipe = '7';
        public const char SouthEastPipe = 'F';
        public const char Ground = '.';

        public static GridPos GetStartPosition(string[] inputLines)
        {
            for (int row = 0; row < inputLines.Length; row++)
            {
                for (int column = 0; column < inputLines[row].Length; column++)
                {
                    if (inputLines[row][column] == StartPipe)
                    {
                        return new GridPos(row, column);
                    }
                }
            }

            Debug.Assert(false, "Failed to find start position");
            return new GridPos(0, 0);
        }

        // A valid direction to move in from the start position is any direction with
        // a pipe connecting to the start position.
        public static Direction FindValidDirectionFromStart(GridPos position, string[] gridLines)
        {
            if (position.row > 0)
            {
                char northChar = gridLines[position.row - 1][position.column];
                if ((northChar == NorthSouthPipe) ||
                    (northChar == SouthEastPipe) ||
                    (northChar == SouthWestPipe))
                {
                    return Direction.North;
                }
            }

            if (position.row < gridLines.Length - 1)
            {
                char southChar = gridLines[position.row + 1][position.column];
                if ((southChar == NorthSouthPipe) ||
                    (southChar == NorthEastPipe) ||
                    (southChar == NorthWestPipe))
                {
                    return Direction.South;
                }
            }

            if (position.column > 0)
            {
                char westChar = gridLines[position.row][position.column - 1];
                if ((westChar == EastWestPipe) ||
                    (westChar == SouthEastPipe) ||
                    (westChar == NorthEastPipe))
                {
                    return Direction.West;
                }
            }

            Debug.Assert(position.column < gridLines[0].Length);

            char eastChar = gridLines[position.row][position.column + 1];
            Debug.Assert((eastChar == EastWestPipe) ||
                (eastChar == NorthWestPipe) ||
                (eastChar == SouthWestPipe));

            return Direction.East;
        }

        // As we go around the loop, keep track of positions to the left or right of the
        // loop's path. It's enough to only do this from the straight pieces, those adjacent
        // to corners will be found by a later floodfill.
        public static void TakeNextStepAroundLoop(ref JourneyAroundLoop loopJourney,
                                                  string[] puzzleInputLines,
                                                  List<List<bool>> positionsExplored)
        {
            GridPos pipePosition = loopJourney.currentPosition;
            Direction directionEnteredFrom = loopJourney.lastDirectionMovedFrom;

            Direction nextPipeEntranceDirection;

            positionsExplored[pipePosition.row][pipePosition.column] = true;

            switch (puzzleInputLines[pipePosition.row][pipePosition.column])
            {
                case NorthSouthPipe:
                    if (directionEnteredFrom == Direction.North)
                    {
                        loopJourney.positionsToRightOfLoop.Push(new GridPos(pipePosition.row, pipePosition.column - 1));
                        loopJourney.positionsToLeftOfLoop.Push(new GridPos(pipePosition.row, pipePosition.column + 1));
                        ++pipePosition.row;
                        nextPipeEntranceDirection = Direction.North;
                    }
                    else
                    {
                        loopJourney.positionsToRightOfLoop.Push(new GridPos(pipePosition.row, pipePosition.column + 1));
                        loopJourney.positionsToLeftOfLoop.Push(new GridPos(pipePosition.row, pipePosition.column - 1));
                        --pipePosition.row;
                        nextPipeEntranceDirection = Direction.South;
                    }
                    break;
                case EastWestPipe:
                    if (directionEnteredFrom == Direction.East)
                    {
                        loopJourney.positionsToRightOfLoop.Push(new GridPos(pipePosition.row - 1, pipePosition.column));
                        loopJourney.positionsToLeftOfLoop.Push(new GridPos(pipePosition.row + 1, pipePosition.column));
                        --pipePosition.column;
                        nextPipeEntranceDirection = Direction.East;
                    }
                    else
                    {
                        loopJourney.positionsToRightOfLoop.Push(new GridPos(pipePosition.row + 1, pipePosition.column));
                        loopJourney.positionsToLeftOfLoop.Push(new GridPos(pipePosition.row - 1, pipePosition.column));
                        ++pipePosition.column;
                        nextPipeEntranceDirection = Direction.West;
                    }
                    break;
                case NorthEastPipe:
                    if (directionEnteredFrom == Direction.North)
                    {
                        loopJourney.positionsToRightOfLoop.Push(new GridPos(pipePosition.row, pipePosition.column - 1));
                        loopJourney.positionsToRightOfLoop.Push(new GridPos(pipePosition.row + 1, pipePosition.column));
                        loopJourney.positionsToLeftOfLoop.Push(new GridPos(pipePosition.row - 1, pipePosition.column + 1));
                        ++pipePosition.column;
                        nextPipeEntranceDirection = Direction.West;
                        ++loopJourney.leftTurnsTaken;
                    }
                    else
                    {
                        loopJourney.positionsToLeftOfLoop.Push(new GridPos(pipePosition.row, pipePosition.column - 1));
                        loopJourney.positionsToLeftOfLoop.Push(new GridPos(pipePosition.row + 1, pipePosition.column));
                        loopJourney.positionsToRightOfLoop.Push(new GridPos(pipePosition.row - 1, pipePosition.column + 1));
                        --pipePosition.row;
                        nextPipeEntranceDirection = Direction.South;
                        ++loopJourney.rightTurnsTaken;
                    }
                    break;
                case NorthWestPipe:
                    if (directionEnteredFrom == Direction.North)
                    {
                        loopJourney.positionsToLeftOfLoop.Push(new GridPos(pipePosition.row, pipePosition.column + 1));
                        loopJourney.positionsToLeftOfLoop.Push(new GridPos(pipePosition.row + 1, pipePosition.column));
                        loopJourney.positionsToRightOfLoop.Push(new GridPos(pipePosition.row - 1, pipePosition.column - 1));
                        --pipePosition.column;
                        nextPipeEntranceDirection = Direction.East;
                        ++loopJourney.rightTurnsTaken;
                    }
                    else
                    {
                        loopJourney.positionsToRightOfLoop.Push(new GridPos(pipePosition.row, pipePosition.column + 1));
                        loopJourney.positionsToRightOfLoop.Push(new GridPos(pipePosition.row + 1, pipePosition.column));
                        loopJourney.positionsToLeftOfLoop.Push(new GridPos(pipePosition.row - 1, pipePosition.column - 1));
                        --pipePosition.row;
                        nextPipeEntranceDirection = Direction.South;
                        ++loopJourney.leftTurnsTaken;
                    }
                    break;
                case SouthEastPipe:
                    if (directionEnteredFrom == Direction.South)
                    {
                        loopJourney.positionsToLeftOfLoop.Push(new GridPos(pipePosition.row, pipePosition.column - 1));
                        loopJourney.positionsToLeftOfLoop.Push(new GridPos(pipePosition.row - 1, pipePosition.column));
                        loopJourney.positionsToRightOfLoop.Push(new GridPos(pipePosition.row + 1, pipePosition.column + 1));
                        ++pipePosition.column;
                        nextPipeEntranceDirection = Direction.West;
                        ++loopJourney.rightTurnsTaken;
                    }
                    else
                    {
                        loopJourney.positionsToRightOfLoop.Push(new GridPos(pipePosition.row, pipePosition.column - 1));
                        loopJourney.positionsToRightOfLoop.Push(new GridPos(pipePosition.row - 1, pipePosition.column));
                        loopJourney.positionsToLeftOfLoop.Push(new GridPos(pipePosition.row + 1, pipePosition.column + 1));
                        ++pipePosition.row;
                        nextPipeEntranceDirection = Direction.North;
                        ++loopJourney.leftTurnsTaken;
                    }
                    break;
                case SouthWestPipe:
                    if (directionEnteredFrom == Direction.South)
                    {
                        --pipePosition.column;
                        nextPipeEntranceDirection = Direction.East;
                        ++loopJourney.leftTurnsTaken;
                    }
                    else
                    {
                        ++pipePosition.row;
                        nextPipeEntranceDirection = Direction.North;
                        ++loopJourney.rightTurnsTaken;
                    }
                    break;
                default:
                    Debug.Assert(false, "Invalid pipe type at position");
                    nextPipeEntranceDirection = Direction.North;
                    break;
            }

            loopJourney.lastDirectionMovedFrom = nextPipeEntranceDirection;
            loopJourney.currentPosition = pipePosition;
            ++loopJourney.stepsTaken;
        }

        // Positions explored are represented as a grid of booleans, for whether or not that location
        // has been explored. To begin with all pipe locations should be marked explored, so that we
        // don't go past them on our explorations.
        public static int PositionsWithinLoop(Stack<GridPos> positionsToExploreFrom, List<List<bool>> positionsExplored)
        {
            int positionsWithinLoop = 0;

            while (positionsToExploreFrom.Count > 0) 
            {
                GridPos position = positionsToExploreFrom.Pop();
                if (position.row < 0 ||
                    position.column < 0 ||
                    position.row >= positionsExplored.Count ||
                    position.column >= positionsExplored[0].Count)
                {
                    continue;
                }

                if (!positionsExplored[position.row][position.column]) 
                { 
                    positionsWithinLoop++;
                    positionsExplored[position.row][position.column] = true;

                    positionsToExploreFrom.Push(new GridPos(position.row + 1, position.column));
                    positionsToExploreFrom.Push(new GridPos(position.row - 1, position.column));
                    positionsToExploreFrom.Push(new GridPos(position.row, position.column + 1));
                    positionsToExploreFrom.Push(new GridPos(position.row, position.column - 1));
                }
            }

            return positionsWithinLoop;
        }
    }
}