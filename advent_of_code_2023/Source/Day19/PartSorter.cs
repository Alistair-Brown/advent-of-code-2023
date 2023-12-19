using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.DayNineteen
{
    struct Part
    {
        public int xVal;
        public int mVal;
        public int aVal;
        public int sVal;

        public int Total { get { return xVal + mVal + aVal + sVal; } }

        public Part(string inputLine)
        {
            string[] individualParts = inputLine.Split(',');
            xVal = Convert.ToInt32(individualParts[0].Split('=')[1]);
            mVal = Convert.ToInt32(individualParts[1].Split('=')[1]);
            aVal = Convert.ToInt32(individualParts[2].Split('=')[1]);
            sVal = Convert.ToInt32(individualParts[3].Split('=')[1]);

            Debug.Assert(individualParts[0].Split('=')[0] == "x");
            Debug.Assert(individualParts[1].Split('=')[0] == "m");
            Debug.Assert(individualParts[2].Split('=')[0] == "a");
            Debug.Assert(individualParts[3].Split('=')[0] == "s");
        }
    }

    struct PotentialPart
    {
        public int minX;
        public int maxX;
        public int minM;
        public int maxM;
        public int minA;
        public int maxA;
        public int minS;
        public int maxS;
    }

    enum PartProperty
    {
        x,
        m,
        a,
        s,
        none
    }

    enum WorkflowResult
    {
        Accept,
        Reject,
        NextStep
    }

    enum StepOperation
    {
        AutoAccept,
        MustBeGreater,
        MustBeLess
    }

    class WorkflowStep
    {
        public PartProperty partPropertyToCheck;
        public StepOperation operation;
        public int requiredValue;

        public WorkflowResult result;
        public string nextStep; // Only used if result is NextStep

        // Assumes we just have the single step in the workflow with no extraneous characters
        public WorkflowStep(string rawStep)
        {
            if (rawStep.Contains(":"))
            {
                string[] stepAndResult = rawStep.Split(':');

                SetResultFromString(stepAndResult[1]);

                if (stepAndResult[0][1] == '>')
                {
                    operation = StepOperation.MustBeGreater;
                    SetPartPropertyFromChar(stepAndResult[0][0]);
                    requiredValue = Convert.ToInt32(stepAndResult[0].Substring(2));
                }
                else
                {
                    Debug.Assert(stepAndResult[0][1] == '<');
                    operation = StepOperation.MustBeLess;
                    SetPartPropertyFromChar(stepAndResult[0][0]);
                    requiredValue = Convert.ToInt32(stepAndResult[0].Substring(2));
                }
            }
            else
            {
                operation = StepOperation.AutoAccept;
                SetResultFromString(rawStep);
            }
        }

        public bool MeetsRequirements(Part part)
        {
            if (operation == StepOperation.AutoAccept)
            {
                return true;
            }
            else if (operation == StepOperation.MustBeGreater) 
            {
                switch (partPropertyToCheck)
                {
                    case PartProperty.x:
                        return part.xVal > requiredValue;
                    case PartProperty.m:
                        return part.mVal > requiredValue;
                    case PartProperty.a:
                        return part.aVal > requiredValue;
                    case PartProperty.s:
                    default:
                        return part.sVal > requiredValue;
                }
            }
            else
            {
                switch (partPropertyToCheck)
                {
                    case PartProperty.x:
                        return part.xVal < requiredValue;
                    case PartProperty.m:
                        return part.mVal < requiredValue;
                    case PartProperty.a:
                        return part.aVal < requiredValue;
                    case PartProperty.s:
                    default:
                        return part.sVal < requiredValue;
                }
            }
        }

        public WorkflowResult GetResult(ref string nextStep)
        {
            if (this.result == WorkflowResult.NextStep)
            {
                nextStep = this.nextStep;
            }

            return this.result;
        }

        private void SetResultFromString(string result)
        {
            if (result == "A")
            {
                this.result = WorkflowResult.Accept;
            }
            else if (result == "R")
            {
                this.result = WorkflowResult.Reject;
            }
            else
            {
                this.result = WorkflowResult.NextStep;
                this.nextStep = result;
            }
        }

        private void SetPartPropertyFromChar(char rawProperty)
        {
            switch (rawProperty)
            {
                case 'a':
                    partPropertyToCheck = PartProperty.a;
                    break;
                case 'm':
                    partPropertyToCheck = PartProperty.m;
                    break;
                case 'x':
                    partPropertyToCheck = PartProperty.x;
                    break;
                case 's':
                    partPropertyToCheck = PartProperty.s;
                    break;
                default:
                    Debug.Assert(false, "Character doesn't match part property");
                    partPropertyToCheck = PartProperty.none;
                    break;
                    
            }
        }
    }

    internal class PartWorklow
    {
        List<WorkflowStep> workflowSteps;

        public PartWorklow(string inputLine)
        {
            string[] rawSteps = inputLine.Split(',');
            workflowSteps = new List<WorkflowStep>();

            foreach (string rawStep in rawSteps)
            {
                workflowSteps.Add(new WorkflowStep(rawStep));
            }
        }

        public WorkflowResult GetResultForPart(Part part, ref string nextStep)
        {
            foreach (WorkflowStep step in workflowSteps)
            {
                if (step.MeetsRequirements(part))
                {
                    return step.GetResult(ref nextStep);
                }
            }

            Debug.Assert(false, "The final workflow step should have auto accepted");
            return WorkflowResult.Reject;
        }

        public List<(PotentialPart, WorkflowResult, string nextStep)> SplitPartIntoPotentials(PotentialPart part)
        {
            List<(PotentialPart, WorkflowResult, string nextStep)> potentialParts = new List<(PotentialPart, WorkflowResult, string nextStep)>();

            foreach (WorkflowStep step in workflowSteps)
            {
                if (step.operation == StepOperation.AutoAccept)
                {
                    potentialParts.Add((part, step.result, step.nextStep));
                    break;
                }
                else if (step.operation == StepOperation.MustBeGreater)
                {
                    // Account for the possibilty of part already meeting or failing this entirely
                    switch (step.partPropertyToCheck)
                    {
                        case PartProperty.x:
                            if (part.minX > step.requiredValue)
                            {
                                potentialParts.Add((part, step.result, step.nextStep));
                                break;
                            }
                            else if (part.maxX <= step.requiredValue)
                            {
                                continue;
                            }
                            else
                            {
                                PotentialPart newPart = part;
                                newPart.minX = step.requiredValue + 1;
                                potentialParts.Add((newPart, step.result, step.nextStep));

                                part.maxX = step.requiredValue;
                            }
                            break;
                        case PartProperty.m:
                            if (part.minM > step.requiredValue)
                            {
                                potentialParts.Add((part, step.result, step.nextStep));
                                break;
                            }
                            else if (part.maxM <= step.requiredValue)
                            {
                                continue;
                            }
                            else
                            {
                                PotentialPart newPart = part;
                                newPart.minM = step.requiredValue + 1;
                                potentialParts.Add((newPart, step.result, step.nextStep));

                                part.maxM = step.requiredValue;
                            }
                            break;
                        case PartProperty.a:
                            if (part.minA > step.requiredValue)
                            {
                                potentialParts.Add((part, step.result, step.nextStep));
                                break;
                            }
                            else if (part.maxA <= step.requiredValue)
                            {
                                continue;
                            }
                            else
                            {
                                PotentialPart newPart = part;
                                newPart.minA = step.requiredValue + 1;
                                potentialParts.Add((newPart, step.result, step.nextStep));

                                part.maxA = step.requiredValue;
                            }
                            break;
                        case PartProperty.s:
                            if (part.minS > step.requiredValue)
                            {
                                potentialParts.Add((part, step.result, step.nextStep));
                                break;
                            }
                            else if (part.maxS <= step.requiredValue)
                            {
                                continue;
                            }
                            else
                            {
                                PotentialPart newPart = part;
                                newPart.minS = step.requiredValue + 1;
                                potentialParts.Add((newPart, step.result, step.nextStep));

                                part.maxS = step.requiredValue;
                            }
                            break;
                    }
                }
                else
                {
                    // Account for the possibilty of part already meeting or failing this entirely
                    switch (step.partPropertyToCheck)
                    {
                        case PartProperty.x:
                            if (part.maxX < step.requiredValue)
                            {
                                potentialParts.Add((part, step.result, step.nextStep));
                                break;
                            }
                            else if (part.minX >= step.requiredValue)
                            {
                                continue;
                            }
                            else
                            {
                                PotentialPart newPart = part;
                                newPart.maxX = step.requiredValue - 1;
                                potentialParts.Add((newPart, step.result, step.nextStep));

                                part.minX = step.requiredValue;
                            }
                            break;
                        case PartProperty.m:
                            if (part.maxM < step.requiredValue)
                            {
                                potentialParts.Add((part, step.result, step.nextStep));
                                break;
                            }
                            else if (part.minM >= step.requiredValue)
                            {
                                continue;
                            }
                            else
                            {
                                PotentialPart newPart = part;
                                newPart.maxM = step.requiredValue - 1;
                                potentialParts.Add((newPart, step.result, step.nextStep));

                                part.minM = step.requiredValue;
                            }
                            break;
                        case PartProperty.a:
                            if (part.maxA < step.requiredValue)
                            {
                                potentialParts.Add((part, step.result, step.nextStep));
                                break;
                            }
                            else if (part.minA >= step.requiredValue)
                            {
                                continue;
                            }
                            else
                            {
                                PotentialPart newPart = part;
                                newPart.maxA = step.requiredValue - 1;
                                potentialParts.Add((newPart, step.result, step.nextStep));

                                part.minA = step.requiredValue;
                            }
                            break;
                        case PartProperty.s:
                            if (part.maxS < step.requiredValue)
                            {
                                potentialParts.Add((part, step.result, step.nextStep));
                                break;
                            }
                            else if (part.minS >= step.requiredValue)
                            {
                                continue;
                            }
                            else
                            {
                                PotentialPart newPart = part;
                                newPart.maxS = step.requiredValue - 1;
                                potentialParts.Add((newPart, step.result, step.nextStep));

                                part.minS = step.requiredValue;
                            }
                            break;
                    }
                }
            }

            return potentialParts;
        }
    }
}
