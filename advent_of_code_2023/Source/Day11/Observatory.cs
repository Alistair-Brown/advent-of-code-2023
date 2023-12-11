using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.DayEleven
{
    internal class Observatory
    {
        private List<string> universeMap;
        private List<(int row, int column)> galaxyLocations;
        private List<int> emptyRows;
        private List<int> emptyColumns;

        public Observatory(string[] inputLines)
        {
            emptyRows = EmptyRows(inputLines);
            emptyColumns = EmptyColumns(inputLines);

            universeMap = inputLines.ToList();

            FindGalaxyLocations();
        }

        private List<int> EmptyRows(string[] inputLines)
        {
            List<int> emptyRows = new List<int>();

            for (int row = 0; row < inputLines.Length; ++row)
            {
                bool isEmpty = true;

                for (int column = 0;  column < inputLines[row].Length; ++column)
                {
                    if (inputLines[row][column] == '#')
                    {
                        isEmpty = false;
                        break;
                    }
                }

                if (isEmpty)
                {
                    emptyRows.Add(row);
                }
            }

            return emptyRows;
        }

        private List<int> EmptyColumns(string[] inputLines)
        {
            List<int> emptyColumns = new List<int>();

            for (int column = 0; column < inputLines[0].Length; ++column)
            {
                bool isEmpty = true;

                for (int row = 0; row < inputLines.Length; ++row)
                {
                    if (inputLines[row][column] == '#')
                    {
                        isEmpty = false;
                        break;
                    }
                }

                if (isEmpty)
                {
                    emptyColumns.Add(column);
                }
            }

            return emptyColumns;
        }

        private void ExpandUniverse(List<int> emptyRows,
                                    List<int> emptyColumns)
        {
            string emptyRow = "";
            for (int ii = 0; ii < universeMap.Count + emptyColumns.Count; ++ii) 
            {
                emptyRow += ".";
            }

            int columnsInserted = 0;
            foreach (int emptyColumnIndex in emptyColumns)
            {
                int indexToInsertAt = emptyColumnIndex + columnsInserted;
                for (int ii = 0; ii <universeMap.Count; ++ii)
                {
                    universeMap[ii] = universeMap[ii].Insert(indexToInsertAt, ".");
                }
                ++columnsInserted;
            }

            int rowsInserted = 0;
            foreach (int emptyRowIndex in emptyRows)
            {
                int rowIndexToInsertAt = emptyRowIndex + rowsInserted;
                universeMap.Insert(rowIndexToInsertAt, emptyRow);
                ++rowsInserted;
            }
        }

        private void FindGalaxyLocations()
        {
            galaxyLocations = new List<(int row, int column)>();

            for (int row = 0; row < universeMap.Count; ++row)
            {
                for (int column = 0; column < universeMap[0].Length; ++column)
                {
                    if (universeMap[row][column] == '#')
                    {
                        galaxyLocations.Add((row, column));
                    }
                }
            }
        }

        public ulong SumOfPathsBetweenGalaxies(ulong galaxyExpansion)
        {
            ulong sumOfPaths = 0;

            for (int ii = 0; ii < galaxyLocations.Count - 1; ++ii)
            {
                for (int jj = ii + 1; jj < galaxyLocations.Count; ++jj)
                {
                    ulong unexpandedPathLength =
                        (ulong)Math.Abs(galaxyLocations[ii].row - galaxyLocations[jj].row) +
                        (ulong)Math.Abs(galaxyLocations[ii].column - galaxyLocations[jj].column);
                    ulong expandedPathLength = unexpandedPathLength +
                        galaxyExpansion * (ulong)EmptyRowsAndColumnsBetweenGalaxyLocations(
                        (galaxyLocations[ii].row, galaxyLocations[ii].column),
                        (galaxyLocations[jj].row, galaxyLocations[jj].column));

                    // Console.WriteLine($"Path from {ii + 1} to {jj + 1} is {expandedPathLength}");

                    sumOfPaths += expandedPathLength;
                }
            }

            return sumOfPaths;
        }

        private int EmptyRowsAndColumnsBetweenGalaxyLocations((int row, int column)galaxyOne, (int row, int column) galaxyTwo)
        {
            int numEmpties = 0;

            foreach(int row in emptyRows)
            {
                if ((row > galaxyOne.row && row < galaxyTwo.row) ||
                    (row > galaxyTwo.row && row < galaxyOne.row))
                {
                    ++numEmpties;
                }
            }

            foreach (int column in emptyColumns)
            {
                if ((column > galaxyOne.column && column < galaxyTwo.column) ||
                    (column > galaxyTwo.column && column < galaxyOne.column))
                {
                    ++numEmpties;
                }
            }

            return numEmpties;
        }

        public void PrintUniverse()
        {
            foreach (string row in universeMap)
            {
                Console.WriteLine(row);
            }
        }
    }
}
