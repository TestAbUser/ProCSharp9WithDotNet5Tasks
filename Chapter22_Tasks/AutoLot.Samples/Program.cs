using AutoLot.Samples;
using Microsoft.EntityFrameworkCore;

public class Program
{
    // Uncomment the following line to resolve.
    public static void Main()
    {
        var context = new ApplicationDbContextFactory().CreateDbContext(null);
        var queryable = context.Cars.IgnoreQueryFilters().Include(c => c.MakeNavigation).ToList();
    }
}