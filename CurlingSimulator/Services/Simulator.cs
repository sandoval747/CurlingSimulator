using CurlingSimulator.Interfaces;
using CurlingSimulator.Models;
using System.Collections.Generic;

namespace CurlingSimulator.Services
{
    public class Simulator: ISimulator
    {
        public Disk[] Simulate(int radius, int[] startPositions)
        {
            var disks = GenerateDisks(radius, startPositions);

            LaunchDisks(disks);
            
            return disks.ToArray();
        }

        private List<Disk> GenerateDisks(int radius, int[] startPositions)
        {
            var disks = new List<Disk>();
            foreach (int x in startPositions)
            {
                disks.Add(new Disk(x, radius));
            }
            return disks;
        }

        private void LaunchDisks(List<Disk> disks)
        {
            var launchedDisks = new List<Disk>();
            foreach (var disk in disks)
            {
                disk.CenterPoint = disk.GetCollisionPoint(null);
                for (int i = launchedDisks.Count - 1; i >= 0; i--)
                {
                    if (disk.CenterPoint.Y > launchedDisks[i].CenterPoint.Y + disk.Radius + launchedDisks[i].Radius)
                    {
                        break;
                    }
                    var collisionPoint = disk.GetCollisionPoint(launchedDisks[i]);
                    if (collisionPoint.Y > disk.CenterPoint.Y)
                    {
                        disk.CenterPoint = collisionPoint;
                    }
                }                

                var index = launchedDisks.BinarySearch(disk);
                if (index < 0) index = ~index;
                launchedDisks.Insert(index, disk);
            }
        }
    }
}
