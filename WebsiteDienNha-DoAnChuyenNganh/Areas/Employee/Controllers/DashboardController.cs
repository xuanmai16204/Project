using Microsoft.AspNetCore.Mvc;

namespace WebsiteDienNha_DoAnChuyenNganh.Areas.Employee.Controllers
{
	[Area("Employee")]
	public class DashboardController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}


