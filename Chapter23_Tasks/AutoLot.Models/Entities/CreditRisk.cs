using AutoLot.Models.Entities.Base;
using AutoLot.Models.Entities.Owned;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoLot.Models.Entities;

[Table("CreditRisks", Schema = "dbo")]
public partial class CreditRisk : BaseEntity
{
    public int CustomerId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    [InverseProperty(nameof(Customer.CreditRisks))]
    public Customer? CustomerNavigation { get; set; }

    public Person PersonalInformation { get; set; } = new Person();
}
