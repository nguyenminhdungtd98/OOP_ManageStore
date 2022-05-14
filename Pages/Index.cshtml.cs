using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using Services;

namespace DoAnQuanLyCuaHang.Pages
{
    [BindProperties]
    public class IndexModel : PageModel
    {
        IProductService productService;
        private readonly ILogger<IndexModel> _logger;

        public List<Product> products { get; set; }

        [BindProperty(SupportsGet = true)]
        public int id { get; set; }

        public int productNumber { get; set; }
        public string productName { get; set; }

        public DateTime expireDate { get; set; }

        public string company { get; set; }

        public DateTime dateOfManufacture { get; set; }

        public string productType { get; set; }

        public int price { get; set; }

        public string typeSearch { get; set; }

        public string key { get; set; }

        [BindProperty(SupportsGet = true)]
        public int error { get; set; }
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

            if (error.Equals(Code.DUPLICATE))
            {
                SetAlert(ErrorMessage.DUPLICATE, Code.ERROR);
            }
            else if (error.Equals(Code.ZERO))
            {
                SetAlert(ErrorMessage.ZERO, Code.ERROR);
            }
            else if (error.Equals(Code.SUCCESS))
            {
                SetAlert(ErrorMessage.SUCCESS, Code.SUCCESS);
            }
            else if (error.Equals(Code.EMPTY_PRODUCTTYPE))
            {
                SetAlert(ErrorMessage.EMPTY_PRODUCTTYPE, Code.ERROR);
            }
            productService = new ProductService();
            products = new List<Product>();

            ServiceResult<List<Product>> serviceResult = productService.FindAll();
            if (serviceResult.isSuccess)
            {
                products = serviceResult.data;
            }

            if (id > 0)
            {
                ServiceResult<bool> deleteResult = productService.DeleteById(id);
                if (deleteResult.isSuccess)
                {
                    Response.Redirect("index?error="+Code.SUCCESS);
                }
                else
                {
                    Response.Redirect("index?error=" + Code.ERROR);
                }
            }

        }
        public void OnPost()
        {
            productService = new ProductService();
            if (key != null)
            {
                ServiceResult<List<Product>> serviceResult = productService.Search(typeSearch,key);
                if (serviceResult.isSuccess)
                {
                    products = serviceResult.data;
                }
            }
        }

        protected void SetAlert(string message, int type)
        {
            TempData["AlertMessage"] = message;
            if (type == -2)
            {
                TempData["AlertType"] = "alert-success";

            }
            else if (type == 2)
            {
                TempData["AlertType"] = "alert-warning";
            }
            else if (type == -3)
            {
                TempData["AlertType"] = "alert-danger";
            }
            else
            {
                TempData["AlertType"] = "alert-info";
            }
        }
    }
}
