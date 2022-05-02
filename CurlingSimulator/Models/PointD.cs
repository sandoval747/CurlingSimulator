using System;

namespace CurlingSimulator.Models
{
    public class PointD : IEquatable<PointD>
    {
        public double X;
        public double Y;

        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(PointD other)
        {
            return X == other.X && Y == other.Y;
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }
}
