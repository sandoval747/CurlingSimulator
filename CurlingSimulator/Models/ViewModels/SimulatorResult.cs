using CurlingSimulator.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CurlingSimulator.Models.ViewModels
{
    public class SimulatorResult
    {
        public Disk[] Disks { get; set; }

        [Display(Name = "Y-Coordinates")]
        public string YCoordinates {
            get
            {
                return Disks != null ? String.Join(",", Disks.Select(d => d.CenterPoint.Y)) : "";
            }
        }
    }
}
