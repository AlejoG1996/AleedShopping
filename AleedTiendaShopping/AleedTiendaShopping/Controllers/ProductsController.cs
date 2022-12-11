using AleedTiendaShopping.Data;
using AleedTiendaShopping.Data.Entities;
using AleedTiendaShopping.Helpers;
using AleedTiendaShopping.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System;
using System.IO;
using Vereyon.Web;
using static AleedTiendaShopping.Helpers.ModalHelper;

namespace AleedTiendaShopping.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IFlashMessage _flashMessage;


        public ProductsController(DataContext context, ICombosHelper combosHelper, IBlobHelper blobHelper, IFlashMessage flashMessage)
        {
            _context = context;
            _combosHelper = combosHelper;
            _blobHelper = blobHelper;
            _flashMessage = flashMessage;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .ToListAsync());
        }

        [NoDirectAccess]
        public async Task<IActionResult> Create()
        {
            CreateProductViewModel model = new()
            {
                Categories = await _combosHelper.GetComboCategoriesAsync(),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                //guardar img en azure
                //Guid imageId = Guid.Empty;
                //if (model.ImageFile != null)
                //{
                //    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "products");
                //}

                //validar img
                //validacion de imagenes
                string filename = "";
                if (model.ImageFile == null)
                {

                    filename = "noexiste.png";
                }
                else
                {
                    filename = model.ImageFile.FileName;
                }


                if (!Regex.IsMatch(filename.ToLower(), @"^.*\.(jpg|gif|png|jpeg)$"))
                {
                    _flashMessage.Danger(string.Empty, "la imagen debe ser tipo .jpg .gift .png .jpeg");
                    model.Categories = await _combosHelper.GetComboCategoriesAsync();

                   

                }
                else
                {
                    //almacenamiento foto
                    string ruta = "https://localhost:7116/images/noimage.png";
                    string path = "";
                    string pic = "";
                    string nameimg = "noimage.png";
                    if (model.ImageFile != null)
                    {

                        pic = Path.GetFileName(model.Name.ToString() + ".png");

                        path = Path.Combine("wwwroot\\images\\products", pic);
                        ruta = "https://localhost:7116/images/products/" + pic;
                        nameimg = pic;
                        using (FileStream stream = new FileStream(path, FileMode.Create))
                        {
                            model.ImageFile.CopyTo(stream);


                        }


                    }

                    Products product = new()
                    {
                        Description = model.Description,
                        Name = model.Name,
                        Price = model.Price,
                        Stock = model.Stock,
                    };

                    product.ProductCategories = new List<ProductCategory>()
                    {
                        new ProductCategory
                           {
                              Category = await _context.Categories.FindAsync(model.CategoryId)
                           }
                    };

                    if (ruta != "https://localhost:7116/images/noimage.png")

                    {
                        product.ProductImages = new List<ProductImage>()
                       {
                           new ProductImage { ImageFullPath = ruta, Name=nameimg }
                       };
                    }

                    try
                    {
                        _context.Add(product);
                        await _context.SaveChangesAsync();
                        _flashMessage.Confirmation("Registro creado.");
                        return Json(new
                        {
                            isValid = true,
                            html = ModalHelper.RenderRazorViewToString(this, "_ViewAllProducts", _context.Products
                            .Include(p => p.ProductImages)
                            .Include(p => p.ProductCategories)
                            .ThenInclude(pc => pc.Category).ToList())
                        });

                    }
                    catch (DbUpdateException dbUpdateException)
                    {
                        if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                        {
                            _flashMessage.Danger("Ya existe un producto con el mismo nombre.");
                        }
                        else
                        {
                            _flashMessage.Danger(string.Empty, dbUpdateException.InnerException.Message);
                        }
                    }
                    catch (Exception exception)
                    {
                        _flashMessage.Danger(string.Empty, exception.Message);
                    }
                }
               
            }

            model.Categories = await _combosHelper.GetComboCategoriesAsync();
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "Create", model) });
        }

        [NoDirectAccess]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Products product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            EditProductViewModel model = new()
            {
                Description = product.Description,
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateProductViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            try
            {
                Products product = await _context.Products.FindAsync(model.Id);
                product.Description = model.Description;
                product.Name = model.Name;
                product.Price = model.Price;
                product.Stock = model.Stock;
                _context.Update(product);
                await _context.SaveChangesAsync();
                _flashMessage.Confirmation("Registro actualizado.");
                return Json(new
                {
                    isValid = true,
                    html = ModalHelper.RenderRazorViewToString(this, "_ViewAllProducts", _context.Products
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category).ToList())
                });
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                {
                    _flashMessage.Danger("Ya existe un producto con el mismo nombre.");
                }
                else
                {
                    _flashMessage.Danger(string.Empty, dbUpdateException.InnerException.Message);
                }
            }
            catch (Exception exception)
            {
                _flashMessage.Danger(string.Empty, exception.Message);
            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "Edit", model) });
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Products product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Products product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstAsync(pr =>pr.Id==id);
            if (product == null)
            {
                return NotFound();
            }

            AddProductImageViewModel model = new()
            {
                ProductId = product.Id,
                ImagesNumber=product.ImagesNumber,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImage(AddProductImageViewModel model)
        {
            if (ModelState.IsValid)
            {
                //guarda imagen azure
                //Guid imageId = Guid.Empty;
                //if (model.ImageFile != null)
                //{
                //    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "products");
                //}

                //validacion de imagenes
                string filename = "";
                if (model.ImageFile == null)
                {

                    filename = "noexiste.png";
                }
                else
                {
                    filename = model.ImageFile.FileName;
                }


                if (!Regex.IsMatch(filename.ToLower(), @"^.*\.(jpg|gif|png|jpeg)$"))
                {
                    _flashMessage.Danger("la imagen debe ser tipo .jpg .gift .png .jpeg");
                   

                  

                }
                else
                {
                    Products product = await _context.Products
                   .Include(c => c.ProductImages)
                   .FirstOrDefaultAsync(c => c.Id == model.ProductId);
                    //almacenamiento foto
                    string ruta = "https://localhost:7116/images/noimage.png";
                    string path = "";
                    string pic = "";
                    if (model.ImageFile != null)
                    {

                        pic = Path.GetFileName(product.Name.ToString() + product.ImagesNumber.ToString() + ".png");

                        path = Path.Combine("wwwroot\\images\\products", pic);
                        ruta = "https://localhost:7116/images/products/" + pic;

                        using (FileStream stream = new FileStream(path, FileMode.Create))
                        {
                            model.ImageFile.CopyTo(stream);


                        }


                    }

                    ProductImage productImage = new()
                    {
                        Product = product,
                        ImageFullPath = ruta,
                        Name = pic,
                    };

                    try
                    {
                        _context.Add(productImage);
                        await _context.SaveChangesAsync();
                        _flashMessage.Info(" imagen agregada");
                        return Json(new
                        {
                            isValid = true,
                            html = ModalHelper.RenderRazorViewToString(this, "Details", _context.Products
                                           .Include(p => p.ProductImages)
                                           .Include(p => p.ProductCategories)
                                           .ThenInclude(pc => pc.Category)
                                           .FirstOrDefaultAsync(p => p.Id == model.ProductId))
                        });
                    }
                    catch (Exception exception)
                    {
                        _flashMessage.Danger(string.Empty, exception.Message);
                    }
                }
               
            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddImage", model) });
        }


        public async Task<IActionResult> DeleteImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductImage productImage = await _context.ProductImages
                .Include(pi => pi.Product)
                .FirstOrDefaultAsync(pi => pi.Id == id);
            if (productImage == null)
            {
                return NotFound();
            }

            // await _blobHelper.DeleteBlobAsync(productImage.ImageId, "products");
            //eliminar foto
            try
            {
                string path = Path.Combine("wwwroot\\images\\products", productImage.Name);
                System.IO.File.Delete(path);
            }
            catch (Exception)
            {

                throw;
            }
           
               
            
            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();
            _flashMessage.Info("Registro Borrado");
            return RedirectToAction(nameof(Details), new { Id = productImage.Product.Id });
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddCategory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Products product = await _context.Products
                .Include(p=> p.ProductCategories)
                .ThenInclude(pc=> pc.Category)
                .FirstOrDefaultAsync(p=>p.Id==id);
            if (product == null)
            {
                return NotFound();
            }
            List<Category> categories = product.ProductCategories.Select(pc => new Category
            {
                Id = pc.Category.Id,
                Name = pc.Category.Name,
            }).ToList();
            AddCategoryProductViewModel model = new()
            {
                ProductId = product.Id,
                Categories = await _combosHelper.GetComboCategoriesAsync(categories),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(AddCategoryProductViewModel model)
        {
            Products product = await _context.Products
        .Include(p => p.ProductCategories)
        .ThenInclude(pc => pc.Category)
        .FirstOrDefaultAsync(p => p.Id == model.ProductId);
            if (ModelState.IsValid)
            {
               
                ProductCategory productCategory = new()
                {
                    Category = await _context.Categories.FindAsync(model.CategoryId),
                    Product = product,
                };

                try
                {
                    _context.Add(productCategory);
                    await _context.SaveChangesAsync();
                    _flashMessage.Danger("Categoria agregada.");
                    return Json(new
                    {
                        isValid = true,
                        html = ModalHelper.RenderRazorViewToString(this, "Details", _context.Products
                                       .Include(p => p.ProductImages)
                                       .Include(p => p.ProductCategories)
                                       .ThenInclude(pc => pc.Category)
                                       .FirstOrDefaultAsync(p => p.Id == model.ProductId))
                    });
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(string.Empty, exception.Message);
                }
            }
     

            List<Category> categories = product.ProductCategories.Select(pc => new Category
            {
                Id = pc.Category.Id,
                Name = pc.Category.Name,
            }).ToList();
            model.Categories = await _combosHelper.GetComboCategoriesAsync(categories);
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddCategory", model) });

        }

        public async Task<IActionResult> DeleteCategory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductCategory productCategory = await _context.ProductCategories
                .Include(pc => pc.Product)
                .FirstOrDefaultAsync(pc => pc.Id == id);
            if (productCategory == null)
            {
                return NotFound();
            }

            _context.ProductCategories.Remove(productCategory);
            await _context.SaveChangesAsync();
            _flashMessage.Info("Registro Borrado");
            return RedirectToAction(nameof(Details), new { Id = productCategory.Product.Id });
        }



        [NoDirectAccess]
        public async Task<IActionResult> Delete(Products model)
        {
            Products product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductCategories)
                .FirstOrDefaultAsync(pi => pi.Id == model.Id);
            if (product == null)
            {
                return NotFound();
            }

            //borrar azure
            //foreach (ProductImage productImage in product.ProductImages)
            //{
            //    await _blobHelper.DeleteBlobAsync(productImage.ImageId, "products");
            //}



            

            //borar carpeta
            foreach (ProductImage productImage in product.ProductImages)
            {
                try
                {
                    string path = Path.Combine("wwwroot\\images\\products", productImage.Name);
                    System.IO.File.Delete(path);
                }
                catch (Exception)
                {

                    throw;
                }

            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            _flashMessage.Info("Registro Borrado");
            return RedirectToAction(nameof(Index));
        }



    }

}
