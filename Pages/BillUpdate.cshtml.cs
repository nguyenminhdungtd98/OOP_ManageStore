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
    public class BillUpdateModel : PageModel
    {
        IProductService productService;
        IProductTypeService productTypeService;
        IBillService billService;
        public int numberBill { get; set; }
        public DateTime createDate { get; set; }
        public int productNumber { get; set; }
        public string productName { get; set; }

        public DateTime expireDate { get; set; }

        public int number { get; set; }
        public string productType { get; set; }
        public List<ProductType> productTypes { get; set; }
        public List<Product> products { get; set; }
        [BindProperty(SupportsGet = true)]
        public int error { get; set; }

        [BindProperty(SupportsGet = true)]
        public int idUpdate { get; set; }
        public void OnGet()
        {
            try
            {
                productService = new ProductService();
                productTypeService = new ProductTypeService();
                billService = new BillService();

                if (error == Code.ZERO)
                {
                    SetAlert(ErrorMessage.ZERO, Code.ERROR);
                }
                else if (error == Code.NULL_VALUE)
                {
                    SetAlert(ErrorMessage.NULL_VALUE, Code.ERROR);
                }
                else if (error == Code.NOT_FOUND)
                {
                    SetAlert(ErrorMessage.NOT_FOUND, Code.ERROR);
                }
                else if (error == Code.ERROR)
                {
                    SetAlert(ErrorMessage.ERROR, Code.ERROR);
                }
                ServiceResult<List<ProductType>> serviceResult = productTypeService.FindAll();
                if (serviceResult.isSuccess)
                {
                    productTypes = serviceResult.data;
                }

                ServiceResult<List<Product>> productResult = productService.FindAll();
                if (serviceResult.isSuccess)
                {
                    products = productResult.data;
                }
                ServiceResult<Bill> billResult = billService.FindById(idUpdate);
                Bill bill = new Bill();
                if (billResult.isSuccess)
                {
                    bill = billResult.data;
                    numberBill = idUpdate;
                    productName = bill.productName;
                    productType = bill.productType;
                    createDate = bill.createDate;
                    number = bill.number;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Response.Redirect("/Bill?error=" + Code.ERROR);
            }
        }
        public void OnPost()
        {
            try
            {
                productService = new ProductService();
                productTypeService = new ProductTypeService();
                billService = new BillService();
                ServiceResult<int> productResult = productService.FindIdByName(productName);
                if (productResult.isSuccess)
                {
                    productNumber = productResult.data;
                }
                Bill bill = new Bill(numberBill, createDate, productNumber, productName, number, productType);

                ServiceResult<int> editResult = billService.Edit(bill, numberBill);
                if (editResult.isSuccess)
                {
                    Response.Redirect("/Bill?error=" + Code.SUCCESS);
                }
                else
                {
                    Response.Redirect("/Bill?error=" + Code.ERROR);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Response.Redirect("/BillUpdate?error=" + Code.ERROR);
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
