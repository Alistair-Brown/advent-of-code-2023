using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AOC.DayTwo
{
    internal class CubeGame
    {
        public struct SingleDraw
        {
            public readonly ulong numRed;
            public readonly ulong numGreen;
            public readonly ulong numBlue;

            public SingleDraw(string drawInfo)
            {
                string[] splitDraw = drawInfo.Split(',');

                numRed = 0;
                numGreen = 0;
                numBlue = 0;

                foreach (string cubesDrawn in splitDraw)
                {
                    string[] colourAndNumber = cubesDrawn.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (colourAndNumber[1] == "blue")
                    {
                        numBlue = Convert.ToUInt64(colourAndNumber[0]);
                    }
                    else if (colourAndNumber[1] == "red")
                    {
                        numRed = Convert.ToUInt64(colourAndNumber[0]);
                    }
                    else if (colourAndNumber[1] == "green")
                    {
                        numGreen = Convert.ToUInt64(colourAndNumber[0]);
                    }
                    else
                    {
                        Debug.Assert(false, "Unrecognised colour");
                    }
                }
            }
        }

        public readonly int gameID;
        private readonly List<SingleDraw> draws;

        public CubeGame(string inputLine)
        {
            string[] gameAndDraw = inputLine.Split(':');
            string[] gameAndID = gameAndDraw[0].Split(' ');

            Debug.Assert(gameAndID[0] == "Game", "String parsing broken");
            gameID = Convert.ToInt32(gameAndID[1]);

            string[] rawDraws = gameAndDraw[1].Split(';');
            draws = new List<SingleDraw>();
            foreach(string singleRawDraw in rawDraws)
            {
                draws.Add(new SingleDraw(singleRawDraw));
            }
        }

        public bool GameIsPossible(ulong numRed, ulong numGreen, ulong numBlue)
        {
            foreach (SingleDraw singleDraw in draws)
            {
                if ((singleDraw.numRed > numRed) ||
                    (singleDraw.numGreen > numGreen) ||
                    (singleDraw.numBlue > numBlue))
                {
                    return false;
                }
            }
            return true;
        }

        public ulong MinimumCubeSetPower()
        {
            ulong minRedCubes = 0;
            ulong minBlueCubes = 0;
            ulong minGreenCubes = 0;

            foreach (SingleDraw singleDraw in draws)
            {
                if (singleDraw.numRed > minRedCubes)
                {
                    minRedCubes = singleDraw.numRed;
                }
                if (singleDraw.numGreen > minGreenCubes)
                {
                    minGreenCubes = singleDraw.numGreen;
                }
                if (singleDraw.numBlue > minBlueCubes)
                {
                    minBlueCubes = singleDraw.numBlue;
                }
            }

            return minBlueCubes * minGreenCubes * minRedCubes;
        }
    }
}
