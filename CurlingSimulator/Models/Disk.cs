using System.Windows;
using System.Drawing;
using System;
using System.Diagnostics.CodeAnalysis;

namespace CurlingSimulator.Models
{
    public class Disk: IComparable<Disk>
    {
        public int Radius;
        public PointD CenterPoint;

        public Disk(int x, int radius)
        {
            this.Radius = radius;
            this.CenterPoint = new PointD(x, -1);
        }

        public PointD GetCollisionPoint(Disk other)
        {
            if (other == null)
            {
                return new PointD(CenterPoint.X, Radius);
            }
            var y = Math.Sqrt(Math.Pow(Radius + other.Radius, 2) - Math.Pow(CenterPoint.X - other.CenterPoint.X, 2)) + other.CenterPoint.Y;
            return new PointD(CenterPoint.X, y);
        }

        public int CompareTo(Disk other)
        {
            return CenterPoint.Y.CompareTo(other.CenterPoint.Y);
        }
    }
}
