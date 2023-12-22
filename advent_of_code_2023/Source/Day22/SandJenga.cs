using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.DayTwentyTwo
{
    class SandBlock
    {
        public int blockIndex;

        public int highestPoint;
        public (int x, int y) startPoint;
        public (int x, int y) endPoint;

        public List<SandBlock> supportedBy;
        public List<SandBlock> supportFor;
    }

    internal class SandJenga
    {
        private SortedList<int, List<SandBlock>> sandblocksByHighestSettledPoint;

        public SandJenga(string[] rawBlocks)
        {
            sandblocksByHighestSettledPoint = new SortedList<int, List<SandBlock>>();
            SortedList<int, List<SandBlock>> sandblocksByLowestFallingPoint = new SortedList<int, List<SandBlock>>();

            int blockIndex = 0;
            foreach (string rawBlock in rawBlocks)
            {
                string[] twoEnds = rawBlock.Split('~');
                string[] endOne = twoEnds[0].Split(',');
                string[] endTwo = twoEnds[1].Split(',');

                int lowestZ = Math.Min(Convert.ToInt32(endOne[2]), Convert.ToInt32(endTwo[2]));

                SandBlock sandBlock = new SandBlock()
                {
                    blockIndex = blockIndex,
                    startPoint = (Convert.ToInt32(endOne[0]), Convert.ToInt32(endOne[1])),
                    endPoint = (Convert.ToInt32(endTwo[0]), Convert.ToInt32(endTwo[1])),
                    supportedBy = new List<SandBlock>(),
                    supportFor = new List<SandBlock>(),
                    highestPoint = Math.Max(Convert.ToInt32(endOne[2]), Convert.ToInt32(endTwo[2]))
                };

                if (!sandblocksByLowestFallingPoint.ContainsKey(lowestZ))
                {
                    sandblocksByLowestFallingPoint[lowestZ] = new List<SandBlock>();
                }

                sandblocksByLowestFallingPoint[lowestZ].Add(sandBlock);
                ++blockIndex;
            }

            // Need to process the blocks in the order from lowest first, to make sure all blocks
            // below a given block are already set up ready to potentially support the higher blocks.
            foreach (KeyValuePair<int, List<SandBlock>> blocksByLowest in sandblocksByLowestFallingPoint)
            {
                foreach (SandBlock block in blocksByLowest.Value)
                {

                    bool supported = false;
                    foreach (int height in sandblocksByHighestSettledPoint.Keys.Reverse())
                    {
                        if (height >= blocksByLowest.Key)
                            continue;

                        foreach (SandBlock otherBlock in sandblocksByHighestSettledPoint[height])
                        {
                            if (SandblocksIntersect(block, otherBlock))
                            {
                                supported = true;
                                block.supportedBy.Add(otherBlock);
                                otherBlock.supportFor.Add(block);
                            }
                        }

                        if (supported)
                        {
                            block.highestPoint -= blocksByLowest.Key - height - 1;
                            break;
                        }
                    }

                    if (!supported)
                    {
                        block.highestPoint -= blocksByLowest.Key - 1;
                    }

                    if (!sandblocksByHighestSettledPoint.ContainsKey(block.highestPoint))
                    {
                        sandblocksByHighestSettledPoint[block.highestPoint] = new List<SandBlock>();
                    }

                    sandblocksByHighestSettledPoint[block.highestPoint].Add(block);
                }
            }
        }

        // Need some overlap in x, and some overlap in y
        private bool SandblocksIntersect(SandBlock blockOne, SandBlock blockTwo)
        {
            return 
                RangeOverlaps((blockOne.startPoint.x, blockOne.endPoint.x),
                              (blockTwo.startPoint.x, blockTwo.endPoint.x)) &&
                RangeOverlaps((blockOne.startPoint.y, blockOne.endPoint.y),
                              (blockTwo.startPoint.y, blockTwo.endPoint.y));
        }

        // Anything other than one range being entirely outside the other constitues overlap
        private bool RangeOverlaps((int start, int end) rangeOne, (int start, int end) rangeTwo)
        {
            // Make sure start and end are in order to make it easier to reason about
            if (rangeOne.start > rangeOne.end)
            {
                int temp = rangeOne.end;
                rangeOne.end = rangeOne.start;
                rangeOne.start = temp;
            }

            // Make sure start and end are in order to make it easier to reason about
            if (rangeTwo.start > rangeTwo.end)
            {
                int temp = rangeTwo.end;
                rangeTwo.end = rangeTwo.start;
                rangeTwo.start = temp;
            }

            return !((rangeOne.end < rangeTwo.start) || (rangeOne.start > rangeTwo.end));
        }

        public int DisintegratableBricks()
        {
            int total = 0;
            int totalBlocks = 0;

            foreach (List<SandBlock> blockList in sandblocksByHighestSettledPoint.Values)
            {
                foreach (SandBlock block in blockList)
                {
                    ++totalBlocks;
                    bool canBeDisintegrated = true;
                    foreach (SandBlock supportedBlock in block.supportFor)
                    {
                        if (supportedBlock.supportedBy.Count == 1)
                        {
                            canBeDisintegrated = false;
                        }
                    }
                    if (canBeDisintegrated)
                    {
                        ++total;
                    }
                }
            }

            return total;
        }

        // For each brick, find out how many other bricks would fall if we disintegrated it.
        // Sum up the total for all bricks.
        public int SumOfChainReactions()
        {
            int sum = 0;

            // I considered doing this by some form of recursion where I make sure I never check a given
            // brick more than once, but that falls apart if I have one brick supporting two others, which
            // together support another. Removing either two of those middle bricks alone won't cause the
            // top one to fall, but removing the bottom brick would get rid of *both* of the middle bricks,
            // and then top one would fall, which we wouldn't have discovered by checking each alone.
            // There probably is some clever way of resolving that, but the slow method probably isn't that
            // slow.
            foreach (List<SandBlock> blockList in sandblocksByHighestSettledPoint.Values)
            {
                foreach (SandBlock block in blockList)
                {
                    HashSet<int> removedBlockIndices = new HashSet<int>();
                    Stack<SandBlock> blocksToRemove = new Stack<SandBlock>();
                    blocksToRemove.Push(block);

                    while (blocksToRemove.Count > 0)
                    {
                        SandBlock removedBlock = blocksToRemove.Pop();
                        removedBlockIndices.Add(removedBlock.blockIndex);

                        foreach (SandBlock supportedBlock in removedBlock.supportFor)
                        {
                            if (BlockIsUnsupported(supportedBlock, removedBlockIndices))
                            {
                                blocksToRemove.Push(supportedBlock);
                                ++sum;
                            }
                        }
                    }

                }
            }

            return sum;
        }

        private bool BlockIsUnsupported(SandBlock block, HashSet<int> indicesToIgnore)
        {
            bool unsupported = true;

            foreach (SandBlock supportingBlock in block.supportedBy)
            {
                if (!indicesToIgnore.Contains(supportingBlock.blockIndex))
                {
                    unsupported = false;
                    break;
                }
            }

            return unsupported;
        }
    }
}
