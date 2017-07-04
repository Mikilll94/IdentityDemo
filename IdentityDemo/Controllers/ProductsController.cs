using IdentityDemo.Authorization;
using IdentityDemo.Data;
using IdentityDemo.Models;
using IdentityDemo.Models.ProductsViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private IHostingEnvironment _hostingEnvironment;
        private IAuthorizationService _authorizationService;

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IHostingEnvironment hostingEnvironment, IAuthorizationService authorizationService)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _authorizationService = authorizationService;
        }

        // GET: Products
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Product.ToListAsync());
        }

        // GET: Products/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .SingleOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel productViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(productViewModel);
            }

            IFormFile imageFile = productViewModel.Image;
            var product = new Product()
            {
                Name = productViewModel.Name,
                ImageName = imageFile.FileName,
                Price = productViewModel.Price,
                Description = productViewModel.Description,
                SellerID = _userManager.GetUserId(User)
            };

            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, 
                "images", productViewModel.Image.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            _context.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");

        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.SingleOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(
                                                User, product.SellerID, ProductOperations.Update);

            if (!isAuthorized)
            {
                return new ChallengeResult();
            }

            var productViewModel = new ProductViewModel()
            {
                Name = product.Name,
                Price = product.Price,
                Description = product.Description
            };

            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("ProductID, Name,Price,Description")] ProductViewModel productViewModel,
            IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("ImagePath", "Zdjêcie jest wymagane");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var product = _context.Product.SingleOrDefault(p => p.ProductID == id);
                    if (product == null)
                    {
                        return NotFound();
                    }

                    var isAuthorized = await _authorizationService.AuthorizeAsync(
                                User, product.SellerID, ProductOperations.Update);
                    if (!isAuthorized)
                    {
                        return new ChallengeResult();
                    }

                    var filePath = _hostingEnvironment.ContentRootPath +
                        "\\wwwroot\\images\\" + file.FileName;

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    product.Name = product.Name;
                    product.ImageName = "~/images/" + file.FileName;
                    product.Price = product.Price;
                    product.Description = product.Description;
                    product.SellerID = _userManager.GetUserId(User);

                    _context.Update(product);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction("Index");
            }
            return View(productViewModel);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .SingleOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, product,
                                ProductOperations.Delete);
            if (!isAuthorized)
            {
                return new ChallengeResult();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.SingleOrDefaultAsync(m => m.ProductID == id);

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, product,
                                ProductOperations.Delete);
            if (!isAuthorized)
            {
                return new ChallengeResult();
            }

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductID == id);
        }
    }
}
