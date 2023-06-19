using AutoLot.Dal.EfStructures;
//using AutoLot.Dal.Models.Entities;
using AutoLot.Dal.Repos.Base;
using AutoLot.Dal.Repos.Interfaces;
using AutoLot.Models.Entities;
using AutoLot.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AutoLot.Dal.Repos
{
    public class OrderRepo : BaseRepo<Order>, IOrderRepo
    {
        public OrderRepo(ApplicationDbContext context)
        : base(context)
        {
        }
        internal OrderRepo(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public IQueryable<CustomerOrderViewModel> GetOrdersViewModel()
        {
            return Context.CustomerOrderViewModels!.AsQueryable();
        }

    }
}
