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
	// Regular Point2Ds are used to indicate location on the physical board, indexed from the center of the board. They range from -11 to 11 and -7 to 7
	public struct Point2D
	{
		public Point2D(int x, int y)
		{
			X = x;
			Y = y;
#if DEBUG
			parameterCheck();
#endif
		}

		public Point2D(Point2DSmall smallPoint)
		{
			X = xOffset + spacing * smallPoint.X;
			Y = xOffset + spacing * smallPoint.Y;
#if DEBUG
			parameterCheck();
#endif
		}

		public Point2D(Vector2D vec)
		{
			X = vec.X;
			Y = vec.Y;
#if DEBUG
			parameterCheck();
#endif
		}
		public Point2D(Vector2D vec, Point2D origin)
		{
			X = vec.X + origin.X;
			Y = vec.Y + origin.Y;
#if DEBUG
			parameterCheck();
#endif
		}
		public Point2D(Position p)
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
				System.Diagnostics.Debug.WriteLine("X input of " + X.ToString() + " to Point2D out of range");
				throw new System.ArgumentException("X input of " + X.ToString() + " to Point2D out of range");
			}
			if (Y < yLowerBound || Y > yUpperBound)
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
			if (obj is Point2D)
			{
				var p = (Point2D)obj;
				return p == this;
			}
			return base.Equals(obj);
		}

		public override string ToString() => $"[{X}, {Y}]";

		public override int GetHashCode() => 37 * Y + X;

		public static Point2D operator -(Point2D p) => new Point2D(-p.X, -p.Y);
		public static Point2D operator +(Point2D origin, Vector2D v) => new Point2D(origin.X + v.X, origin.Y + v.Y);
		public static Point2D operator +(Vector2D v, Point2D origin) => origin + v;
		public static Vector2D operator -(Point2D pA, Point2D pB) => new Vector2D(pA.X - pB.X, pA.Y - pB.Y);
		public static bool operator ==(Point2D pLhs, Point2D pRhs) => pLhs.X == pRhs.X && pLhs.Y == pRhs.Y;
		public static bool operator !=(Point2D pLhs, Point2D pRhs) => !(pLhs == pRhs);

		//constants pointConversion needs
		public const int spacing = 2;    //number of points per square in one dimension
		public const int xOffset = -7;    //x-index of the centre of the left most playing board column
		public const int yOffset = -7;    //y-index of the centre of the bottom row
#if DEBUG
		public const int xLowerBound = -11;
		public const int xUpperBound = 11;
		public const int yLowerBound = -8;
		public const int yUpperBound = 8;
#endif
	}
}
