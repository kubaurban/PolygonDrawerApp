﻿using System;
using System.Collections.Generic;

namespace Project_1.Models.Shapes
{
    public class Polygon : Shape
    {
        private List<Point> _vertices;

        public List<Point> Vertices {
            get => _vertices;
            set
            {
                _vertices = value;
                _vertices.ForEach(x => x.PolygonId = Id);
            }
        }

        public Polygon(int id) : base(id) { }

        public void RemoveVertex(Point p)
        {
            Vertices.Remove(p);
        }

        public System.Drawing.Point GravityCenterPoint {
            // below code reused from: https://stackoverflow.com/questions/9815699/how-to-calculate-centroid
            get
            {
                var vertices = Vertices.ToArray();

                float accumulatedArea = 0.0f;
                float centerX = 0.0f;
                float centerY = 0.0f;

                for (int i = 0, j = vertices.Length - 1; i < vertices.Length; j = i++)
                {
                    float temp = vertices[i].X * vertices[j].Y - vertices[j].X * vertices[i].Y;
                    accumulatedArea += temp;
                    centerX += (vertices[i].X + vertices[j].X) * temp;
                    centerY += (vertices[i].Y + vertices[j].Y) * temp;
                }

                if (Math.Abs(accumulatedArea) < 1E-7f)
                    return System.Drawing.Point.Empty;  // Avoid division by zero

                accumulatedArea *= 3f;
                return new((int)(centerX / accumulatedArea), (int)(centerY / accumulatedArea));
            }
        }
    }
}
