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
    public class BillAddModel : PageModel
    {
        IBillService billService;
        IProductService productService;
        IProductTypeService productTypeService;
        IWarehouseService warehouseService;
        public int numberBill { get; set; }
        public DateTime createDate { get; set; }
        public int productNumber { get; set; }
        public string productName { get; set; }

        public DateTime expireDate { get; set; }

        public int number { get; set; }
        public List<ProductType> productTypes { get; set; }
        public List<Product> products { get; set; }
        [BindProperty(SupportsGet = true)]
        public int error { get; set; }

        public void OnGet()
        {
            try
            {
                billService = new BillService();
                productService = new ProductService();
                productTypeService = new ProductTypeService();
                warehouseService = new WarehouseService();
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
                ServiceResult<int> getIdResult = billService.GetMaxId();
                if(getIdResult.isSuccess){
                    numberBill = getIdResult.data + 1;
                }
                else
                {
                    Response.Redirect("BillAdd?error=" + Code.ERROR);
                }
                ServiceResult<List<Product>> productsResult = productService.FindAll();
                if (productsResult.isSuccess)
                {
                    products = productsResult.data;
                }
                else
                {
                    Response.Redirect("BillAdd?error=" + Code.ERROR);
                }
                ServiceResult<List<ProductType>> productTypesResult = productTypeService.FindAll();
                if (productTypesResult.isSuccess)
                {
                    productTypes = productTypesResult.data;
                }
                else
                {
                    Response.Redirect("BillAdd?error=" + Code.ERROR);
                }
                createDate = DateTime.Now;
            }
            catch (Exception e)
            {
                SetAlert(e.Message, Code.ERROR);
            }
        }
        public void OnPost()
        {
            try
            {
                productService = new ProductService();
                billService = new BillService();
                warehouseService = new WarehouseService();
                int id = Code.NOT_FOUND;
                ServiceResult<int> getIdResult = productService.FindIdByName(productName);
                if (getIdResult.isSuccess) {
                    id = getIdResult.data;
                }
                else
                {
                    Response.Redirect("BillAdd?error=" + Code.ERROR);
                }

                ServiceResult<Product> getProductResult = productService.FindById(id);
                Product productTemp = new Product();
                if (getProductResult.isSuccess)
                {
                    productTemp = getProductResult.data;

                    //add bill in
                    Bill bill = new Bill();
                    bill.createDate = DateTime.Now;
                    bill.productNumber = id;
                    bill.productName = productName;
                    bill.number = number;
                    bill.productType = productTemp.productType;

                    ServiceResult<int> saveResult = billService.Save(bill);
                    if(!saveResult.isSuccess){
                        Response.Redirect("/BillIn?error=" + Code.ERROR);
                    }
                    else if(saveResult.message.Equals(ErrorMessage.ZERO))
                    {
                        Response.Redirect("/BillIn?error=" + Code.ZERO);
                    }
                    else
                    {
                        // add item to warehouse
                        Warehouse warehouse = new Warehouse();
                        warehouse.productNumber = id;
                        warehouse.productName = productName;
                        warehouse.number = number;
                        warehouse.expireDate = productTemp.expireDate;
                        warehouse.productType = productTemp.productType;

                        ServiceResult<int> warehouseResult = warehouseService.Save(warehouse);
                        if (warehouseResult.isSuccess)
                        {
                            Response.Redirect("/Bill?error=" + Code.SUCCESS);
                        }
                        else
                        {
                            Response.Redirect("BillAdd?error=" + Code.ERROR);
                        }
                    }
                }
                else
                {
                    Response.Redirect("BillAdd?error=" + Code.ERROR);
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
