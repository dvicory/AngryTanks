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
    public class RotatedRectangle : RectangleF
    {
        #region Properties

        public Single Rotation;
        public Vector2 Origin;

        public Vector2 UpperLeft
        {
            get
            {
                Vector2 upperLeft = new Vector2(Left, Top);
                return RotatePoint(upperLeft, upperLeft + Origin, Rotation);
            }
        }

        public Vector2 UpperRight
        {
            get
            {
                Vector2 upperRight = new Vector2(Right, Top);
                return RotatePoint(upperRight, upperRight + new Vector2(-Origin.X, Origin.Y), Rotation);
            }
        }

        public Vector2 LowerLeft
        {
            get
            {
                Vector2 lowerLeft = new Vector2(Left, Bottom);
                return RotatePoint(lowerLeft, lowerLeft + new Vector2(Origin.X, -Origin.Y), Rotation);
            }
        }

        public Vector2 LowerRight
        {
            get
            {
                Vector2 lowerRight = new Vector2(Right, Bottom);
                return RotatePoint(lowerRight, lowerRight + new Vector2(-Origin.X, -Origin.Y), Rotation);
            }
        }

        #endregion

        public RotatedRectangle(Single x, Single y, Single width, Single height, Single rotation)
            : base(x, y, width, height)
        {
            this.Rotation = rotation;

            // Calculate the Rectangles origin. We assume the center of the Rectangle will
            // be the point that we will be rotating around and we use that for the origin
            Origin = new Vector2(Width / 2, Height / 2);
        }

        public RotatedRectangle(RectangleF rectangle, Single rotation)
            : base(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height)
        {
            this.Rotation = rotation;

            // Calculate the Rectangles origin. We assume the center of the Rectangle will
            // be the point that we will be rotating around and we use that for the origin
            Origin = new Vector2(Width / 2, Height / 2);
        }

        /// <summary>
        /// This intersects method can be used to check a standard XNA framework Rectangle
        /// object and see if it collides with a Rotated Rectangle object
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public bool Intersects(RectangleF rectangle)
        {
            return Intersects(new RotatedRectangle(rectangle, 0));
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
            Vector2[] rectangleAxis =
            {
                UpperRight - UpperLeft,
                UpperRight - LowerRight,
                rectangle.UpperLeft - rectangle.LowerLeft,
                rectangle.UpperLeft - rectangle.UpperRight
            };

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
            Single[] otherRectangleScalars =
            {
                GenerateScalar(rectangle.UpperLeft, axis),
                GenerateScalar(rectangle.UpperRight, axis),
                GenerateScalar(rectangle.LowerLeft, axis),
                GenerateScalar(rectangle.LowerRight, axis)
            };

            // Project the corners of the current Rectangle on to the Axis and
            // get a scalar value of that project we can then use for comparison
            Single[] curRectangleScalars =
            {
                GenerateScalar(UpperLeft, axis),
                GenerateScalar(UpperRight, axis),
                GenerateScalar(LowerLeft, axis),
                GenerateScalar(LowerRight, axis)
            };

            // Get the Maximum and Minimum Scalar values for each of the Rectangles
            Single otherRectangleMinimum = otherRectangleScalars.Min();
            Single otherRectangleMaximum = otherRectangleScalars.Max();
            Single curRectangleMinimum   = curRectangleScalars.Min();
            Single curRectangleMaximum   = curRectangleScalars.Max();

            // If we have overlaps between the Rectangles (i.e. Min of B is less than Max of A)
            // then we are detecting a collision between the rectangles on this Axis
            if (curRectangleMinimum <= otherRectangleMaximum && curRectangleMaximum >= otherRectangleMaximum)
            {
                return true;
            }
            else if (otherRectangleMinimum <= curRectangleMaximum && otherRectangleMaximum >= curRectangleMaximum)
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
        private Single GenerateScalar(Vector2 rectangleCorner, Vector2 axis)
        {
            // Take the corner being passed in and project it onto the given Axis
            Vector2 cornerProjected = Vector2.Reflect(rectangleCorner, axis);

            // Now that we have our projected Vector, calculate a scalar of that projection
            // that can be used to more easily do comparisons
            return Vector2.Dot(axis, cornerProjected);
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
            return Vector2.Transform(point,
                                     Matrix.CreateTranslation(new Vector3(-origin, 0)) *
                                     Matrix.CreateRotationZ(rotation) *
                                     Matrix.CreateTranslation(new Vector3(origin, 0)));
        }
    }
}
