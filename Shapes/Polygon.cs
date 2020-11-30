using System;
using System.Collections.Generic;
using Differ.Data;
using Differ.Sat;
using Microsoft.Xna.Framework;

namespace Differ.Shapes
{
    public class Polygon : Shape
    {
        public List<Vector2> Vertices { get; set; }

        /** The transformed (rotated/scale) vertices cache */
        public List<Vector2> TransformedVertices
        {
            get
            {
                if (transformed) 
                    return transformedVertices;
                
                transformedVertices = new List<Vector2>();
                transformed = true;
                
                for (int i = 0; i < Vertices.Count; i++)
                {
                    transformedVertices.Add(Vector2.Transform(Vertices[i], transformMatrix));
                }

                return transformedVertices;
            }
        }
        
        private List<Vector2> transformedVertices;

        public Polygon(float x, float y, List<Vector2> vertices) : base(x, y)
        {
            transformedVertices = new List<Vector2>();
            Vertices = vertices;
        }

        public override bool CollidesWith(Shape shape, out ShapeCollision shapeCollision) 
            => shape.CollidesWithPolygon(this, out shapeCollision, true);

        public override bool CollidesWithCircle(Circle circle, out ShapeCollision shapeCollision, bool flip = false) 
            => Sat2D.TestCircleVsPolygon(circle, this, out shapeCollision, !flip);

        public override bool CollidesWithPolygon(Polygon polygon, out ShapeCollision shapeCollision, bool flip = false) 
            => Sat2D.TestPolygonVsPolygon(this, polygon, out shapeCollision, flip);

        public override bool IntersectsRay(Ray ray, out RayCollision rayCollision) 
            => Sat2D.TestRayVsPolygon(ray, this, out rayCollision);

        public override bool OverlapsPoint(Vector2 point)
        {
            int sides = TransformedVertices.Count;
            var verts = TransformedVertices;

            int j = sides - 1;
            bool oddNodes = false;
            
            for (int i = 0; i < sides; i++)
            {
                if ((verts[i].Y < point.Y && verts[j].Y >= point.Y)
                    || (verts[j].Y < point.Y && verts[i].Y >= point.Y))
                {
                    if (verts[i].X +
                        (point.Y - verts[i].Y) /
                        (verts[j].Y - verts[i].Y) *
                        (verts[j].X - verts[i].X) < point.X)
                    {
                        oddNodes = !oddNodes;
                    }
                }

                j = i;
            }

            return oddNodes;
        }

        /** Helper to create an Ngon at x,y with given number of sides, and radius.
            A default radius of 100 if unspecified. Returns a ready made `Polygon` collision `Shape` */
        public static Polygon Create(float x, float y, int sides, float radius)
        {
            if (sides < 3)
            {
                throw new ArgumentException("A polygon must have a least 3 sides.");
            }

            float rotation = (float) (Math.PI * 2) / sides;
            var vertices = new List<Vector2>();

            for (int i = 0; i < sides; i++)
            {
                float angle = (float) (i * rotation + (Math.PI - rotation) * 0.5);
                var vector = new Vector2
                {
                    X = (float) Math.Cos(angle) * radius, 
                    Y = (float) Math.Sin(angle) * radius
                };
                vertices.Add(vector);
            }

            return new Polygon(x, y, vertices);
        }

        /** Helper generate a rectangle at x,y with a given width/height and centered state.
            Centered by default. Returns a ready made `Polygon` collision `Shape` */
        public static Polygon Rectangle(float x, float y, float width, float height, bool centered = true)
        {
            var vertices = new List<Vector2>();

            if (centered)
            {
                vertices.Add(new Vector2(-width / 2, -height / 2));
                vertices.Add(new Vector2(width / 2, -height / 2));
                vertices.Add(new Vector2(width / 2, height / 2));
                vertices.Add(new Vector2(-width / 2, height / 2));
            }
            else
            {
                vertices.Add(new Vector2(0, 0));
                vertices.Add(new Vector2(width, 0));
                vertices.Add(new Vector2(width, height));
                vertices.Add(new Vector2(0, height));
            }

            return new Polygon(x, y, vertices);
        }

        /** Helper generate a square at x,y with a given width/height with given centered state.
            Centered by default. Returns a ready made `Polygon` collision `Shape` */
        public static Polygon Square(float x, float y, float width, bool centered = true) 
            => Rectangle(x, y, width, width, centered);

        /** Helper generate a triangle at x,y with a given radius.
            Returns a ready made `Polygon` collision `Shape` */
        public static Polygon Triangle(float x, float y, float radius) 
            => Create(x, y, 3, radius);
    }
}