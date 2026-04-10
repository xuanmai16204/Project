using Microsoft.AspNetCore.Mvc;

namespace WebsiteDienNha_DoAnChuyenNganh.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class NotificationController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}


