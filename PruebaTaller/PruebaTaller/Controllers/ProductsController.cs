using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PruebaTaller.Models;
using PruebaTaller.Services;
using OfficeOpenXml; // Para manejar archivos Excel
using System.IO;
using System.Collections.Generic;

namespace PruebaTaller.Controllers
{
    /// <summary>
    /// Controlador para gestionar las operaciones CRUD relacionadas con productos.
    /// Permite la creación, edición, eliminación y listado de productos, así como la carga masiva desde archivos Excel.
    /// Solo accesible por usuarios con el rol de "admin".
    /// </summary>
    [Authorize(Roles = "admin")]
    public class ProductsController : Controller
    {
        // Variables globales
        private readonly IWebHostEnvironment _environment; // Entorno de la aplicación
        private readonly ApplicationDbContext _context; // Contexto de la base de datos
        private readonly int pageSize = 5; // Tamaño de página para la paginación

        /// <summary>
        /// Constructor que inicializa el controlador con el contexto de base de datos y el entorno de la aplicación.
        /// </summary>
        /// <param name="context">El contexto de base de datos de la aplicación.</param>
        /// <param name="environment">El entorno de hospedaje web.</param>
        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this._environment = environment;
            this._context = context;
        }

        /// <summary>
        /// Muestra la vista principal de productos con paginación y búsqueda.
        /// </summary>
        /// <param name="pageIndex">El número de la página a mostrar.</param>
        /// <param name="search">Cadena de búsqueda para filtrar productos por nombre o marca.</param>
        /// <returns>Vista de productos paginada y filtrada.</returns>
        public IActionResult Index(int pageIndex, string? search)
        {
            IQueryable<Product> query = _context.Products;

            // Filtrar los productos según la búsqueda
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.Brand.Contains(search));
            }

            // Ordenar los productos de forma descendente por ID
            query = query.OrderByDescending(p => p.ProductId);

            // Paginación
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            // Pasar datos a la vista mediante ViewData
            ViewData["PageIndex"] = pageIndex;
            ViewData["TotalPages"] = totalPages;
            ViewData["Search"] = search ?? "";
            var products = query.ToList();

            return View(products);
        }

        /// <summary>
        /// Muestra la vista para crear un nuevo producto.
        /// </summary>
        /// <returns>Vista de creación de producto.</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Procesa la creación de un nuevo producto.
        /// </summary>
        /// <param name="productDto">DTO que contiene los datos del producto a crear.</param>
        /// <returns>Redirige a la vista principal si la creación es exitosa, de lo contrario muestra los errores.</returns>
        [HttpPost]
        public IActionResult Create(ProductDTOO productDto)
        {
            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            // Generar el nombre del archivo de la imagen
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(productDto.ImageFile!.FileName);

            // Definir la ruta completa del archivo
            string imageFullPath = Path.Combine(_environment.WebRootPath, "images", "products");

            // Crear el directorio si no existe
            if (!Directory.Exists(imageFullPath))
            {
                Directory.CreateDirectory(imageFullPath);
            }

            // Guardar la imagen en el directorio
            imageFullPath = Path.Combine(imageFullPath, newFileName);
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            // Crear el producto a partir del DTO
            var product = new Product
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now
            };

            // Agregar el producto a la base de datos
            _context.Products.Add(product);
            _context.SaveChanges();

            TempData["Success"] = "Product created successfully.";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Muestra la vista de edición de un producto existente.
        /// </summary>
        /// <param name="id">ID del producto a editar.</param>
        /// <returns>Vista de edición del producto, o redirige si no se encuentra.</returns>
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            var productDto = new ProductDTOO()
            {
                Id = product.ProductId,
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,
                CreatedAt = product.CreatedAt
            };

            return View(productDto);
        }

        /// <summary>
        /// Procesa la edición de un producto existente.
        /// </summary>
        /// <param name="id">ID del producto a editar.</param>
        /// <param name="productDto">DTO con los nuevos datos del producto.</param>
        /// <returns>Redirige a la vista principal si la edición es exitosa, o muestra errores.</returns>
        [HttpPost]
        public IActionResult Edit(int id, ProductDTOO productDto)
        {
            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            var product = _context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            // Actualizar el producto con los datos del DTO
            product.Name = productDto.Name;
            product.Brand = productDto.Brand;
            product.Category = productDto.Category;
            product.Price = productDto.Price;
            product.Description = productDto.Description;

            _context.SaveChanges();

            TempData["Success"] = "Product updated successfully.";
            return RedirectToAction("Index", "Products");
        }

        /// <summary>
        /// Muestra la vista para confirmar la eliminación de un producto.
        /// </summary>
        /// <param name="id">ID del producto a eliminar.</param>
        /// <returns>Vista de confirmación de eliminación del producto.</returns>
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            return View(product);
        }

        /// <summary>
        /// Procesa la eliminación de un producto de forma confirmada.
        /// </summary>
        /// <param name="id">ID del producto a eliminar.</param>
        /// <returns>Redirige a la vista principal si la eliminación es exitosa.</returns>
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            _context.Products.Remove(product);
            _context.SaveChanges();

            return RedirectToAction("Index", "Products");
        }

        /// <summary>
        /// Permite la carga masiva de productos mediante un archivo Excel.
        /// </summary>
        /// <param name="excelFile">Archivo Excel que contiene los productos a cargar.</param>
        /// <returns>Redirige a la vista principal si la carga es exitosa, o muestra errores.</returns>
        [HttpPost]
        public IActionResult UploadExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                TempData["Error"] = "Please upload a valid Excel file.";
                return RedirectToAction("Index");
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var stream = new MemoryStream())
            {
                excelFile.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    if (worksheet == null)
                    {
                        TempData["Error"] = "The Excel file is empty or not properly formatted.";
                        return RedirectToAction("Index");
                    }

                    int rowCount = worksheet.Dimension.Rows;
                    var errorMessages = new List<string>();

                    for (int row = 2; row <= rowCount; row++)
                    {
                        try
                        {
                            string name = worksheet.Cells[row, 1].Value?.ToString()?.Trim() ?? string.Empty;
                            string brand = worksheet.Cells[row, 2].Value?.ToString()?.Trim() ?? string.Empty;
                            string category = worksheet.Cells[row, 3].Value?.ToString()?.Trim() ?? string.Empty;
                            string priceStr = worksheet.Cells[row, 4].Value?.ToString()?.Trim() ?? string.Empty;
                            string description = worksheet.Cells[row, 5].Value?.ToString()?.Trim() ?? string.Empty;
                            string imageFileName = worksheet.Cells[row, 6].Value?.ToString()?.Trim() ?? string.Empty;

                            // Validaciones y procesamiento de datos
                            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(brand) || string.IsNullOrEmpty(category) ||
                                string.IsNullOrEmpty(priceStr) || !decimal.TryParse(priceStr, out decimal price) || price < 0 ||
                                string.IsNullOrEmpty(description) || string.IsNullOrEmpty(imageFileName))
                            {
                                errorMessages.Add($"Row {row}: Missing or invalid required data.");
                                continue;
                            }

                            // Verificar la existencia del archivo de imagen
                            string productsPath = Path.Combine("images/products", imageFileName);
                            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", productsPath);
                            if (!System.IO.File.Exists(fullPath))
                            {
                                errorMessages.Add($"Row {row}: Image '{imageFileName}' not found in server path.");
                                continue;
                            }

                            // Crear el producto y agregarlo al contexto
                            var product = new Product
                            {
                                Name = name,
                                Brand = brand,
                                Category = category,
                                Price = price,
                                Description = description,
                                ImageFileName = imageFileName,
                                CreatedAt = DateTime.Now
                            };

                            _context.Products.Add(product);
                        }
                        catch (Exception ex)
                        {
                            errorMessages.Add($"Row {row}: Error processing data - {ex.Message}");
                        }
                    }

                    if (errorMessages.Count > 0)
                    {
                        TempData["Error"] = string.Join("<br/>", errorMessages);
                    }
                    else
                    {
                        _context.SaveChanges();
                        TempData["Success"] = "Products have been successfully uploaded.";
                    }
                }
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Muestra los detalles de un producto específico.
        /// </summary>
        /// <param name="id">ID del producto a visualizar.</param>
        /// <returns>Vista con los detalles del producto, o redirige si no se encuentra.</returns>
        public IActionResult Details(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return RedirectToAction("Index");
            }

            return View(product);
        }
    }
}
