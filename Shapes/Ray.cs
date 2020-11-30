using Microsoft.Xna.Framework;

namespace Differ.Shapes
{
	public class Ray
	{
		public Vector2 Start;
		public Vector2 End;

		public Vector2 Direction => new Vector2(End.X - Start.X, End.Y - Start.Y);

		public readonly bool IsInfinite;

		public Ray (Vector2 start, Vector2 end, bool isInfinite = true)
		{
			Start = start;
			End = end;
			IsInfinite = isInfinite;
		}
	}
}