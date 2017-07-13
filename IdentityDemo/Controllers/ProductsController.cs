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
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, 
                "images", product.Image.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await product.Image.CopyToAsync(stream);
            }

            product.ImagePath = product.Image.FileName;
            product.SellerID = _userManager.GetUserId(User);

            _context.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");

        }

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

            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var productInDb = _context.Product.AsNoTracking().
                        SingleOrDefault(p => p.ProductID == id);
                    if (productInDb == null)
                    {
                        return NotFound();
                    }

                    var isAuthorized = await _authorizationService.AuthorizeAsync(
                                User, productInDb.SellerID, ProductOperations.Update);
                    if (!isAuthorized)
                    {
                        return new ChallengeResult();
                    }

                    string filePath = Path.Combine(_hostingEnvironment.WebRootPath,
                        "images", product.Image.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.Image.CopyToAsync(stream);
                    }
                    product.ImagePath = product.Image.FileName;
                    product.SellerID = _userManager.GetUserId(User);

                    _context.Update(product);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction("Index");
            }
            return View(product);
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

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, product.SellerID,
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

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, product.SellerID,
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
