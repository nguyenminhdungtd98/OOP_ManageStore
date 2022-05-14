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
    public class BillOutUpdateModel : PageModel
    {
        IBillOutService billOutService;
        IProductService productService;
        IWarehouseService warehouseService;
        [BindProperty]
        public int numberBillOut { get; set; }
        [BindProperty]
        public DateTime createDate { get; set; }
        [BindProperty]
        public int productNumber { get; set; }
        [BindProperty]
        public string productName { get; set; }
        [BindProperty]
        public int price { get; set; }
        [BindProperty]
        public int number { get; set; }
        [BindProperty]
        public int total { get; set; }
        [BindProperty]
        public List<ProductType> productTypes { get; set; }
        [BindProperty]
        public List<Product> products { get; set; }
        [BindProperty]
        public List<ProductInBill> productInBills { get; set; }
        [BindProperty(SupportsGet = true)]
        public int error { get; set; }
        [BindProperty(SupportsGet = true)]
        public int idDeleteP { get; set; }
        [BindProperty]
        public string edit { get; set; }
        [BindProperty(SupportsGet = true)]
        public int idUpdate { get; set; }

        public BillOut billOut { get; set; }
        public BillOut billOutOld = new BillOut();

        [BindProperty]
        public List<Warehouse> productsInStock { get; set; }
        public void OnGet()
        {
            try
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
                List<BillOut> billOutTemps = new List<BillOut>();
                ServiceResult<List<BillOut>> getBillOutTemps = billOutService.LoadBillOutTemp();
                if (getBillOutTemps.isSuccess)
                {
                    billOutTemps = getBillOutTemps.data;
                }
                // if bill out temp  = null then Get bill out and save to txt file
                if (billOut == null)
                {
                    ServiceResult<BillOut> getBillOut = billOutService.FindById(idUpdate);
                    if (getBillOut.isSuccess)
                    {
                        billOut = getBillOut.data;
                    }

                    if (billOutTemps == null || billOutTemps.Count == 0)
                    {
                        billOutTemps = new List<BillOut>();
                        billOutTemps.Add(billOut);
                        billOutService.SaveBillOutTemp(billOutTemps);
                    }
                }
                ServiceResult<List<ProductInBill>> getProductInBillsTemp = billOutService.FindAllProductInBill();
                if (!getProductInBillsTemp.isSuccess)
                {
                    if (!billOut.Equals(null) && getProductInBillsTemp.data.Count == 0 /*&& getProductInBillsTemp.data.Count.Equals(0)*/)
                    {
                        billOutService.SaveAllProduct(billOut.productInBill);
                    }
                }


                if (productInBills == null)
                {
                    ServiceResult<List<ProductInBill>> getProductInBills = billOutService.FindAllProductInBill();
                    if (getProductInBills.isSuccess)
                    {
                        productInBills = getProductInBills.data;
                    }
                }
                numberBillOut = billOutTemps[0].numberBillOut;
                createDate = billOutTemps[0].createDate;
                ServiceResult<List<Product>> getProducts = productService.FindAll();
                if (getProducts.isSuccess)
                {
                    products = getProducts.data;
                }

                if (idDeleteP > 0)
                {
                    
                    if (billOutService.DeleteProductInBill(idDeleteP).isSuccess)
                    {
                        Response.Redirect("/BillOutUpdate");
                    }

                }
            }
            catch (Exception e)
            {
                SetAlert(e.Message, Code.ERROR);
            }

        }
        private void Initialize()
        {
            billOutService = new BillOutService();
            productService = new ProductService();
            warehouseService = new WarehouseService();
        }
        public void OnPost()
        {
            Initialize();

            List<BillOut> billOutTemps = new List<BillOut>();
            ServiceResult<List<BillOut>> getBillOutTemp = billOutService.LoadBillOutTemp();
            if (getBillOutTemp.isSuccess)
            {
                billOutTemps = getBillOutTemp.data;
            }
            //get bill out temp

            if (number == 0 && edit == null)
            {
                Response.Redirect("/BillOutUpdate?error=" + Code.ZERO + "&idUpdate=" + billOut.numberBillOut);
            }
            else
            {
                if (edit == null)
                {
                    if (billOutService.CheckExistProductToEdit(productName, billOutTemps[0].productInBill).isSuccess)
                    {
                        List<ProductInBill> productInBillsTemp = billOutService.FindAllProductInBill().data;
                        productInBills = billOutService.UpdateProductToEdit(productName, number, productInBillsTemp).data;
                        billOutService.DeleteAllProductInBill();
                        billOutService.SaveAllProduct(productInBills);
                        //BillOutServices.updateWareHouseOutToEdit(billOut, billOutOld);
                        Response.Redirect("/BillOutUpdate?idUpdate=" + numberBillOut);
                    }
                    else
                    {
                        int id =  0;
                        ServiceResult<int> getIdBillOut = productService.FindIdByName(productName);
                        if (getIdBillOut.isSuccess)
                        {
                            id = getIdBillOut.data;
                        }

                        productNumber = id;
                        ServiceResult<Product> getProduct = productService.FindById(id);
                        Product product = new Product();
                        if (getProduct.isSuccess)
                        {
                            product = getProduct.data;
                        }

                        ProductInBill productInBill = new ProductInBill();
                        productInBill.productNumber = product.productNumber;
                        productInBill.productName = productName;
                        productInBill.number = number;
                        productInBill.price = product.price;
                        productInBill.total = product.price * number;

                        productInBills = billOutService.FindAllProductInBill().data;
                        productInBills.Add(productInBill);
                        billOutService.DeleteAllProductInBill();
                        billOutService.SaveAllProduct(productInBills);
                        //BillOutServices.updateWareHouseOutToEdit(billOut);
                        Response.Redirect("/BillOutUpdate?idUpdate=" + numberBillOut);
                    }
                }
                else
                {
                    billOut = billOutTemps[0];
                    productInBills = billOutService.FindAllProductInBill().data;

                    BillOut billOutNew = new BillOut();
                    billOutNew.productInBill = productInBills;
                    billOutNew.numberBillOut = billOut.numberBillOut;
                    billOutNew.createDate = billOut.createDate;

                    ServiceResult<int> editBillOut = billOutService.Edit(billOutNew);
                    if (editBillOut.isSuccess)
                    {
                        ServiceResult<bool> updateWarehouse = billOutService.UpdateWareHouseOutToEdit(billOutNew, billOut);
                        if (updateWarehouse.isSuccess)
                        {
                            List<BillOut> deleteBillOuts = new List<BillOut>();
                            billOutService.DeleteAllProductInBill();
                            billOutService.SaveBillOutTemp(deleteBillOuts);
                            Response.Redirect("/BillOutManage?error=" + Code.SUCCESS);
                        }
                        else
                        {
                            Response.Redirect("/BillOutManage?error=" + Code.ERROR);
                        }
                    }
                    else
                    {
                        Response.Redirect("/BillOutManage?error=" + Code.ERROR);
                    }
                }
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
