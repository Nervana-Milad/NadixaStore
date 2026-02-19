using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nadixa.Core.Entities;
using Nadixa.Infrastructure.Data;
using Nadixa.Web.Models.ViewModels;



namespace Nadixa.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly NadixaDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string[] _allowedExtension = { ".jpg", ".jpeg", ".png", ".jfif" };

        public ProductController(NadixaDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]

        public IActionResult Create()
        {
            var productViewModel = new ProductViewModel();
            productViewModel.Categories = _context.Categories.Select(c =>
            new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }
            ).ToList();

           
            return View(productViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel productViewModel)
        {
            
            if (ModelState.IsValid)
            {
                var inputFileExtension = Path.GetExtension(productViewModel.MainImageUrl.FileName).ToLower();
                bool isAllowed = _allowedExtension.Contains(inputFileExtension);

                if(!isAllowed)
                {
                    ModelState.AddModelError("", "Invalid Image Format. Allowed formats are .jpg, .jpeg, .png, .jfif");
                    return View(productViewModel);
                }

                productViewModel.Product.MainImageUrlPath = await UploadFileToFolder(productViewModel.MainImageUrl);
                await _context.Products.AddAsync(productViewModel.Product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            productViewModel.Categories = _context.Categories.Select(c =>
            new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }
            ).ToList();
            return View(productViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var product = _context.Products.Include(p => p.Category).Include(p => p.Colors).Include(p => p.Images).FirstOrDefault(p => p.Id == id);

            if(product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var productFromDb = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if(productFromDb == null)
            {
                return NotFound();
            }
            EditViewModel editViewModel = new EditViewModel
            {
                Product = productFromDb,
                Categories = _context.Categories.Select(cat =>
                new SelectListItem
                {
                    Value = cat.Id.ToString(),
                    Text = cat.Name
                }).ToList()
            };

            return View(editViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel editViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editViewModel);
            }
            var productFromDb = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == editViewModel.Product.Id);
            
            if(productFromDb == null)
            {
                return NotFound();
            }

            if(editViewModel.MainImageUrl != null)
            {
                var inputFileExtension = Path.GetExtension(editViewModel.MainImageUrl.FileName).ToLower();
                bool isAlllowed = _allowedExtension.Contains(inputFileExtension);

                if (!isAlllowed)
                {
                    ModelState.AddModelError("", "Invalid Image format. Allowed Formats are .jpg, .jpeg, .png, .jfif");
                    return View(editViewModel);
                }
                var existingFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", Path.GetFileName(productFromDb.MainImageUrlPath));

                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                }
                editViewModel.Product.MainImageUrlPath = await UploadFileToFolder(editViewModel.MainImageUrl);
            }
            else
            {
                editViewModel.Product.MainImageUrlPath = productFromDb.MainImageUrlPath;
            }

            editViewModel.Product.UpdatedAt = DateTime.Now;

            _context.Products.Update(editViewModel.Product);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productFromDb = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (string.IsNullOrEmpty(productFromDb.MainImageUrlPath))
            {
                var existingFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", Path.GetFileName(productFromDb.MainImageUrlPath));
                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                }
            }
            _context.Products.Remove(productFromDb);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        private async Task<string> UploadFileToFolder(IFormFile file)
        {
            var inputFileExtension = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid().ToString() + inputFileExtension;
            var wwwRootPath = _webHostEnvironment.WebRootPath;
            var imagesFolderPath = Path.Combine(wwwRootPath, "images");

            if(!Directory.Exists(imagesFolderPath))
            {
                Directory.CreateDirectory(imagesFolderPath);
            }

            var filePath = Path.Combine(imagesFolderPath, fileName);
            try
            {
                await using(var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            catch(Exception ex)
            {
                return "Error Uploading Images: " + ex.Message;
            }
            return "/images/" + fileName;
        }


    }
}
