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

        public long PartTwoPositionSumNew()
        {            
            // Choose first three hailstones for now
            Hailstone hailstoneOne = hailstones[0];
            Hailstone hailstoneTwo = hailstones[1];

            // Not sure this works actually, it assume the rock starts at hailstone one's start position,
            // which won't be the case. Maybe we can work around that a little actually. We're doing everything
            // relative to hailstone one, so hailstone one never moves in that frame of reference. And so we
            // can probably introduce some 'start time' to the problem.
            // Can we write a formula for x rock relative velocity that includes start time? And then try different
            // values of x-vel to find the start time. We can feed that into similar equations for y and z vel to
            // get various valid relative velocities, and try each of them against the third hailstone?
            (long x, long y, long z) twoStartPosRelativeToOne = 
               (hailstoneTwo.position.x - hailstoneOne.position.x,
                hailstoneTwo.position.y - hailstoneOne.position.y,
                hailstoneTwo.position.z - hailstoneOne.position.z);
            (long x, long y, long z) twoStartVelRelativeToOne =
               (hailstoneTwo.velocity.x - hailstoneOne.velocity.x,
                hailstoneTwo.velocity.y - hailstoneOne.velocity.y,
                hailstoneTwo.velocity.z - hailstoneOne.velocity.z);

            //(long x, long y, long z) testRock = (-1, 0, 0);
            //long? test = TimeOfCollisionWithOtherHailstone(
            //    ref testRock,
            //    twoStartPosRelativeToOne,
            //    twoStartVelRelativeToOne);

            //// Problem is we're not testing enough hailstones.
            //Debug.Assert(test.HasValue);
            //Debug.Assert(test == 5);
            //Debug.Assert(testRock.z == 4);

            (long x, long y, long z) relativeRockVelocity = (0, 0, 0);
            long velocityRangeToTest = 1;
            long? timeBeforeCollision = null;
            bool foundAnswer = false;
            while (true)
            {
                relativeRockVelocity.x = -velocityRangeToTest;
                for (long ii = -velocityRangeToTest; ii <= velocityRangeToTest; ++ii)
                {
                    relativeRockVelocity.y = ii;

                    timeBeforeCollision = TimeOfCollisionWithOtherHailstone(
                        ref relativeRockVelocity,
                        twoStartPosRelativeToOne,
                        twoStartVelRelativeToOne);

                    if (timeBeforeCollision != null)
                    {
                        long rockZ = relativeRockVelocity.z;
                        foundAnswer = true;

                        for (int num = 2; num < hailstones.Count; ++num)
                        {
                            (long x, long y, long z) relPos =
                               (hailstones[num].position.x - hailstoneOne.position.x,
                                hailstones[num].position.y - hailstoneOne.position.y,
                                hailstones[num].position.z - hailstoneOne.position.z);
                            (long x, long y, long z) relVel =
                               (hailstones[num].velocity.x - hailstoneOne.velocity.x,
                                hailstones[num].velocity.y - hailstoneOne.velocity.y,
                                hailstones[num].velocity.z - hailstoneOne.velocity.z);

                            long? timeAgain = TimeOfCollisionWithOtherHailstone(
                                ref relativeRockVelocity,
                                relPos,
                                relVel);

                            if (!((timeAgain != null) &&
                                 (timeAgain == timeBeforeCollision) &&
                                 (rockZ == relativeRockVelocity.z)))
                            {
                                foundAnswer = false;
                                break;
                            }
                        }
                    }
                    if (foundAnswer)
                    {
                        break;
                    }
                }
                if (foundAnswer)
                {
                    break;
                }

                relativeRockVelocity.x = velocityRangeToTest;
                for (long ii = -velocityRangeToTest; ii <= velocityRangeToTest; ++ii)
                {
                    relativeRockVelocity.y = ii;

                    timeBeforeCollision = TimeOfCollisionWithOtherHailstone(
                        ref relativeRockVelocity,
                        twoStartPosRelativeToOne,
                        twoStartVelRelativeToOne);

                    if (timeBeforeCollision != null)
                    {
                        long rockZ = relativeRockVelocity.z;
                        foundAnswer = true;

                        for (int num = 2; num < hailstones.Count; ++num)
                        {
                            (long x, long y, long z) relPos =
                               (hailstones[num].position.x - hailstoneOne.position.x,
                                hailstones[num].position.y - hailstoneOne.position.y,
                                hailstones[num].position.z - hailstoneOne.position.z);
                            (long x, long y, long z) relVel =
                               (hailstones[num].velocity.x - hailstoneOne.velocity.x,
                                hailstones[num].velocity.y - hailstoneOne.velocity.y,
                                hailstones[num].velocity.z - hailstoneOne.velocity.z);

                            long? timeAgain = TimeOfCollisionWithOtherHailstone(
                                ref relativeRockVelocity,
                                relPos,
                                relVel);

                            if (!((timeAgain != null) &&
                                 (timeAgain == timeBeforeCollision) &&
                                 (rockZ == relativeRockVelocity.z)))
                            {
                                foundAnswer = false;
                                break;
                            }
                        }
                    }
                    if (foundAnswer)
                    {
                        break;
                    }
                }
                if (foundAnswer)
                {
                    break;
                }

                relativeRockVelocity.y = -velocityRangeToTest;
                for (long ii = -velocityRangeToTest; ii <= velocityRangeToTest; ++ii)
                {
                    relativeRockVelocity.x = ii;

                    timeBeforeCollision = TimeOfCollisionWithOtherHailstone(
                        ref relativeRockVelocity,
                        twoStartPosRelativeToOne,
                        twoStartVelRelativeToOne);

                    if (timeBeforeCollision != null)
                    {
                        long rockZ = relativeRockVelocity.z;
                        foundAnswer = true;

                        for (int num = 2; num < hailstones.Count; ++num)
                        {
                            (long x, long y, long z) relPos =
                               (hailstones[num].position.x - hailstoneOne.position.x,
                                hailstones[num].position.y - hailstoneOne.position.y,
                                hailstones[num].position.z - hailstoneOne.position.z);
                            (long x, long y, long z) relVel =
                               (hailstones[num].velocity.x - hailstoneOne.velocity.x,
                                hailstones[num].velocity.y - hailstoneOne.velocity.y,
                                hailstones[num].velocity.z - hailstoneOne.velocity.z);

                            long? timeAgain = TimeOfCollisionWithOtherHailstone(
                                ref relativeRockVelocity,
                                relPos,
                                relVel);

                            if (!((timeAgain != null) &&
                                 (timeAgain == timeBeforeCollision) &&
                                 (rockZ == relativeRockVelocity.z)))
                            {
                                foundAnswer = false;
                                break;
                            }
                        }
                    }
                    if (foundAnswer)
                    {
                        break;
                    }
                }
                if (foundAnswer)
                {
                    break;
                }

                relativeRockVelocity.y = velocityRangeToTest;
                for (long ii = -velocityRangeToTest; ii <= velocityRangeToTest; ++ii)
                {
                    relativeRockVelocity.x = ii;

                    timeBeforeCollision = TimeOfCollisionWithOtherHailstone(
                        ref relativeRockVelocity,
                        twoStartPosRelativeToOne,
                        twoStartVelRelativeToOne);

                    if (timeBeforeCollision != null)
                    {
                        long rockZ = relativeRockVelocity.z;
                        foundAnswer = true;

                        for (int num = 2; num < hailstones.Count; ++num)
                        {
                            (long x, long y, long z) relPos =
                               (hailstones[num].position.x - hailstoneOne.position.x,
                                hailstones[num].position.y - hailstoneOne.position.y,
                                hailstones[num].position.z - hailstoneOne.position.z);
                            (long x, long y, long z) relVel =
                               (hailstones[num].velocity.x - hailstoneOne.velocity.x,
                                hailstones[num].velocity.y - hailstoneOne.velocity.y,
                                hailstones[num].velocity.z - hailstoneOne.velocity.z);

                            long? timeAgain = TimeOfCollisionWithOtherHailstone(
                                ref relativeRockVelocity,
                                relPos,
                                relVel);

                            if (!((timeAgain != null) &&
                                 (timeAgain == timeBeforeCollision) &&
                                 (rockZ == relativeRockVelocity.z)))
                            {
                                foundAnswer = false;
                                break;
                            }
                        }
                    }
                    if (foundAnswer)
                    {
                        break;
                    }
                }
                if (foundAnswer)
                {
                    break;
                }

                ++velocityRangeToTest;
            }

            (long x, long y, long z) rockAbsoluteVelocity = (
                hailstoneOne.velocity.x + relativeRockVelocity.x,
                hailstoneOne.velocity.y + relativeRockVelocity.y,
                hailstoneOne.velocity.z + relativeRockVelocity.z);
            (long x, long y, long z) collisionPosition = (
                hailstoneOne.position.x + hailstoneOne.velocity.x * timeBeforeCollision.Value,
                hailstoneOne.position.y + hailstoneOne.velocity.y * timeBeforeCollision.Value,
                hailstoneOne.position.z + hailstoneOne.velocity.z * timeBeforeCollision.Value);
            (long x, long y, long z) rockStartPosition = (
                collisionPosition.x - rockAbsoluteVelocity.x * timeBeforeCollision.Value,
                collisionPosition.y - rockAbsoluteVelocity.y * timeBeforeCollision.Value,
                collisionPosition.z - rockAbsoluteVelocity.z * timeBeforeCollision.Value);

            return rockStartPosition.x + rockStartPosition.y + rockStartPosition.z;
        }

        // Take the velocity and start position of a hailstone relative to another hailstone, and
        // also the velocity of the tock relative to this other hailstone. I've done some algebra on
        // paper to use this to produce the time between the collision of the rock with the other hailstone
        // and the collision of the hailstone provided here. This can then be used to return the time of the
        // collision with that other hailstone.
        // If this is a valid integer time, then this function will return a non-null long, and also
        // populate the z component of the rock velocity.
        private long? TimeOfCollisionWithOtherHailstone(
            ref (long x, long y, long z) rockRelativeVelocity,
            (long x, long y, long z) hailstoneRelativeStartPosition,
            (long x, long y, long z) hailstoneRelativeVelocity)
        {
            long numerator = (hailstoneRelativeVelocity.y * hailstoneRelativeStartPosition.x) -
                (hailstoneRelativeVelocity.x * hailstoneRelativeStartPosition.y);
            long denominator = (hailstoneRelativeVelocity.y * (rockRelativeVelocity.x - hailstoneRelativeVelocity.x)) -
                (hailstoneRelativeVelocity.x * (rockRelativeVelocity.y - hailstoneRelativeVelocity.y));

            if (denominator == 0 || numerator % denominator != 0)
            {
                return null;
            }

            long timeBetween = numerator / denominator;

            if (hailstoneRelativeVelocity.x == 0 && hailstoneRelativeVelocity.y == 0)
            {
                Console.WriteLine("Error");
            }

            long timeBefore = 0;
            if (hailstoneRelativeVelocity.x != 0)
            {
                long timeBeforeNumerator =
                    ((rockRelativeVelocity.x - hailstoneRelativeVelocity.x) * timeBetween) - hailstoneRelativeStartPosition.x;

                // Check that this also gives an integer time between the collision of the rock and the
                // first hailstone
                if ((timeBeforeNumerator % hailstoneRelativeVelocity.x) != 0)
                {
                    return null;
                }

                timeBefore = timeBeforeNumerator / hailstoneRelativeVelocity.x;
            }
            else
            {
                long timeBeforeNumerator =
                    ((rockRelativeVelocity.y - hailstoneRelativeVelocity.y) * timeBetween) - hailstoneRelativeStartPosition.y;

                // Check that this also gives an integer time between the collision of the rock and the
                // first hailstone
                if ((timeBeforeNumerator % hailstoneRelativeVelocity.y) != 0)
                {
                    return null;
                }

                timeBefore = timeBeforeNumerator / hailstoneRelativeVelocity.y;
            }

            if (timeBefore < 0 || (timeBefore + timeBetween < 0))
            {
                return null;
            }

            long rockZVelocityNumerator = ((timeBetween + timeBefore) * hailstoneRelativeVelocity.z) + hailstoneRelativeStartPosition.z;
        
            if ((rockZVelocityNumerator % timeBetween) != 0)
            {
                return null;
            }

            rockRelativeVelocity.z = rockZVelocityNumerator / timeBetween;

            return timeBefore;
        }

        public long PartTwoPositionSumOld()
        {
            // Choose first three hailstones for now
            Hailstone hailstoneOne = hailstones[0];
            Hailstone hailstoneTwo = hailstones[1];
            Hailstone hailstoneThree = hailstones[2];

            long max_nanoseconds = 2;
            bool foundVelocity = false;
            (long x, long y, long z) intersectionVelocity = (0, 0 ,0);
            long hailstoneOneIntersectionTime = 0;

            long hailstoneOnePositionX;
            long hailstoneOnePositionY;
            long hailstoneOnePositionZ;

            long hailstoneTwoPositionX;
            long hailstoneTwoPositionY;
            long hailstoneTwoPositionZ;

            while (true)
            {
                hailstoneOnePositionX = hailstoneOne.position.x + (hailstoneOne.velocity.x * max_nanoseconds);
                hailstoneOnePositionY = hailstoneOne.position.y + (hailstoneOne.velocity.y * max_nanoseconds);
                hailstoneOnePositionZ = hailstoneOne.position.z + (hailstoneOne.velocity.z * max_nanoseconds);

                for  (long nanoseconds = 0; nanoseconds < max_nanoseconds; ++nanoseconds)
                {
                    hailstoneTwoPositionX = hailstoneTwo.position.x + (hailstoneTwo.velocity.x * nanoseconds);
                    hailstoneTwoPositionY = hailstoneTwo.position.y + (hailstoneTwo.velocity.y * nanoseconds);
                    hailstoneTwoPositionZ = hailstoneTwo.position.z + (hailstoneTwo.velocity.z * nanoseconds);

                    // Pass the hailstones in the order that the rock will hit them in this case.
                    if (PartTwoPathsIntersect(
                        (hailstoneTwoPositionX, hailstoneTwoPositionY, hailstoneTwoPositionZ),
                        (hailstoneOnePositionX, hailstoneOnePositionY, hailstoneOnePositionZ),
                        nanoseconds,
                        max_nanoseconds,
                        hailstoneThree,
                        out intersectionVelocity))
                    {
                        hailstoneOneIntersectionTime = max_nanoseconds;
                        foundVelocity = true;
                        break;
                    }
                }

                if (foundVelocity)
                {
                    break;
                }

                hailstoneTwoPositionX = hailstoneTwo.position.x + (hailstoneTwo.velocity.x * max_nanoseconds);
                hailstoneTwoPositionY = hailstoneTwo.position.y + (hailstoneTwo.velocity.y * max_nanoseconds);
                hailstoneTwoPositionZ = hailstoneTwo.position.z + (hailstoneTwo.velocity.z * max_nanoseconds);

                for (long nanoseconds = 0; nanoseconds < max_nanoseconds - 1; ++nanoseconds)
                {
                    hailstoneOnePositionX = hailstoneOne.position.x + (hailstoneOne.velocity.x * nanoseconds);
                    hailstoneOnePositionY = hailstoneOne.position.y + (hailstoneOne.velocity.y * nanoseconds);
                    hailstoneOnePositionZ = hailstoneOne.position.z + (hailstoneOne.velocity.z * nanoseconds);

                    // Pass the hailstones in the order that the rock will hit them in this case.
                    if (PartTwoPathsIntersect(
                        (hailstoneOnePositionX, hailstoneOnePositionY, hailstoneOnePositionZ),
                        (hailstoneTwoPositionX, hailstoneTwoPositionY, hailstoneTwoPositionZ),                        
                        nanoseconds,
                        max_nanoseconds,
                        hailstoneThree,
                        out intersectionVelocity))
                    {
                        hailstoneOneIntersectionTime = nanoseconds;
                        foundVelocity = true;
                        break;
                    }
                }

                if (foundVelocity)
                {
                    break;
                }

                ++max_nanoseconds;
            }

            // We already have velocty by here. Now just use it together with hailstone one's intersection
            // time to get the starting position for the rock.
            (long x, long y, long z) rockStartPosition = (
                hailstoneOnePositionX - (intersectionVelocity.x * hailstoneOneIntersectionTime),
                hailstoneOnePositionY - (intersectionVelocity.y * hailstoneOneIntersectionTime),
                hailstoneOnePositionZ - (intersectionVelocity.z * hailstoneOneIntersectionTime));

            return rockStartPosition.x + rockStartPosition.y + rockStartPosition.z;
        }

        // Requires that 'first' and 'second' hailstone are in the order they get hit, but the order of contact for
        // the third hailstone doesn't matter.
        private bool PartTwoPathsIntersect(
            (long x, long y, long z) firstHailstoneContactPosition,
            (long x, long y, long z) secondHailstoneContactPosition,
            long firstHailstoneContactTime,
            long secondHailstoneContactTime,
            Hailstone thirdHailstone,
            out (long x, long y, long z) velocityNeededForIntersection)
        {
            velocityNeededForIntersection = (0, 0, 0);

            velocityNeededForIntersection.x = (secondHailstoneContactPosition.x - firstHailstoneContactPosition.x) / (secondHailstoneContactTime - firstHailstoneContactTime);
            velocityNeededForIntersection.y = (secondHailstoneContactPosition.y - firstHailstoneContactPosition.y) / (secondHailstoneContactTime - firstHailstoneContactTime);
            velocityNeededForIntersection.z = (secondHailstoneContactPosition.z - firstHailstoneContactPosition.z) / (secondHailstoneContactTime - firstHailstoneContactTime);

            // See if there's an integer velocity that will get the rock between those two points in the time
            if ((Math.Abs(secondHailstoneContactPosition.x - firstHailstoneContactPosition.x) % (secondHailstoneContactTime - firstHailstoneContactTime) == 0) &&
                (Math.Abs(secondHailstoneContactPosition.y - firstHailstoneContactPosition.y) % (secondHailstoneContactTime - firstHailstoneContactTime) == 0) &&
                (Math.Abs(secondHailstoneContactPosition.z - firstHailstoneContactPosition.z) % (secondHailstoneContactTime - firstHailstoneContactTime) == 0))
            {
                // Check for intersection with the third line. Can first find the time at which x intersects, then
                // see if this stacks up for y and z too.
                (long x, long y, long z) hailstoneThreePositionDuringHailstoneOneContact =
                    (thirdHailstone.position.x + (thirdHailstone.velocity.x * firstHailstoneContactTime),
                     thirdHailstone.position.y + (thirdHailstone.velocity.y * firstHailstoneContactTime),
                     thirdHailstone.position.z + (thirdHailstone.velocity.z * firstHailstoneContactTime));
                (long x, long y, long z) rockVelocityRelativeToThirdHailstone =
                    (velocityNeededForIntersection.x - thirdHailstone.velocity.x,
                     velocityNeededForIntersection.y - thirdHailstone.velocity.y,
                     velocityNeededForIntersection.z - thirdHailstone.velocity.z);

                // I will find the contact on the x axis first, and then see if that stacks up on the y and z too.

                // If there's no relative velocity between the rock and third hailstone, they need
                // to already be in contact
                if (rockVelocityRelativeToThirdHailstone.x == 0)
                {
                    return ((rockVelocityRelativeToThirdHailstone.y == 0) &&
                            (rockVelocityRelativeToThirdHailstone.z == 0) &&
                            (thirdHailstone.position.x == firstHailstoneContactPosition.x) &&
                            (thirdHailstone.position.y == firstHailstoneContactPosition.y) &&
                            (thirdHailstone.position.z == firstHailstoneContactPosition.z));
                }

                (long x, long y, long z) thirdHailPosRelToRock =
                    (hailstoneThreePositionDuringHailstoneOneContact.x - firstHailstoneContactPosition.x,
                     hailstoneThreePositionDuringHailstoneOneContact.y - firstHailstoneContactPosition.y,
                     hailstoneThreePositionDuringHailstoneOneContact.z - firstHailstoneContactPosition.z);

                // If the relative velocity of rock to hail is positive, then relative position of hail to
                // rock will decrease over time. And so time to collision is positive if the current relative
                // position is positive, and vice versa. Remember only integer positions.
                // Actually might need same calculation for negative velocity too.
                if (true) //rockVelocityRelativeToThirdHailstone.x > 0)
                {
                    if (thirdHailPosRelToRock.x % rockVelocityRelativeToThirdHailstone.x == 0)
                    {
                        long timeToIntersection = thirdHailPosRelToRock.x / rockVelocityRelativeToThirdHailstone.x;

                        return (
                            (thirdHailPosRelToRock.y % rockVelocityRelativeToThirdHailstone.y == 0) &&
                            (thirdHailPosRelToRock.z % rockVelocityRelativeToThirdHailstone.z == 0) &&
                            ((thirdHailPosRelToRock.y / rockVelocityRelativeToThirdHailstone.y) == timeToIntersection) &&
                            ((thirdHailPosRelToRock.z / rockVelocityRelativeToThirdHailstone.z) == timeToIntersection));


                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
    }
}
