using System;
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

        // Any vertex of the complete trench polygon has a position, and a trench going off it in two of the
        // four cardinal directions.
        struct TrenchVertex
        {
            public ulong row;
            public ulong column;

            public bool up;
            public bool down;
            public bool left;
            public bool right;
        }

        List<DigInstruction> digInstructions;
        SortedList<(ulong row, ulong column), TrenchVertex> trenchVertices;

        // For quicker iteration on the sizing algorithm, we also hold trench vertices according to their columns and rows.
        // In each case, the list is sorted by the other direction, i.e. in the dict of columns, the list is sorted by rows.
        Dictionary<ulong, SortedList<ulong, TrenchVertex>> trenchVerticesByColumn;
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
            trenchVertices = new SortedList<(ulong row, ulong column), TrenchVertex>();
            trenchVerticesByColumn = new Dictionary<ulong, SortedList<ulong, TrenchVertex>>();
            trenchVerticesByRow = new SortedList<ulong, SortedList<ulong, TrenchVertex>>();

            ulong totalUp = 0;
            ulong totalDown = 0;
            ulong totalLeft = 0;
            ulong totalRight = 0;

            foreach (DigInstruction instruction in digInstructions)
            {
                switch (instruction.direction)
                {
                    case Direction.UP:
                        totalUp += instruction.distance;
                        break;
                    case Direction.DOWN:
                        totalDown += instruction.distance;
                        break;
                    case Direction.LEFT:
                        totalLeft += instruction.distance;
                        break;
                    default:
                        Debug.Assert(instruction.direction == Direction.RIGHT);
                        totalRight += instruction.distance;
                        break;
                }
            }

            // Place the starting position far enough into the grid that it will never reach above
            // the top or out to the left.
            // The 'inbound direction' of a given node is the reverse of the direction that was moved
            // to reach it, since we give node directions relative to that node itself.
            (ulong row, ulong column) currentPosition = (totalUp + 1, totalLeft + 1);
            Direction inboundDirection = ReverseDirection(digInstructions[digInstructions.Count - 1].direction);

            foreach (DigInstruction instruction in digInstructions)
            {
                TrenchVertex vertex = new TrenchVertex()
                {
                    row = currentPosition.row,
                    column = currentPosition.column
                };
                vertex.up = inboundDirection == Direction.UP || instruction.direction == Direction.UP;
                vertex.down = inboundDirection == Direction.DOWN || instruction.direction == Direction.DOWN;
                vertex.left = inboundDirection == Direction.LEFT || instruction.direction == Direction.LEFT;
                vertex.right = inboundDirection == Direction.RIGHT || instruction.direction == Direction.RIGHT;

                InsertVertex(vertex);

                inboundDirection = ReverseDirection(instruction.direction);

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

        public ulong TrenchSizeOld()
        {
            ulong size = 0;

            while (trenchVertices.Count > 0)
            {
                // Get the two vertices representing the top line of the topmost square making up the complete pattern
                TrenchVertex topLeftVertex = trenchVertices.Values[0];
                TrenchVertex topRightVertex = trenchVertices.Values[1];

                RemoveVertex(topLeftVertex);
                RemoveVertex(topRightVertex);

                // And the two vertices most closely directly below them. At least one of these represents a corner of the square we're
                // going to remove.
                TrenchVertex potentialBottomLeftVertex = trenchVerticesByColumn[topLeftVertex.column].Values[0];
                TrenchVertex potentialBottomRightVertex = trenchVerticesByColumn[topRightVertex.column].Values[0];

                if (potentialBottomLeftVertex.row < potentialBottomRightVertex.row)
                {
                    if (potentialBottomLeftVertex.left)
                    {
                        size += (topRightVertex.column - topLeftVertex.column + 1) * (potentialBottomLeftVertex.row - topLeftVertex.row);

                        RemoveVertex(potentialBottomLeftVertex);

                        TrenchVertex vertexCreated = new TrenchVertex()
                        {
                            row = potentialBottomLeftVertex.row,
                            column = topRightVertex.column,
                            up = false,
                            down = true,
                            left = true,
                            right = false
                        };

                        InsertVertex(vertexCreated);
                    }
                    else
                    {
                        size += (topRightVertex.column - topLeftVertex.column + 1) * (potentialBottomLeftVertex.row - topLeftVertex.row + 1);

                        RemoveVertex(potentialBottomLeftVertex);

                        TrenchVertex vertexCreated = new TrenchVertex()
                        {
                            row = potentialBottomLeftVertex.row + 1,
                            column = topRightVertex.column,
                            up = false,
                            down = true,
                            left = true,
                            right = false
                        };

                        InsertVertex(vertexCreated);

                        // Also need to shift the inner corner down a space
                        TrenchVertex vertexToMove = FindNextVertexToRight(potentialBottomLeftVertex);
                        RemoveVertex(vertexToMove);
                        vertexToMove.left = false;
                        vertexToMove.right = true;
                        ++vertexToMove.row;
                        InsertVertex(vertexToMove);
                    }
                }
                else if (potentialBottomLeftVertex.row > potentialBottomRightVertex.row)
                {
                    if (potentialBottomRightVertex.right)
                    {
                        size += (topRightVertex.column - topLeftVertex.column + 1) * (potentialBottomRightVertex.row - topRightVertex.row);

                        RemoveVertex(potentialBottomRightVertex);

                        TrenchVertex vertexCreated = new TrenchVertex()
                        {
                            row = potentialBottomRightVertex.row,
                            column = topRightVertex.column,
                            up = false,
                            down = true,
                            left = false,
                            right = true
                        };

                        InsertVertex(vertexCreated);
                    }
                    else
                    {
                        size += (topRightVertex.column - topLeftVertex.column + 1) * (potentialBottomRightVertex.row - topLeftVertex.row + 1);

                        RemoveVertex(potentialBottomRightVertex);

                        TrenchVertex vertexCreated = new TrenchVertex()
                        {
                            row = potentialBottomRightVertex.row + 1,
                            column = topRightVertex.column,
                            up = false,
                            down = true,
                            left = false,
                            right = true
                        };

                        InsertVertex(vertexCreated);

                        // Also need to shift the inner corner down a space
                        TrenchVertex vertexToMove = FindNextVertexToLeft(potentialBottomRightVertex);
                        RemoveVertex(vertexToMove);
                        vertexToMove.right = false;
                        vertexToMove.left = true;
                        ++vertexToMove.row;
                        InsertVertex(vertexToMove);
                    }
                }
                else
                {
                    if (potentialBottomRightVertex.left && potentialBottomLeftVertex.right)
                    {
                        size += (topRightVertex.column - topLeftVertex.column + 1) * (potentialBottomRightVertex.row - topLeftVertex.row + 1);

                        RemoveVertex(potentialBottomLeftVertex);
                        RemoveVertex(potentialBottomRightVertex);

                        if (!(FindNextVertexToRight(potentialBottomLeftVertex).column == potentialBottomRightVertex.column))
                        {
                            // Not a complete square, need to move the inner vertices in

                            // Actually this entire design is bugged, what if we have a portion that's shaped like 
                            // lower case n?
                        }
                    }
                    else
                    {
                        size += (topRightVertex.column - topLeftVertex.column + 1) * (potentialBottomRightVertex.row - topLeftVertex.row + 1);

                        RemoveVertex(potentialBottomLeftVertex);
                        RemoveVertex(potentialBottomRightVertex);
                    }
                }
            }

            return size;
        }

        private void InsertVertex(TrenchVertex vertex)
        {
            trenchVertices.Add((vertex.row, vertex.column), vertex);

            if (!trenchVerticesByColumn.ContainsKey(vertex.column))
            {
                trenchVerticesByColumn[vertex.column] = new SortedList<ulong, TrenchVertex>();
            }
            trenchVerticesByColumn[vertex.column].Add(vertex.row, vertex);

            if (!trenchVerticesByRow.ContainsKey(vertex.row))
            {
                trenchVerticesByRow[vertex.row] = new SortedList<ulong, TrenchVertex>();
            }
            trenchVerticesByRow[vertex.row].Add(vertex.column, vertex);
        }

        private void RemoveVertex(TrenchVertex vertex)
        {
            trenchVertices.Remove((vertex.row, vertex.column));
            trenchVerticesByColumn[vertex.column].Remove(vertex.row);
            trenchVerticesByRow[vertex.row].Remove(vertex.column);
        }

        private TrenchVertex FindNextVertexToRight(TrenchVertex vertexIn)
        {
            foreach (TrenchVertex vertex in trenchVerticesByRow[vertexIn.row].Values)
            {
                if (vertex.column > vertexIn.column)
                {
                    return vertex;
                }
            }

            Debug.Assert(false, "Should never fail to find a vertex");
            return vertexIn;
        }

        private TrenchVertex FindNextVertexToLeft(TrenchVertex vertexIn)
        {
            for (int ii = trenchVerticesByRow[vertexIn.row].Values.Count - 1; ii >= 0; --ii)
            {
                TrenchVertex vertex = trenchVerticesByRow[vertexIn.row].Values[ii];
                if (vertex.column < vertexIn.column)
                {
                    return vertex;
                }
            }

            Debug.Assert(false, "Should never fail to find a vertex");
            return vertexIn;
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

        private Direction ReverseDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.UP:
                    return Direction.DOWN;
                case Direction.DOWN:
                    return Direction.UP;
                case Direction.LEFT:
                    return Direction.RIGHT;
                default:
                    Debug.Assert(direction == Direction.RIGHT);
                    return Direction.LEFT;
            }
        }
    }
}
