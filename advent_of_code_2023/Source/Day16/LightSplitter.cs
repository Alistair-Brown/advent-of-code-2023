using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.DaySixteen
{
    enum Direction
    {
        North = 0,
        South = 1,
        East = 2,
        West = 3
    }

    struct LightSplitterTile
    {
        public char tileCharacter;
        public bool hasBeenIlluminated;
        public bool[] directionsLightHasEnteredFrom;
    }

    struct LightBeamInput
    {
        public int row;
        public int column;
        public Direction directionEnteredFrom;

        public LightBeamInput(int row_in, int column_in, Direction direction_in)
        {
            row = row_in;
            column = column_in;
            directionEnteredFrom = direction_in;
        }
    }

    internal class LightSplitter
    {
        public LightSplitterTile[][] lightSplitterGrid;

        public LightSplitter(string[] inputLines)
        {
            lightSplitterGrid = new LightSplitterTile[inputLines.Length][];
            for (int row = 0; row < inputLines.Length; ++row)
            {
                lightSplitterGrid[row] = new LightSplitterTile[inputLines[0].Length];

                for (int column = 0; column < inputLines[0].Length; ++column)
                {
                    lightSplitterGrid[row][column] = new LightSplitterTile()
                    {
                        tileCharacter = inputLines[row][column],
                        hasBeenIlluminated = false,
                        directionsLightHasEnteredFrom = new bool[4]
                    };

                    for (int ii = 0; ii < 4; ii++)
                    {
                        lightSplitterGrid[row][column].directionsLightHasEnteredFrom[ii] = false;
                    }
                }
            }
        }

        public void ResetTiles()
        {
            for (int row = 0; row < lightSplitterGrid.Length; ++row)
            {
                for (int column = 0; column < lightSplitterGrid[0].Length; ++column)
                {
                    lightSplitterGrid[row][column].hasBeenIlluminated = false;
                    for (int ii = 0; ii < 4; ++ii)
                    {
                        lightSplitterGrid[row][column].directionsLightHasEnteredFrom[ii] = false;
                    }
                }
            }
        }

        public int TilesIlluminatedByLightBeam(LightBeamInput lightBeamInput)
        {
            int numTilesIlluminated = 0;

            Stack<LightBeamInput> beamsToTrack = new Stack<LightBeamInput>();
            beamsToTrack.Push(lightBeamInput);

            while (beamsToTrack.Count > 0)
            {
                TrackNextLightBeamFromStack(ref numTilesIlluminated, beamsToTrack);
            }

            return numTilesIlluminated;
        }

        public void TrackNextLightBeamFromStack(ref int numTilesIlluminated, Stack<LightBeamInput> lightBeams)
        {
            LightBeamInput lightBeam = lightBeams.Pop();
            int row = lightBeam.row;
            int column = lightBeam.column;
            Direction directionEnteredFrom = lightBeam.directionEnteredFrom;

            if (row < 0 || row >= lightSplitterGrid.Length || 
                column < 0 || column >= lightSplitterGrid[0].Length)
            {
                return;
            }

            LightSplitterTile tile = lightSplitterGrid[row][column];

            // Break out of a possible endless loop
            if (tile.directionsLightHasEnteredFrom[(int)directionEnteredFrom] == true)
            {
                return;
            }

            // Remember structs are passed by value, so need to re-index into the grid to
            // change to edit the tile.
            lightSplitterGrid[row][column].directionsLightHasEnteredFrom[(int)directionEnteredFrom] = true;
            if (!tile.hasBeenIlluminated)
            {
                lightSplitterGrid[row][column].hasBeenIlluminated = true;
                ++numTilesIlluminated;
            }

            switch (tile.tileCharacter)
            {
                case '.':
                    switch (directionEnteredFrom)
                    {
                        case Direction.North:
                            lightBeams.Push( new LightBeamInput(row + 1, column, Direction.North));
                            break;
                        case Direction.South:
                            lightBeams.Push( new LightBeamInput(row - 1, column, Direction.South));
                            break;
                        case Direction.East:
                            lightBeams.Push(new LightBeamInput(row, column - 1, Direction.East));
                            break;
                        case Direction.West:
                            lightBeams.Push(new LightBeamInput(row, column + 1, Direction.West));
                            break;
                        default:
                            Debug.Assert(false, "Unrecognised direction");
                            break;
                    }
                    break;
                case '|':
                    switch (directionEnteredFrom)
                    {
                        case Direction.North:
                            lightBeams.Push(new LightBeamInput(row + 1, column, Direction.North));
                            break;
                        case Direction.South:
                            lightBeams.Push(new LightBeamInput(row - 1, column, Direction.South));
                            break;
                        case Direction.East:
                        case Direction.West:
                            lightBeams.Push(new LightBeamInput(row + 1, column, Direction.North));
                            lightBeams.Push(new LightBeamInput(row - 1, column, Direction.South));
                            break;
                        default:
                            Debug.Assert(false, "Unrecognised direction");
                            break;
                    }
                    break;
                case '-':
                    switch (directionEnteredFrom)
                    {
                        case Direction.North:
                        case Direction.South:
                            lightBeams.Push(new LightBeamInput(row, column - 1, Direction.East));
                            lightBeams.Push(new LightBeamInput(row, column + 1, Direction.West));
                            break;
                        case Direction.East:
                            lightBeams.Push(new LightBeamInput(row, column - 1, Direction.East));
                            break;
                        case Direction.West:
                            lightBeams.Push(new LightBeamInput(row, column + 1, Direction.West));
                            break;
                        default:
                            Debug.Assert(false, "Unrecognised direction");
                            break;
                    }
                    break;
                case '/':
                    switch (directionEnteredFrom)
                    {
                        case Direction.North:
                            lightBeams.Push(new LightBeamInput(row, column - 1, Direction.East));
                            break;
                        case Direction.South:
                            lightBeams.Push(new LightBeamInput(row, column + 1, Direction.West));
                            break;
                        case Direction.East:
                            lightBeams.Push(new LightBeamInput(row + 1, column, Direction.North));
                            break;
                        case Direction.West:
                            lightBeams.Push(new LightBeamInput(row - 1, column, Direction.South));
                            break;
                        default:
                            Debug.Assert(false, "Unrecognised direction");
                            break;
                    }
                    break;
                case '\\':
                    switch (directionEnteredFrom)
                    {
                        case Direction.North:
                            lightBeams.Push(new LightBeamInput(row, column + 1, Direction.West));
                            break;
                        case Direction.South:
                            lightBeams.Push(new LightBeamInput(row, column - 1, Direction.East));
                            break;
                        case Direction.East:
                            lightBeams.Push(new LightBeamInput(row - 1, column, Direction.South));
                            break;
                        case Direction.West:
                            lightBeams.Push(new LightBeamInput(row + 1, column, Direction.North));
                            break;
                        default:
                            Debug.Assert(false, "Unrecognised direction");
                            break;
                    }
                    break;
                default:
                    Debug.Assert(false, "Unrecognised tile character");
                    break;
            }
        }

    }
}
