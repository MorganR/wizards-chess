using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Chess;

namespace WizardsChess.Movement
{
	// This is a value type, meaning it is passed by value, not by reference.
	// Because it is a small value, this is generally a good thing.
	// It is immutable to avoid confusion when updating values and having to propagate those changes elsewhere.
	// Small Point2Ds reference locations on the soft copy of the board, which range from 0-7 in both dimensions 
	public struct Point2DSmall
	{

		public Point2DSmall(int x, int y)
		{
			X = x;
			Y = y;
#if DEBUG
			parameterCheck();
#endif
		}

		public Point2DSmall(Vector2D vec)
		{
			X = vec.X;
			Y = vec.Y;
#if DEBUG
			parameterCheck();
#endif
		}
		public Point2DSmall(Vector2D vec, Point2DSmall origin)
		{
			X = vec.X + origin.X;
			Y = vec.Y + origin.Y;
#if DEBUG
			parameterCheck();
#endif
		}
		public Point2DSmall(Position p)
		{
			X = p.Column;
			Y = p.Row;
#if DEBUG
			parameterCheck();
#endif
		}
#if DEBUG
		void parameterCheck()
		{
			if (X < lowerBound || X > upperBound)
			{
				System.Diagnostics.Debug.WriteLine("X input of " + X.ToString() + " to Point2D out of range");
				throw new System.ArgumentException("X input of " + X.ToString() + " to Point2D out of range");
			}
			if (Y < lowerBound || Y > upperBound)
			{
				System.Diagnostics.Debug.WriteLine("Y input of " + Y.ToString() + " to Point2D out of range");
				throw new System.ArgumentException("Y input of " + Y.ToString() + " to Point2D out of range");
			}
		}
#endif
		public int X { get; }
		public int Y { get; }

		public override bool Equals(object obj)
		{
			if (obj is Point2DSmall)
			{
				var p = (Point2DSmall)obj;
				return p == this;
			}
			return base.Equals(obj);
		}

		public override string ToString() => $"[{X}, {Y}]";

		public override int GetHashCode() => 37 * Y + X;

		public static Point2DSmall operator -(Point2DSmall p) => new Point2DSmall(-p.X, -p.Y);
		public static Point2DSmall operator +(Point2DSmall origin, Vector2D v) => new Point2DSmall(origin.X + v.X, origin.Y + v.Y);
		public static Point2DSmall operator +(Vector2D v, Point2DSmall origin) => origin + v;
		public static Vector2D operator -(Point2DSmall pA, Point2DSmall pB) => new Vector2D(pA.X - pB.X, pA.Y - pB.Y);
		public static bool operator ==(Point2DSmall pLhs, Point2DSmall pRhs) => pLhs.X == pRhs.X && pLhs.Y == pRhs.Y;
		public static bool operator !=(Point2DSmall pLhs, Point2DSmall pRhs) => !(pLhs == pRhs);
#if DEBUG
		public const int lowerBound = 0;
		public const int upperBound = 7;
#endif
	}
}
