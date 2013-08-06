//
// Contains basic data types used by Clipper and PolygonMath: 
// - Int128 (128-bit integer)
// - PointL (Point made of two longs)
// - RectangleL (Rectangle made of four longs)
// - ExPolygon (a main polygon + holes)
// Note that the Polygon type is simply an alias for List<PointL>,
// and Polygons is simply an alias for List<List<PointL>>.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ClipperLib
{
	using Polygon = List<Vector2>;
	using Polygons = List<List<Vector2>>;

	//------------------------------------------------------------------------------
	// Int128 class (enables safe math on signed 64bit integers)
	// eg Int128 val1((long)9223372036854775807); //ie 2^63 -1
	//    Int128 val2((long)9223372036854775807);
	//    Int128 val3 = val1 * val2;
	//    val3.ToString => "85070591730234615847396907784232501249" (8.5e+37)
	//------------------------------------------------------------------------------

	internal struct Int128
	{
		private long hi;
		private long lo;

		public Int128(long _lo)
		{
			lo = _lo;
			hi = 0;
			if (_lo < 0)
				hi = -1;
		}

		public Int128(Int128 val)
		{
			hi = val.hi;
			lo = val.lo;
		}

		public Int128(long lo, long hi)
		{
			this.lo = lo;
			this.hi = hi;
		}

		public bool IsNegative()
		{
			return hi < 0;
		}

		public static bool operator ==(Int128 val1, Int128 val2)
		{
			if ((object)val1 == (object)val2)
				return true;
			else if ((object)val1 == null || (object)val2 == null)
				return false;
			return (val1.hi == val2.hi && val1.lo == val2.lo);
		}

		public static bool operator !=(Int128 val1, Int128 val2)
		{
			return !(val1 == val2);
		}

		public override bool Equals(System.Object obj)
		{
			if (obj == null || !(obj is Int128))
				return false;
			Int128 i128 = (Int128)obj;
			return (i128.hi == hi && i128.lo == lo);
		}

		public override int GetHashCode()
		{
			return hi.GetHashCode() ^ lo.GetHashCode();
		}

		public static bool operator >(Int128 val1, Int128 val2)
		{
			if (System.Object.ReferenceEquals(val1, val2))
				return false;
			else if (val2 == null)
				return true;
			else if (val1 == null)
				return false;
			else if (val1.hi > val2.hi)
				return true;
			else if (val1.hi < val2.hi)
				return false;
			else
				return (ulong)val1.lo > (ulong)val2.lo;
		}

		public static bool operator <(Int128 val1, Int128 val2)
		{
			if (System.Object.ReferenceEquals(val1, val2))
				return false;
			else if (val2 == null)
				return false;
			else if (val1 == null)
				return true;
			if (val1.hi < val2.hi)
				return true;
			else if (val1.hi > val2.hi)
				return false;
			else
				return (ulong)val1.lo < (ulong)val2.lo;
		}

		public static Int128 operator +(Int128 lhs, Int128 rhs)
		{
			lhs.hi += rhs.hi;
			lhs.lo += rhs.lo;
			if ((ulong)lhs.lo < (ulong)rhs.lo)
				lhs.hi++;
			return lhs;
		}

		public static Int128 operator -(Int128 lhs, Int128 rhs)
		{
			return lhs + -rhs;
		}

		public static Int128 operator -(Int128 val)
		{
			if (val.lo == 0) {
				if (val.hi == 0)
					return val;
				return new Int128(~val.lo, ~val.hi + 1);
			} else {
				return new Int128(~val.lo + 1, ~val.hi);
			}
		}

		//nb: Constructing two new Int128 objects every time we want to multiply longs  
		//is slow. So, although calling the Mul method doesn't look as clean, the 
		//code runs significantly faster than if we'd used the * operator.
		public static Int128 Mul(long lhs, long rhs)
		{
			bool negate = (lhs < 0) != (rhs < 0);
			if (lhs < 0)
				lhs = -lhs;
			if (rhs < 0)
				rhs = -rhs;
			ulong int1Hi = (ulong)lhs >> 32;
			ulong int1Lo = (ulong)lhs & 0xFFFFFFFF;
			ulong int2Hi = (ulong)rhs >> 32;
			ulong int2Lo = (ulong)rhs & 0xFFFFFFFF;

			//nb: see comments in clipper.pas
			ulong a = int1Hi * int2Hi;
			ulong b = int1Lo * int2Lo;
			ulong c = int1Hi * int2Lo + int1Lo * int2Hi;

			long lo, hi;
			hi = (long)(a + (c >> 32));

			lo = (long)(c << 32);
			lo += (long)b;
			if ((ulong)lo < b)
				hi++;
			var result = new Int128(lo, hi);
			return negate ? -result : result;
		}

		public static Int128 operator /(Int128 lhs, Int128 rhs)
		{
			if (rhs.lo == 0 && rhs.hi == 0)
				throw new ArithmeticException("Int128: divide by zero");
			bool negate = (rhs.hi < 0) != (lhs.hi < 0);
			Int128 result = new Int128(lhs), denom = new Int128(rhs);
			if (result.hi < 0)
				result = -result;
			if (denom.hi < 0)
				denom = -denom;
			if (denom > result)
				return new Int128(0); //result is only a fraction of 1
			denom = -denom;

			Int128 p = new Int128(0);
			for (int i = 0; i < 128; ++i) {
				p.hi = p.hi << 1;
				if (p.lo < 0)
					p.hi++;
				p.lo = (long)p.lo << 1;
				if (result.hi < 0)
					p.lo++;
				result.hi = result.hi << 1;
				if (result.lo < 0)
					result.hi++;
				result.lo = (long)result.lo << 1;
				if (p.hi >= 0) {
					p += denom;
					result.lo++;
				}
			}
			if (negate)
				return -result;
			return result;
		}

		public double ToDouble()
		{
			const double shift64 = 18446744073709551616.0; //2^64
			const double bit64 = 9223372036854775808.0;
			if (hi < 0) {
				Int128 tmp = -this;
				if (tmp.lo < 0)
					return (double)tmp.lo - bit64 - tmp.hi * shift64;
				else
					return -(double)tmp.lo - tmp.hi * shift64;
			} else if (lo < 0)
				return -(double)lo + bit64 + hi * shift64;
			else
				return (double)lo + (double)hi * shift64;
		}

		////for bug testing ...
		//public override string ToString()
		//{
		//    int r = 0;
		//    Int128 tmp = new Int128(0), val = new Int128(this);
		//    if (hi < 0) Negate(val);
		//    StringBuilder builder = new StringBuilder(50);
		//    while (val.hi != 0 || val.lo != 0)
		//    {
		//        Div10(val, ref tmp, ref r);
		//        builder.Insert(0, (char)('0' + r));
		//        val.Assign(tmp);
		//    }
		//    if (hi < 0) return '-' + builder.ToString();
		//    if (builder.Length == 0) return "0";
		//    return builder.ToString();
		//}

		////debugging only ...
		//private void Div10(Int128 val, ref Int128 result, ref int remainder)
		//{
		//    remainder = 0;
		//    result = new Int128(0);
		//    for (int i = 63; i >= 0; --i)
		//    {
		//        if ((val.hi & ((long)1 << i)) != 0)
		//            remainder = (remainder * 2) + 1;
		//        else
		//            remainder *= 2;
		//        if (remainder >= 10)
		//        {
		//            result.hi += ((long)1 << i);
		//            remainder -= 10;
		//        }
		//    }
		//    for (int i = 63; i >= 0; --i)
		//    {
		//        if ((val.lo & ((long)1 << i)) != 0)
		//            remainder = (remainder * 2) + 1;
		//        else
		//            remainder *= 2;
		//        if (remainder >= 10)
		//        {
		//            result.lo += ((long)1 << i);
		//            remainder -= 10;
		//        }
		//    }
		//}
	};

	//------------------------------------------------------------------------------
	//------------------------------------------------------------------------------

	public struct PointL
	{
		public long X;
		public long Y;

		public PointL(long X, long Y)
		{
			this.X = X;
			this.Y = Y;
		}

		public PointL(PointL pt)
		{
			this.X = pt.X;
			this.Y = pt.Y;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is PointL))
				return false;
			return (PointL)obj == this;
		}
		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode();
		}
		public static bool operator== (PointL a, PointL b)
		{
			return a.X == b.X && a.Y == b.Y;
		}
		public static bool operator !=(PointL a, PointL b)
		{
			return a.X != b.X || a.Y != b.Y;
		}

		public override string ToString()
		{
			return string.Format("({0},{1})", X, Y);
		}
	}

	public class RectangleL
	{
		public float left { get; set; }
		public float top { get; set; }
		public float right { get; set; }
		public float bottom { get; set; }

		public RectangleL(long l, long t, long r, long b)
		{
			this.left = l;
			this.top = t;
			this.right = r;
			this.bottom = b;
		}

		public RectangleL()
		{
			this.left = 0;
			this.top = 0;
			this.right = 0;
			this.bottom = 0;
		}
	}

	public class ExPolygon
	{
		public Polygon outer;
		public Polygons holes;
	}
}
