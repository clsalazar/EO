using System.Web.Mvc;
using EO.Web.Models;
using System.Linq;
using EO.Core.Utilities.RateParser;

namespace EO.Web.Controllers
{
    public class HomeController : Controller
    {

        [HttpGet]
        public ActionResult Index()
        {
            var model = new HomeModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(HomeModel model)
        {
            if (ModelState.IsValid)
            {
                //User choice from multiple providers
                if (!string.IsNullOrWhiteSpace(model.ProviderId))
                    model.Rates = ExampleREP.GetRates(model.ZipCode, model.ProviderId);

                else if (!string.IsNullOrWhiteSpace(model.ZipCode))
                {
                    model.Ids = ExampleREP.GetIds(model.ZipCode);

                    if (model.Ids != null && model.Ids.Count == 1)
                        model.Rates = ExampleREP.GetRates(model.ZipCode, model.Ids.First().Key);

                    //No plans found for entered zip code
                    else if (model.Ids == null)
                        return View();
                }
            }

            return View(model);
        }
    }
}