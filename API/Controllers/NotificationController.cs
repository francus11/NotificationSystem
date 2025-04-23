using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class NotificationController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
