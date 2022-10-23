using Project_1.Models.Shapes.Abstract;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace Project_1.Models.Shapes
{
    public class Edge : IEdge
    {
        public IPoint U { get; set; }
        public IPoint V { get; set; }

        public PointF Center => new((U.X + V.X) / 2, (U.Y + V.Y) / 2);

        public static EdgeMoveWithConstraints ConstrainedMove { get; set; }

        public Edge(IPoint u, IPoint v)
        {
            U = u;
            V = v;
        }

        public int Length => (int)new Vector2(U.X - V.X, U.Y - V.Y).Length();

        public void Move(Vector2 vector)
        {
            U.Move(vector);
            V.Move(vector);
        }

        public bool WasClicked(PointF click, int clickRadius)
        {
            var u = U.Center;
            var v = V.Center;

            var uv = new Vector2(v.X - u.X, v.Y - u.Y);
            var a = uv.Length() / clickRadius;
            uv /= a;

            var uvPerpendicular = new Vector2(v.Y - u.Y, u.X - v.X);
            var b = uvPerpendicular.Length() / clickRadius;
            uvPerpendicular /= b;

            var polygon = new List<PointF>
            {
                new(u.ToVector2() + uv + uvPerpendicular),
                new(v.ToVector2() - uv + uvPerpendicular),
                new(v.ToVector2() - uv - uvPerpendicular),
                new(u.ToVector2() + uv - uvPerpendicular)
            };

            // code reused from https://stackoverflow.com/questions/4243042/c-sharp-point-in-polygon
            bool result = false;
            int j = polygon.Count - 1;
            for (int i = 0; i < polygon.Count; i++)
            {
                if (polygon[i].Y < click.Y && polygon[j].Y >= click.Y || polygon[j].Y < click.Y && polygon[i].Y >= click.Y)
                {
                    if (polygon[i].X + (click.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < click.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        public void MoveWithConstraints(Vector2 vector)
        {
            ConstrainedMove?.Invoke(this, vector);
        }

        public (IPoint toMove, Vector2 move) GetMakePerpendicularInstruction(IEdge edge, int? fixedLength = null)
        {
            IPoint u, v, w, z;

            // check intersection
            if (U == edge.U)
            {
                v = w = U;
                u = V;
                z = edge.V;
            }
            else if (U == edge.V) 
            {
                v = w = U;
                u = V;
                z = edge.U;
            }
            else if (V == edge.U)
            {
                v = w = V;
                u = U;
                z = edge.V;
            }
            else if (V == edge.V)
            {
                v = w = V;
                u = U;
                z = edge.U;
            }
            else
            {
                u = U;
                v = V;

                w = edge.U;
                z = edge.V;
            }

            var uv = v - u;
            var wz = z - w;

            var uvLength = fixedLength ?? uv.Length();

            var P = new Vector2(wz.Y, -wz.X);
            P = Vector2.Normalize(P) * uvLength;

            // check better direction
            var direction = Vector2.Dot(uv, P) / (uvLength * P.Length());
            if (direction > 0)
            {
                P = Vector2.Negate(P);
            }

            return (u, uv + P);
        }

        public void MakePerpendicularWithConstraints(IEdge edge)
        {
            IPoint z;

            // check intersection
            if (U == edge.U)
            {
                z = edge.V;
            }
            else if (U == edge.V)
            {
                z = edge.U;
            }
            else if (V == edge.U)
            {
                z = edge.V;
            }
            else if (V == edge.V)
            {
                z = edge.U;
            }
            else
            {
                z = edge.V;
            }

            z.MoveWithConstraints(Vector2.Zero);
        }
    }
}
