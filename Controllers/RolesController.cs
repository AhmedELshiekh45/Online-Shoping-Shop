using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Repos;
using ServiceLayer.ViewModels;

namespace Presentaion.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly RoleRepo roleServices;

        public RolesController(RoleRepo roleServices)
        {
            this.roleServices = roleServices;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddRole(AddRoleVM vm)
        { if (ModelState.IsValid)
            {
                var result = await roleServices.AddAsync(vm);
                if (result.Succeeded)
                {
                    return RedirectToAction();

                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View(vm);
        }

    }
}
