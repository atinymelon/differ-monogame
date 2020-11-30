using System;
using Differ.Data;
using Differ.Sat;
using Microsoft.Xna.Framework;

namespace Differ.Shapes
{
	public class Circle : Shape
	{
		public float Radius { get; private set; }
		public float TransformedRadius => Radius * ScaleX;

		public Circle (float x, float y, float radius) : base(x, y)
		{
			Radius = radius;
		}

		public override bool CollidesWith (Shape shape, out ShapeCollision shapeCollision) 
			=> shape.CollidesWithCircle(this, out shapeCollision, true);

		public override bool CollidesWithCircle (Circle circle, out ShapeCollision shapeCollision, bool flip = false) 
			=> Sat2D.TestCircleVsCircle(this, circle, out shapeCollision, flip);

		public override bool CollidesWithPolygon (Polygon polygon, out ShapeCollision shapeCollision, bool flip = false) 
			=> Sat2D.TestCircleVsPolygon( this, polygon, out shapeCollision, flip );

		public override bool IntersectsRay (Ray ray, out RayCollision rayCollision) 
			=> Sat2D.TestRayVsCircle(ray, this, out rayCollision);

		public override bool OverlapsPoint(Vector2 point) 
			=> Vector2.Distance(point, Position) <= Radius;
	}
}