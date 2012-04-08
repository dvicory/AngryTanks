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
        /// Tests if this <see cref="Projection"/> and another <paramref name="otherProjection"/> overlap.
        /// </summary>
        /// <param name="otherProjection">The other <see cref="Projection"/> to test against.</param>
        /// <returns>True if this <see cref="Projection"/> and <paramref name="otherProjection"/> overlap, false otherwise.</returns>
        public bool Overlaps(Projection otherProjection)
        {
            return !(this.Min > otherProjection.Max || otherProjection.Min > this.Max);
        }

        /// <summary>
        /// Gets the overlap between this <see cref="Projection"/> and another <paramref name="otherProjection"/>.
        /// </summary>
        /// <param name="otherProjection">The other <see cref="Projection"/> to test against.</param>
        /// <returns>The amount of overlap with <paramref name="otherProjection"/> (will be negative if no overlap).</returns>
        public Single GetOverlap(Projection otherProjection)
        {
            return Math.Min(otherProjection.Max - this.Min, this.Max - otherProjection.Min);
        }

        /// <summary>
        /// Tests if this <see cref="Projection"/> is lower than another <paramref name="otherProjection"/>.
        /// </summary>
        /// <param name="otherProjection">The other <see cref="Projection"/> to test against.</param>
        /// <returns>True if this <see cref="Projection"/> is lower than <paramref name="otherProjection"/>, false otherwise.</returns>
        public bool LowerOf(Projection otherProjection)
        {
            if ((otherProjection.Max - this.Min) < (this.Max - otherProjection.Min))
                return true;

            return false;
        }

        /// <summary>
        /// Tests if this <see cref="Projection"/> is higher than <paramref name="otherProjection"/>.
        /// </summary>
        /// <param name="otherProjection">The other <see cref="Projection"/> to test against.</param>
        /// <returns>True if this <see cref="Projection"/> is higher than <paramref name="otherProjection"/>, false otherwise.</returns>
        public bool HigherOf(Projection otherProjection)
        {
            return !LowerOf(otherProjection);
        }

        /// <summary>
        /// Tests if <paramref name="otherProjection"/> is contained within this <see cref="Projection"/>.
        /// </summary>
        /// <param name="otherProjection">The other projection to test against.</param>
        /// <returns>True if <paramref name="otherProjection"/> is contained within this <see cref="Projection"/>, otherwise false.</returns>
        public bool Contains(Projection otherProjection)
        {
            return otherProjection.Min > this.Min && otherProjection.Max < this.Max;
        }
    }
}
