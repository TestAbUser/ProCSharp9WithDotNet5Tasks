using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoLot.Samples.Models
{
    [Table("Inventory", Schema = "dbo")]
    [Index(nameof(MakeId), Name = "IX_Inventory_MakeId")]
    public class Car : BaseEntity
    {
        private bool? _isDrivable;

        [Required, StringLength(50)]
        public string Color { get; set; }
        [Required, StringLength(50)]
        public string PetName { get; set; }

        public string FullName { get; set; }
        public int MakeId { get; set; }
        [ForeignKey(nameof(MakeId))]
        public Make MakeNavigation { get; set; }
        public bool IsDrivable
        {
            get => _isDrivable ?? true;
            set => _isDrivable = value;
        }
        public DateTime DateBuilt { get; set; }
        public Radio RadioNavigation { get; set; }
        [InverseProperty(nameof(Driver.Cars))]
        public IEnumerable<Driver> Drivers { get; set; } //= new List<Driver>();
    }
}
