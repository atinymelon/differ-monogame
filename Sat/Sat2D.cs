using System.Collections.Generic;
using Differ.Data;
using Differ.Shapes;
using Microsoft.Xna.Framework;
using Ray = Differ.Shapes.Ray;

namespace Differ.Sat
{
    public static class Sat2D
    {
        /** Internal api - test a circle against a polygon */
        public static bool TestCircleVsPolygon(Circle circle, Polygon polygon, out ShapeCollision shapeCollision, bool flip = false)
        {
            shapeCollision = new ShapeCollision();
            var verts = polygon.TransformedVertices;

            float testDistance = float.MaxValue;
            Vector2 closest = Vector2.Zero;

            for (int i = 0; i < verts.Count; i++)
            {
                float distance = (circle.Position - verts[i]).LengthSquared();

                if (distance < testDistance)
                {
                    testDistance = distance;
                    closest = verts[i];
                }
            }

            Vector2 normalAxis = closest - circle.Position;
            normalAxis.Normalize();

            //project all its points, 0 outside the loop
            float test = 0.0f;
            float min1 = Vector2.Dot(normalAxis, verts[0]);
            float max1 = min1;

            for (int j = 1; j < verts.Count; j++)
            {
                test = Vector2.Dot(normalAxis, verts[j]);
                if (test < min1) min1 = test;
                if (test > max1) max1 = test;
            }

            // project the circle
            float max2 = circle.TransformedRadius;
            float min2 = -circle.TransformedRadius;
            float offset = Vector2.Dot(normalAxis, -circle.Position);

            min1 += offset;
            max1 += offset;

            float test1 = min1 - max2;
            float test2 = min2 - max1;

            //if either test is greater than 0, there is a gap, we can give up now.
            if (test1 > 0 || test2 > 0) 
                return false;

            // circle distance check
            float distMin = -(max2 - min1);
            if (flip) distMin *= -1;

            shapeCollision.Overlap = distMin;
            shapeCollision.Normal = normalAxis;
            float closestDist = System.Math.Abs(distMin);

            // find the normal axis for each point and project
            for (int i = 0; i < verts.Count; i++)
            {
                normalAxis = FindNormalAxis(verts, i);
                normalAxis.Normalize();

                // project the polygon(again? yes, circles vs. polygon require more testing...)
                min1 = Vector2.Dot(normalAxis, verts[0]);
                max1 = min1; //set max and min

                //project all the other points(see, cirlces v. polygons use lots of this...)
                for (int j = 1; j < verts.Count; j++)
                {
                    test = Vector2.Dot(normalAxis, verts[j]);
                    if (test < min1) min1 = test;
                    if (test > max1) max1 = test;
                }

                // project the circle(again)
                max2 = circle.TransformedRadius; //max is radius
                min2 = -circle.TransformedRadius; //min is negative radius

                //offset points
                offset = Vector2.Dot(normalAxis, -circle.Position);
                min1 += offset;
                max1 += offset;

                // do the test, again
                test1 = min1 - max2;
                test2 = min2 - max1;

                //failed.. quit now
                if (test1 > 0 || test2 > 0)
                {
                    return false;
                }

                distMin = -(max2 - min1);
                if (flip) distMin *= -1;

                if (System.Math.Abs(distMin) < closestDist)
                {
                    shapeCollision.Normal = normalAxis;
                    shapeCollision.Overlap = distMin;
                    closestDist = System.Math.Abs(distMin);
                }
            } //for

            //if you made it here, there is a collision!!!!!

            if (flip)
            {
                shapeCollision.Shape1 = polygon;
                shapeCollision.Shape2 = circle;
            }
            else
            {
                shapeCollision.Shape1 = circle;
                shapeCollision.Shape2 = polygon;
            }

            shapeCollision.Separation *= shapeCollision.Overlap;

            if (!flip)
            {
                shapeCollision.Normal = -shapeCollision.Normal;
            }

            return true;
        }

        /** Internal api - test a circle against a circle */
        public static bool TestCircleVsCircle(Circle circleA, Circle circleB, out ShapeCollision shapeCollision, bool flip = false)
        {
            shapeCollision = new ShapeCollision();
            Circle circle1 = flip ? circleB : circleA;
            Circle circle2 = flip ? circleA : circleB;

            //add both radii together to get the colliding distance
            float totalRadius = circle1.TransformedRadius + circle2.TransformedRadius;
            //find the distance between the two circles using Pythagorean theorem. No square roots for optimization
            float distancesq = (circle1.Position - circle2.Position).LengthSquared();

            //if your distance is less than the totalRadius square(because distance is squared)
            if (distancesq < totalRadius * totalRadius)
            {
                //find the difference. Square roots are needed here.
                float difference = (float) (totalRadius - System.Math.Sqrt(distancesq));

                shapeCollision.Shape1 = circle1;
                shapeCollision.Shape2 = circle2;

                Vector2 normal = circle1.Position - circle2.Position;
                normal.Normalize();

                shapeCollision.Normal = normal;

                //find the movement needed to separate the circles
                shapeCollision.Separation *= difference;

                //the magnitude of the overlap
                shapeCollision.Overlap = shapeCollision.Separation.Length();

                return true;
            } //if distancesq < r^2

            return false;
        }

        public static bool TestPolygonVsPolygon(Polygon polygon1, Polygon polygon2, out ShapeCollision shapeCollision, bool flip = false)
        {
            if (!CheckPolygons(polygon1, polygon2, out ShapeCollision tmp1, flip)
                || !CheckPolygons(polygon2, polygon1, out ShapeCollision tmp2, flip))
            {
                shapeCollision = new ShapeCollision();
                return false;
            }

            ShapeCollision result;
            ShapeCollision other;

            if (System.Math.Abs(tmp1.Overlap) < System.Math.Abs(tmp2.Overlap))
            {
                result = tmp1;
                other = tmp2;
            }
            else
            {
                result = tmp2;
                other = tmp1;
            }

            result.OtherOverlap = other.Overlap;
            result.Separation = other.Separation;
            result.Normal = other.Normal;
            shapeCollision = result;
            return true;
        } //testPolygonVsPolygon

        /** Internal api - test a ray against a circle */
        public static bool TestRayVsCircle(Ray ray, Circle circle, out RayCollision rayCollision)
        {
            Vector2 delta = ray.End - ray.Start;
            Vector2 ray2Circle = ray.Start - circle.Position;

            float a = delta.LengthSquared();
            float b = 2 * Vector2.Dot(delta, ray2Circle);
            float c = Vector2.Dot(ray2Circle, ray2Circle) - (circle.Radius * circle.Radius);
            float d = b * b - 4 * a * c;

            if (d >= 0)
            {
                d = (float) System.Math.Sqrt(d);

                float t1 = (-b - d) / (2 * a);
                float t2 = (-b + d) / (2 * a);

                if (ray.IsInfinite || (t1 <= 1.0 && t1 >= 0.0))
                {
                    rayCollision = new RayCollision
                    {
                        Shape = circle, 
                        Ray = ray, 
                        Start = t1, 
                        End = t2
                    };
                    return true;
                } //
            } //d >= 0

            rayCollision = new RayCollision();
            return false;
        }

        /** Internal api - test a ray against a polygon */
        public static bool TestRayVsPolygon(Ray ray, Polygon polygon, out RayCollision rayCollision)
        {
            float minU = float.PositiveInfinity;
            float maxU = 0.0f;

            float startX = ray.Start.X;
            float startY = ray.Start.Y;
            float deltaX = ray.End.X - startX;
            float deltaY = ray.End.Y - startY;

            var verts = polygon.TransformedVertices;
            Vector2 v1 = verts[^1];
            Vector2 v2 = verts[0];

            float ud = (v2.Y - v1.Y) * deltaX - (v2.X - v1.X) * deltaY;
            float ua = RayU(ud, startX, startY, v1.X, v1.Y, v2.X - v1.X, v2.Y - v1.Y);
            float ub = RayU(ud, startX, startY, v1.X, v1.Y, deltaX, deltaY);

            if (ud != 0.0 && ub >= 0.0 && ub <= 1.0)
            {
                if (ua < minU) minU = ua;
                if (ua > maxU) maxU = ua;
            }

            for (int i = 1; i < verts.Count; i++)
            {
                v1 = verts[i - 1];
                v2 = verts[i];

                ud = (v2.Y - v1.Y) * deltaX - (v2.X - v1.X) * deltaY;
                ua = RayU(ud, startX, startY, v1.X, v1.Y, v2.X - v1.X, v2.Y - v1.Y);
                ub = RayU(ud, startX, startY, v1.X, v1.Y, deltaX, deltaY);

                if (ud != 0.0 && ub >= 0.0 && ub <= 1.0)
                {
                    if (ua < minU) minU = ua;
                    if (ua > maxU) maxU = ua;
                }
            } //each vert

            if (ray.IsInfinite || (minU <= 1.0 && minU >= 0.0))
            {
                rayCollision = new RayCollision
                {
                    Shape = polygon, 
                    Ray = ray, 
                    Start = minU, 
                    End = maxU
                };
                return true;
            }

            rayCollision = new RayCollision();
            return false;
        }

        /** Internal api - test a ray against another ray */
        public static bool TestRayVsRay(Ray ray1, Ray ray2, out RayIntersection rayIntersection)
        {
            float delta1X = ray1.End.X - ray1.Start.X;
            float delta1Y = ray1.End.Y - ray1.Start.Y;
            float delta2X = ray2.End.X - ray2.Start.X;
            float delta2Y = ray2.End.Y - ray2.Start.Y;
            float diffX = ray1.Start.X - ray2.Start.X;
            float diffY = ray1.Start.Y - ray2.Start.Y;
            float ud = delta2Y * delta1X - delta2X * delta1Y;

            if (ud == 0.0)
            {
                rayIntersection = new RayIntersection();
                return false;
            }

            float u1 = (delta2X * diffY - delta2Y * diffX) / ud;
            float u2 = (delta1X * diffY - delta1Y * diffX) / ud;

            if ((ray1.IsInfinite || u1 > 0.0 && u1 <= 1.0) && (ray2.IsInfinite || u2 > 0.0 && u2 <= 1.0))
            {
                rayIntersection = new RayIntersection
                {
                    Ray1 = ray1, 
                    Ray2 = ray2, 
                    U1 = u1, 
                    U2 = u2
                };
                return true;
            }

            rayIntersection = new RayIntersection();
            return false;
        }

        /** Internal api - implementation details for testPolygonVsPolygon */
        public static bool CheckPolygons(Polygon polygon1, Polygon polygon2, out ShapeCollision shapeCollision, bool flip = false)
        {
            shapeCollision = new ShapeCollision();

            float closest = float.MaxValue;

            var verts1 = polygon1.TransformedVertices;
            var verts2 = polygon2.TransformedVertices;

            // loop to begin projection
            for (int i = 0; i < verts1.Count; i++)
            {
                Vector2 axis = FindNormalAxis(verts1, i);
                axis.Normalize();

                // project polygon1
                float min1 = Vector2.Dot(axis, verts1[0]);
                float max1 = min1;

                float testNum = 0.0f;
                for (int j = 1; i < verts1.Count; i++)
                {
                    testNum = Vector2.Dot(axis, verts1[j]);
                    if (testNum < min1) min1 = testNum;
                    if (testNum > max1) max1 = testNum;
                }

                // project polygon2
                float min2 = Vector2.Dot(axis, verts2[0]);
                float max2 = min2;

                for (int j = 1; i < verts2.Count; i++)
                {
                    testNum = Vector2.Dot(axis, verts2[j]);
                    if (testNum < min2) min2 = testNum;
                    if (testNum > max2) max2 = testNum;
                }

                float test1 = min1 - max2;
                float test2 = min2 - max1;

                if (test1 > 0 || test2 > 0) 
                    return false;

                float distMin = -(max2 - min1);
                if (flip) distMin *= -1;

                if (System.Math.Abs(distMin) < closest)
                {
                    shapeCollision.Normal = axis;
                    shapeCollision.Overlap = distMin;
                    closest = System.Math.Abs(distMin);
                }
            }

            shapeCollision.Shape1 = flip ? polygon2 : polygon1;
            shapeCollision.Shape2 = flip ? polygon1 : polygon2;
            shapeCollision.Separation = -shapeCollision.Normal * shapeCollision.Overlap;

            if (flip)
            {
                shapeCollision.Normal = -shapeCollision.Normal;
            }

            return true;
        }

        /** Internal helper for ray overlaps */
        public static float RayU(float udelta, float aX, float aY, float bX, float bY, float dX, float dY)
        {
            return (dX * (aY - bY) - dY * (aX - bX)) / udelta;
        }

        public static Vector2 FindNormalAxis(IList<Vector2> verts, int index)
        {
            Vector2 v2 = (index >= verts.Count - 1) ? verts[0] : verts[index + 1];
            return -(v2 - verts[index]);
        }
    }
}