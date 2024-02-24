﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreFront.DATA.EF.Models;
using StoreFront.UI.MVC.Utilities;
using System.Drawing;
using X.PagedList;

namespace StoreFront.UI.MVC.Controllers
{
    [Authorize(Roles ="Admin")]
    public class ProductsController : Controller
    {
        private readonly StoreFrontContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(StoreFrontContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products
        [AllowAnonymous]
        public async Task<IActionResult> Index(bool? discontinued = false)
        {
            //Potentially redirect customers to the tiled index:
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction(nameof(TiledIndex));
            }
            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.ProductOrders);
            return View(await products.ToListAsync());
        }


        // GET: Products
        [AllowAnonymous]
        public async Task<IActionResult> TiledIndex(string? searchTerm, int categoryId =0, int page = 1)
        {
            var products =await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier).ToListAsync();
            if (searchTerm != null)
            {
                ViewBag.SearchTerm = searchTerm;
                searchTerm = searchTerm.ToLower();

                products = products.Where(p => p.SearchString.ToLower().Contains(searchTerm)).ToList();

                ViewBag.NbrResults = products.Count;
            }
            else { }

            ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "CategoryName", categoryId);
            if (categoryId != 0)
            {
                products = products.Where(p => p.CategoryId == categoryId).ToList();
                ViewBag.NbrResults = products.Count;
                var cat = await _context.Categories.FirstOrDefaultAsync(c=>c.CategoryId == categoryId);
                ViewBag.CatName = cat?.CategoryName;
                ViewBag.CatId = cat?.CategoryId;
            }

            return View(products.ToPagedList(page, 6));
        }

        // GET: Products/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Status)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles ="Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["StatusId"] = new SelectList(_context.ProductStatuses, "StatusId", "Status");
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "MainContact");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,Price,StatusId,SupplierId,CategoryId,Image,ImageFile")] Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.ImageFile != null && product.ImageFile.Length < 4_194_303)
                {

                    product.Image = Guid.NewGuid() + Path.GetExtension(product.ImageName);

                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string fullImagePath = webRootPath + "/img/";
                    using var memoryStream = new MemoryStream();
                    await product.ImageFile.CopyToAsync(memoryStream);
                    using Image img = Image.FromStream(memoryStream);
                    int maxImageSize = 500; // in pixels
                    int maxThumbSize = 100;
                    ImageUtility.ResizeImage(fullImagePath, product.Image, img, maxImageSize, maxThumbSize);
                }
                else
                {
                    product.Image = "noimage.png";
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["StatusId"] = new SelectList(_context.ProductStatuses, "StatusId", "Status", product.StatusId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "MainContact", product.SupplierId);
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["StatusId"] = new SelectList(_context.ProductStatuses, "StatusId", "Status", product.StatusId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "MainContact", product.SupplierId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,Price,StatusId,SupplierId,CategoryId,Image,ImageFile")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                #region File Upload - EDIT
                string? oldImageName = product.Image;

                if (product.ImageFile != null && product.ImageFile.Length < 4_194_303)
                {
                    product.Image = Guid.NewGuid() + Path.GetExtension(product.ImageName);
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string fullImagePath = webRootPath + "/img/";
                    if (oldImageName != null && oldImageName != "noimage.png")
                    {
                        ImageUtility.Delete(fullImagePath, oldImageName);
                    }
                    using var memoryStream = new MemoryStream();
                    await product.ImageFile.CopyToAsync(memoryStream);
                    using Image img = Image.FromStream(memoryStream); 
                    int maxImageSize = 500;
                    int maxThumbSize = 100;
                    ImageUtility.ResizeImage(fullImagePath, product.Image, img, maxImageSize, maxThumbSize);
                }
                else
                {
                    product.Image = oldImageName;
                }
                #endregion


                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["StatusId"] = new SelectList(_context.ProductStatuses, "StatusId", "Status", product.StatusId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "MainContact", product.SupplierId);
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Status)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'StoreFrontContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
