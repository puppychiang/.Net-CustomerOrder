using CustomerOrder2.Logic;
using CustomerOrder2.Models;
using CustomerOrder2.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerOrder2.Controllers
{
    /// <summary>
    /// HomeController 繼承 Controller 類別
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// 宣告私有的 readonly 參數 _dbOrderContext
        /// </summary>
        private readonly dbOrderContext _dbOrderContext;
        private readonly ILogger<HomeController> _logger;

        // 正常程序是每次使用到DB就要重新new DB的物件，要讀的時候還要Open，讀完之後還需要Close，最後還要Dispose釋放掉資源
        // 但以底下的方式撰寫的話就可以省去每次都要new的寫法
        public HomeController(dbOrderContext dborderContext , ILogger<HomeController> logger)
        {
            this._dbOrderContext = dborderContext;
            this._logger = logger;
        }

        #region Ajax Alert
        /// <summary>
        /// 取得JSON格式資料
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetCustomerDetail(Int64 Id)
        {
            var customer = await _dbOrderContext.CustomerInfos.FindAsync(Id);
            return Json(new { customer });
        }
        #endregion

        #region 查詢客戶基本資料
        /// <summary>
        /// 查詢客戶基本資料
        /// </summary>
        [HttpGet]
        public ActionResult Index()
        {
            // var customerList = (from a in _dbOrderContext.CustomerInfos select a).ToList(); // LinQ語法
            var customerList = _dbOrderContext.CustomerInfos.Where(x => x.Visible == 1)
                                                            .OrderBy(x => x.Name)
                                                            .ToList();  // 只顯示未隱藏的資料，Lambda語法
            return View(customerList);
        }
        #endregion

        #region 新增客戶資料
        /// <summary>
        /// 跳轉客戶基本資料畫面
        /// </summary>
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 新增客戶基本資料
        /// </summary>
        /// <param name="customerInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(CustomerInfo customerInfo)
        {
            // 系統輸入
            customerInfo.CreateTime = DateTime.Now;          
            customerInfo.Visible = 1;  // 1為顯示、0為隱藏
           
            try
            {
                // 寫入DB
                _dbOrderContext.CustomerInfos.Add(customerInfo);
                _dbOrderContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            // 跳轉新增訂單畫面，並傳送 customerId 參數
            return RedirectToAction("Order", new { customerId = customerInfo.Id });
        }
        #endregion

        #region 新增訂單
        /// <summary>
        /// 跳轉新增訂單畫面
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Order(Int64 customerId)
        {
            // 接收傳入的CustomerId參數, 並將CustomerId加入要新增的OrderInfo訂單資訊裡面
            OrderInfo orderInfo = new OrderInfo();
            orderInfo.CustomerId = customerId;
            return View(orderInfo);
        }

        /// <summary>
        /// 新增訂單
        /// </summary>
        /// <param name="orderInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Order(OrderInfo orderInfo)
        {
            // 建立產品金額
            int PriceProduct1 = 250; // 生乳捲
            int PriceProduct2 = 200; // 戚風蛋糕

            // 系統輸入資料，其餘沒有寫的屬性為使用者輸入
            orderInfo.Price = PriceProduct1 * orderInfo.Product1 + PriceProduct2 * orderInfo.Product2;
            orderInfo.OrderTime = DateTime.Now;           
            orderInfo.Visible = 1;  // 1為顯示、0為隱藏

            try
            {
                _dbOrderContext.OrderInfos.Add(orderInfo);
                _dbOrderContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // 跳轉確認畫面，並傳送 orderId 參數
            //return RedirectToAction("Confirm", new {orderId = orderInfo.OrderId });
            return RedirectToAction("Confirm", new { customerId = orderInfo.CustomerId });
        }
        #endregion

        #region 確認畫面
        /// <summary>
        /// 跳轉確認畫面
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Confirm(Int64 customerId)
        {
            OrderInfo orderInfo = _dbOrderContext.OrderInfos.FirstOrDefault(x=>x.CustomerId == customerId);
            return View(orderInfo);
        }
        #endregion

        #region 編輯客戶資料
        /// <summary>
        /// 跳轉編輯客戶資料畫面
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(Int64 Id)
        {
            var customer = _dbOrderContext.CustomerInfos.Find(Id);
            return View(customer);
        }

        /// <summary>
        /// 編輯客戶資料
        /// </summary>
        /// <param name="edit"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(CustomerInfo edit)
        {
            var customer = _dbOrderContext.CustomerInfos.Find(edit.Id);
            // 人員輸入
            customer.Name = edit.Name;
            customer.Mail = edit.Mail;
            customer.Address = edit.Address;
            // 系統輸入
            customer.UpdateTime = DateTime.Now;

            try
            {
                _dbOrderContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            // 回首頁
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region 刪除資料
        /// <summary>
        /// 跳轉刪除資料畫面
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Delete(Int64 Id)
        {
            var customer = _dbOrderContext.CustomerInfos.Find(Id);
            return View(customer);
        }

        /// <summary>
        /// 刪除資料(隱藏資料)
        /// </summary>
        /// <param name="delete"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(CustomerInfo delete)
        {
            var customer = _dbOrderContext.CustomerInfos.Find(delete.Id);
            //_dbOrderContext.CustomerInfos.Remove(customer);
            // 系統輸入
            customer.UpdateTime = DateTime.Now;
            customer.Visible = 0;  // 1為顯示、0為隱藏

            try
            {
                _dbOrderContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            // 回首頁
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region 查看訂單        
        /// <summary>
        /// 查看所有訂單
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OrderDownload()
        {
            // 只顯示未隱藏的資料
            // 查詢對照SQL語法
            List<OrderInfoDto> result = await (from cus in _dbOrderContext.CustomerInfos
                                           join order in _dbOrderContext.OrderInfos on cus.Id equals order.CustomerId
                                           where cus.Visible == 1
                                           select new OrderInfoDto
                                           {
                                               OrderId = order.OrderId,
                                               OrderTime = order.OrderTime,
                                               Product1 = order.Product1,
                                               Product2 = order.Product2,
                                               Price = order.Price,
                                               CustomerId = order.CustomerId,
                                               Name = cus.Name,
                                               Mail = cus.Mail,
                                               Address = cus.Address
                                           })
                                           .OrderBy(x => x.Name)
                                           .ToListAsync();
            return View(result);
        }
        #endregion

        #region Excel輸出
        /// <summary>
        /// 匯出客戶資訊 Excel，使用 ExcelPackage (EPPlus)
        /// </summary>
        /// <returns></returns>
        public ActionResult CustomerExport()
        {
            // 取得匯出資料
            // 資料條件為 Visible = 1
            // 依照 Name 字母排序
            List<CustomerInfo> listData = _dbOrderContext.CustomerInfos.Where(x => x.Visible == 1)
                                                                       .OrderBy(x => x.Name)
                                                                       .ToList();
            ExportLogic exportLogic = new ExportLogic();
            var itemFile = exportLogic.ExportCus(listData);

            #region 移至Logic ExportLogic，2021/6/25 修改
            //// 若在非商業環境使用 EPPlus 需要加上底下這段忽略錯誤訊息指令，在 appsetting 加指令也可以
            ////ExcelPackage.LicenseContext = LicenseContext.NonCommercial;  
            //ExcelPackage ep = new ExcelPackage();
            //ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("CustomerSheet");

            //int col = 1; // 行初始
            //int row = 2; // 列初始

            //// 欄位名稱 (Header) (姓名、電郵、地址)
            //sheet.Cells[1, col++].Value = "刪除狀態";
            //sheet.Cells[1, col++].Value = "客戶名稱";
            //sheet.Cells[1, col++].Value = "電子郵件";
            //sheet.Cells[1, col++].Value = "寄送地址";
            //sheet.Cells[1, col++].Value = "生效時間";
            //sheet.Cells[1, col++].Value = "修改時間";

            //// 陣列內容存入 sheet 中
            //foreach (var element in listData)
            //{
            //    col = 1; // 行初始
            //    sheet.Cells[row, col++].Value = element.Visible;
            //    sheet.Cells[row, col++].Value = element.Name;
            //    sheet.Cells[row, col++].Value = element.Mail;
            //    sheet.Cells[row, col++].Value = element.Address;
            //    sheet.Cells[row, col++].Value = element.CreateTime;
            //    sheet.Cells[row, col++].Value = element.UpdateTime;
            //    row++;
            //}

            //MemoryStream itemFile = new MemoryStream();
            //ep.SaveAs(itemFile);
            //ep.Dispose();  // 關閉檔案、釋放資源

            //itemFile.Position = 0;  // 通知檔案需要從頭開始存取, 如果不初始 position 也可以存取 file.ToArray()
            #endregion

            var fileName = "CustomerExport" + ".xlsx";
            return File(itemFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);           
        }

        /// <summary>
        /// 匯出訂單資訊 Excel，使用 ExcelPackage (EPPlus)
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderExport()
        {
            // 取得匯出資料，LINQ語法
            List<OrderInfoDto> listDataLINQ = (from cus in _dbOrderContext.CustomerInfos
                                           join order in _dbOrderContext.OrderInfos on cus.Id equals order.CustomerId
                                           where cus.Visible == 1
                                           select new OrderInfoDto
                                           {
                                               OrderId = order.OrderId,
                                               OrderTime = order.OrderTime,
                                               Product1 = order.Product1,
                                               Product2 = order.Product2,
                                               Price = order.Price,
                                               CustomerId = order.CustomerId,
                                               Name = cus.Name,
                                               Mail = cus.Mail,
                                               Address = cus.Address
                                           })
                                           .OrderBy(x => x.Name)
                                           .ToList();

            // 取得匯出資料，Lambda語法
            List<OrderInfoDto> listDataLambda = _dbOrderContext.CustomerInfos.Where(cus => cus.Visible==1).Join(_dbOrderContext.OrderInfos,
                cus => cus.Id,
                order => order.CustomerId,
                (cus, order) => new OrderInfoDto
                {
                    OrderId = order.OrderId,
                    OrderTime = order.OrderTime,
                    Product1 = order.Product1,
                    Product2 = order.Product2,
                    Price = order.Price,
                    CustomerId = order.CustomerId,
                    Name = cus.Name,
                    Mail = cus.Mail,
                    Address = cus.Address

                }).OrderBy(x => x.Name).ToList();

            ExportLogic exportLogic = new ExportLogic();
            var itemFile = exportLogic.ExportOder(listDataLambda);

            #region 移至Logic ExportLogic，2021/6/25 修改
            //// 若在非商業環境使用 EPPlus 需要加上底下這段忽略錯誤訊息指令，在 appsetting 加指令也可以
            ////ExcelPackage.LicenseContext = LicenseContext.NonCommercial;  
            //ExcelPackage ep = new ExcelPackage();
            //ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("OrderSheet");

            //int col = 1; // 行初始
            //int row = 2; // 列初始

            //// 欄位名稱 (Header) (客戶編號、訂單時間、生乳捲、戚風蛋糕、總金額)
            //sheet.Cells[1, col++].Value = "刪除狀態";
            //sheet.Cells[1, col++].Value = "客戶編號";
            //sheet.Cells[1, col++].Value = "客戶名稱";
            //sheet.Cells[1, col++].Value = "電子郵件";
            //sheet.Cells[1, col++].Value = "寄送地址";
            //sheet.Cells[1, col++].Value = "生乳捲數量";
            //sheet.Cells[1, col++].Value = "戚風蛋糕數量";
            //sheet.Cells[1, col++].Value = "總金額";
            //sheet.Cells[1, col++].Value = "訂單時間";

            //// 陣列內容存入 sheet 中
            //foreach (var element in listData)
            //{
            //    col = 1; // 行初始
            //    sheet.Cells[row, col++].Value = element.Visible;
            //    sheet.Cells[row, col++].Value = element.CustomerId;
            //    sheet.Cells[row, col++].Value = element.Name;
            //    sheet.Cells[row, col++].Value = element.Mail;
            //    sheet.Cells[row, col++].Value = element.Address;
            //    sheet.Cells[row, col++].Value = element.Product1;
            //    sheet.Cells[row, col++].Value = element.Product2;
            //    sheet.Cells[row, col++].Value = element.Price;
            //    sheet.Cells[row, col++].Value = element.OrderTime.ToString("yyyy/MM/dd :mm:ss");
            //    row++;
            //}

            //MemoryStream itemFile = new MemoryStream();
            //ep.SaveAs(itemFile);
            //ep.Dispose();  // 關閉檔案、釋放資源           
            //itemFile.Position = 0;
            #endregion

            var fileName = "OrderExport" + ".xlsx";
            return File(itemFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        #endregion


        /// <summary>
        /// ViewData、ViewBag、ViewData.Model 測試
        /// </summary>
        /// <returns></returns>
        public ActionResult Privacy()
        {
            // 標題
            ViewData["Title"] = "deBug Paradise";

            // ViewData 資料型態在 View 要做轉換，除了 string 不用轉
            ViewData["ShopName"] = "Hello Shop";
            ViewData["Establishment"] = 2022;

            // ViewBag 資料型態在 View 不用做轉換
            ViewBag.ContactUs = "0912345678";
            ViewBag.Employees = 2;

            // ViewData.Model 傳遞方法
            List<string> Product = new List<string>();
            Product.Add("無敵好吃生乳捲");
            Product.Add("爆好呷戚風蛋糕");
            ViewData.Model = Product;

            return View();
        }

        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}
