using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AOC.DayTen;

namespace AOC
{
    internal class DayTenSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] puzzleInputLines)
        {
            GridPos startPosition = PipeUtils.GetStartPosition(puzzleInputLines);
            Direction initialDirectionToMoveIn = PipeUtils.FindValidDirectionFromStart(startPosition, puzzleInputLines);

            List<List<bool>> positionsExplored = new List<List<bool>>();
            for (int rows = 0; rows < puzzleInputLines.Length; ++rows)
            {
                positionsExplored.Add(new List<bool>());
                for (int columns = 0; columns < puzzleInputLines[0].Length; ++columns)
                {
                    positionsExplored[positionsExplored.Count - 1].Add(false);
                }
            }

            GridPos firstPositionMovedTo = startPosition;
            if (initialDirectionToMoveIn == Direction.North) { --firstPositionMovedTo.row; }
            else if (initialDirectionToMoveIn == Direction.South) { ++firstPositionMovedTo.row; }
            else if (initialDirectionToMoveIn == Direction.East) { ++firstPositionMovedTo.column; }
            else { --firstPositionMovedTo.column; }

            // Need to reverse the direction moved IN to get the direction moved FROM.
            Direction initialDirectionMovedFrom =
                initialDirectionToMoveIn == Direction.North ? Direction.South :
                initialDirectionToMoveIn == Direction.South ? Direction.North :
                initialDirectionToMoveIn == Direction.East ? Direction.West :
                Direction.East;

            JourneyAroundLoop loopJourney = new JourneyAroundLoop(firstPositionMovedTo, initialDirectionMovedFrom);
            ++loopJourney.stepsTaken;

            positionsExplored[startPosition.row][startPosition.column] = true;

            while (puzzleInputLines[loopJourney.currentPosition.row][loopJourney.currentPosition.column] != PipeUtils.StartPipe)
            {
                PipeUtils.TakeNextStepAroundLoop(ref loopJourney, puzzleInputLines, positionsExplored);
            }

            int totalLoopLength = loopJourney.stepsTaken;

            Debug.Assert(totalLoopLength % 2 == 0);
            int partOneFurthestStepsFromStart = totalLoopLength / 2;

            // Depending on whether the loop is clockwise, we need to look to the left or
            // right of the straight edges to see the inside.
            Debug.Assert(Math.Abs(loopJourney.rightTurnsTaken - loopJourney.leftTurnsTaken) == 3 ||
                Math.Abs(loopJourney.rightTurnsTaken - loopJourney.leftTurnsTaken) == 4);
            bool loopIsClockwise = loopJourney.rightTurnsTaken > loopJourney.leftTurnsTaken;

            Stack<GridPos> positionsToExplore = loopIsClockwise ?
                loopJourney.positionsToRightOfLoop : loopJourney.positionsToLeftOfLoop;

            int partTwo = PipeUtils.PositionsWithinLoop(positionsToExplore, positionsExplored);

            return new PuzzleSolution(partOneFurthestStepsFromStart.ToString(), partTwo.ToString());
        }
    }
}
