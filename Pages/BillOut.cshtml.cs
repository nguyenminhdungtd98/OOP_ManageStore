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
    public class BillOutModel : PageModel
    {
        IBillOutService billOutService;
        IProductService productService;
        IProductTypeService productTypeService;
        IWarehouseService warehouseService;
        public int numberBillOut { get; set; }
        public DateTime createDate { get; set; }

        public int productNumber { get; set; }
        public string productName { get; set; }

        public int price { get; set; }
        public int number { get; set; }

        public int total { get; set; }
        public List<ProductType> productTypes { get; set; }
        public List<Product> products { get; set; }

        public List<Warehouse> productsInStock { get; set; }

        public List<ProductInBill> productInBills { get; set; }
        public List<BillOut> billOuts { get; set; }
        [BindProperty(SupportsGet = true)]
        public int error { get; set; }
        [BindProperty(SupportsGet = true)]
        public int idDeleteP { get; set; }

        public string save { get; set; }
        public void OnGet()
        {
            Initialize();
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
            else if (error == Code.OUTSTOCK)
            {
                SetAlert(ErrorMessage.OUTSTOCK, Code.ERROR);
            }
            ServiceResult<List<Warehouse>> warehouseResult = warehouseService.FindAll();
            if (warehouseResult.isSuccess)
            {
                productsInStock = warehouseResult.data;
            }
            ServiceResult<List<ProductType>> productTypeResult = productTypeService.FindAll();
            if (productTypeResult.isSuccess)
            {
                productTypes = productTypeResult.data;
            }
            ServiceResult<List<Product>> productResult = productService.FindAll();
            if (productResult.isSuccess)
            {
                products = productResult.data;
            }
            ServiceResult<int> numberBillOutResult = billOutService.GetMaxId();
            if (numberBillOutResult.isSuccess)
            {
                numberBillOut = numberBillOutResult.data + 1;
            }
            createDate = DateTime.Now;
            ServiceResult<List<ProductInBill>> productInBillResult = billOutService.FindAllProductInBill();
            if (productInBillResult.isSuccess)
            {
                productInBills = productInBillResult.data;
            }
            ServiceResult<bool> deleteAllBillOut = billOutService.DeleteAllBillOutTemp();
            if (idDeleteP > 0)
            {
                ServiceResult<bool> deleteP = billOutService.DeleteProductInBill(idDeleteP);
                if (deleteP.isSuccess)
                {
                    Response.Redirect("/BillOut");
                }

            }
        }
        private void Initialize()
        {
            billOutService = new BillOutService();
            productService = new ProductService();
            productTypeService = new ProductTypeService();
            warehouseService = new WarehouseService();
        }
        public void OnPost()
        {
            Initialize();
            try
            {
                ServiceResult<int> getIdMax = billOutService.GetMaxId();
                if (getIdMax.isSuccess)
                {
                    numberBillOut = getIdMax.data + 1;
                }
                if (save == null)
                {
                    ServiceResult<int> getIdResult = productService.FindIdByName(productName);
                    int id = 0;
                    if (getIdResult.isSuccess)
                    {
                        id = getIdResult.data;
                    }
                    productNumber = id;
                    ServiceResult<Product> getProduct = productService.FindById(id);
                    Product product = new Product();
                    if (getProduct.isSuccess)
                    {
                        product = getProduct.data;
                    }

                    //BillOut billOut = new BillOut();
                    //billOut.numberBillOut = numberBillOut + 1;
                    //billOut.createDate = DateTime.Now;
                    //billOuts = new List<BillOut>();
                    //billOuts.Add(billOut);

                    ProductInBill productInBill = new ProductInBill();
                    productInBill.productNumber = productNumber;
                    productInBill.productName = productName;
                    productInBill.number = number;
                    productInBill.price = product.price;
                    productInBill.total = product.price * number;

                    ServiceResult<bool> checkNumber = warehouseService.CheckNumberProduct(productInBill);
                    if (!checkNumber.isSuccess)
                    {
                        Response.Redirect("/BillOut?error=" + Code.OUTSTOCK);
                    }
                    else
                    {
                        ServiceResult<bool> saveProductInBill = billOutService.SaveProductInBill(productInBill);
                        if (saveProductInBill.isSuccess)
                        {
                            Response.Redirect("/BillOut");
                        } else if (saveProductInBill.message.Equals(ErrorMessage.ZERO))
                        {
                            Response.Redirect("/BillOut?error=" + Code.ZERO);
                        }

                    }
                }
                else
                {
                    ServiceResult<List<ProductInBill>> productInBillResult = billOutService.FindAllProductInBill();
                    if (productInBillResult.isSuccess)
                    {
                        productInBills = productInBillResult.data;
                    }
                    BillOut billOut = new BillOut();
                    billOut.numberBillOut = numberBillOut;
                    billOut.productInBill = productInBills;
                    billOut.createDate = DateTime.Now;
                    ServiceResult<int> saveBillOut = billOutService.Save(billOut);
                    if (saveBillOut.isSuccess)
                    {
                        Response.Redirect("/BillOutManage?error=" + Code.SUCCESS);
                    }
                    else
                    {
                        Response.Redirect("/BillOutManage?error=" + Code.ERROR);
                    }

 

                }



            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Response.Redirect("/BillOut?error=" + Code.ERROR);
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
