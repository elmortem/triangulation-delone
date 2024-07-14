using System;
using UnityEngine;

namespace Delone
{
	public class Triangle
    {
        public Vector2[] Points { get; } = new Vector2[3];
        public Arc[] Arcs { get; } = new Arc[3];

        public Vector2 Centroid => Points[2] - (Points[2] - (Points[0] + (Points[1] - Points[0]) * 0.5f)) * 0.6666666f;
 
        public Triangle(Vector2 a, Vector2 b, Vector2 c)
        {
            Points[0] = a;
            Points[1] = b;
            Points[2] = c;
 
            Arcs[0] = new Arc(a, b);
            Arcs[1] = new Arc(b, c);
            Arcs[2] = new Arc(c, a);
        }
 
        public Triangle(Arc arc, Vector2 a)
        {
            Points[0] = arc.A;
            Points[1] = arc.B;
            Points[2] = a;
 
            Arcs[0] = arc;
            Arcs[1] = new Arc(Points[1], Points[2]);
            Arcs[2] = new Arc(Points[2], Points[0]);
        }
 
        public Triangle(Arc arc0, Arc arc1, Arc arc2)
        {
            Arcs[0] = arc0;
            Arcs[1] = arc1;
            Arcs[2] = arc2;
 
            Points[0] = arc0.A;
            Points[1] = arc0.B;
 
            if (arc1.A == arc0.A || arc1.A == arc0.B)
                Points[2] = arc1.B;
            else if (arc1.B == arc0.A || arc1.B == arc0.B)
                Points[2] = arc1.A;
            else if (Points[2] != arc2.A && Points[2] != arc2.B)
            {
                throw new Exception("Trying to create a triangle from three disjoint edges.");
            }
        }
 
        public Vector2 GetThirdPoint(Arc arc)
        {
            for (int i = 0; i < 3; i++)
                if (arc.A != Points[i] && arc.B != Points[i])
                    return Points[i];
 
            return default;
        }
 
        public Arc GetArcBetween2Points(Vector2 a, Vector2 b)
        {
            for (int i = 0; i < 3; i++)
                if (Arcs[i].A == a && Arcs[i].B == b || Arcs[i].A == b && Arcs[i].B == a)
                    return Arcs[i];
 
            return null;
        }
 
 
        public static Vector2 Get4Point(Triangle tri1, Triangle tri2)
        {
            for (int i = 0; i < 3; i++)
                if (tri2.Points[i] != tri1.Points[0] && tri2.Points[i] != tri1.Points[1] && tri2.Points[i] != tri1.Points[2])
                    return tri2.Points[i];
 
            return default;
        }
 
        public void GetTwoOtherArcs(Arc a0, out Arc a1, out Arc a2)
        {
            if (Arcs[0] == a0)
            { a1 = Arcs[1]; a2 = Arcs[2]; }
            else if (Arcs[1] == a0)
            { a1 = Arcs[0]; a2 = Arcs[2]; }
            else if (Arcs[2] == a0)
            { a1 = Arcs[0]; a2 = Arcs[1]; }
            else
            { a1 = null; a2 = null; }
        }
 
        public static Vector2[] Get4Point2(Triangle tri1, Triangle tri2)
        {
            Vector2[] points = new Vector2[4];
 
            for (int i = 0; i < 3; i++)
            {
                if (tri2.Points[i] != tri1.Points[0] && tri2.Points[i] != tri1.Points[1] && tri2.Points[i] != tri1.Points[2])
                    points[3] = tri2.Points[i];
 
                if (tri1.Points[i] != tri2.Points[0] && tri1.Points[i] != tri2.Points[1] && tri1.Points[i] != tri2.Points[2])
                    points[0] = tri1.Points[i];
 
                if (tri2.Points[i] == tri1.Points[1])
                    points[1] = tri2.Points[i];
                else if (tri2.Points[i] == tri1.Points[2])
                    points[2] = tri2.Points[i];
 
            }
 
            return points;
        }
    }
}