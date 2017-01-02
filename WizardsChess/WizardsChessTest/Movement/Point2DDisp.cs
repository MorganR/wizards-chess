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
	// Display Point2Ds are used for displaying points in the path visualizer and range from 0-22 and 0-16
	public struct Point2DDisp
	{
		public Point2DDisp(int x, int y)
		{
			X = x;
			Y = y;
#if DEBUG
			parameterCheck();
#endif
		}

		public Point2DDisp(Point2DSmall smallPoint)
		{
			X = xOffset + spacing * smallPoint.X;
			Y = xOffset + spacing * smallPoint.Y;
#if DEBUG
			parameterCheck();
#endif
		}

		public Point2DDisp(Vector2D vec)
		{
			X = vec.X;
			Y = vec.Y;
#if DEBUG
			parameterCheck();
#endif
		}
		public Point2DDisp(Vector2D vec, Point2DDisp origin)
		{
			X = vec.X + origin.X;
			Y = vec.Y + origin.Y;
#if DEBUG
			parameterCheck();
#endif
		}
		public Point2DDisp(Position p)
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
			if (X < xLowerBound || X > xUpperBound)
			{
				System.Diagnostics.Debug.WriteLine("X input of " + X.ToString() + " to Point2DDisp out of range");
				throw new System.ArgumentException("X input of " + X.ToString() + " to Point2DDisp out of range");
			}
			if (Y < yLowerBound || Y > yUpperBound)
			{
				System.Diagnostics.Debug.WriteLine("Y input of " + Y.ToString() + " to Point2DDisp out of range");
				throw new System.ArgumentException("Y input of " + Y.ToString() + " to Point2DDisp out of range");
			}
		}
#endif

		public int X { get; }
		public int Y { get; }

		public override bool Equals(object obj)
		{
			if (obj is Point2DDisp)
			{
				var p = (Point2DDisp)obj;
				return p == this;
			}
			return base.Equals(obj);
		}

		public override string ToString() => $"[{X}, {Y}]";

		public override int GetHashCode() => 37 * Y + X;

		public static Point2DDisp operator -(Point2DDisp p) => new Point2DDisp(-p.X, -p.Y);
		public static Point2DDisp operator +(Point2DDisp origin, Vector2D v) => new Point2DDisp(origin.X + v.X, origin.Y + v.Y);
		public static Point2DDisp operator +(Vector2D v, Point2DDisp origin) => origin + v;
		public static Vector2D operator -(Point2DDisp pA, Point2DDisp pB) => new Vector2D(pA.X - pB.X, pA.Y - pB.Y);
		public static bool operator ==(Point2DDisp pLhs, Point2DDisp pRhs) => pLhs.X == pRhs.X && pLhs.Y == pRhs.Y;
		public static bool operator !=(Point2DDisp pLhs, Point2DDisp pRhs) => !(pLhs == pRhs);

		//constants pointConversion needs
		public const int spacing = 2;    //number of points per square in one dimension
		public const int xOffset = 5;    //x-index of the centre of the left most playing board column
		public const int yOffset = 1;    //y-index of the centre of the bottom row
#if DEBUG
		public const int xLowerBound = 0;
		public const int xUpperBound = 22;
		public const int yLowerBound = 0;
		public const int yUpperBound = 16;
#endif
	}
}
