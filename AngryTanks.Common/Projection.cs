using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngryTanks.Common
{
    public struct Projection
    {
        public readonly Single Min;
        public readonly Single Max;

        /// <summary>
        /// Creates a new <see cref="Projection"/>.
        /// </summary>
        /// <param name="min">The minimum value of the projection.</param>
        /// <param name="max">The maximum value of the projection.</param>
        public Projection(Single min, Single max)
        {
            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        /// Tests if this projection and another <see cref="Projection"/> overlap.
        /// </summary>
        /// <param name="projection">The other projection to test against.</param>
        /// <returns>True if this <see cref="Projection"/> and <paramref name="projection"/> overlap, false otherwise.</returns>
        public bool Overlaps(Projection projection)
        {
            return !(this.Min > projection.Max || projection.Min > this.Max);
        }

        /// <summary>
        /// Gets the overlap between this projection and another <see cref="Projection"/>.
        /// </summary>
        /// <param name="projection">The other projection to test against.</param>
        /// <returns>The amount of overlap with <paramref name="projection"/> (will be negative if no overlap).</returns>
        public Single GetOverlap(Projection projection)
        {
            return Math.Min(projection.Max - this.Min, this.Max - projection.Min);
        }

        /// <summary>
        /// Tests if another <see cref="Projection"/> is contained within this projection.
        /// </summary>
        /// <param name="projection">The other projection to test against.</param>
        /// <returns>True if <paramref name="projection"/> is contained within this <see cref="Projection"/>, otherwise false.</returns>
        public bool Contains(Projection projection)
        {
            return projection.Min > this.Min && projection.Max < this.Max;
        }
    }
}
