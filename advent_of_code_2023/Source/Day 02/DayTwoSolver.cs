using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AOC.DayTwo;

namespace AOC
{
    internal class DayTwoSolver : IPuzzleSolver
    {
        public PuzzleSolution SolvePuzzle(string[] puzzleInputLines)
        {
            int partOnePossibleGamesSum = 0;
            ulong partTwoMinimumPower = 0;

            List<CubeGame> cubeGames = new List<CubeGame>();

            foreach (string inputLine in puzzleInputLines)
            {
                cubeGames.Add(new CubeGame(inputLine));
            }

            foreach (CubeGame cubeGame in cubeGames)
            {
                if (cubeGame.GameIsPossible(12,13,14))
                {
                    partOnePossibleGamesSum += cubeGame.gameID;
                }

                partTwoMinimumPower += cubeGame.MinimumCubeSetPower();
            }

            return new PuzzleSolution(partOnePossibleGamesSum.ToString(), partTwoMinimumPower.ToString());

        }
    }
}
