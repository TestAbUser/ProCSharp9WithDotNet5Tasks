﻿using AutoLot.Dal.EfStructures;
//using AutoLot.Dal.Models.Entities;
using AutoLot.Dal.Repos.Base;
using AutoLot.Dal.Repos.Interfaces;
using AutoLot.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoLot.Dal.Repos
{
    public class CreditRiskRepo : BaseRepo<CreditRisk>, ICreditRiskRepo
    {
        public CreditRiskRepo(ApplicationDbContext context) : base(context)
        {
        }

        internal CreditRiskRepo( DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
    }
}
