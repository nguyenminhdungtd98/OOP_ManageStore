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
    public class WarehouseModel : PageModel
    {
        IWarehouseService warehouseService;
        IProductTypeService productTypeService;
        public List<Warehouse> itemsInWarehouse { get; set; }
        public List<ProductType> productTypes { get; set; }

        public List<Warehouse> itemsExpire { get; set; }
        public int error { get; set; }
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
            warehouseService = new WarehouseService();
            productTypeService = new ProductTypeService();
            ServiceResult<List<Warehouse>> serviceResult = warehouseService.FindAll();
            if (serviceResult.isSuccess)
            {
                itemsInWarehouse = serviceResult.data;
            }
            else
            {
                Response.Redirect("Warehouse?error=" + Code.ERROR);
            }
            ServiceResult<List<ProductType>> productTypeResult = productTypeService.FindAll();
            if (productTypeResult.isSuccess)
            {
                productTypes = productTypeResult.data;
            }
            else
            {
                Response.Redirect("Warehouse?error=" + Code.ERROR);
            }
            ServiceResult<List<Warehouse>> expireItems = warehouseService.FindItemsExpire();
            if (expireItems.isSuccess)
            {
                itemsExpire = expireItems.data;
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
