using System.Linq;
using System.Threading.Tasks;
using AutoLot.Dal.Repos.Interfaces;
using AutoLot.Services.ApiWrapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace AutoLot.MVC.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
       // private readonly IMakeRepo _makeRepo;

        private readonly IApiServiceWrapper _serviceWrapper;
        public MenuViewComponent(IApiServiceWrapper serviceWrapper)
        {
            _serviceWrapper = serviceWrapper;
        }

        //public MenuViewComponent(IMakeRepo makeRepo)
        //{
        //    _makeRepo = makeRepo;
        //}

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var makes = await _serviceWrapper.GetMakesAsync();
            if (makes == null)
            {
                return new ContentViewComponentResult("Unable to get the makes");
            }
            return View("MenuView", makes);
        }

        //public IViewComponentResult Invoke()
        //{
        //    var makes = _makeRepo.GetAll().ToList();
        //    if (!makes.Any())
        //    {
        //        return new ContentViewComponentResult("Unable to get the makes");
        //    }
        //    return View("MenuView", makes);
        //}
    }
}