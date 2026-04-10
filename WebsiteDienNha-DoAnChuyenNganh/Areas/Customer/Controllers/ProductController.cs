using Microsoft.AspNetCore.Mvc;

namespace WebsiteDienNha_DoAnChuyenNganh.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class ProductController : Controller
	{
		public IActionResult Index(int? categoryId)
		{
			// Redirect to root Product controller
			return RedirectToAction("Index", "Product", new { area = "", categoryId = categoryId });
		}

		public IActionResult Details(int? id)
		{
			// Redirect to root Product controller
			return RedirectToAction("Details", "Product", new { area = "", id = id });
		}
	}
}

