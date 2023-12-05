using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.DayFive
{
    internal class GardenMap
    {
        public struct MapEntry
        {
            public ulong sourceStart;
            public ulong destStart;
            public ulong range;
            public ulong SourceEnd { get { return sourceStart + range - 1; } }
            public ulong DestEnd { get { return destStart + range - 1; } }
            public ulong SourceToDest { get { return destStart - sourceStart; } }
        }

        List<MapEntry> entries;

        public GardenMap()
        {
            entries = new List<MapEntry>();
        }

        public void AddEntryToMap(string rawEntry)
        {
            string[] splitEntry = rawEntry.Split(' ');
            Debug.Assert(splitEntry.Length == 3, "Invalid map entry string");

            entries.Add(new MapEntry { destStart = Convert.ToUInt64(splitEntry[0]), sourceStart = Convert.ToUInt64(splitEntry[1]), range = Convert.ToUInt64(splitEntry[2]) });
        
            // Later functions assume this list is sorted
            entries.Sort((x, y) => x.sourceStart.CompareTo(y.sourceStart));
        }

        // If the source is contained in the map, use the mapping to get the corresponding destination.
        // If it isn't present, the destination just has the same value.
        public ulong GetDestinationFromSource(ulong source)
        {
            foreach (MapEntry entry in entries)
            {
                if (source >= entry.sourceStart && source < entry.SourceEnd)
                {
                    return source + entry.SourceToDest;
                }
            }

            return source;
        }

        // Given range of source values (with a start and end value), return the range(s) that it maps to.
        // A single source range will usually map to multiple destination ranges, since different portions of
        // the source range will overlap with different entries in the GardenMap.
        public List<(ulong destStart, ulong destEnd)> GetDestinationRangeFromSourceRange((ulong sourceStart, ulong sourceEnd) source)
        {
            ulong currentSourceStart = source.sourceStart;
            bool sourceRangeCompleted = false;
            List<(ulong destStart, ulong destEnd)> destinationRanges = new List<(ulong destStart, ulong destEnd)>();

            // Work along the source range until we've reached the end of it, creating new destination ranges
            // as we overlap with various entries in the GardenMap.
            while (!sourceRangeCompleted)
            {
                bool currentLoopDone = false;
                ulong destStart;
                ulong destEnd;

                // First see if the current source position is within any of the ranges in this map.
                // (Definitely scope for optimising this by re-defining a GardenMap to have its entries in
                // order, and just allow us to walk through them).
                foreach (MapEntry entry in entries)
                {
                    if (currentSourceStart >= entry.sourceStart && currentSourceStart < entry.SourceEnd)
                    {
                        // We're within an entry range in the garden map, define the start value of the equivalent
                        // destinatin range.
                        destStart = currentSourceStart + entry.SourceToDest;

                        // If the remaining source values are fully in this map entry, then we have found the
                        // final destination range that this source range transforms into. Otherwise, mark
                        // the end of this particular destination range and update the current source range
                        // position ready for the next pass.
                        if (source.sourceEnd >= entry.sourceStart && source.sourceEnd < entry.SourceEnd)
                        {
                            destEnd = source.sourceEnd + entry.SourceToDest;
                            sourceRangeCompleted = true;
                        }
                        else
                        {
                            destEnd = entry.DestEnd;
                            currentSourceStart = entry.SourceEnd + 1;
                        }

                        destinationRanges.Add((destStart, destEnd));

                        currentLoopDone = true;
                        break;
                    }
                }

                // If we didn't find a match for this source value within the map entries, then the equivalent
                // destination range starts at the same value. Now we need to check to find the next point at
                // which this source range *does* touch an entry.
                if (!currentLoopDone)
                {
                    destStart = currentSourceStart;
                    destEnd = 0;
                    bool hitARange = false;

                    // Check to see if any of the entries in the GardenMap have a sourceStart position within the
                    // remaining input source range. If they do, we need to cut off our destination range and that
                    // point and reset for the next loop.
                    // This relies on the fact that the entries list is sorted by start position.
                    foreach (MapEntry entry in entries)
                    {
                        if ((currentSourceStart < entry.sourceStart) && (source.sourceEnd >= entry.sourceStart))
                        {
                            destEnd = entry.sourceStart - 1;
                            currentSourceStart = entry.sourceStart;
                            hitARange = true;
                            break;
                        }
                    }

                    // Didn't intersect a range, we've completed the final destination range for this source.
                    if (!hitARange)
                    {
                        destEnd = source.sourceEnd;
                        sourceRangeCompleted = true;
                    }

                    destinationRanges.Add((destStart, destEnd));
                }
            }

            return destinationRanges;
        }
    }
}
