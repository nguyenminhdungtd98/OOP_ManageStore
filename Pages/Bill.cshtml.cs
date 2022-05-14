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
    public class BillModel : PageModel
    {
        IBillService billService;
        public List<Bill> bills { get; set; }

        public string typeSearch { get; set; }

        public string key { get; set; }
        [BindProperty(SupportsGet = true)]
        public int id { get; set; }
        [BindProperty(SupportsGet = true)]
        public int error { get; set; }
        public void OnGet()
        {
            billService = new BillService();
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
            if (id > 0)
            {
                ServiceResult<bool> deleteResult = billService.DeleteById(id);
                if (deleteResult.isSuccess)
                {
                    Response.Redirect("/Bill?error="+Code.SUCCESS);
                }
            }
            billService = new BillService();
            ServiceResult<List<Bill>> serviceResult = billService.FindAll();
            if (serviceResult.isSuccess)
            {
                bills = serviceResult.data;
            }

        }
        public void OnPost()
        {
            billService = new BillService();
            if (key != null)
            {
                ServiceResult<List<Bill>> searchResult = billService.Search(typeSearch, key.Trim());
                if (searchResult.isSuccess)
                {
                    bills = searchResult.data;
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
