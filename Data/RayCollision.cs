using System;
using Differ.Shapes;

namespace Differ.Data
{
    public struct RayCollision
    {
        /** Shape the intersection was with. */
        public Shape Shape { get; set; }

        /** The ray involved in the intersection. */
        public Ray Ray { get; set; }

        /** Distance along ray that the intersection start at. */
        public float Start { get; set; }

        /** Distance along ray that the intersection ended at. */
        public float End { get; set; }
    }
}

// Need helper extensions later