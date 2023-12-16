using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AOC.DaySixteen;

namespace AOC
{
    internal class DaySixteenSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] puzzleInputLines)
        {
            LightSplitter lightSplitter = new LightSplitter(puzzleInputLines);

            int partOneIlluminatedTiles = lightSplitter.TilesIlluminatedByLightBeam(new LightBeamInput(0, 0, Direction.West));

            int partTwoMax = 0;
            lightSplitter.ResetTiles();

            // I feel like I should be able to make this faster by getting the grid to remember, for a given
            // tile and direction entered from, how many further tiles were illuminated. There'll be a bit more
            // to it than that though, becuase I'd also have to then mark those tiles as illuminated for the
            // remainder of that particular stack loop too to avoid double counting.
            for (int row = 0; row < puzzleInputLines.Length; ++row)
            {
                int numIlluminated = lightSplitter.TilesIlluminatedByLightBeam(new LightBeamInput(row, puzzleInputLines[0].Length - 1, Direction.East));
                if (numIlluminated > partTwoMax) { partTwoMax = numIlluminated; }
                lightSplitter.ResetTiles();

                numIlluminated = lightSplitter.TilesIlluminatedByLightBeam(new LightBeamInput(row, 0, Direction.West));
                if (numIlluminated > partTwoMax) { partTwoMax = numIlluminated; }
                lightSplitter.ResetTiles();
            }

            for (int column = 0; column < puzzleInputLines.Length; ++column)
            {
                int numIlluminated = lightSplitter.TilesIlluminatedByLightBeam(new LightBeamInput(0, column, Direction.North));
                if (numIlluminated > partTwoMax) { partTwoMax = numIlluminated; }
                lightSplitter.ResetTiles();

                numIlluminated = lightSplitter.TilesIlluminatedByLightBeam(new LightBeamInput(puzzleInputLines.Length - 1, column, Direction.South));
                if (numIlluminated > partTwoMax) { partTwoMax = numIlluminated; }
                lightSplitter.ResetTiles();
            }

            return new PuzzleSolution(partOneIlluminatedTiles.ToString(), partTwoMax.ToString());
        }
    }
}
