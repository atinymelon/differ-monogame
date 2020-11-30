using System;
using Differ.Shapes;

namespace Differ.Data
{
	public struct RayIntersection
	{
		/** The first ray in the test */
		public Ray Ray1 { get; set; }

		/** The second ray in the test */
		public Ray Ray2 { get; set; }

		/** u value for ray1. */
		public float U1;

		/** u value for ray2. */
		public float U2;
	}
}