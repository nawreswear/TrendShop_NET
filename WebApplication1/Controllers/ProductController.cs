using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Models;
using WebApplication1.Models.Repositories;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _repository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment hostingEnvironment;
        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository, IWebHostEnvironment hostingEnvironment)
        {
            _repository = productRepository;
            _categoryRepository = categoryRepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        // GET: Product
       /* [AllowAnonymous]
        public IActionResult Index()
        {
            var products = _repository.GetAll();
            return View(products);
        }*/

        // GET: Product/Details/5
        public IActionResult Details(int id)
        {
            var categories = _categoryRepository.GetAll();
            ViewData["Categories"] = categories;
            var product = _repository.GetById(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            var categories = _categoryRepository.GetAll();
            ViewData["Categories"] = categories;
            ViewBag.CategoryId = new SelectList(_categoryRepository.GetAll(), "CategoryId", "CategoryName");
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                // If the Photo property on the incoming model object is not null,
                // then the user has selected an image to upload.
                if (model.ImagePath != null)
                {
                    // The image must be uploaded to the images folder in wwwroot
                    // To get the path of the wwwroot folder we are using the inject
                    // HostingEnvironment service provided by ASP.NET Core
                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                    // To make sure the file name is unique we are appending a new
                    // GUID value and an underscore to the file name
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImagePath.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Use CopyTo() method provided by IFormFile interface to
                    // copy the file to wwwroot/images folder
                    model.ImagePath.CopyTo(new FileStream(filePath, FileMode.Create));
                }
                Product newProduct = new Product
                {
                    Name = model.Name,
                    Price = model.Price,
                    QteStock = model.QteStock,
                    CategoryId = model.CategoryId,
                    // Store the file name in PhotoPath property of the employee object
                    // which gets saved to the Employees database table
                    ImagePath = uniqueFileName
                };
                _repository.Add(newProduct);
                return RedirectToAction("details", new { id = newProduct.ProductId });
            }
            return View();
        }

        // GET: Product/Edit/5
        public ActionResult Edit(int id)
        {
            var categories = _categoryRepository.GetAll();
            ViewData["Categories"] = categories;
            ViewBag.CategoryId = new SelectList(_categoryRepository.GetAll(),
            "CategoryId"
            ,
            "CategoryName");
            Product product = _repository.GetById(id);
            EditViewModel productEditViewModel = new EditViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                QteStock = product.QteStock,
                CategoryId = product.CategoryId,
                ExistingImagePath = product.ImagePath
            };
            return View(productEditViewModel);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditViewModel model)
        {
            ViewBag.CategoryId = new SelectList(_categoryRepository.GetAll(), "CategoryId", "CategoryName");
            // Check if the provided data is valid, if not rerender the edit view
            // so the user can correct and resubmit the edit form
            if (ModelState.IsValid)
            {
                // Retrieve the product being edited from the database
                Product product = _repository.GetById(model.ProductId);
                // Update the product object with the data in the model object
                product.Name = model.Name;
                product.Price = model.Price;
                product.QteStock = model.QteStock;
                product.CategoryId = model.CategoryId;
                // If the user wants to change the photo, a new photo will be
                // uploaded and the Photo property on the model object receives
                // the uploaded photo. If the Photo property is null, user did
                // not upload a new photo and keeps his existing photo
                if (model.ImagePath != null)
                {
                    // If a new photo is uploaded, the existing photo must be
                    // deleted. So check if there is an existing photo and delete
                    if (!string.IsNullOrEmpty(model.ExistingImagePath))
                    {
                        string filePath = Path.Combine(hostingEnvironment.WebRootPath, "images", model.ExistingImagePath);

                        if (System.IO.File.Exists(filePath))
                        {
                            try
                            {
                                // Libère les verrous sur le fichier si encore utilisé
                                GC.Collect();
                                GC.WaitForPendingFinalizers();

                                System.IO.File.Delete(filePath);
                            }
                            catch (IOException ex)
                            {
                                // Log l’erreur ou ignore si tu ne veux pas bloquer
                                Console.WriteLine($"Erreur suppression image : {ex.Message}");
                            }
                        }
                    }

                    // Save the new photo in wwwroot/images folder and update
                    // PhotoPath property of the product object which will be
                    // eventually saved in the database
                    product.ImagePath = ProcessUploadedFile(model);
                }
                Product updatedProduct = _repository.Update(product);
                if (updatedProduct != null)
                    return RedirectToAction("Index");
                else
                    return NotFound();
            }
            return View(model);
        }
        [NonAction]
        private string ProcessUploadedFile(EditViewModel model)
        {
            string uniqueFileName = null;
            if (model.ImagePath != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImagePath.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImagePath.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }


        // GET: Product/Delete/5
        public IActionResult Delete(int id)
        {
            var categories = _categoryRepository.GetAll();
            ViewData["Categories"] = categories;
            var product = _repository.GetById(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        // Optional: Search by name
        [AllowAnonymous]
        public IActionResult Search(string val)
        {
            if (string.IsNullOrWhiteSpace(val))
                return RedirectToAction(nameof(Index));

            var searchTerm = val.ToLower();

            // Recherche par nom de produit ou nom de catégorie
            var results = _repository.GetAll()
                .Where(p => p.Name.ToLower().Contains(searchTerm)
                         || (p.Category != null && p.Category.CategoryName.ToLower().Contains(searchTerm)))
                .ToList();

            return View("Index", results);
        }

        /* public ActionResult Search(string val)
         {
             var result = ProductRepository.FindByName(val);
             return View("Index", result);
         }*/
        [AllowAnonymous]
        public IActionResult Index(int? categoryId, int page = 1)
        {
            
            int pageSize = 4; // Nombre de produits par page
            var categories = _categoryRepository.GetAll();
            // Passer les catégories à la vue
            ViewData["Categories"] = categories;
            // Récupérer les produits en fonction de categoryId, s'il est spécifié
            IQueryable<Product> productsQuery = _repository.GetAllProducts();
            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId);
            }
            // Pagination
            var totalProducts = productsQuery.Count();
            var products = productsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.CategoryId = categoryId; // Passer categoryId à la vue
            return View(products);
        }
    }
    }
