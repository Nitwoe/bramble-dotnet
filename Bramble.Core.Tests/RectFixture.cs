﻿using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Bramble.Core;

namespace Bramble.Core.Tests
{
    [TestFixture]
    public class RectFixture
    {
        #region Public static properties

        [Test]
        public void TestEmpty()
        {
            Rect r = Rect.Empty;

            Assert.AreEqual(0, r.Position.X);
            Assert.AreEqual(0, r.Position.Y);
            Assert.AreEqual(0, r.Size.X);
            Assert.AreEqual(0, r.Size.Y);
        }

        [Test]
        public void TestRow()
        {
            Rect r = Rect.Row(4);

            Assert.AreEqual(0, r.Position.X);
            Assert.AreEqual(0, r.Position.Y);
            Assert.AreEqual(4, r.Size.X);
            Assert.AreEqual(1, r.Size.Y);
        }

        [Test]
        public void TestRowPos()
        {
            Rect r = Rect.Row(-2, 3, 4);

            Assert.AreEqual(-2, r.Position.X);
            Assert.AreEqual(3, r.Position.Y);
            Assert.AreEqual(4, r.Size.X);
            Assert.AreEqual(1, r.Size.Y);
        }

        [Test]
        public void TestRowVecPos()
        {
            Rect r = Rect.Row(new Vector2D(-2, 3), 4);

            Assert.AreEqual(-2, r.Position.X);
            Assert.AreEqual(3, r.Position.Y);
            Assert.AreEqual(4, r.Size.X);
            Assert.AreEqual(1, r.Size.Y);
        }

        [Test]
        public void TestColumn()
        {
            Rect r = Rect.Column(4);

            Assert.AreEqual(0, r.Position.X);
            Assert.AreEqual(0, r.Position.Y);
            Assert.AreEqual(1, r.Size.X);
            Assert.AreEqual(4, r.Size.Y);
        }

        [Test]
        public void TestColumnPos()
        {
            Rect r = Rect.Column(-2, 3, 4);

            Assert.AreEqual(-2, r.Position.X);
            Assert.AreEqual(3, r.Position.Y);
            Assert.AreEqual(1, r.Size.X);
            Assert.AreEqual(4, r.Size.Y);
        }

        [Test]
        public void TestColumnVecPos()
        {
            Rect r = Rect.Column(new Vector2D(-2, 3), 4);

            Assert.AreEqual(-2, r.Position.X);
            Assert.AreEqual(3, r.Position.Y);
            Assert.AreEqual(1, r.Size.X);
            Assert.AreEqual(4, r.Size.Y);
        }

        #endregion

        #region Constructors

        [Test]
        public void TestConstructorDefault()
        {
            Rect r = new Rect();

            Assert.AreEqual(0, r.Position.X);
            Assert.AreEqual(0, r.Position.Y);
            Assert.AreEqual(0, r.Size.X);
            Assert.AreEqual(0, r.Size.Y);
        }

        [Test]
        public void TestConstructorSizeVec()
        {
            Rect r = new Rect(new Vector2D(3, 5));

            Assert.AreEqual(0, r.Position.X);
            Assert.AreEqual(0, r.Position.Y);
            Assert.AreEqual(3, r.Size.X);
            Assert.AreEqual(5, r.Size.Y);
        }

        [Test]
        public void TestConstructorPositionVecSizeVec()
        {
            Rect r = new Rect(new Vector2D(2, 4), new Vector2D(3, 5));

            Assert.AreEqual(2, r.Position.X);
            Assert.AreEqual(4, r.Position.Y);
            Assert.AreEqual(3, r.Size.X);
            Assert.AreEqual(5, r.Size.Y);
        }

        [Test]
        public void TestConstructorPositionIntSizeInt()
        {
            Rect r = new Rect(2, 4, 3, 5);

            Assert.AreEqual(2, r.Position.X);
            Assert.AreEqual(4, r.Position.Y);
            Assert.AreEqual(3, r.Size.X);
            Assert.AreEqual(5, r.Size.Y);
        }

        [Test]
        public void TestConstructorPositionVecSizeInt()
        {
            Rect r = new Rect(new Vector2D(2, 4), 3, 5);

            Assert.AreEqual(2, r.Position.X);
            Assert.AreEqual(4, r.Position.Y);
            Assert.AreEqual(3, r.Size.X);
            Assert.AreEqual(5, r.Size.Y);
        }

        [Test]
        public void TestConstructorPositionIntSizeVec()
        {
            Rect r = new Rect(2, 4, new Vector2D(3, 5));

            Assert.AreEqual(2, r.Position.X);
            Assert.AreEqual(4, r.Position.Y);
            Assert.AreEqual(3, r.Size.X);
            Assert.AreEqual(5, r.Size.Y);
        }

        #endregion

        #region Enumeration

        [Test]
        public void TestEnumerateNegativeWidth()
        {
            Assert.That(() => TestEnumeration(new Rect(3, 2, -1, 1)), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void TestEnumerateNegativeHeight()
        {
            Assert.That(() => TestEnumeration(new Rect(3, 2, 1, -1)), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void TestEnumerateEmpty()
        {
            TestEnumeration(Rect.Empty);
        }

        [Test]
        public void TestEnumerateZeroWidth()
        {
            TestEnumeration(new Rect(-3, 2, 0, 10));
        }

        [Test]
        public void TestEnumerateZeroHeight()
        {
            TestEnumeration(new Rect(3, -2, 10, 0));
        }

        [Test]
        public void TestEnumerateRow()
        {
            TestEnumeration(Rect.Row(4, 5, 3),
                new Vector2D(4, 5),
                new Vector2D(5, 5),
                new Vector2D(6, 5));
        }

        [Test]
        public void TestEnumerateColumn()
        {
            TestEnumeration(Rect.Column(4, 5, 3),
                new Vector2D(4, 5),
                new Vector2D(4, 6),
                new Vector2D(4, 7));
        }

        [Test]
        public void TestEnumerate()
        {
            TestEnumeration(new Rect(4, 5, 3, 2),
                new Vector2D(4, 5),
                new Vector2D(5, 5),
                new Vector2D(6, 5),
                new Vector2D(4, 6),
                new Vector2D(5, 6),
                new Vector2D(6, 6));
        }

        [Test]
        public void TestEnumerateTrace()
        {
            // tracing a zero-dimension does nothing
            TestEnumeration(new Rect(4, 5, 0, 0).Trace());
            TestEnumeration(new Rect(4, 5, 4, 0).Trace());
            TestEnumeration(new Rect(4, 5, 0, 3).Trace());

            // a single unit
            TestEnumeration(new Rect(4, 5, 1, 1).Trace(),
                new Vector2D(4, 5));

            // a row
            TestEnumeration(new Rect(4, 5, 3, 1).Trace(),
                new Vector2D(4, 5),
                new Vector2D(5, 5),
                new Vector2D(6, 5));

            // a column
            TestEnumeration(new Rect(4, 5, 1, 3).Trace(),
                new Vector2D(4, 5),
                new Vector2D(4, 6),
                new Vector2D(4, 7));

            // a 3x3 square
            TestEnumeration(new Rect(4, 5, 3, 3).Trace(),
                new Vector2D(4, 5),
                new Vector2D(5, 5),
                new Vector2D(6, 5),
                new Vector2D(6, 6),
                new Vector2D(6, 7),
                new Vector2D(5, 7),
                new Vector2D(4, 7),
                new Vector2D(4, 6));
        }

        #endregion

        [Test]
        public void Contains()
        {
            // identical rect is inside
            Assert.IsTrue(new Rect(0, 0, 3, 4).Contains(new Rect(0, 0, 3, 4)));

            // zero size rect can still be inside
            Assert.IsTrue(new Rect(0, 0, 3, 4).Contains(new Rect(1, 2, 0, 0)));

            // outer corners of rect are included
            Assert.IsTrue(new Rect(0, 0, 3, 4).Contains(new Rect(0, 0, 0, 0)));
            Assert.IsTrue(new Rect(0, 0, 3, 4).Contains(new Rect(3, 4, 0, 0)));

            // point must be in
            Assert.IsFalse(new Rect(0, 0, 3, 4).Contains(new Rect(-1, 0, 0, 0)));

            // off left side
            Assert.IsFalse(new Rect(0, 0, 3, 4).Contains(new Rect(-1, 1, 2, 2)));

            // off right side
            Assert.IsFalse(new Rect(0, 0, 3, 4).Contains(new Rect(5, 1, 2, 2)));

            // off top side
            Assert.IsFalse(new Rect(0, 0, 3, 4).Contains(new Rect(1, -3, 2, 2)));

            // off bottom side
            Assert.IsFalse(new Rect(0, 0, 3, 4).Contains(new Rect(1, 5, 2, 2)));

            // completely surrounded
            Assert.IsFalse(new Rect(0, 0, 3, 4).Contains(new Rect(-1, -1, 5, 6)));

            // off two sides
            Assert.IsFalse(new Rect(0, 0, 3, 4).Contains(new Rect(-1, 1, 6, 2)));
        }

        [Test]
        public void TestCoordinates()
        {
            Rect rect = new Rect(1, 2, 3, 4);

            // x, y
            Assert.AreEqual(1, rect.X);
            Assert.AreEqual(2, rect.Y);

            // size
            Assert.AreEqual(3, rect.Width);
            Assert.AreEqual(4, rect.Height);

            // ltrb
            Assert.AreEqual(1, rect.Left);
            Assert.AreEqual(2, rect.Top);
            Assert.AreEqual(1 + 3, rect.Right);
            Assert.AreEqual(2 + 4, rect.Bottom);

            // ltrb vecs
            Assert.AreEqual(new Vector2D(1, 2), rect.TopLeft);
            Assert.AreEqual(new Vector2D(1 + 3, 2), rect.TopRight);
            Assert.AreEqual(new Vector2D(1, 2 + 4), rect.BottomLeft);
            Assert.AreEqual(new Vector2D(1 + 3, 2 + 4), rect.BottomRight);
        }

        private void TestEnumeration(IEnumerable<Vector2D> enumerable, params Vector2D[] expected)
        {
            // build the queue of expected vectors
            Queue<Vector2D> queue = new Queue<Vector2D>();
            foreach (Vector2D pos in expected)
            {
                queue.Enqueue(pos);
            }

            // enumerate and compare
            foreach (Vector2D pos in enumerable)
            {
                Assert.AreEqual(queue.Dequeue(), pos);
            }

            // make sure we got as many as expected
            Assert.AreEqual(0, queue.Count);
        }
    }
}
