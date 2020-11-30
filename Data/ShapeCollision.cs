using System;
using Differ.Shapes;
using Microsoft.Xna.Framework;

namespace Differ.Data
{
	public struct ShapeCollision
	{
		/** The overlap amount */
		public float Overlap;

		/** Separation vector, when subtracted from shape 1 will separate it from shape 2 */
		public Vector2 Separation;

		/** X component of the unit vector, on the axis of the collision (i.e the normal of the face that was collided with) */
		public Vector2 Normal;

        public float OtherOverlap;
        public Vector2 OtherSeparation;
        public Vector2 OtherNormal;

        /** The shape that was tested */
    	public Shape Shape1;
        /** The shape that shape1 was tested against */
    	public Shape Shape2;
	}
}