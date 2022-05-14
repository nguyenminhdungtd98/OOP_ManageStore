using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using Entities;

namespace DoAnQuanLyCuaHang.Pages
{
    [BindProperties]
    public class UpdateModel : PageModel
    {
        IProductService productService;
        [BindProperty(SupportsGet = true)]
        public int id { get; set; }
        public int productNumber { get; set; }
        public string productName { get; set; }

        public DateTime expireDate { get; set; }

        public string company { get; set; }

        public DateTime dateOfManufacture { get; set; }

        public string productType { get; set; }

        public int price { get; set; }
        public void OnGet()
        {
            productService = new ProductService();
            if (id > 0)
            {
                ServiceResult<int> serviceResult = productService.CheckExsitById(id);
                if (!serviceResult.isSuccess)
                {
                    SetAlert("Mã sản phẩm không tồn tại", 3);
                    Response.Redirect("/index?error=" + Code.NOT_FOUND);
                }
                else
                {
                    ServiceResult<Product> productResult = productService.FindById(id);
                    Product product = new Product();
                    if (productResult.isSuccess)
                    {
                        product = productResult.data;
                        if (product.productNumber > 0)
                        {
                            productNumber = product.productNumber;
                            productName = product.productName;
                            expireDate = product.expireDate;
                            company = product.company;
                            dateOfManufacture = product.dateOfManufacture;
                            productType = product.productType;
                            price = product.price;
                        }
                    }
                }
            }
        }
        public void OnPost()
        {
            productService = new ProductService();
            Product product = new Product(productNumber, productName, expireDate, company, dateOfManufacture, productType, price);
            ServiceResult<int> productResult = productService.Edit(product,id);
            if (productResult.isSuccess)
            {
                Response.Redirect("/index?error=" + Code.SUCCESS);
            }else if (productResult.message.Equals(ErrorMessage.DUPLICATE))
            {
                Response.Redirect("/Update?error=" + Code.DUPLICATE);
            } else if (productResult.message.Equals(ErrorMessage.ZERO))
            {
                Response.Redirect("/Update?error=" + Code.ZERO);
            }
        }
        protected void SetAlert(string message, int type)
        {
            TempData["AlertMessage"] = message;
            if (type == Code.SUCCESS)
            {
                TempData["AlertType"] = "alert-success";

            }
            else if (type == 2)
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
