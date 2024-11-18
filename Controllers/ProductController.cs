using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.ViewModels;
using Services_Layer.Services;

namespace Presentaion.Controllers
{
    public class ProductController : Controller
    {
        private ProductVM vM;
        private readonly ProductService _productService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ProductService service, IWebHostEnvironment webHostEnvironment)
        {
            this._productService = service;
            this._webHostEnvironment = webHostEnvironment;
            vM = new ProductVM();
        }


        public async Task<IActionResult> ShowAll()
        {
            

            return View(await _productService.GetProducts());
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            return View(await _productService.CategoriesAndBrands());
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(ProductVM vM)
        {
            if (ModelState.IsValid)
            {
                vM.ImagePath = await GenerateImagePath(vM.File);
                await _productService.Add(vM);
                return RedirectToAction("ShowAll");
            }
            return View(vM);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            vM = await _productService.EditView(id);
            return View(vM);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(ProductVM vM)
        {
            if (ModelState.IsValid)
            {
                vM.ImagePath = await GenerateImagePath(vM.File);
                await _productService.Edit(vM);
                return RedirectToAction("Deatils");
            }
            return View(vM);
        }


        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var pro =await _productService.Deatils(id);
            return View(pro);
        }


        [HttpGet]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            await _productService.Delete(id);
            return RedirectToAction("ShowAll");
        }





        private async Task<string> GenerateImagePath(IFormFile file)
        {
            string ImagePath = "Images/Products/";
            ImagePath += Guid.NewGuid().ToString() + "_" + file.FileName;
            var server = Path.Combine(_webHostEnvironment.WebRootPath, ImagePath);
            await file.CopyToAsync(new FileStream(server, FileMode.Create));

            return "/" + ImagePath;
        }
    }
}
