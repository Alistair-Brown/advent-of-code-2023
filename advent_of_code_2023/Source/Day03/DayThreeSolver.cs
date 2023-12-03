using AOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AOC.DayThree;

namespace AOC
{
    internal class DayThreeSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] puzzleInputLines)
        {
            List<EnginePart> engineParts = GetEngineParts(puzzleInputLines);

            int partOneEngineSum = 0;
            ulong partTwoGearRatioSum = 0;
            Dictionary<(int row, int column), List<int>> gearsSurroundedByParts = new Dictionary<(int row, int column), List<int>>();

            foreach (EnginePart enginePart in engineParts)
            {
                List<(int row, int column)> locationsToCheckForSymbols = GetLocationsAdjacentToEnginePart(enginePart);
                bool foundASymbol = false;

                foreach ((int row, int column) locationToCheck in locationsToCheckForSymbols)
                {
                    if (!LocationIsValid(locationToCheck.row, locationToCheck.column, puzzleInputLines))
                    {
                        continue;
                    }

                    char charAtLocation = puzzleInputLines[locationToCheck.row][locationToCheck.column];

                    // An engine part adjacent to a symbol (anything other than a number or '.' is a valid engine part, and
                    // counts towards the part one answer. Mark that we've found a symbol to avoid double counting, since we
                    // need to continue iterating over the adjacent spaces looking for 'gears'.
                    if (!foundASymbol && charAtLocation != '.' && !(charAtLocation >= '0' && charAtLocation <= '9'))
                    {
                        partOneEngineSum += enginePart.value;
                        foundASymbol = true;
                    }

                    // If we find a gear (i.e. '*'), note this engine part's value as being a value adjacent to that gear.
                    if (charAtLocation == '*')
                    {
                        if (!gearsSurroundedByParts.ContainsKey((locationToCheck.row, locationToCheck.column)))
                        {
                            gearsSurroundedByParts.Add((locationToCheck.row, locationToCheck.column), new List<int>());
                        }

                        gearsSurroundedByParts[(locationToCheck.row, locationToCheck.column)].Add(enginePart.value);
                    }
                }
            }

            // Part two needs us to get the ratio sum of each gear (the product of all surrounding engine part values, where a
            // gear only counts towards this if it has at least 2 adjacent numbers), and sum them.
            foreach (List<int> partsSurroundingGear in  gearsSurroundedByParts.Values)
            {
                if (partsSurroundingGear.Count == 2)
                {
                    partTwoGearRatioSum += (ulong)partsSurroundingGear[0] * (ulong)partsSurroundingGear[1];
                }
            }

            return new PuzzleSolution(partOneEngineSum.ToString(), partTwoGearRatioSum.ToString());
        }

        private List<EnginePart> GetEngineParts(string[] puzzleInputLines)
        {
            List<EnginePart> engineParts = new List<EnginePart>();

            for (int row = 0; row < puzzleInputLines.Length; ++row)
            {
                string partNumberInProgress = "";
                int partNumberStartColumnIndex = 0;

                for (int column = 0; column < puzzleInputLines[row].Length; ++column)
                {
                    if (puzzleInputLines[row][column] >= '0' && puzzleInputLines[row][column] <= '9')
                    {
                        partNumberInProgress += puzzleInputLines[row][column];
                        if (partNumberInProgress.Length == 1)
                        {
                            partNumberStartColumnIndex = column;
                        }

                        // Catch the case where we've reached the end of the row while building up an engine part.
                        if (column == puzzleInputLines[row].Length - 1)
                        {
                            engineParts.Add(new EnginePart()
                            {
                                value = Convert.ToInt32(partNumberInProgress),
                                rowIndex = row,
                                startColumnIndex = partNumberStartColumnIndex,
                                endColumnIndex = column - 1
                            });
                            partNumberInProgress = "";
                        }
                    }
                    // If the current character isn't part of an engine part, but we were part way through reading one, then
                    // we've reached the end of that engine part and have all of the info needed to construct it.
                    else if (partNumberInProgress.Length > 0)
                    {
                        engineParts.Add(new EnginePart()
                        {
                            value = Convert.ToInt32(partNumberInProgress),
                            rowIndex = row,
                            startColumnIndex = partNumberStartColumnIndex,
                            endColumnIndex = column - 1
                        });
                        partNumberInProgress = "";
                    }
                }
            }

            return engineParts;
        }

        // Will also return 'adjacent' locations that don't really exist on the grid, identifiable by having values of -1 or
        // larger than the row or column size. The caller needs to filter these out.
        private List<(int,int)> GetLocationsAdjacentToEnginePart(EnginePart enginePart)
        {
            List<(int row, int column)> adjacentLocations = new List<(int, int)>();

            foreach (int column in Enumerable.Range(enginePart.startColumnIndex - 1, enginePart.endColumnIndex - enginePart.startColumnIndex + 3))
            {
                adjacentLocations.Add((enginePart.rowIndex - 1, column));
                adjacentLocations.Add((enginePart.rowIndex + 1, column));
            }
            adjacentLocations.Add((enginePart.rowIndex, enginePart.startColumnIndex - 1));
            adjacentLocations.Add((enginePart.rowIndex, enginePart.endColumnIndex + 1));

            return adjacentLocations;
        }

        private bool LocationIsValid(int row, int column, string[] puzzleInputLines)
        {
            if (row < 0 ||
                row > puzzleInputLines.Length - 1 ||
                column < 0 ||
                column > puzzleInputLines[0].Length - 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
