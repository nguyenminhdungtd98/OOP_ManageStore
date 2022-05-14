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
    public class ProductTypeModel : PageModel
    {
        IProductTypeService productTypeService;
        public List<ProductType> productTypes { get; set; }

        [BindProperty(SupportsGet = true)]
        public int id { get; set; }

        [BindProperty(SupportsGet = true)]
        public int idDelete { get; set; }

        [BindProperty(SupportsGet = true)]
        public int idUpdate { get; set; }

        public int productTypeNumber { get; set; }
        public string productTypeName { get; set; }

        public string typeSearch { get; set; }

        public string key { get; set; }
        [BindProperty(SupportsGet = true)]
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
            else if (error.Equals(Code.PERMISION))
            {
                SetAlert(ErrorMessage.PERMISION, Code.ERROR);
            }
            else if (error.Equals(Code.ZERO))
            {
                SetAlert(ErrorMessage.ZERO, Code.ZERO);
            }
            productTypeService = new ProductTypeService();
            productTypes = new List<ProductType>();
            int idTemp = 0;
            if (idDelete > 0)
            {
                idTemp = 1;
            }
            else if (idUpdate > 0)
            {
                idTemp = 2;
            }
            else
            {
                idTemp = -1;
            }
            switch (idTemp)
            {
                case 1:
                    ServiceResult<bool> serviceDeleteResult = productTypeService.DeleteById(idDelete);
                    if (serviceDeleteResult.isSuccess)
                    {
                        Response.Redirect("/ProductType?error=" + Code.SUCCESS);

                    }
                    else if(serviceDeleteResult.message.Equals(ErrorMessage.ERROR))
                    {
                        Response.Redirect("/ProductType?error=" + Code.ERROR);
                    }
                    else if (serviceDeleteResult.message.Equals(ErrorMessage.ZERO))
                    {
                        Response.Redirect("/ProductType?error=" + Code.ZERO);
                    }
                        //if (result == Error.ERROR)
                        //{
                        //    Response.Redirect("/ProductType?error=" + Error.ERROR);
                        //}
                        //else if (result == Error.PERMISION)
                        //{
                        //    Response.Redirect("/ProductType?error=" + Error.PERMISION);
                        //}


                        break;
                case 2:
                    //ProductType productType = ProductTypeServices.findById(idUpdate);
                    //if (productType.productTypeNumber > 0)
                    //{
                    //    productTypeNumber = productType.productTypeNumber;
                    //    productTypeName = productType.productTypeName;
                    //}
                    break;
            }
            ServiceResult<List<ProductType>> serviceResult = productTypeService.FindAll();
            if (serviceResult.isSuccess)
            {
                productTypes = serviceResult.data;
            }
            else
            {
                SetAlert(ErrorMessage.NOT_FOUND, Code.NOT_FOUND);
            }
        }
        public void OnPost()
        {
            productTypeService = new ProductTypeService();
            if (idUpdate <= 0 && key == null)
            {

                ProductType productType = new ProductType(productTypeNumber, productTypeName);

                ServiceResult<int> serviceResult = productTypeService.Save(productType);
                if (serviceResult.isSuccess)
                {
                    Response.Redirect("/ProductType?error=" + Code.SUCCESS);
                }
                else if(serviceResult.message.Equals(ErrorMessage.DUPLICATE))
                {
                    Response.Redirect("/ProductType?error=" + Code.DUPLICATE);
                }
                else if (serviceResult.message.Equals(ErrorMessage.ERROR))
                {
                    Response.Redirect("/ProductType?error=" + Code.ERROR);
                }
                else if (serviceResult.message.Equals(ErrorMessage.ZERO))
                {
                    Response.Redirect("/ProductType?error=" + Code.ZERO);
                }


            }
            else if (idUpdate > 0)
            {
                ProductType productType = new ProductType(productTypeNumber, productTypeName);
                ServiceResult<int> serviceResult = productTypeService.Edit(productType,idUpdate);
                if (serviceResult.isSuccess)
                {
                    Response.Redirect("/ProductType?error=" + Code.SUCCESS);
                }
                else if(serviceResult.message.Equals(ErrorMessage.DUPLICATE))
                {
                    Response.Redirect("/ProductType?error=" + Code.DUPLICATE);

                }
                else if (serviceResult.data.Equals(Code.ERROR))
                {
                    Response.Redirect("/ProductType?error=" + Code.ERROR);
                }
            }
            else
            {
                ServiceResult<List<ProductType>> serviceResult = productTypeService.Search(typeSearch,key);
                if (serviceResult.isSuccess)
                {
                    productTypes = serviceResult.data;
                }
                else
                {
                    SetAlert(ErrorMessage.NOT_FOUND, Code.NOT_FOUND);
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
