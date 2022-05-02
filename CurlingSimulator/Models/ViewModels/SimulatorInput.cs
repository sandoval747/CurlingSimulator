using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CurlingSimulator.Models.ViewModels
{
    public class SimulatorInput: IValidatableObject
    {
        [Required]
        [Range(1,1000, ErrorMessage = "Number of Disks must be between 1 and 1000")]
        [Display(Name="Number of Disks")]
        public int NumDisks { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage="Radius must be between 1 and 1000")]
        [Display(Name = "Disk Radius")]
        public int DiskRadius { get; set; }

        [Required]
        [RegularExpression(@"^(([1-9]\d{0,2})|1000)(,(([1-9]\d{0,2})|1000))*$", ErrorMessage="X-Coordinates must be a comma-separated list of integers from 1 to 1000")]
        [Display(Name = "X-Coordinates")]
        public string XCoordinates { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return XCoordinates.Split(',').Length == NumDisks ? ValidationResult.Success : new ValidationResult("Number of X-Coordinates must match Number of Disks", new string[] { "XCoordinates" });
            yield return XCoordinates.Split(',').Select(Int32.Parse).Where(x => x < DiskRadius).ToArray().Length == 0 ? ValidationResult.Success : new ValidationResult("X-coordinates must all be greater than or equal to the disk radius.", new string[] { "XCoordinates" } );            
        }
    }
}
