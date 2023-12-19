using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AOC.DayNineteen;

namespace AOC
{
    internal class DayNineteenSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] puzzleInputLines)
        {
            Dictionary<string, PartWorklow> workflows = new Dictionary<string, PartWorklow>();
            List<Part> parts = new List<Part>();

            bool gettingWorkflows = true;
            foreach(string line in puzzleInputLines)
            {
                if (line.Length == 0)
                {
                    gettingWorkflows = false;
                    continue;
                }

                if (gettingWorkflows)
                {
                    string[] nameAndRules = line.Split('{');
                    workflows.Add(nameAndRules[0], new PartWorklow(nameAndRules[1].Substring(0, nameAndRules[1].Length - 1)));
                }
                else
                {
                    parts.Add(new Part(line.Substring(1, line.Length - 2)));
                }
            }

            int partOneAcceptedTotal = 0;
            foreach (Part part in  parts)
            {
                string nextWorkflow = "in";
                while (true)
                {
                    WorkflowResult result = workflows[nextWorkflow].GetResultForPart(part, ref nextWorkflow);

                    if (result == WorkflowResult.Accept)
                    {
                        partOneAcceptedTotal += part.Total;
                        break;
                    }
                    else if (result == WorkflowResult.Reject)
                    {
                        break;
                    }
                }
            }

            ulong partTwoPossibilities = 0;
            Stack<(PotentialPart, WorkflowResult, string)> potentialParts = new Stack<(PotentialPart, WorkflowResult, string)>();
            PotentialPart partToStart = new PotentialPart()
            {
                minX = 1,
                maxX = 4000,
                minM = 1,
                maxM = 4000,
                minA = 1,
                maxA = 4000,
                minS = 1,
                maxS = 4000
            };
            potentialParts.Push((partToStart, WorkflowResult.NextStep, "in"));

            while (potentialParts.Count > 0)
            {
                (PotentialPart part, WorkflowResult result, string nextStep) nextPart = potentialParts.Pop();

                if (nextPart.result == WorkflowResult.Reject)
                {
                    continue;
                }
                else if (nextPart.result == WorkflowResult.Accept)
                {
                    partTwoPossibilities += (ulong)
                        ((ulong)(nextPart.part.maxX - nextPart.part.minX + 1) *
                         (ulong)(nextPart.part.maxM - nextPart.part.minM + 1) *
                         (ulong)(nextPart.part.maxA - nextPart.part.minA + 1) *
                         (ulong)(nextPart.part.maxS - nextPart.part.minS + 1));
                }
                else
                {
                    foreach (var potPart in workflows[nextPart.nextStep].SplitPartIntoPotentials(nextPart.part)) 
                    {
                        potentialParts.Push(potPart);
                    }
                }
            }

            return new PuzzleSolution(partOneAcceptedTotal.ToString(), partTwoPossibilities.ToString());

        }
    }
}
