using System;
using System.Collections.Generic;
using System.Text;

namespace Bramble.Core
{
    /// <summary>
    /// A 2D integer rectangle class. Similar to Rectangle, but not dependent on System.Drawing
    /// and much more feature-rich.
    /// </summary>
    [Serializable]
    public struct Rect : IEquatable<Rect>, IEnumerable<Vector2D>
    {
        /// <summary>
        /// Gets the empty rectangle.
        /// </summary>
        public readonly static Rect Empty;

        /// <summary>
        /// Creates a new rectangle a single row in height, as wide as the given size.
        /// </summary>
        /// <param name="size">The width of the rectangle.</param>
        /// <returns>The new rectangle.</returns>
        public static Rect Row(int size)
        {
            return new Rect(0, 0, size, 1);
        }

        /// <summary>
        /// Creates a new rectangle a single row in height, as wide as the given size,
        /// starting at the given top-left corner.
        /// </summary>
        /// <param name="x">The left edge of the rectangle.</param>
        /// <param name="y">The top of the rectangle.</param>
        /// <param name="size">The width of the rectangle.</param>
        /// <returns>The new rectangle.</returns>
        public static Rect Row(int x, int y, int size)
        {
            return new Rect(x, y, size, 1);
        }

        /// <summary>
        /// Creates a new rectangle a single row in height, as wide as the given size,
        /// starting at the given top-left corner.
        /// </summary>
        /// <param name="pos">The top-left corner of the rectangle.</param>
        /// <returns>The new rectangle.</returns>
        public static Rect Row(Vector2D pos, int size)
        {
            return new Rect(pos.X, pos.Y, size, 1);
        }

        /// <summary>
        /// Creates a new rectangle a single column in width, as tall as the given size.
        /// </summary>
        /// <param name="size">The height of the rectangle.</param>
        /// <returns>The new rectangle.</returns>
        public static Rect Column(int size)
        {
            return new Rect(0, 0, 1, size);
        }

        /// <summary>
        /// Creates a new rectangle a single column in width, as tall as the given size,
        /// starting at the given top-left corner.
        /// </summary>
        /// <param name="x">The left edge of the rectangle.</param>
        /// <param name="y">The top of the rectangle.</param>
        /// <param name="size">The height of the rectangle.</param>
        /// <returns>The new rectangle.</returns>
        public static Rect Column(int x, int y, int size)
        {
            return new Rect(x, y, 1, size);
        }

        /// <summary>
        /// Creates a new rectangle a single column in width, as tall as the given size,
        /// starting at the given top-left corner.
        /// </summary>
        /// <param name="pos">The top-left corner of the rectangle.</param>
        /// <param name="size">The height of the rectangle.</param>
        /// <returns>The new rectangle.</returns>
        public static Rect Column(Vector2D pos, int size)
        {
            return new Rect(pos.X, pos.Y, 1, size);
        }

        /// <summary>
        /// Creates a new rectangle that is the intersection of the two given rectangles.
        /// </summary>
        /// <example><code>
        /// .----------.
        /// | a        |
        /// | .--------+----.
        /// | | result |  b |
        /// | |        |    |
        /// '-+--------'    |
        ///   |             |
        ///   '-------------'
        /// </code></example>
        /// <param name="a">The first rectangle.</param>
        /// <param name="b">The rectangle to intersect it with.</param>
        /// <returns></returns>
        public static Rect Intersect(Rect a, Rect b)
        {
            int left = Math.Max(a.Left, b.Left);
            int right = Math.Min(a.Right, b.Right);
            int top = Math.Max(a.Top, b.Top);
            int bottom = Math.Min(a.Bottom, b.Bottom);

            int width = Math.Max(0, right - left);
            int height = Math.Max(0, bottom - top);

            return new Rect(left, top, width, height);
        }

        public static Rect CenterIn(Rect toCenter, Rect main)
        {
            Vector2D pos = main.Position + ((main.Size - toCenter.Size) / 2);

            return new Rect(pos, toCenter.Size);
        }

        #region Operators

        public static bool operator ==(Rect r1, Rect r2)
        {
            return r1.Equals(r2);
        }

        public static bool operator !=(Rect r1, Rect r2)
        {
            return !r1.Equals(r2);
        }

        public static Rect operator +(Rect r1, Vector2D v2)
        {
            return new Rect(r1.Position + v2, r1.Size);
        }

        public static Rect operator +(Vector2D v1, Rect r2)
        {
            return new Rect(r2.Position + v1, r2.Size);
        }

        public static Rect operator -(Rect r1, Vector2D v2)
        {
            return new Rect(r1.Position - v2, r1.Size);
        }

        #endregion

        public Vector2D Position { get { return mPos; } }
        public Vector2D Size { get { return mSize; } }

        public int X { get { return mPos.X; } }
        public int Y { get { return mPos.Y; } }
        public int Width { get { return mSize.X; } }
        public int Height { get { return mSize.Y; } }

        public int Left { get { return X; } }
        public int Top { get { return Y; } }
        public int Right { get { return X + Width; } }
        public int Bottom { get { return Y + Height; } }

        public Vector2D TopLeft { get { return new Vector2D(Left, Top); } }
        public Vector2D TopRight { get { return new Vector2D(Right, Top); } }
        public Vector2D BottomLeft { get { return new Vector2D(Left, Bottom); } }
        public Vector2D BottomRight { get { return new Vector2D(Right, Bottom); } }

        public Vector2D Center { get { return new Vector2D((Left + Right) / 2, (Top + Bottom) / 2); } }

        public int Area { get { return mSize.Area; } }

        public Rect(Vector2D pos, Vector2D size)
        {
            mPos = pos;
            mSize = size;
        }

        public Rect(Vector2D size)
            : this(Vector2D.Zero, size)
        {
        }

        public Rect(int x, int y, int width, int height)
            : this(new Vector2D(x, y), new Vector2D(width, height))
        {
        }

        public Rect(Vector2D pos, int width, int height)
            : this(pos, new Vector2D(width, height))
        {
        }

        public Rect(int width, int height)
            : this(new Vector2D(width, height))
        {
        }

        public Rect(int x, int y, Vector2D size)
            : this(new Vector2D(x, y), size)
        {
        }

        public override string ToString()
        {
            return String.Format("({0})-({1})", mPos, mSize);
        }

        public override bool Equals(object obj)
        {
            if (obj is Rect) return Equals((Rect)obj);

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return mPos.GetHashCode() + mSize.GetHashCode();
        }
        
        public Rect Offset(Vector2D pos, Vector2D size)
        {
            return new Rect(mPos + pos, mSize + size);
        }

        public Rect Offset(int x, int y, int width, int height)
        {
            return Offset(new Vector2D(x, y), new Vector2D(width, height));
        }

        public Rect Inflate(int distance)
        {
            return new Rect(mPos.Offset(-distance, -distance), mSize.Offset(distance * 2, distance * 2));
        }

        public bool Contains(Vector2D pos)
        {
            if (pos.X < mPos.X) return false;
            if (pos.X >= mPos.X + mSize.X) return false;
            if (pos.Y < mPos.Y) return false;
            if (pos.Y >= mPos.Y + mSize.Y) return false;

            return true;
        }

        public bool Contains(Rect rect)
        {
            // all sides must be within
            if (rect.Left < Left) return false;
            if (rect.Right > Right) return false;
            if (rect.Top < Top) return false;
            if (rect.Bottom > Bottom) return false;

            return true;
        }

        public bool Overlaps(Rect rect)
        {
            // fail if they do not overlap on any axis
            if (Left > rect.Right) return false;
            if (Right < rect.Left) return false;
            if (Top > rect.Bottom) return false;
            if (Bottom < rect.Top) return false;

            // then they must overlap
            return true;
        }

        public Rect Intersect(Rect rect)
        {
            return Intersect(this, rect);
        }

        public Rect CenterIn(Rect rect)
        {
            return CenterIn(this, rect);
        }

        public IEnumerable<Vector2D> Trace()
        {
            if ((Width > 1) && (Height > 1))
            {
                // trace all four sides
                foreach (Vector2D top in Row(TopLeft, Width - 1)) yield return top;
                foreach (Vector2D right in Column(TopRight.OffsetX(-1), Height - 1)) yield return right;
                foreach (Vector2D bottom in Row(Width - 1)) yield return BottomRight.Offset(-1, -1) - bottom;
                foreach (Vector2D left in Column(Height - 1)) yield return BottomLeft.OffsetY(-1) - left;
            }
            else if ((Width > 1) && (Height == 1))
            {
                // a single row
                foreach (Vector2D pos in Row(TopLeft, Width)) yield return pos;
            }
            else if ((Height >= 1) && (Width == 1))
            {
                // a single column, or one unit
                foreach (Vector2D pos in Column(TopLeft, Height)) yield return pos;
            }

            // otherwise, the rect doesn't have a positive size, so there's nothing to trace
        }

        #region IEquatable<Rect> Members

        public bool Equals(Rect other)
        {
            return mPos.Equals(other.mPos) && mSize.Equals(other.mSize);
        }

        #endregion

        #region IEnumerable<Vec> Members

        public IEnumerator<Vector2D> GetEnumerator()
        {
            if (mSize.X < 0) throw new ArgumentOutOfRangeException("Cannot enumerate a Rectangle with a negative width.");
            if (mSize.Y < 0) throw new ArgumentOutOfRangeException("Cannot enumerate a Rectangle with a negative height.");

            for (int y = mPos.Y; y < mPos.Y + mSize.Y; y++)
            {
                for (int x = mPos.X; x < mPos.X + mSize.X; x++)
                {
                    yield return new Vector2D(x, y);
                }
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private Vector2D mPos;
        private Vector2D mSize;
    }
}
