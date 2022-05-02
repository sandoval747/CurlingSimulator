using CurlingSimulator.Models;

namespace CurlingSimulator.Interfaces
{
    public interface ISimulator
    {
        Disk[] Simulate(int radius, int[] startPositions);
    }
}
