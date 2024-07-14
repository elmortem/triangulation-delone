using System;
using UnityEngine;

namespace Delone
{
	public class Arc
    {
        public Vector2 A { get; set; }
        public Vector2 B { get; set; }

        public Triangle TriAB { get; set; }
        public Triangle TriBA { get; set; }

        public bool IsBorder
        {
            get
            {
                if (TriAB == null || TriBA == null)
                    return true;
                
                return false;
            }
        }
 
        public Arc(Vector2 a, Vector2 b)
        {
            A = a;
            B = b;
        }
 
        public static bool ArcIntersect(Arc a1, Arc a2)
        {
            Vector2 p1 = a1.A;
            Vector2 p2 = a1.B;
            Vector2 p3 = a2.A;
            Vector2 p4 = a2.B;

            double d1 = Direction(p3, p4, p1);
            double d2 = Direction(p3, p4, p2);
            double d3 = Direction(p1, p2, p3);
            double d4 = Direction(p1, p2, p4);

            if (p1 == p3 || p1 == p4 || p2 == p3 || p2 == p4)
                return false;
            else if (((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) && ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0)))
                return true;
            else if ((d1 == 0) && OnSegment(p3, p4, p1))
                return true;
            else if ((d2 == 0) && OnSegment(p3, p4, p2))
                return true;
            else if ((d3 == 0) && OnSegment(p1, p2, p3))
                return true;
            else if ((d4 == 0) && OnSegment(p1, p2, p4))
                return true;
            else
                return false;
        }

        public static bool ArcIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            double d1 = Direction(p3, p4, p1);
            double d2 = Direction(p3, p4, p2);
            double d3 = Direction(p1, p2, p3);
            double d4 = Direction(p1, p2, p4);

            if (p1 == p3 || p1 == p4 || p2 == p3 || p2 == p4)
                return false;
            else if (((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) && ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0)))
                return true;
            else if ((d1 == 0) && OnSegment(p3, p4, p1))
                return true;
            else if ((d2 == 0) && OnSegment(p3, p4, p2))
                return true;
            else if ((d3 == 0) && OnSegment(p1, p2, p3))
                return true;
            else if ((d4 == 0) && OnSegment(p1, p2, p4))
                return true;
            else
                return false;
        }
 
        public Vector2 GetSecondNode(Vector2 firstnode)
        {
            if (A == firstnode)
                return B;
            
            if (B == firstnode)
                return A;
            
            return default;
        }
 
        public Triangle GetOtherTriangle(Triangle tri)
        {
            if (TriAB == tri)
                return TriBA;
            
            if (TriBA == tri)
                return TriAB;
            
            return null;
        }
 
        public static Vector2 GetCommonPoint(Arc a1, Arc a2)
        {
            if (a1.A == a2.A)
                return a1.A;
            
            if (a1.A == a2.B)
                return a1.A;
            
            if (a1.B == a2.A)
                return a1.B;
            
            if (a1.B == a2.B)
                return a1.B;
            
            return default;
        }

        public bool IsConnectedWith(Arc arc)
        {
            if (A == arc.A || A == arc.B)
                return true;
 
            if (B == arc.A || B == arc.B)
                return true;
 
            return false;
        }
 
        private static double Direction(Vector2 pi, Vector2 pj, Vector2 pk)
        {
            return Vector2Utility.CrossProduct(pk - pi, pj - pi);
        }
        private static bool OnSegment(Vector2 pi, Vector2 pj, Vector2 pk)
        {
            if (Math.Min(pi.x, pj.x) <= pk.x && pk.x <= Math.Max(pi.x, pj.x) && 
                Math.Min(pi.y, pj.y) <= pk.y && pk.y <= Math.Max(pi.y, pj.y))
                return true;
            
            return false;
        }
    }
}