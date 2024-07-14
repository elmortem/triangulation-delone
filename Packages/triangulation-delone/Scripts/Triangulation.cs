using System;
using System.Collections.Generic;
using UnityEngine;

namespace Delone
{
	public class Triangulation
    {
        private readonly DynamicCache _cache;
        
        private readonly List<Vector2> _points;
        private readonly Vector2 _min;
        private readonly Vector2 _max;
        
        public List<Triangle> Triangles { get; } = new();

        public Triangulation(List<Vector2> points, Vector2 min, Vector2 max)
        {
            _points = points;
            _min = min;
            _max = max;

            _cache = new DynamicCache(_min, _max);
        }
        
        public void Calc()
        {
            _cache.Reset();
            
            Triangles.Clear();
            
            Triangles.Add(new Triangle(_min, new Vector2(_max.x, _min.y), _max));
            Triangles.Add(new Triangle(Triangles[0].Arcs[2], new Vector2(_min.x, _max.y)));

            Triangles[0].Arcs[2].TriAB = Triangles[1];
            Triangles[1].Arcs[0].TriBA = Triangles[0];

            _cache.Add(Triangles[0]);
            _cache.Add(Triangles[1]);
            
            for (int i = 0; i < _points.Count; i++)
            {
                var currentTriangle = GetTriangleForPoint(_points[i]);

                if (currentTriangle != null)
                {
                    var newArc0 = new Arc(currentTriangle.Points[0], _points[i]);
                    var newArc1 = new Arc(currentTriangle.Points[1], _points[i]);
                    var newArc2 = new Arc(currentTriangle.Points[2], _points[i]);
 
                    var oldArc0 = currentTriangle.GetArcBetween2Points(currentTriangle.Points[0], currentTriangle.Points[1]);
                    var oldArc1 = currentTriangle.GetArcBetween2Points(currentTriangle.Points[1], currentTriangle.Points[2]);
                    var oldArc2 = currentTriangle.GetArcBetween2Points(currentTriangle.Points[2], currentTriangle.Points[0]);
 
                    var newTriangle0 = currentTriangle;
                    newTriangle0.Arcs[0] = oldArc0;
                    newTriangle0.Arcs[1] = newArc1;
                    newTriangle0.Arcs[2] = newArc0;
                    newTriangle0.Points[2] = _points[i];
 
                    var newTriangle1 = new Triangle(oldArc1, newArc2, newArc1);
                    var newTriangle2 = new Triangle(oldArc2, newArc0, newArc2);
 
                    newArc0.TriAB = newTriangle0;
                    newArc0.TriBA = newTriangle2;
                    newArc1.TriAB = newTriangle1;
                    newArc1.TriBA = newTriangle0;
                    newArc2.TriAB = newTriangle2;
                    newArc2.TriBA = newTriangle1;
 
                    if (oldArc0.TriAB == currentTriangle)
                        oldArc0.TriAB = newTriangle0;
                    if (oldArc0.TriBA == currentTriangle)
                        oldArc0.TriBA = newTriangle0;
 
                    if (oldArc1.TriAB == currentTriangle)
                        oldArc1.TriAB = newTriangle1;
                    if (oldArc1.TriBA == currentTriangle)
                        oldArc1.TriBA = newTriangle1;
 
                    if (oldArc2.TriAB == currentTriangle)
                        oldArc2.TriAB = newTriangle2;
                    if (oldArc2.TriBA == currentTriangle)
                        oldArc2.TriBA = newTriangle2;
                    
                    Triangles.Add(newTriangle1);
                    Triangles.Add(newTriangle2);
 
                    _cache.Add(newTriangle0);
                    _cache.Add(newTriangle1);
                    _cache.Add(newTriangle2);
 
                    CheckDelaunayAndRebuild(oldArc0);
                    CheckDelaunayAndRebuild(oldArc1);
                    CheckDelaunayAndRebuild(oldArc2);
                }
            }
 
            for (int i = 0; i < Triangles.Count; i++)
            {
                CheckDelaunayAndRebuild(Triangles[i].Arcs[0]);
                CheckDelaunayAndRebuild(Triangles[i].Arcs[1]);
                CheckDelaunayAndRebuild(Triangles[i].Arcs[2]);
            }
        }
 
        private Triangle GetTriangleForPoint(Vector2 point)
        {
            Triangle link = _cache.FindTriangle(point);
 
            if (link == null)
            {
                link = Triangles[0];
            }
 
            if (IsPointInTriangle(link, point))
            {
                return link;
            }
            else
            {
                Arc way = new Arc(point, link.Centroid);

                while (!IsPointInTriangle(link, point))
                {
                    var currentArc = GetIntersectedArc(way, link);
                    if (currentArc == null)
                    {
                        throw new Exception("No intersecting edge found. Check the input data.");
                    }
                    
                    if (link != currentArc.TriAB)
                        link = currentArc.TriAB;
                    else
                        link = currentArc.TriBA;
 
                    way = new Arc(point, link.Centroid);
                }
                return link;
            }
        }
 
        private Arc GetIntersectedArc(Arc line, Triangle target)
        {
            if (Arc.ArcIntersect(target.Arcs[0], line))
                return target.Arcs[0];
            if (Arc.ArcIntersect(target.Arcs[1], line))
                return target.Arcs[1];
            if (Arc.ArcIntersect(target.Arcs[2], line))
                return target.Arcs[2];
 
            return null;
        }
 
        private bool IsPointInTriangle(Triangle triangle, Vector2 point)
        {
            Vector2 p1 = triangle.Points[0];
            Vector2 p2 = triangle.Points[1];
            Vector2 p3 = triangle.Points[2];
            Vector2 p4 = point;
 
            double a = (p1.x - p4.x) * (p2.y - p1.y) - (p2.x - p1.x) * (p1.y - p4.y);
            double b = (p2.x - p4.x) * (p3.y - p2.y) - (p3.x - p2.x) * (p2.y - p4.y);
            double c = (p3.x - p4.x) * (p1.y - p3.y) - (p1.x - p3.x) * (p3.y - p4.y);
 
            if ((a >= 0 && b >= 0 && c >= 0) || (a <= 0 && b <= 0 && c <= 0))
                return true;
            
            return false;
        }
 
        private bool IsDelaunay(Vector2 aP, Vector2 bP, Vector2 cP, Vector2 checkNode)
        {
            float x0 = checkNode.x;
            float y0 = checkNode.y;
            float x1 = aP.x;
            float y1 = aP.y;
            float x2 = bP.x;
            float y2 = bP.y;
            float x3 = cP.x;
            float y3 = cP.y;
 
            float[] matrix = { (x1 - x0)*(x1 - x0) + (y1 - y0)*(y1 - y0), x1 - x0, y1 - y0,
                                 (x2 - x0)*(x2 - x0) + (y2 - y0)*(y2 - y0), x2 - x0, y2 - y0,
                                 (x3 - x0)*(x3 - x0) + (y3 - y0)*(y3 - y0), x3 - x0, y3 - y0};
 
            float matrixDeterminant = matrix[0] * matrix[4] * matrix[8] + matrix[1] * matrix[5] * matrix[6] + matrix[2] * matrix[3] * matrix[7] -
                                      matrix[2] * matrix[4] * matrix[6] - matrix[0] * matrix[5] * matrix[7] - matrix[1] * matrix[3] * matrix[8];
 
            float a = x1 * y2 * 1 + y1 * 1 * x3 + 1 * x2 * y3
                      - 1 * y2 * x3 - y1 * x2 * 1 - 1 * y3 * x1;
            
            if (a < 0f)
                matrixDeterminant *= -1f;

            return matrixDeterminant < 0f;
        }
 
        private bool IsDelaunay2(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            float x0 = v0.x;
            float y0 = v0.y;
            float x1 = v1.x;
            float y1 = v1.y;
            float x2 = v3.x;
            float y2 = v3.y;
            float x3 = v2.x;
            float y3 = v2.y;
 
            float cosA = (x0 - x1) * (x0 - x3) + (y0 - y1) * (y0 - y3);
            float cosB = (x2 - x1) * (x2 - x3) + (y2 - y1) * (y2 - y3);
 
            if (cosA < 0 && cosB < 0)
                return false;
 
            if (cosA >= 0 && cosB >= 0)
                return true;
 
            float sinA = (x0 - x1) * (y0 - y3) - (x0 - x3) * (y0 - y1);
            float sinB = (x2 - x3) * (y2 - y1) - (x2 - x1) * (y2 - y3);
 
            if (sinA * cosB + cosA * sinB >= 0)
                return true;
            
            return false;
        }
 
        private void CheckDelaunayAndRebuild(Arc arc)
        {
            Triangle tri1;
            Triangle tri2;
 
            if (arc.TriAB != null && arc.TriBA != null)
            {
                tri1 = arc.TriAB;
                tri2 = arc.TriBA;
            }
            else
                return;
 
            Vector2[] currentPoints = new Vector2[4];

            currentPoints[0] = tri1.GetThirdPoint(arc);
            currentPoints[1] = arc.A;
            currentPoints[2] = arc.B;
            currentPoints[3] = tri2.GetThirdPoint(arc);
 
            if (Arc.ArcIntersect(currentPoints[0], currentPoints[3], currentPoints[1], currentPoints[2]))
            {
                if (!IsDelaunay(currentPoints[0], currentPoints[1], currentPoints[2], currentPoints[3]))
                {
                    tri1.GetTwoOtherArcs(arc, out var oldArcT1A1, out var oldArcT1A2);
                    tri2.GetTwoOtherArcs(arc, out var oldArcT2A1, out var oldArcT2A2);

                    Arc newArcT1A1;
                    Arc newArcT1A2;
                    Arc newArcT2A1;
                    Arc newArcT2A2;
                    if (oldArcT1A1.IsConnectedWith(oldArcT2A1))
                    {
                        newArcT1A1 = oldArcT1A1;
                        newArcT1A2 = oldArcT2A1;
                        newArcT2A1 = oldArcT1A2;
                        newArcT2A2 = oldArcT2A2;
                    }
                    else
                    {
                        newArcT1A1 = oldArcT1A1;
                        newArcT1A2 = oldArcT2A2;
                        newArcT2A1 = oldArcT1A2;
                        newArcT2A2 = oldArcT2A1;
                    }

                    arc.A = currentPoints[0];
                    arc.B = currentPoints[3];

                    tri1.Arcs[0] = arc;
                    tri1.Arcs[1] = newArcT1A1;
                    tri1.Arcs[2] = newArcT1A2;

                    tri2.Arcs[0] = arc;
                    tri2.Arcs[1] = newArcT2A1;
                    tri2.Arcs[2] = newArcT2A2;

                    tri1.Points[0] = arc.A;
                    tri1.Points[1] = arc.B;
                    tri1.Points[2] = Arc.GetCommonPoint(newArcT1A1, newArcT1A2);

                    tri2.Points[0] = arc.A;
                    tri2.Points[1] = arc.B;
                    tri2.Points[2] = Arc.GetCommonPoint(newArcT2A1, newArcT2A2);

                    if (newArcT1A2.TriAB == tri2)
                        newArcT1A2.TriAB = tri1;
                    else if (newArcT1A2.TriBA == tri2)
                        newArcT1A2.TriBA = tri1;

                    if (newArcT2A1.TriAB == tri1)
                        newArcT2A1.TriAB = tri2;
                    else if (newArcT2A1.TriBA == tri1)
                        newArcT2A1.TriBA = tri2;

                    _cache.Add(tri1);
                    _cache.Add(tri2);
                }
            }
        }
    }
}