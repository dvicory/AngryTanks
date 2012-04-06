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

        private Vector2[] Vertices
        {
            get
            {
                Vector2[] vertices = { UpperLeft, UpperRight, LowerLeft, LowerRight };
                return vertices;
            }
        }

        #endregion

        public RotatedRectangle(Single x, Single y, Single width, Single height, Single rotation)
            : base(x, y, width, height)
        {
            this.Rotation = rotation;

            // Calculate the Rectangle's origin. We assume the center of the Rectangle will
            // be the point that we will be rotating around and we use that for the origin
            Origin = new Vector2(Width / 2, Height / 2);
        }

        public RotatedRectangle(RectangleF rectangle, Single rotation)
            : base(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height)
        {
            this.Rotation = rotation;

            // Calculate the Rectangle's origin. We assume the center of the Rectangle will
            // be the point that we will be rotating around and we use that for the origin
            Origin = new Vector2(Width / 2, Height / 2);
        }

        /// <summary>
        /// Checks to see if a <see cref="RectangleF"/> has collided with a <see cref="RotatedRectangle"/>.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="overlap"></param>
        /// <returns></returns>
        public bool Intersects(RectangleF rectangle, out Single overlap)
        {
            return Intersects(new RotatedRectangle(rectangle, 0), out overlap);
        }

        /// <summary>
        /// Check to see if two <see cref="RotatedRectangle"/>s have collided.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="overlap"></param>
        /// <returns></returns>
        public bool Intersects(RotatedRectangle rectangle, out Single overlap)
        {
            // Calculate the axes we will use to determine if a collision has occurred
            // Since the objects are rectangles, we only have to generate 4 axes (2 for
            // each rectangle) since we know the other 2 on a rectangle are parallel.
            Vector2[] axes =
            {
                UpperRight - UpperLeft,
                UpperRight - LowerRight,
                rectangle.UpperLeft - rectangle.LowerLeft,
                rectangle.UpperLeft - rectangle.UpperRight
            };

            // Cycle through all of the axes we need to check. If a collision does not occur
            // on ALL of the axes, then a collision is NOT occurring. We can then exit out 
            // immediately and notify the calling function that no collision was detected. If
            // a collision DOES occur on ALL of the axes, then there is a collision occurring
            // between the rotated rectangles. We know this to be true by the Seperating Axis Theorem.

            // In addition, overlap is tracked so that the smallest overlap can be stored for the caller.
            Single bestOverlap = Single.MaxValue;
            Single o;

            foreach (Vector2 axis in axes)
            {
                if (!IsAxisCollision(rectangle, axis, out o))
                {
                    // if there is no axis collision, we can guarantee they do not overlap
                    overlap = 0;
                    return false;
                }
                else
                {
                    // do we have the smallest overlap yet?
                    if (o < bestOverlap)
                    {
                        bestOverlap = o;
                    }
                }
            }

            // it is now guaranteed that the rectangles intersect for us to have gotten this far
            overlap = bestOverlap;
            return true;
        }

        /// <summary>
        /// Determines if a collision has occurred on an Axis of one of the
        /// planes parallel to the Rectangle
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="axes"></param>
        /// <returns></returns>
        private bool IsAxisCollision(RotatedRectangle rectangle, Vector2 axis, out Single overlap)
        {
            // project both rectangles onto the axis
            Projection curProj   = this.Project(axis);
            Projection otherProj = rectangle.Project(axis);

            // do the projections overlap?
            if (!curProj.Overlaps(otherProj))
            {
                overlap = 0;
                return false;
            }
            else
            {
                // get the overlap
                overlap = curProj.GetOverlap(otherProj);

                // TODO does this work?
                // check for containment
                if (curProj.Contains(otherProj) || otherProj.Contains(curProj))
                {
                    // get the overlap plus the distance from the minimum end points
                    Single mins = Math.Abs(curProj.Min - otherProj.Min);
                    Single maxs = Math.Abs(curProj.Max - otherProj.Max);

                    // NOTE: depending on which is smalelr you may need to negate the separating axis
                    if (mins < maxs)
                        overlap += mins;
                    else
                        overlap += maxs;
                }

                // and return true for an axis collision
                return true;
            }
        }

        /// <summary>
        /// Projects the <see cref="RotatedRectangle"/> onto an axis.
        /// </summary>
        /// <param name="axis">The axis to project.</param>
        /// <returns>A <see cref="Projection"/> of this <see cref="RotatedRectangle"/> on the given <paramref name="axis"/>.</returns>
        private Projection Project(Vector2 axis)
        {
            // required to get accurate projections
            axis.Normalize();

            Vector2[] vertices = this.Vertices;

            Single min = Vector2.Dot(axis, vertices[0]);
            Single max = min;

            foreach (Vector2 vertice in vertices)
            {
                Single p = Vector2.Dot(axis, vertice);
                if (p < min)
                    min = p;
                else if (p > max)
                    max = p;
            }

            return new Projection(min, max);
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

        public override String ToString()
        {
            return String.Format("{{X:{0} Y:{1} Width:{2} Height:{3} Rotation:{4}}}", X, Y, Width, Height, Rotation);
        }
    }
}
