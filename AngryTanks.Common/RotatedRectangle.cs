using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AngryTanks.Common
{
    /// <summary>
    /// Class that provides for a rotatable rectangle (XNA includes only an axis-aligned Rectangle).
    /// It is used for collision detection.
    /// Based on in part from code by George W. Clingerman.
    /// </summary>
    public class RotatedRectangle
    {
        public Rectangle CollisionRectangle;
        public Single Rotation;
        public Vector2 Origin;

        public RotatedRectangle(Rectangle rectangle, Single rotation)
        {
            this.CollisionRectangle = rectangle;
            this.Rotation = rotation;

            // Calculate the Rectangle's origin. We assume the center of the Rectangle will
            // be the point that we will be rotating around and we use that for the origin
            Origin = new Vector2((int)rectangle.Width / 2, (int)rectangle.Height / 2);
        }

        /// <summary>
        /// Used for changing the X and Y position of the RotatedRectangle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ChangePosition(int x, int y)
        {
            CollisionRectangle.X += x;
            CollisionRectangle.Y += y;
        }

        /// <summary>
        /// This intersects method can be used to check a standard XNA framework Rectangle
        /// object and see if it collides with a Rotated Rectangle object
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public bool Intersects(Rectangle rectangle)
        {
            return Intersects(new RotatedRectangle(rectangle, 0.0f));
        }

        /// <summary>
        /// Check to see if two Rotated Rectangles have collided
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public bool Intersects(RotatedRectangle rectangle)
        {
            // Calculate the Axis we will use to determine if a collision has occurred
            // Since the objects are rectangles, we only have to generate 4 Axis (2 for
            // each rectangle) since we know the other 2 on a rectangle are parallel.
            List<Vector2> rectangleAxis = new List<Vector2>();
            rectangleAxis.Add(UpperRightCorner() - UpperLeftCorner());
            rectangleAxis.Add(UpperRightCorner() - LowerRightCorner());
            rectangleAxis.Add(rectangle.UpperLeftCorner() - rectangle.LowerLeftCorner());
            rectangleAxis.Add(rectangle.UpperLeftCorner() - rectangle.UpperRightCorner());

            // Cycle through all of the Axis we need to check. If a collision does not occur
            // on ALL of the Axis, then a collision is NOT occurring. We can then exit out 
            // immediately and notify the calling function that no collision was detected. If
            // a collision DOES occur on ALL of the Axis, then there is a collision occurring
            // between the rotated rectangles. We know this to be true by the Seperating Axis Theorem
            foreach (Vector2 axis in rectangleAxis)
            {
                if (!IsAxisCollision(rectangle, axis))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines if a collision has occurred on an Axis of one of the
        /// planes parallel to the Rectangle
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        private bool IsAxisCollision(RotatedRectangle rectangle, Vector2 axis)
        {
            // Project the corners of the Rectangle we are checking on to the Axis and
            // get a scalar value of that project we can then use for comparison
            List<int> rectangleAScalars = new List<int>();
            rectangleAScalars.Add(GenerateScalar(rectangle.UpperLeftCorner(), axis));
            rectangleAScalars.Add(GenerateScalar(rectangle.UpperRightCorner(), axis));
            rectangleAScalars.Add(GenerateScalar(rectangle.LowerLeftCorner(), axis));
            rectangleAScalars.Add(GenerateScalar(rectangle.LowerRightCorner(), axis));

            // Project the corners of the current Rectangle on to the Axis and
            // get a scalar value of that project we can then use for comparison
            List<int> rectangleBScalars = new List<int>();
            rectangleBScalars.Add(GenerateScalar(UpperLeftCorner(), axis));
            rectangleBScalars.Add(GenerateScalar(UpperRightCorner(), axis));
            rectangleBScalars.Add(GenerateScalar(LowerLeftCorner(), axis));
            rectangleBScalars.Add(GenerateScalar(LowerRightCorner(), axis));

            // Get the Maximum and Minium Scalar values for each of the Rectangles
            int rectangleAMinimum = rectangleAScalars.Min();
            int rectangleAMaximum = rectangleAScalars.Max();
            int rectangleBMinimum = rectangleBScalars.Min();
            int rectangleBMaximum = rectangleBScalars.Max();

            // If we have overlaps between the Rectangles (i.e. Min of B is less than Max of A)
            // then we are detecting a collision between the rectangles on this Axis
            if (rectangleBMinimum <= rectangleAMaximum && rectangleBMaximum >= rectangleAMaximum)
            {
                return true;
            }
            else if (rectangleAMinimum <= rectangleBMaximum && rectangleAMaximum >= rectangleBMaximum)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Generates a scalar value that can be used to compare where corners of 
        /// a rectangle have been projected onto a particular axis. 
        /// </summary>
        /// <param name="rectangleCorner"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        private int GenerateScalar(Vector2 rectangleCorner, Vector2 axis)
        {
            // Using the formula for Vector projection. Take the corner being passed in
            // and project it onto the given Axis
            Single aNumerator = (rectangleCorner.X * axis.X) + (rectangleCorner.Y * axis.Y);
            Single denominator = (axis.X * axis.X) + (axis.Y * axis.Y);
            Single divisionResult = aNumerator / denominator;
            Vector2 cornerProjected = new Vector2(divisionResult * axis.X, divisionResult * axis.Y);

            // Now that we have our projected Vector, calculate a scalar of that projection
            // that can be used to more easily do comparisons
            Single scalar = (axis.X * cornerProjected.X) + (axis.Y * cornerProjected.Y);
            return (int)scalar;
        }

        /// <summary>
        /// Rotate a point from a given location and adjust using the Origin we
        /// are rotating around
        /// </summary>
        /// <param name="point"></param>
        /// <param name="origin"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        private Vector2 RotatePoint(Vector2 point, Vector2 origin, Single rotation)
        {
            Vector2 translatedPoint = new Vector2();
            translatedPoint.X = (Single)(origin.X + (point.X - origin.X) * Math.Cos(rotation)
                - (point.Y - origin.Y) * Math.Sin(rotation));
            translatedPoint.Y = (Single)(origin.Y + (point.Y - origin.Y) * Math.Cos(rotation)
                + (point.X - origin.X) * Math.Sin(rotation));
            return translatedPoint;
        }

        public Vector2 UpperLeftCorner()
        {
            Vector2 upperLeft = new Vector2(CollisionRectangle.Left, CollisionRectangle.Top);
            upperLeft = RotatePoint(upperLeft, upperLeft + Origin, Rotation);
            return upperLeft;
        }

        public Vector2 UpperRightCorner()
        {
            Vector2 upperRight = new Vector2(CollisionRectangle.Right, CollisionRectangle.Top);
            upperRight = RotatePoint(upperRight, upperRight + new Vector2(-Origin.X, Origin.Y), Rotation);
            return upperRight;
        }

        public Vector2 LowerLeftCorner()
        {
            Vector2 lowerLeft = new Vector2(CollisionRectangle.Left, CollisionRectangle.Bottom);
            lowerLeft = RotatePoint(lowerLeft, lowerLeft + new Vector2(Origin.X, -Origin.Y), Rotation);
            return lowerLeft;
        }

        public Vector2 LowerRightCorner()
        {
            Vector2 lowerRight = new Vector2(CollisionRectangle.Right, CollisionRectangle.Bottom);
            lowerRight = RotatePoint(lowerRight, lowerRight + new Vector2(-Origin.X, -Origin.Y), Rotation);
            return lowerRight;
        }

        public int X
        {
            get { return CollisionRectangle.X; }
        }

        public int Y
        {
            get { return CollisionRectangle.Y; }
        }

        public int Width
        {
            get { return CollisionRectangle.Width; }
        }

        public int Height
        {
            get { return CollisionRectangle.Height; }
        }

    }
}
