using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace AOC.DayTwentyOne
{
    // In this struct, step 1 is the first step *into* that particular grid, from
    // whatever entry point we're using
    struct PlotsFoundAfterSteps
    {
        // The lists are 0-indexed, but step numbering starts at 1. So e.g. the 0th element in
        // each list is the plots found after 1 step (which must be 0 for the even list, since 1
        // is an odd number).
        public List<int> oddPlotsAfterSteps;
        public List<int> evenPlotsAfterSteps;

        public int stepsToFindAllPlots;
    }

    internal class GardenMap
    {
        private char[][] plotMap;
        (int row, int column) startingPosition;

        public GardenMap(string[] rawMap)
        {
            plotMap = new char[rawMap.Length][];
            foreach (int ii in Enumerable.Range(0,rawMap.Length))
            {
                plotMap[ii] = new char[rawMap[ii].Length];
                foreach (int jj in Enumerable.Range(0, rawMap[ii].Length))
                {
                    plotMap[ii][jj] = rawMap[ii][jj];
                    if (plotMap[ii][jj] == 'S')
                    {
                        startingPosition = (ii, jj);
                    }
                }
            }
        }

        // Explore the garden from the starting positions for the given number of steps, and then depending
        // whether than number is odd or even, return the number of plots found after an even or odd number of
        // steps for that number. This assumes that the garden is finite, and that we don't manage to fully explore
        // it in the number of steps we have available - i.e. only suitable for part one.
        public int FiniteGardensVisitedAfterNSteps(int numSteps)
        {
            PlotsFoundAfterSteps plotsFound = ExploreGarden(numSteps, startingPosition);

            if (numSteps % 2 == 0)
            {
                return plotsFound.evenPlotsAfterSteps[numSteps - 1];
            }
            else
            {
                return plotsFound.oddPlotsAfterSteps[numSteps - 1];
            }
        }

        public ulong InfiniteGardensVisitedAfterNSteps(int numSteps)
        {
            // To get the mid points along each edge, we use the knowledge that the garden in our input has
            // an odd number of rows and columns. If I wanted to optimise this further, it seems from looking
            // at the debug values that you get the same results starting from any corner or any mid point,
            // but I'll leave it for now just in case I'm wrong about that.
            int bottomRow = plotMap.Length - 1;
            int rightColumn = plotMap[0].Length - 1;
            int midRow = plotMap.Length / 2;
            int midColumn = plotMap[0].Length / 2;
            PlotsFoundAfterSteps plotsFoundStartingCentre = ExploreGarden(int.MaxValue, (midRow, midColumn));
            PlotsFoundAfterSteps plotsFoundStartingBottomLeft = ExploreGarden(int.MaxValue, (bottomRow, 0));
            PlotsFoundAfterSteps plotsFoundStartingMidLeft = ExploreGarden(int.MaxValue, (midRow, 0));
            PlotsFoundAfterSteps plotsFoundStartingTopLeft = ExploreGarden(int.MaxValue, (0, 0));
            PlotsFoundAfterSteps plotsFoundStartingTop = ExploreGarden(int.MaxValue, (0, midColumn));
            PlotsFoundAfterSteps plotsFoundStartingTopRight = ExploreGarden(int.MaxValue, (0, rightColumn));
            PlotsFoundAfterSteps plotsFoundStartingMidRight = ExploreGarden(int.MaxValue, (midRow, rightColumn));
            PlotsFoundAfterSteps plotsFoundStartingBottomRight = ExploreGarden(int.MaxValue, (bottomRow, rightColumn)); 
            PlotsFoundAfterSteps plotsFoundStartingBottom = ExploreGarden(int.MaxValue, (bottomRow, midColumn));

            // Find the number of garden units from the central garden we will be able to process completely in
            // the number of steps. Consider just the cardinal directions first, since they're easier to visualise.
            // Don't count the central garden for this. For the outermost complete garden, the number of steps it will
            // take to complete is the half-width to leave the centre garden, the steps to pass through all intermediate
            // gardens, and the steps to complete that garden itself. So subtracting that half width and the steps to fill
            // the final garden from the total steps we have available will leave the steps to pass through the intermediate
            // garden, and any spare. Divide this by the garden width to get the number of intermediate gardens passed through
            // and add one for that final garden to get the total complete gardens in that direction.
            int gardensToLeft = ((numSteps - (plotMap.Length / 2) - plotsFoundStartingMidRight.stepsToFindAllPlots) / plotMap.Length) + 1;
            int gardensToRight = ((numSteps - (plotMap.Length / 2) - plotsFoundStartingMidLeft.stepsToFindAllPlots) / plotMap.Length) + 1;
            int gardensUp = ((numSteps - (plotMap.Length / 2) - plotsFoundStartingBottom.stepsToFindAllPlots) / plotMap.Length) + 1;
            int gardensDown = ((numSteps - (plotMap.Length / 2) - plotsFoundStartingTop.stepsToFindAllPlots) / plotMap.Length) + 1;

            int filledGardensInShortestDirection = Math.Min(Math.Min(gardensToLeft, gardensToRight), Math.Min(gardensUp, gardensDown));
            
            // How many steps do we have left after reaching the edge of our completely explored region in a cardinal
            // direction. Remember to account for the steps to leave the central garden too.
            int remainingSteps = numSteps - (plotMap.Length / 2) - (filledGardensInShortestDirection * plotMap.Length);

            // The number of gardens we can touch in a cardinal direction from this point is fairly simple. However
            // it's possible that the thickness of the outer diagonal layer is one more than this number, since we can get
            // to the corner of one of those in half the number of steps it takes to reach the next cardinal direction.
            int numberOfExtraCardinalGardens = remainingSteps / plotMap.Length;
            if (remainingSteps % plotMap.Length > 0) { ++numberOfExtraCardinalGardens; }
            int diagonalOuterGardensWidth = (remainingSteps + (plotMap.Length / 2)) / plotMap.Length;
            if ((remainingSteps + (plotMap.Length / 2)) % plotMap.Length > 0) { ++diagonalOuterGardensWidth; }

            // For the section of completed gardens, the even and oddness of step count is out of sync. The central garden
            // has experienced either odd or even steps according to the total number,
            // but the next ring out experienced the opposite, the next ring out the same, next the opposite, etc. Need
            // to find out how many of the gardens are in sync vs out of sync with the overall odd/evenness. There's probably
            // a neater formula for this, but I'm just going to work outwards through the rings and count them.
            ulong numberOfInSyncFilledGardens = 1; // Include the central garden
            ulong numberOfOutofSyncFilledGardens = 0;
            bool nextRingInSync = false;
            for (int ringNumber = 1; ringNumber <= filledGardensInShortestDirection; ++ringNumber)
            {
                if (!nextRingInSync)
                {
                    numberOfOutofSyncFilledGardens += (ulong)(4 * ringNumber);
                }
                else
                {
                    numberOfInSyncFilledGardens += (ulong)(4 * ringNumber);
                }
                nextRingInSync = !nextRingInSync;
            }

            int maxEvenTiles = plotsFoundStartingCentre.evenPlotsAfterSteps[plotsFoundStartingCentre.evenPlotsAfterSteps.Count - 1];
            int maxOddTiles = plotsFoundStartingCentre.oddPlotsAfterSteps[plotsFoundStartingCentre.oddPlotsAfterSteps.Count - 1];

            ulong totalGardens = 0;
            if (numSteps % 2 == 0)
            {
                totalGardens += numberOfInSyncFilledGardens * (ulong)maxEvenTiles;
                totalGardens += numberOfOutofSyncFilledGardens * (ulong)maxOddTiles;
            }
            else
            {
                totalGardens += numberOfInSyncFilledGardens * (ulong)maxOddTiles;
                totalGardens += numberOfOutofSyncFilledGardens * (ulong)maxEvenTiles;
            }

            // Note that my remaining steps are given from the final tile of the outermost complete garden, but
            // for my pre-measured other gardens I always started from just within that garden, so need to reduce
            // the remaining step count by 1 to account for that extra step.
            int remainingStepsFromCardinalDirectionStart = remainingSteps - 1;
            for (int ii = 0; ii < numberOfExtraCardinalGardens; ++ii)
            {
                if (remainingStepsFromCardinalDirectionStart % 2 == 0)
                {
                    totalGardens += (ulong)(plotsFoundStartingMidRight.evenPlotsAfterSteps[remainingStepsFromCardinalDirectionStart - 1]);
                    totalGardens += (ulong)(plotsFoundStartingMidLeft.evenPlotsAfterSteps[remainingStepsFromCardinalDirectionStart - 1]);
                    totalGardens += (ulong)(plotsFoundStartingTop.evenPlotsAfterSteps[remainingStepsFromCardinalDirectionStart - 1]);
                    totalGardens += (ulong)(plotsFoundStartingBottom.evenPlotsAfterSteps[remainingStepsFromCardinalDirectionStart - 1]);
                }
                else
                {
                    totalGardens += (ulong)(plotsFoundStartingMidRight.oddPlotsAfterSteps[remainingStepsFromCardinalDirectionStart - 1]);
                    totalGardens += (ulong)(plotsFoundStartingMidLeft.oddPlotsAfterSteps[remainingStepsFromCardinalDirectionStart - 1]);
                    totalGardens += (ulong)(plotsFoundStartingTop.oddPlotsAfterSteps[remainingStepsFromCardinalDirectionStart - 1]);
                    totalGardens += (ulong)(plotsFoundStartingBottom.oddPlotsAfterSteps[remainingStepsFromCardinalDirectionStart - 1]);
                }

                // My input only has one extra garden in each cardinal direction after the fully explored
                // zone so this code is never executed. But in theory there could have been another layer,
                // in which case we'd need to reduce the remaining step count like so.
                remainingStepsFromCardinalDirectionStart -= plotMap.Length;
            }

            // Getting to the right number of remaining steps for the diagonal start points is a bit more convoluted.
            // We have to step back almost a full width back from our current position at the edge of the fully
            // explored zone, to the other edge of that garden (stepping back is effectively increasing our remaining
            // steps), then up (I'm describing the case to reach a bottom right corner here, but the same maths
            // applies for others) half a width, then one step further to enter that diagonal garden).
            int remainingStepsFromDiagonalStart = remainingSteps + (plotMap.Length - 1) - (plotMap.Length / 2) - 1;
            int numberOfGardensOnEachDiagonal = filledGardensInShortestDirection;
            for (int ii = 0; ii < diagonalOuterGardensWidth; ++ii)
            {
                if (remainingStepsFromDiagonalStart % 2 == 0)
                {
                    totalGardens += (ulong)(numberOfGardensOnEachDiagonal * (plotsFoundStartingBottomRight.evenPlotsAfterSteps[remainingStepsFromDiagonalStart - 1]));
                    totalGardens += (ulong)(numberOfGardensOnEachDiagonal * (plotsFoundStartingTopLeft.evenPlotsAfterSteps[remainingStepsFromDiagonalStart - 1]));
                    totalGardens += (ulong)(numberOfGardensOnEachDiagonal * (plotsFoundStartingBottomLeft.evenPlotsAfterSteps[remainingStepsFromDiagonalStart - 1]));
                    totalGardens += (ulong)(numberOfGardensOnEachDiagonal * (plotsFoundStartingTopRight.evenPlotsAfterSteps[remainingStepsFromDiagonalStart - 1]));
                }
                else
                {
                    totalGardens += (ulong)(numberOfGardensOnEachDiagonal * (plotsFoundStartingBottomRight.oddPlotsAfterSteps[remainingStepsFromDiagonalStart - 1]));
                    totalGardens += (ulong)(numberOfGardensOnEachDiagonal * (plotsFoundStartingTopLeft.oddPlotsAfterSteps[remainingStepsFromDiagonalStart - 1]));
                    totalGardens += (ulong)(numberOfGardensOnEachDiagonal * (plotsFoundStartingBottomLeft.oddPlotsAfterSteps[remainingStepsFromDiagonalStart - 1]));
                    totalGardens += (ulong)(numberOfGardensOnEachDiagonal * (plotsFoundStartingTopRight.oddPlotsAfterSteps[remainingStepsFromDiagonalStart - 1]));
                }

                // Each subsequent layer of diagonal will be one garden longer, and have 1 garden width fewer steps
                // left to use
                remainingStepsFromDiagonalStart -= plotMap.Length;
                ++numberOfGardensOnEachDiagonal;
            }

            return totalGardens;
        }

        private PlotsFoundAfterSteps ExploreGarden(int maxSteps, (int row, int column) startPosition)
        {
            PlotsFoundAfterSteps plotsFound = new PlotsFoundAfterSteps()
            {
                evenPlotsAfterSteps = new List<int>(),
                oddPlotsAfterSteps = new List<int>(),
                stepsToFindAllPlots = 0
            };

            HashSet<(int row, int column)> locationsVisitedOnEvenSteps = new HashSet<(int row, int column)>();
            HashSet<(int row, int column)> locationsVisitedOnOddSteps = new HashSet<(int row, int column)>();

            bool currentStepEven = false;
            List<(int row, int column)> thisTurnLocationsToStepFrom = new List<(int row, int column)>();
            List<(int row, int column)> nextTurnLocationsToStepFrom = new List<(int row, int column)>();

            thisTurnLocationsToStepFrom.Add(startPosition);

            for (int stepNumber = 1; stepNumber <= maxSteps; ++stepNumber)
            {
                foreach ((int row, int column) locationsToStepFrom in thisTurnLocationsToStepFrom)
                {
                    (int row, int column)[] locationsReached = new (int row, int column)[4];
                    locationsReached[0] = (locationsToStepFrom.row - 1, locationsToStepFrom.column);
                    locationsReached[1] = (locationsToStepFrom.row + 1, locationsToStepFrom.column);
                    locationsReached[2] = (locationsToStepFrom.row, locationsToStepFrom.column - 1);
                    locationsReached[3] = (locationsToStepFrom.row, locationsToStepFrom.column + 1);

                    foreach ((int row, int column) location in locationsReached)
                    {
                        // Locations are only valid if they're within the bounds of the map, and not
                        // a rock.
                        if (location.row >= 0 &&
                            location.row < plotMap.Length &&
                            location.column >= 0 &&
                            location.column < plotMap[0].Length &&
                            plotMap[location.row][location.column] != '#')
                        {

                            // Only add this as a location if we haven't already visited it on this step type, or
                            // this problem is going to get exponential.
                            if (currentStepEven)
                            {
                                if (!locationsVisitedOnEvenSteps.Contains(location))
                                {
                                    locationsVisitedOnEvenSteps.Add(location);
                                    nextTurnLocationsToStepFrom.Add(location);
                                }
                            }
                            else
                            {
                                if (!locationsVisitedOnOddSteps.Contains(location))
                                {
                                    locationsVisitedOnOddSteps.Add(location);
                                    nextTurnLocationsToStepFrom.Add(location);
                                }
                            }
                        }
                    }
                }

                plotsFound.evenPlotsAfterSteps.Add(locationsVisitedOnEvenSteps.Count);
                plotsFound.oddPlotsAfterSteps.Add(locationsVisitedOnOddSteps.Count);

                thisTurnLocationsToStepFrom = nextTurnLocationsToStepFrom;
                nextTurnLocationsToStepFrom = new List<(int row, int column)>();
                currentStepEven = !currentStepEven;

                if (thisTurnLocationsToStepFrom.Count == 0)
                {
                    plotsFound.stepsToFindAllPlots = stepNumber;
                    break;
                }
            }

            return plotsFound;
        }
    }
}
