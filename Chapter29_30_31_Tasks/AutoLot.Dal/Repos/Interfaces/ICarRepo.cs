﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoLot.Models.Entities;
using AutoLot.Dal.Repos.Base;

namespace AutoLot.Dal.Repos.Interfaces
{
    public interface ICarRepo : IRepo<Car>
    {
        IEnumerable<Car> GetAllBy(int makeId);
        string GetPetName(int id);
    }
}
