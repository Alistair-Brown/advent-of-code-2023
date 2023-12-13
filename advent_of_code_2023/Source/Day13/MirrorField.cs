using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.DayThirteen
{
    internal class MirrorField
    {
        List<string> rows;
        List<string> columns;
        public MirrorField(List<string> rawRows)
        {
            rows = new List<string>();
            columns = new List<string>();

            // Start by setting up each of the columns to contain just the zeroth element in
            // each row. We'll build up the rest of the columns as we work down the row
            rows.Add(rawRows[0]);
            foreach (char value in rawRows[0])
            {
                columns.Add(value.ToString());
            }

            for (int ii = 1; ii < rawRows.Count; ++ii)
            {
                rows.Add(rawRows[ii]);
                for (int jj = 0; jj < columns.Count; ++jj)
                {
                    columns[jj] += rawRows[ii][jj];
                }
            }
        }

        public ulong ColumnsToLeftOfReflection(int smudgeCorrectionsAllowed)
        {
            return ElementsBeforeReflection(columns, smudgeCorrectionsAllowed);
        }

        public ulong RowsAboveReflection(int smudgeCorrectionsAllowed)
        {
            return ElementsBeforeReflection(rows, smudgeCorrectionsAllowed);
        }

        private ulong ElementsBeforeReflection(List<string> lines, int smudgeCorrectionsAllowed)
        {
            ulong elementsBeforeReflection = 0;
            for (int ii = 1; ii < lines.Count; ++ii) 
            {
                bool foundReflection = true;
                int smudgeCorrections = 0;
                for (int jj = 0; jj < ii && (jj + ii) < lines.Count; ++jj)
                {
                    string lineOne = lines[ii - jj - 1];
                    string lineTwo = lines[ii + jj];
                    
                    for (int characterIndex = 0;  characterIndex < lineOne.Length; ++characterIndex)
                    {
                        if (lineOne[characterIndex] != lineTwo[characterIndex])
                        {
                            if (smudgeCorrections < smudgeCorrectionsAllowed)
                            {
                                ++smudgeCorrections;
                            }
                            else
                            {
                                foundReflection = false;
                            }
                        }
                    }
                }

                if (foundReflection && (smudgeCorrections == smudgeCorrectionsAllowed))
                {
                    elementsBeforeReflection = (ulong)ii;
                    break;
                }
            }

            return elementsBeforeReflection;
        }
    }
}
