﻿using AutoLot.Dal.EfStructures;
//using AutoLot.Dal.Models;
using AutoLot.Dal.Repos.Base;
using AutoLot.Dal.Repos.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoLot.Models.Entities;
using Microsoft.Data.SqlClient;

namespace AutoLot.Dal.Repos
{
    public class CarRepo : BaseRepo<Car>, ICarRepo
    {

        public CarRepo(ApplicationDbContext context) : base(context)
        {
        }
        internal CarRepo(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public override IEnumerable<Car> GetAll()
            => Table
            .Include(c => c.MakeNavigation)
            .OrderBy(o => o.PetName);
        public override IEnumerable<Car> GetAllIgnoreQueryFilters()
        => Table
        .Include(c => c.MakeNavigation)
        .OrderBy(o => o.PetName)
        .IgnoreQueryFilters();

        public IEnumerable<Car> GetAllBy(int makeId)
        {
            return Table
            .Where(x => x.MakeId == makeId)
            .Include(c => c.MakeNavigation)
            .OrderBy(c => c.PetName);
        }

        public override Car? Find(int? id)
            => Table
            .IgnoreQueryFilters()
            .Where(x => x.Id == id)
            .Include(m => m.MakeNavigation)
            .FirstOrDefault();

        public string GetPetName(int id)
        {
            var parameterId = new SqlParameter
            {
                ParameterName = "@carId",
                SqlDbType = SqlDbType.Int,
                Value = id,
            };

            var parameterName = new SqlParameter
            {
                ParameterName = "@petName",
                SqlDbType = SqlDbType.NVarChar,
                Size = 50,
                Direction = ParameterDirection.Output
            };
            _ = Context.Database
            .ExecuteSqlRaw("EXEC [dbo].[GetPetName] @carId, @petName OUTPUT", parameterId,
            parameterName);
            return (string)parameterName.Value;
        }
    }
}
