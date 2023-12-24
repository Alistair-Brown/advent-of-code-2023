using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace AOC.DayTwentyFour
{
    struct Hailstone
    {
        public (long x, long y, long z) position;
        public (long x, long y, long z) velocity;

        // For the case where we ignore the z axis, the x-y line is described by y = mx + c
        public double m;
        public double c;

        public Hailstone(string[] rawPosition, string[] rawVelocity)
        {
            position.x = Convert.ToInt64(rawPosition[0]);
            position.y = Convert.ToInt64(rawPosition[1]);
            position.z = Convert.ToInt64(rawPosition[2]);

            velocity.x = Convert.ToInt64(rawVelocity[0]);
            velocity.y = Convert.ToInt64(rawVelocity[1]);
            velocity.z = Convert.ToInt64(rawVelocity[2]);

            // Rearranging the y = mx + c formula
            m = (double)velocity.y / (double)velocity.x;
            c = (double)position.y - (m * (double)position.x);
        }
    }

    internal class Hailstorm
    {
        private List<Hailstone> hailstones;

        public Hailstorm(string[] inputLines)
        {
            hailstones = new List<Hailstone>();

            foreach (string line in inputLines)
            {
                string[] positionAndVelocity = line.Split(new string[] { " @ " }, StringSplitOptions.None);
                string[] rawPosition = positionAndVelocity[0].Split(new string[] { ", " }, StringSplitOptions.None);
                string[] rawVelocity = positionAndVelocity[1].Split(new string[] { ", " }, StringSplitOptions.None);

                hailstones.Add(new Hailstone(rawPosition, rawVelocity));
            }
        }

        public int NumberOfIntersections(double minPosition, double maxPosition)
        {
            int intersections = 0;

            for (int ii = 0; ii < hailstones.Count - 1; ++ii)
            {
                for (int jj = ii + 1; jj < hailstones.Count; ++jj)
                {
                    if (PathsWillIntersect(hailstones[ii], hailstones[jj], minPosition, maxPosition))
                    {
                        ++intersections;
                    }
                }
            }

            return intersections;
        }

        private bool PathsWillIntersectOld(Hailstone hailstoneOne, Hailstone hailstoneTwo, double minPosition, double maxPosition)
        {
            // First solve intersection point for x by solving y = mx + c for the same y on each
            // hailstone. Then we can get y from just one of the equations.

            double xIntersection = (hailstoneTwo.c - hailstoneOne.c) / (hailstoneOne.m - hailstoneTwo.m);
            double yIntersection = (hailstoneOne.m * xIntersection) + hailstoneOne.c;

            // Must cross within the bounds
            if ((xIntersection < minPosition) || (yIntersection < minPosition) ||
                (xIntersection > maxPosition) || (yIntersection > maxPosition))
            {
                return false;
            }

            // Intersection must be in the future for both paths
            if (((xIntersection > hailstoneOne.position.x) && (hailstoneOne.velocity.x < 0)) ||
                ((xIntersection < hailstoneOne.position.x) && (hailstoneOne.velocity.x > 0)) ||
                ((xIntersection > hailstoneTwo.position.x) && (hailstoneTwo.velocity.x < 0)) ||
                ((xIntersection < hailstoneOne.position.x) && (hailstoneTwo.velocity.x > 0)))
            {
                return false;
            }

            return true;
        }

        private bool PathsWillIntersect(Hailstone hailstoneOne, Hailstone hailstoneTwo, double minPosition, double maxPosition)
        {
            // Start by finding the complete range of x values that each path will take within the future and within
            // the min and max range.
            double minXOne = hailstoneOne.velocity.x > 0 ?
                Math.Max(minPosition, hailstoneOne.position.x) : minPosition;
            double minXTwo = hailstoneTwo.velocity.x > 0 ?
                Math.Max(minPosition, hailstoneTwo.position.x) : minPosition;
            double maxXOne = hailstoneOne.velocity.x > 0 ?
                maxPosition : Math.Min(maxPosition, hailstoneOne.position.x);
            double maxXTwo = hailstoneTwo.velocity.x > 0 ?
                maxPosition : Math.Min(maxPosition, hailstoneTwo.position.x);

            // Only the portion of the x range that is shared by both paths is worth considering.
            double minXPosition = Math.Max(minXOne, minXTwo);
            double maxXPosition = Math.Min(maxXOne, maxXTwo);

            // There must be some overlap in the range for there to be an intersection.
            if (minXPosition >= maxXPosition)
            {
                return false;
            }

            // For each hailstone, use mx + c to find the y value at each end of the range.
            double yAtMinXOne = hailstoneOne.c + (hailstoneOne.m * minXPosition);
            double yAtMinXTwo = hailstoneTwo.c + (hailstoneTwo.m * minXPosition);
            double yAtMaxXOne = hailstoneOne.c + (hailstoneOne.m * maxXPosition);
            double yAtMaxXTwo = hailstoneTwo.c + (hailstoneTwo.m * maxXPosition);

            // I'm about to assume that the y values don't intersect on the edges of the range
            // exactly, so double check that
            Debug.Assert((yAtMinXOne != yAtMinXTwo) && (yAtMaxXOne != yAtMaxXTwo));

            // There's only an overlap if the y's swap which is greater over the course of
            // the x range.          
            if ((yAtMinXOne > yAtMinXTwo) != (yAtMaxXOne > yAtMaxXTwo))
            {
                // Need to find the y value at the point of intersection. Start by finding the total amount
                // by which the gap between the y values changes over the range.
                double totalDiff = Math.Abs(yAtMinXOne - yAtMinXTwo) + Math.Abs(yAtMaxXOne - yAtMaxXTwo);

                // Now find the ratio along this range that the y's intersect, and use this to find the 
                // y value at the intersection.
                double ratio = Math.Abs(yAtMinXOne - yAtMinXTwo) / totalDiff;
                double yAtIntersection = yAtMinXOne + ((yAtMaxXOne - yAtMinXOne) * ratio);

                // If the y at this is in range, it's a valid intersection.
                if ((yAtIntersection >= minPosition) && (yAtIntersection <= maxPosition))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
