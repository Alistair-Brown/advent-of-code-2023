using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AOC.DayThirteen;

namespace AOC
{
    internal class DayThirteenSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] puzzleInputLines)
        {
            List<string> singleField = new List<string>();
            List<MirrorField> mirrorFields = new List<MirrorField>();

            foreach(string line in puzzleInputLines)
            {
                if (line.Length == 0)
                {
                    mirrorFields.Add(new MirrorField(singleField));
                    singleField.Clear();
                }
                else
                {
                    singleField.Add(line);
                }
            }
            mirrorFields.Add(new MirrorField(singleField));

            ulong partOne = 0;
            foreach (MirrorField field in mirrorFields)
            {
                ulong reflectedRows = field.RowsAboveReflection(0);
                ulong reflectedColumns = field.ColumnsToLeftOfReflection(0);

                partOne += reflectedColumns;
                partOne += 100 * reflectedRows;
            }

            ulong partTwo = 0;
            foreach (MirrorField field in mirrorFields)
            {
                ulong reflectedRows = field.RowsAboveReflection(1);
                ulong reflectedColumns = field.ColumnsToLeftOfReflection(1);

                partTwo += reflectedColumns;
                partTwo += 100 * reflectedRows;
            }

            return new PuzzleSolution(partOne.ToString(), partTwo.ToString());
        }
    }
}
