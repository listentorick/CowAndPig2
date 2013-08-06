using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ClipperLib
{
	using Polygon = List<Vector2>;
	using Polygons = List<List<Vector2>>;

	public enum JoinType { Square, Round, Miter };

	/// <summary>
	/// Contains three useful polygon algorithms: Area, Orientation, and 
	/// OffsetPolygons.
	/// </summary>
	/// <remarks>
	/// Clipper does not use PolygonMath, but PolygonMath.OffsetPolygons uses
	/// Clipper to "clean up" the results of the offset operation.
	/// </remarks>
	public static class PolygonMath
	{
		internal const long LoRange = 1518500249;           //sqrt(2^63 -1)/2
		internal const long HiRange = 6521908912666391106L; //sqrt(2^127 -1)/2

		internal enum RangeTest { Lo, Hi, Error };
		
		#region Area
		
		
		/*
		 * float s = ploygon[0].y*(ploygon[ploygon.length-1].x-ploygon[1].x);
        
		for(int i=1;i<ploygon.length;i++)
        {
            s += ploygon[i].y*(ploygon[i-1].x - ploygon[(i+1)%ploygon.length].x);
        }
        return Mathf.Abs(s/2);*/
		
		
		
	///http://www.mathopenref.com/coordpolygonarea2.html	
	public static float Area(Polygon polygon) 
    {
			
		float area = 0;
		int j = polygon.Count -1;
			
		for (int i=0; i<polygon.Count; i++) { 
			area = area +  (polygon[j].x + polygon[i].x) * (polygon[j].y-polygon[i].y); 
      		j = i;  //j is previous vertex to i
    	}
        return Mathf.Abs(area/2);
    }
		
		
	
		
		/// <summary>Computes the area of a polygon.</summary>
		/// <returns>The area, which is always a multiple of 0.5 because the
		/// input coordinates are integers. The result is negative if the polygon 
		/// is clockwise (assuming a coordinate system in which increasing Y goes 
		/// upward), or positive if the polygon is counterclockwise.</returns>
		/// 
		/// 
		/// 
		/// 
		/// 
		/*
		public static double Area(Polygon poly)
		{
			int highI = poly.Count - 1;
			if (highI < 2)
				return 0;
			bool UseFullInt64Range = false;
			RangeTest rt = TestRange(poly);
			switch (rt) {
				case RangeTest.Hi:
					UseFullInt64Range = true;
					break;
				case RangeTest.Error:
					throw new ArgumentException("Polygon coordinate is too large to process.");
			}
			if (UseFullInt64Range) {
				Int128 a = new Int128(0);
				a = Int128.Mul(poly[highI].x, poly[0].y) -
					Int128.Mul(poly[0].x, poly[highI].y);
				for (int i = 0; i < highI; ++i)
					a += Int128.Mul(poly[i].x, poly[i + 1].y) -
					Int128.Mul(poly[i + 1].x, poly[i].y);
				return a.ToDouble() / 2;
			} else {
				double area = (double)poly[highI].x * (double)poly[0].y -
					(double)poly[0].x * (double)poly[highI].y;
				for (int i = 0; i < highI; ++i)
					area += (double)poly[i].x * (double)poly[i + 1].y -
						(double)poly[i + 1].x * (double)poly[i].y;
				return area / 2;
			}
		}*/

		private static RangeTest TestRange(Polygon pts)
		{
			RangeTest result = RangeTest.Lo;
			for (int i = 0; i < pts.Count; i++) {
				if (Math.Abs(pts[i].x) > HiRange || Math.Abs(pts[i].y) > HiRange)
					return RangeTest.Error;
				else if (Math.Abs(pts[i].x) > LoRange || Math.Abs(pts[i].y) > LoRange)
					result = RangeTest.Hi;
			}
			return result;
		}

		#endregion
		
		#region Orientation
		
		/// <summary>Computes the orientation of a polygon.</summary>
		/// <returns>A return value of "true" represents a clockwise polygon if 
		/// you are using a "mathematical" coordinate system in which increasing 
		/// Y coordinates move upward, so "false" represents a counterclockwise
		/// polygon. However, if you are using a text-oriented coordinate system 
		/// in which increasing Y coordinates move downward (in that case, (0,0) is 
		/// typically the top-left corner), then "true" represents a 
		/// counterclockwise polygon.</returns>
		public static bool Orientation(Polygon poly)
		{
			int highI = poly.Count - 1;
			if (highI < 2)
				return false;
			bool UseFullInt64Range = false;

			int j = 0, jplus, jminus;
			for (int i = 0; i <= highI; ++i) {
				if (Math.Abs(poly[i].x) > HiRange || Math.Abs(poly[i].y) > HiRange)
					throw new ArgumentException("Polygon coordinate is too large to process.");
				if (Math.Abs(poly[i].x) > LoRange || Math.Abs(poly[i].y) > LoRange)
					UseFullInt64Range = true;
				if (poly[i].y < poly[j].y)
					continue;
				if ((poly[i].y > poly[j].y || poly[i].x < poly[j].x))
					j = i;
			};
			if (j == highI)
				jplus = 0;
			else
				jplus = j + 1;
			if (j == 0)
				jminus = highI;
			else
				jminus = j - 1;

			//get cross product of vectors of the edges adjacent to highest point ...
			Vector2 vec1 = new Vector2(poly[j].x - poly[jminus].x, poly[j].y - poly[jminus].y);
			Vector2 vec2 = new Vector2(poly[jplus].x - poly[j].x, poly[jplus].y - poly[j].y);

			if (UseFullInt64Range) {
				float cross = ((vec1.x * vec2.y)) - ((vec2.x * vec1.y));
				return cross<0;
			} else {
				return (vec1.x * vec2.y - vec2.x * vec1.y) < 0;
			}
		}
 
		#endregion

		#region OffsetPolygons

		internal static long Round(double value)
		{
			if ((value < 0))
				return (long)(value - 0.5);
			else
				return (long)(value + 0.5);
		}

		internal struct DoublePoint
		{
			public readonly double x;
			public readonly double y;
			public DoublePoint(double x = 0, double y = 0)
			{
				this.x = x;
				this.y = y;
			}
		};

		private class PolyOffsetBuilder
		{
			private Polygons pts;
			private Polygon currentPoly;
			private List<DoublePoint> normals;
			private double delta, m_R;
			private int m_i, m_j, m_k;
			private const int buffLength = 128;

			public PolyOffsetBuilder(Polygons pts, Polygons solution, double delta, JoinType jointype, double MiterLimit)
			{
				//precondtion: solution != pts

				if (delta == 0) {
					solution = pts;
					return;
				}

				this.pts = pts;
				this.delta = delta;
				if (MiterLimit <= 1)
					MiterLimit = 1;
				double RMin = 2 / (MiterLimit * MiterLimit);

				normals = new List<DoublePoint>();

				double deltaSq = delta * delta;
				solution.Clear();
				solution.Capacity = pts.Count;
				for (m_i = 0; m_i < pts.Count; m_i++) {
					int len = pts[m_i].Count;
					if (len > 1 && pts[m_i][0].x == pts[m_i][len - 1].x &&
						pts[m_i][0].y == pts[m_i][len - 1].y)
						len--;

					if (len == 0 || (len < 3 && delta <= 0))
						continue;
					else if (len == 1) {
						Polygon arc;
						arc = BuildArc(pts[m_i][len - 1], 0, 2 * Math.PI, delta);
						solution.Add(arc);
						continue;
					}

					//build normals ...
					normals.Clear();
					normals.Capacity = len;
					for (int j = 0; j < len - 1; ++j)
						normals.Add(GetUnitNormal(pts[m_i][j], pts[m_i][j + 1]));
					normals.Add(GetUnitNormal(pts[m_i][len - 1], pts[m_i][0]));

					currentPoly = new Polygon();
					m_k = len - 1;
					for (m_j = 0; m_j < len; ++m_j) {
						switch (jointype) {
							case JoinType.Miter: {
									m_R = 1 + (normals[m_j].x * normals[m_k].x +
										normals[m_j].y * normals[m_k].y);
									if (m_R >= RMin)
										DoMiter();
									else
										DoSquare(MiterLimit);
									break;
								}
							case JoinType.Round:
								DoRound();
								break;
							case JoinType.Square:
								DoSquare(1);
								break;
						}
						m_k = m_j;
					}
					solution.Add(currentPoly);
				}

				//finally, clean up untidy corners ...
				CleanUpSolution(solution, delta);
			}

			private static void CleanUpSolution(Polygons solution, double delta)
			{
				Clipper clpr = new Clipper();
				clpr.AddPolygons(solution, PolyType.Subject);
				if (delta > 0) {
					clpr.Execute(ClipType.Union, solution, PolyFillType.Positive, PolyFillType.Positive);
				} else {
					RectangleL r = clpr.GetBounds();
					Polygon outer = new Polygon(4);

					outer.Add(new Vector2(r.left - 10, r.bottom + 10));
					outer.Add(new Vector2(r.right + 10, r.bottom + 10));
					outer.Add(new Vector2(r.right + 10, r.top - 10));
					outer.Add(new Vector2(r.left - 10, r.top - 10));

					clpr.AddPolygon(outer, PolyType.Subject);
					clpr.Execute(ClipType.Union, solution, PolyFillType.Negative, PolyFillType.Negative);
					if (solution.Count > 0) {
						solution.RemoveAt(0);
						for (int i = 0; i < solution.Count; i++)
							solution[i].Reverse();
					}
				}
			}
			//------------------------------------------------------------------------------

			internal void AddPoint(Vector2 pt)
			{
				int len = currentPoly.Count;
				if (len == currentPoly.Capacity)
					currentPoly.Capacity = len + buffLength;
				currentPoly.Add(pt);
			}
			//------------------------------------------------------------------------------

			internal void DoSquare(double mul)
			{
				Vector2 pt1 = new Vector2((long)Round(pts[m_i][m_j].x + normals[m_k].x * delta),
					(long)Round(pts[m_i][m_j].y + normals[m_k].y * delta));
				Vector2 pt2 = new Vector2((long)Round(pts[m_i][m_j].x + normals[m_j].x * delta),
					(long)Round(pts[m_i][m_j].y + normals[m_j].y * delta));
				if ((normals[m_k].x * normals[m_j].y - normals[m_j].x * normals[m_k].y) * delta >= 0) {
					double a1 = Math.Atan2(normals[m_k].y, normals[m_k].x);
					double a2 = Math.Atan2(-normals[m_j].y, -normals[m_j].x);
					a1 = Math.Abs(a2 - a1);
					if (a1 > Math.PI)
						a1 = Math.PI * 2 - a1;
					double dx = Math.Tan((Math.PI - a1) / 4) * Math.Abs(delta * mul);
					pt1 = new Vector2((long)(pt1.x - normals[m_k].y * dx),
						(long)(pt1.y + normals[m_k].x * dx));
					AddPoint(pt1);
					pt2 = new Vector2((long)(pt2.x + normals[m_j].y * dx),
						(long)(pt2.y - normals[m_j].x * dx));
					AddPoint(pt2);
				} else {
					AddPoint(pt1);
					AddPoint(pts[m_i][m_j]);
					AddPoint(pt2);
				}
			}
			//------------------------------------------------------------------------------

			internal void DoMiter()
			{
				if ((normals[m_k].x * normals[m_j].y - normals[m_j].x * normals[m_k].y) * delta >= 0) {
					double q = delta / m_R;
					AddPoint(new Vector2((long)Round(pts[m_i][m_j].x +
						(normals[m_k].x + normals[m_j].x) * q),
						(long)Round(pts[m_i][m_j].y + (normals[m_k].y + normals[m_j].y) * q)));
				} else {
					Vector2 pt1 = new Vector2((long)Round(pts[m_i][m_j].x + normals[m_k].x * delta),
						(long)Round(pts[m_i][m_j].y + normals[m_k].y * delta));
					Vector2 pt2 = new Vector2((long)Round(pts[m_i][m_j].x + normals[m_j].x * delta),
						(long)Round(pts[m_i][m_j].y + normals[m_j].y * delta));
					AddPoint(pt1);
					AddPoint(pts[m_i][m_j]);
					AddPoint(pt2);
				}
			}
			//------------------------------------------------------------------------------

			internal void DoRound()
			{
				Vector2 pt1 = new Vector2(Round(pts[m_i][m_j].x + normals[m_k].x * delta),
					Round(pts[m_i][m_j].y + normals[m_k].y * delta));
				Vector2 pt2 = new Vector2(Round(pts[m_i][m_j].x + normals[m_j].x * delta),
					Round(pts[m_i][m_j].y + normals[m_j].y * delta));
				AddPoint(pt1);
				//round off reflex angles (ie > 180 deg) unless almost flat (ie < 10deg).
				//cross product normals < 0 . angle > 180 deg.
				//dot product normals == 1 . no angle
				if ((normals[m_k].x * normals[m_j].y - normals[m_j].x * normals[m_k].y) * delta >= 0) {
					if ((normals[m_j].x * normals[m_k].x + normals[m_j].y * normals[m_k].y) < 0.985) {
						double a1 = Math.Atan2(normals[m_k].y, normals[m_k].x);
						double a2 = Math.Atan2(normals[m_j].y, normals[m_j].x);
						if (delta > 0 && a2 < a1)
							a2 += Math.PI * 2;
						else if (delta < 0 && a2 > a1)
							a2 -= Math.PI * 2;
						Polygon arc = BuildArc(pts[m_i][m_j], a1, a2, delta);
						for (int m = 0; m < arc.Count; m++)
							AddPoint(arc[m]);
					}
				} else
					AddPoint(pts[m_i][m_j]);
				AddPoint(pt2);
			}
			//------------------------------------------------------------------------------

		} //end PolyOffsetBuilder

		/// <summary>Encapsulates the algorithm for computing the offsets of a 
		/// polygon. You can either add a positive offset (creating a buffer zone 
		/// around the input) or a negative one (shrinking the input polygons).</summary>
		public static Polygons OffsetPolygons(Polygons poly, double delta,
			JoinType jointype, double MiterLimit)
		{
			Polygons result = new Polygons(poly.Count);
			new PolyOffsetBuilder(poly, result, delta, jointype, MiterLimit);
			return result;
		}

		/// <summary>Encapsulates the algorithm for computing the offsets of a 
		/// polygon. You can either add a positive offset (creating a buffer zone 
		/// around the input) or a negative one (shrinking the input polygons).</summary>
		public static Polygons OffsetPolygons(Polygons poly, double delta, JoinType jointype)
		{
			Polygons result = new Polygons(poly.Count);
			new PolyOffsetBuilder(poly, result, delta, jointype, 2.0);
			return result;
		}

		/// <summary>Encapsulates the algorithm for computing the offsets of a 
		/// polygon. You can either add a positive offset (creating a buffer zone 
		/// around the input) or a negative one (shrinking the input polygons).</summary>
		public static Polygons OffsetPolygons(Polygons poly, double delta)
		{
			Polygons result = new Polygons(poly.Count);
			new PolyOffsetBuilder(poly, result, delta, JoinType.Square, 2.0);
			return result;
		}

		internal static DoublePoint GetUnitNormal(Vector2 pt1, Vector2 pt2)
		{
			double dx = (pt2.x - pt1.x);
			double dy = (pt2.y - pt1.y);
			if ((dx == 0) && (dy == 0))
				return new DoublePoint();

			double f = 1.0 / Math.Sqrt(dx * dx + dy * dy);
			dx *= f;
			dy *= f;

			return new DoublePoint(dy, -dx);
		}
		//------------------------------------------------------------------------------

		internal static Polygon BuildArc(Vector2 pt, double a1, double a2, double r)
		{
			int steps = Math.Max(6, (int)(Math.Sqrt(Math.Abs(r)) * Math.Abs(a2 - a1)));
			Polygon result = new Polygon(steps);
			int n = steps - 1;
			double da = (a2 - a1) / n;
			double a = a1;
			for (int i = 0; i < steps; ++i) {
				result.Add(new Vector2(pt.x + Round(Math.Cos(a) * r), pt.y + Round(Math.Sin(a) * r)));
				a += da;
			}
			return result;
		}
		//------------------------------------------------------------------------------

		#endregion
	}
}