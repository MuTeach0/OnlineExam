using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineExam.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    // GET: Admin
    public IActionResult Index() => View();
}