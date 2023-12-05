using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AOC.DayFive;

namespace AOC
{
    internal class DayFiveSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] puzzleInputLines)
        {
            GardenMap seedToSoil = new GardenMap();
            GardenMap soilToFertilizer = new GardenMap();
            GardenMap fertilizerToWater = new GardenMap();
            GardenMap waterToLight = new GardenMap();
            GardenMap lightToTemperature = new GardenMap();
            GardenMap temperatureToHumidity = new GardenMap();
            GardenMap humidityToLocation = new GardenMap();

            List<(GardenMap, string)> mapsAndTitles = new List<(GardenMap, string)>
                {
                (seedToSoil, "seed-to-soil map:"),
                (soilToFertilizer, "soil-to-fertilizer map:"),
                (fertilizerToWater, "fertilizer-to-water map:"),
                (waterToLight, "water-to-light map:"),
                (lightToTemperature, "light-to-temperature map:"),
                (temperatureToHumidity, "temperature-to-humidity map:"),
                (humidityToLocation, "humidity-to-location map:")
            };

            string[] rawSeeds = puzzleInputLines[0].Split(' ');
            List<ulong> partOneSeeds = new List<ulong>();
            List<(ulong startSeed, ulong range)> partTwoSeeds = new List<(ulong, ulong)>();
            foreach (string seed in rawSeeds)
            {
                if (seed != "seeds:")
                {
                    partOneSeeds.Add(Convert.ToUInt64(seed));
                }
            }

            // For part two, the seed numbers come in pairs that describe a range.
            int seedIndex = 1;
            while (true)
            {
                if (seedIndex >= rawSeeds.Length)
                {
                    break;
                }

                partTwoSeeds.Add((Convert.ToUInt64(rawSeeds[seedIndex]), Convert.ToUInt64(rawSeeds[seedIndex + 1])));
                seedIndex += 2;
            }

            Debug.Assert(puzzleInputLines[1].Length == 0, "Input in unexpected format");

            // Parse the rest of the input into the various maps, and set them up in order ready
            // to iterate through when going from seed to location.
            int currentLine = 2;
            foreach (int ii in Enumerable.Range(0, mapsAndTitles.Count))
            {
                (GardenMap gardenMap, string mapTitle) = mapsAndTitles[ii];
                PopulateGardenMap(puzzleInputLines, mapTitle, ref gardenMap, ref currentLine);
            }

            List<GardenMap> gardenMapsInOrder = new List<GardenMap>();
            foreach ((GardenMap map, string title) in mapsAndTitles)
            {
                gardenMapsInOrder.Add(map);
            }

            // Part one has just a handful on individual seeds to map through to a location.
            ulong lowestLocationpartOne = UInt64.MaxValue;
            foreach (ulong seed in partOneSeeds)
            {
                ulong locationFromSeed = MapSeedToLocation(gardenMapsInOrder, seed);
                if (locationFromSeed < lowestLocationpartOne)
                {
                    lowestLocationpartOne = locationFromSeed;
                }
            }

            // For part two, a single input range will have been split into many 'location' ranges
            // after all the mappings. We just want to find the one with the lowest location,
            // so just check that start of each location range.
            // TODO: I get an off by one error here, it reports one more than the correct answer for
            // both the test and real input.
            ulong lowestLocationpartTwo= UInt64.MaxValue;
            foreach ((ulong startSeed, ulong range)seedRange in partTwoSeeds)
            {
                List<(ulong start, ulong end)> locationRanges = MapSeedRangeToLocationRanges(gardenMapsInOrder, seedRange);

                foreach ((ulong start, ulong end) in locationRanges)
                {
                    if (start < lowestLocationpartTwo)
                    {
                        lowestLocationpartTwo = start;
                    }
                }
            }

            return new PuzzleSolution(lowestLocationpartOne.ToString(), lowestLocationpartTwo.ToString());
        }

        private void PopulateGardenMap(string[] puzzleInputLines, string mapTitle, ref GardenMap gardenMap, ref int currentLine)
        {
            Debug.Assert(puzzleInputLines[currentLine] == mapTitle, "Input parsing expectations broken");
            ++currentLine;

            while (true)
            {
                if (currentLine == puzzleInputLines.Length)
                {
                    break;
                }

                if (puzzleInputLines[currentLine].Length == 0)
                {
                    ++currentLine;
                    break;
                }

                gardenMap.AddEntryToMap(puzzleInputLines[currentLine]);
                ++currentLine;
            }
        }

        private ulong MapSeedToLocation(List<GardenMap> gardenMapsInOrder, ulong seed)
        {
            ulong currentEntry = seed;

            foreach (GardenMap gardenMap in gardenMapsInOrder) 
            {
                currentEntry = gardenMap.GetDestinationFromSource(currentEntry);
            }

            return currentEntry;
        }

        // From a single initial range of seed numbers, iterate through each gardenMap, splitting the range up into
        // new ranges with each step, and return the final list of location ranges.
        private List<(ulong sourceStart, ulong sourceRange)> MapSeedRangeToLocationRanges(List<GardenMap> gardenMapsInOrder, (ulong seed, ulong range)seedRange)
        {
            List<(ulong start, ulong end)> currentRanges = new List<(ulong, ulong)>{ (seedRange.seed, seedRange.seed + seedRange.range - 1) };
            List<(ulong start, ulong end)> nextRanges = new List<(ulong, ulong)>();

            foreach (GardenMap gardenMap in gardenMapsInOrder)
            {
                // Each of the ranges we have to feed into a garden map could be turned into multiple output ranges,
                // depending on how the range interacts with the range entries in the map.
                foreach ((ulong start, ulong end) range in currentRanges)
                {
                    List<(ulong start, ulong end)> rangesFound = gardenMap.GetDestinationRangeFromSourceRange(range);
                    foreach ((ulong, ulong) aRange in rangesFound) { nextRanges.Add(aRange); }
                }

                // There's probably a more idiomatic way to swap our 'next' list into our 'current' one after each loop.
                currentRanges.Clear();
                foreach (var range in nextRanges) { currentRanges.Add(range); }
                nextRanges.Clear();
            }

            return currentRanges;
        }
    }
}
