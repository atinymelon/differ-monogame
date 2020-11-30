using System.Collections.Generic;
using Differ.Data;
using Microsoft.Xna.Framework;

namespace Differ.Shapes
{
    public abstract class Shape
    {
        /** The state of this shape, if inactive can be ignored in results */
        public bool Active = true;

        /** A generic data object where you can store anything you want, for later use */
        public object Data;

        /** The position of this shape */
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                RefreshTransform();
            }
        }

        /** The x position of this shape */
        public float X
        {
            get => position.X;
            set
            {
                position.X = value;
                RefreshTransform();
            }
        }

        /** The y position of this shape */
        public float Y
        {
            get => position.Y;
            set
            {
                position.Y = value;
                RefreshTransform();
            }
        }

        /** The rotation of this shape, in degrees */
        public float Rotation
        {
            get => rotation;
            set
            {
                RefreshTransform();
                rotation = value;
            }
        }

        /** The scale in the x direction of this shape */
        public float ScaleX
        {
            get => scaleX;

            set
            {
                scaleX = value;
                RefreshTransform();
            }
        }

        /** The scale in the y direction of this shape */
        public float ScaleY
        {
            get => scaleY;

            set
            {
                scaleY = value;
                RefreshTransform();
            }
        }

        private Vector2 position;
        private float rotation = 0;

        private float scaleX;
        private float scaleY;

        protected bool transformed = false;
        protected Matrix transformMatrix;

        public Shape(float x, float y)
        {
            position = new Vector2(x, y);
            rotation = 0;

            scaleX = 1;
            scaleY = 1;

            transformMatrix = new Matrix();
            Matrix.CreateTranslation(x, y, 0);
        }

        /** Test this shape against another shape. */
        public abstract bool CollidesWith(Shape shape, out ShapeCollision shapeCollision);

        /** Test this shape against a circle. */
        public abstract bool CollidesWithCircle(Circle circle, out ShapeCollision shapeCollision, bool flip = false);

        /** Test this shape against a polygon. */
        public abstract bool CollidesWithPolygon(Polygon polygon, out ShapeCollision shapeCollision, bool flip = false);

        /** Test this shape against a ray. */
        public abstract bool IntersectsRay(Ray ray, out RayCollision rayCollision);

        public abstract bool OverlapsPoint(Vector2 point);

        private void RefreshTransform()
        {
            transformMatrix = Matrix.CreateTranslation(position.X, position.Y, 0)
                              * Matrix.CreateRotationZ(rotation)
                              * Matrix.CreateScale(scaleX, scaleY, 1f);
            transformed = false;
        }
    }
}