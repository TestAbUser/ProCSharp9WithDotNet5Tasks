using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AutoLot.Models.ViewModels
{
    [Keyless]
    public class CustomerOrderViewModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Color { get; set; }
        public string? PetName { get; set; }
        public string? Make { get; set; }
        public bool? IsDrivable { get; set; }
        [NotMapped]
        public string FullDetail =>
        $"{FirstName} {LastName} ordered a {Color} {Make} named {PetName}";

        public override string ToString() => FullDetail;
    }
}
