using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using TSensor.Web.Models.Entity;
using TSensor.Web.Models.Repository;
using TSensor.Web.ViewModels;
using TSensor.Web.ViewModels.Product;

namespace TSensor.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [Authorize(Policy = "Admin")]
        [Route("product/list")]
        public IActionResult List()
        {
            var viewModel = new SearchViewModel<Product>
            {
                Data = _productRepository.List()
            };

            var successMessage = TempData["Product.List.SuccessMessage"] as string;
            if (!string.IsNullOrEmpty(successMessage))
            {
                viewModel.SuccessMessage = successMessage;
            }
            var errorMessage = TempData["Product.List.ErrorMessage"] as string;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                viewModel.ErrorMessage = errorMessage;
            }

            return View(viewModel);
        }

        [Authorize(Policy = "Admin")]
        [Route("product/new")]
        public IActionResult Create()
        {
            var viewModel = new ProductCreateEditViewModel();

            return View(viewModel);
        }

        [Authorize(Policy = "Admin")]
        [Route("product/new")]
        [HttpPost]
        public IActionResult Create(ProductCreateEditViewModel viewModel)
        {
            if (viewModel == null)
            {
                return RedirectToAction("Create", "Product");
            }

            viewModel.Name = viewModel.Name?.Trim();

            if (ModelState.IsValid)
            {
                var productGuid = _productRepository.Create(viewModel.Name, viewModel.IsGas);
                if (productGuid == null)
                {
                    viewModel.ErrorMessage = Program.GLOBAL_ERROR_MESSAGE;
                }
                else
                {
                    var productUrl = Url.Action("Edit", "Product", new { productGuid });
                    TempData["Product.List.SuccessMessage"] =
                        $"Продукт <a href=\"{productUrl}\">\"{viewModel.Name}\"</a> создан";

                    return RedirectToAction("List", "Product");
                }
            }

            return View(viewModel);
        }

        [Authorize(Policy = "Admin")]
        [Route("product/{productGuid}")]
        public IActionResult Edit(string productGuid)
        {
            if (Guid.TryParse(productGuid, out var _productGuid))
            {
                var product = _productRepository.GetByGuid(_productGuid);
                if (product != null)
                {
                    var viewModel = new ProductCreateEditViewModel
                    {
                        ProductGuid = product.ProductGuid,
                        Name = product.Name,
                        IsGas = product.IsGas
                    };

                    var successMessage = TempData["Product.Edit.SuccessMessage"] as string;
                    if (!string.IsNullOrEmpty(successMessage))
                    {
                        viewModel.SuccessMessage = successMessage;
                    }
                    var errorMessage = TempData["Product.Edit.ErrorMessage"] as string;
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        viewModel.ErrorMessage = errorMessage;
                    }

                    return View(viewModel);
                }
            }

            return NotFound();
        }

        [Authorize(Policy = "Admin")]
        [Route("product/{productGuid}")]
        [HttpPost]
        public IActionResult Edit(ProductCreateEditViewModel viewModel)
        {
            if (viewModel == null)
            {
                return RedirectToAction("List", "Product");
            }

            viewModel.Name = viewModel.Name?.Trim();

            if (ModelState.IsValid)
            {
                var editResult = _productRepository.Edit(viewModel.ProductGuid, viewModel.Name, viewModel.IsGas);
                if (editResult)
                {
                    var productUrl = Url.Action("Edit", "Product", new { productGuid = viewModel.ProductGuid });
                    TempData["Product.List.SuccessMessage"] =
                        $"Продукт <a href=\"{productUrl}\">\"{viewModel.Name}\"</a> изменен";

                    return RedirectToAction("List", "Product");
                }
                else
                {
                    viewModel.ErrorMessage = Program.GLOBAL_ERROR_MESSAGE;
                }
            }

            return View(viewModel);
        }

        [Authorize(Policy = "Admin")]
        [Route("product/remove")]
        [HttpPost]
        public IActionResult Remove(string productGuid)
        {
            if (!Guid.TryParse(productGuid, out var _productGuid))
            {
                return NotFound();
            }
            else
            {
                if (_productRepository.Remove(_productGuid))
                {
                    TempData["Product.List.SuccessMessage"] = "Продукт удален";
                }
                else
                {
                    TempData["Product.List.ErrorMessage"] = "При удалении продукта произошла ошибка";
                }
                return RedirectToAction("List", "Product");
            }
        }

        private new IActionResult NotFound()
        {
            ViewBag.Title = "Продукт не найден";
            ViewBag.BackTitle = "назад к списку продуктов";
            ViewBag.BackUrl = Url.ActionLink("List", "Product");

            return View("NotFound");
        }
    }
}