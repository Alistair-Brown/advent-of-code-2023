﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AOC.DayEighteen
{
    internal class LagoonDigger
    {
        enum Direction
        {
            UP,
            DOWN,
            LEFT,
            RIGHT
        }

        struct DigInstruction
        {
            public Direction direction;
            public ulong distance;
        }

        struct TrenchVertex
        {
            public ulong row;
            public ulong column;
        }

        List<DigInstruction> digInstructions;

        // For quicker iteration on the sizing algorithm, we also hold trench vertices according to their rows.
        // The inner list is then sorted by column position, so the leftmost vertices appear first.
        SortedList<ulong, SortedList<ulong, TrenchVertex>> trenchVerticesByRow;

        public LagoonDigger(string[] rawDigInstructions, bool partTwo)
        {
            digInstructions = new List<DigInstruction>();

            foreach (string rawDigInstruction in rawDigInstructions)
            {
                string[] splitOnSpace = rawDigInstruction.Split(' ');

                if (!partTwo)
                {
                    digInstructions.Add(new DigInstruction()
                    {
                        direction = DirectionFromChar(splitOnSpace[0]),
                        distance = Convert.ToUInt64(splitOnSpace[1]),
                    });
                }
                else
                {
                    string hexNumber = splitOnSpace[2].Substring(2, 5);
                    char directionAsString = splitOnSpace[2][splitOnSpace[2].Length - 2];

                    digInstructions.Add(new DigInstruction()
                    {
                        direction = directionAsString == '0' ? Direction.RIGHT :
                        directionAsString == '1' ? Direction.DOWN :
                        directionAsString == '2' ? Direction.LEFT : Direction.UP,
                        distance = Convert.ToUInt64(hexNumber.ToLower(), 16)
                    });
                }
            }

            InitialiseTrenchNodes();
        }

        private void InitialiseTrenchNodes()
        {
            trenchVerticesByRow = new SortedList<ulong, SortedList<ulong, TrenchVertex>>();

            ulong totalUp = 0;
            ulong totalLeft = 0;

            foreach (DigInstruction instruction in digInstructions)
            {
                switch (instruction.direction)
                {
                    case Direction.UP:
                        totalUp += instruction.distance;
                        break;
                    case Direction.LEFT:
                        totalLeft += instruction.distance;
                        break;
                    default:
                        break;
                }
            }

            // Place the starting position far enough into the grid that it will never reach above
            // the top or out to the left.
            (ulong row, ulong column) currentPosition = (totalUp + 1, totalLeft + 1);

            foreach (DigInstruction instruction in digInstructions)
            {
                TrenchVertex vertex = new TrenchVertex()
                {
                    row = currentPosition.row,
                    column = currentPosition.column
                };

                InsertVertex(vertex);

                switch (instruction.direction)
                {
                    case Direction.UP:
                        currentPosition.row -= instruction.distance;
                        break;
                    case Direction.DOWN:
                        currentPosition.row += instruction.distance;
                        break;
                    case Direction.LEFT:
                        currentPosition.column -= instruction.distance;
                        break;
                    default:
                        Debug.Assert(instruction.direction == Direction.RIGHT);
                        currentPosition.column += instruction.distance;
                        break;
                }
            }
        }

        public ulong TrenchSize()
        {
            ulong size = 0;
            ulong previousRow = 0;
            
            // Ulong is column position, bool is whether it starts or ends a trench
            SortedList<ulong,ulong> columnPositions = new SortedList<ulong,ulong>();

            foreach (ulong row in trenchVerticesByRow.Keys)
            {
                SortedList<ulong, TrenchVertex> newColumns = trenchVerticesByRow[row];

                size += (row - previousRow - 1) * GetWidth(columnPositions);
                size += GetComplicatedWidth(columnPositions, newColumns);

                InsertColumns(columnPositions, newColumns);
                previousRow = row;
            }

            return size;
        }

        private ulong GetWidth(SortedList<ulong, ulong> columnPositions)
        {
            ulong width = 0;

            for (int ii = 0; ii < columnPositions.Count; ii += 2) 
            {
                width += columnPositions.Values[ii + 1] - columnPositions.Values[ii] + 1;
            }

            return width;
        }

        private ulong GetComplicatedWidth(SortedList<ulong, ulong> columnPositions, SortedList<ulong, TrenchVertex> newColumns)
        {
            // Key is the start, value is pair
            SortedList<ulong, (ulong, ulong)> existingColumnsPairs = new SortedList<ulong, (ulong, ulong)>();
            SortedList<ulong, (ulong, ulong)> newColumnsPairs = new SortedList<ulong, (ulong, ulong)>();

            for (int ii = 0; ii < columnPositions.Count; ii += 2)
            {
                existingColumnsPairs.Add(columnPositions.Values[ii], (columnPositions.Values[ii], columnPositions.Values[ii + 1]));
            }

            for (int ii = 0; ii < newColumns.Count; ii += 2)
            {
                newColumnsPairs.Add(newColumns.Keys[ii], (newColumns.Keys[ii], newColumns.Keys[ii + 1]));
            }

            ulong width = 0;
            
            while (existingColumnsPairs.Count > 0 || newColumnsPairs.Count > 0)
            {
                ulong currentStart = 0;
                ulong currentEnd = 0;

                if (newColumnsPairs.Count == 0 || (existingColumnsPairs.Count > 0 && existingColumnsPairs.Keys[0] < newColumnsPairs.Keys[0]))
                {
                    currentStart = existingColumnsPairs.Keys[0];
                    currentEnd = existingColumnsPairs.Values[0].Item2;
                    existingColumnsPairs.RemoveAt(0);
                }
                else
                {
                    currentStart = newColumnsPairs.Keys[0];
                    currentEnd = newColumnsPairs.Values[0].Item2;
                    newColumnsPairs.RemoveAt(0);
                }

                while (true)
                {
                    if (existingColumnsPairs.Count > 0 && existingColumnsPairs.Values[0].Item1 <= currentEnd)
                    {
                        if (existingColumnsPairs.Values[0].Item2 > currentEnd)
                        {
                            currentEnd = existingColumnsPairs.Values[0].Item2;
                        }
                        existingColumnsPairs.RemoveAt(0);
                    }
                    else if (newColumnsPairs.Count > 0 && newColumnsPairs.Values[0].Item1 <= currentEnd)
                    {
                        if (newColumnsPairs.Values[0].Item2 > currentEnd)
                        {
                            currentEnd = newColumnsPairs.Values[0].Item2;
                        }
                        newColumnsPairs.RemoveAt(0);
                    }
                    else
                    {
                        width += currentEnd - currentStart + 1;
                        break;
                    }
                }
            }

            return width;

        }

        // As we insert new columns, return the size of that overlap row
        private void InsertColumns(SortedList<ulong, ulong> columnPositions, SortedList<ulong, TrenchVertex> newColumns)
        {
            foreach (ulong column in newColumns.Keys)
            {
                if (columnPositions.ContainsKey(column))
                {
                    columnPositions.Remove(column);
                }
                else
                {
                    columnPositions.Add(column, column);
                }
            }
        }

        private void InsertVertex(TrenchVertex vertex)
        {
            if (!trenchVerticesByRow.ContainsKey(vertex.row))
            {
                trenchVerticesByRow[vertex.row] = new SortedList<ulong, TrenchVertex>();
            }
            trenchVerticesByRow[vertex.row].Add(vertex.column, vertex);
        }

        private Direction DirectionFromChar(string character)
        {
            Debug.Assert(character.Length == 1);
            switch (character[0])
            {
                case 'U':
                    return Direction.UP;
                case 'D':
                    return Direction.DOWN;
                case 'L':
                    return Direction.LEFT;
                default:
                    Debug.Assert(character[0] == 'R');
                    return Direction.RIGHT;
            }
        }
    }
}
