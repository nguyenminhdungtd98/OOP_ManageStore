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
    public class BillOutManageModel : PageModel
    {
        IBillOutService billOutService;
        public int numberBillOut { get; set; }
        public DateTime createDate { get; set; }

        public List<ProductInBill> productInBill;

        public List<BillOut> billOuts { get; set; }

        public string typeSearch { get; set; }

        public string key { get; set; }

        [BindProperty(SupportsGet = true)]
        public int id { get; set; }

        [BindProperty(SupportsGet = true)]
        public int error { get; set; }

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
            else if (error == Code.ERROR)
            {
                SetAlert(ErrorMessage.ERROR, Code.ERROR);
            }
            else if (error == Code.SUCCESS)
            {
                SetAlert(ErrorMessage.SUCCESS, Code.SUCCESS);
            }
            ServiceResult<List<BillOut>> billOutResult = billOutService.FindAll();
            if (billOutResult.isSuccess)
            {
                billOuts = billOutResult.data;
            }
            ServiceResult<bool> deletePIBResult = billOutService.DeleteAllProductInBill();
            ServiceResult<bool> deleteBOTResult = billOutService.DeleteAllBillOutTemp();

            if (id > 0)
            {
                ServiceResult<bool> deleteResult = billOutService.DeleteById(id);
                if (deleteResult.isSuccess)
                {
                    Response.Redirect("/BillOutManage?error=" + Code.SUCCESS);
                }
            }
        }
        public void OnPost()
        {
            Initialize();
            if (key != null)
            {
                ServiceResult<List<BillOut>> billOutResult = billOutService.Search(typeSearch,key);
                if (billOutResult.isSuccess)
                {
                    billOuts = billOutResult.data;
                }
            }
        }
        private void Initialize()
        {
            billOutService = new BillOutService();
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
