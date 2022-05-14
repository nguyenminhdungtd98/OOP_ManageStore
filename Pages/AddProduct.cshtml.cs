using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Entities;
using Services;
namespace DoAnQuanLyCuaHang.Pages
{
    [BindProperties]
    public class AddProductModel : PageModel
    {
        IProductService productService;
        IProductTypeService productTypeService;
        public int productNumber { get; set; }
        public string productName { get; set; }

        public DateTime expireDate { get; set; }

        public string company { get; set; }

        public DateTime dateOfManufacture { get; set; }

        public string productType { get; set; }

        public int price { get; set; }

        public List<ProductType> productTypes { get; set; }
        [BindProperty(SupportsGet = true)]
        public int error { get; set; }
        public void OnGet()
        {
            productService = new ProductService();
            productTypeService = new ProductTypeService();
            ServiceResult<List<ProductType>> serviceResult = productTypeService.FindAll();
            if (serviceResult.isSuccess)
            {
                productTypes = serviceResult.data;
            }
            if (productTypes == null || productTypes.Count == 0)
            {
                Response.Redirect("/index?error=" + Code.EMPTY_PRODUCTTYPE);
            }
            if (error.Equals(Code.DUPLICATE))
            {
                SetAlert(ErrorMessage.DUPLICATE, Code.ERROR);
            }
            else if (error.Equals(Code.ZERO))
            {
                SetAlert(ErrorMessage.ZERO, Code.ERROR);
            }
        }

        public void OnPost()
        {
            productService = new ProductService();
            productTypeService = new ProductTypeService();
            try
            {
                Product product = new Product(productNumber, productName, expireDate, company, dateOfManufacture, productType, price);
                ServiceResult<int> serviceResult = productService.Save(product);

                if (serviceResult.isSuccess)
                {
                    Response.Redirect("/index?error=" + Code.SUCCESS);
                }
                else if (serviceResult.message.Equals(ErrorMessage.DUPLICATE))
                {
                    Response.Redirect("/AddProduct?error=" + Code.DUPLICATE);
                }
                else if (serviceResult.message.Equals(ErrorMessage.ZERO))
                {
                    Response.Redirect("/AddProduct?error=" + Code.ZERO);
                }

            }
            catch (Exception e)
            {
                SetAlert(e.Message, Code.ERROR);
            }
        }
        protected void SetAlert(string message, int type)
        {
            TempData["AlertMessage"] = message;
            if (type == Code.SUCCESS)
            {
                TempData["AlertType"] = "alert-success";

            }
            else if (type == -2)
            {
                TempData["AlertType"] = "alert-warning";
            }
            else if (type == Code.ERROR)
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
