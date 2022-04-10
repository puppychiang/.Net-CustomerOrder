using CustomerOrder2.Models;
using CustomerOrder2.Models.Dto;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerOrder2.Logic
{
    public class ExportLogic
    {
        /// <summary>
        /// 匯出客戶基本資料
        /// </summary>
        /// <param name="listData"></param>
        /// <returns></returns>
        public MemoryStream ExportCus(List<CustomerInfo> listData)
        {
            // 若在非商業環境使用 EPPlus 需要加上底下這段忽略錯誤訊息指令，在 appsetting 加指令也可以
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;  
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("CustomerSheet");

            int col = 1; // 行初始
            int row = 2; // 列初始

            // 欄位名稱 (Header) (姓名、電郵、地址)
            sheet.Cells[1, col++].Value = "客戶ID";
            sheet.Cells[1, col++].Value = "客戶名稱";
            sheet.Cells[1, col++].Value = "電子郵件";
            sheet.Cells[1, col++].Value = "寄送地址";
            sheet.Cells[1, col++].Value = "生效時間";
            sheet.Cells[1, col++].Value = "修改時間";

            // 陣列內容存入 sheet 中
            foreach (var element in listData)
            {
                col = 1; // 行初始
                sheet.Cells[row, col++].Value = element.Id;
                sheet.Cells[row, col++].Value = element.Name;
                sheet.Cells[row, col++].Value = element.Mail;
                sheet.Cells[row, col++].Value = element.Address;
                sheet.Cells[row, col++].Value = element.CreateTime.ToString("yyyy/MM/dd HH:mm:ss");
                #region 2021/6/29修改
                //sheet.Cells[row, col++].Value = element.UpdateTime;
                if (element.UpdateTime.HasValue)
                {
                    sheet.Cells[row, col++].Value = element.UpdateTime?.ToString("yyyy/MM/dd HH:mm:ss");
                }
                else
                {
                    sheet.Cells[row, col++].Value = element.UpdateTime;
                }
                #endregion
                row++;
            }

            MemoryStream itemFile = new MemoryStream();
            ep.SaveAs(itemFile);
            ep.Dispose();  // 關閉檔案、釋放資源
            itemFile.Position = 0;  // 通知檔案需要從頭開始存取, 如果不初始 position 也可以存取 file.ToArray()
            return (itemFile);
        }

        /// <summary>
        /// 匯出訂單資料及客戶資料
        /// </summary>
        /// <param name="listData"></param>
        /// <returns></returns>
        public MemoryStream ExportOder(List<OrderInfoDto> listData)
        {
            // 若在非商業環境使用 EPPlus 需要加上底下這段忽略錯誤訊息指令，在 appsetting 加指令也可以
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;  
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("OrderSheet");

            int col = 1; // 行初始
            int row = 2; // 列初始

            // 欄位名稱 (Header) (客戶編號、訂單時間、生乳捲、戚風蛋糕、總金額)
            sheet.Cells[1, col++].Value = "客戶ID";
            sheet.Cells[1, col++].Value = "客戶名稱";
            sheet.Cells[1, col++].Value = "電子郵件";
            sheet.Cells[1, col++].Value = "寄送地址";
            sheet.Cells[1, col++].Value = "生乳捲數量";
            sheet.Cells[1, col++].Value = "戚風蛋糕數量";
            sheet.Cells[1, col++].Value = "總金額";
            sheet.Cells[1, col++].Value = "訂單時間";

            // 陣列內容存入 sheet 中
            foreach (var element in listData)
            {
                col = 1; // 行初始
                sheet.Cells[row, col++].Value = element.CustomerId;
                sheet.Cells[row, col++].Value = element.Name;
                sheet.Cells[row, col++].Value = element.Mail;
                sheet.Cells[row, col++].Value = element.Address;
                sheet.Cells[row, col++].Value = element.Product1;
                sheet.Cells[row, col++].Value = element.Product2;
                sheet.Cells[row, col++].Value = element.Price;
                sheet.Cells[row, col++].Value = element.OrderTime.ToString("yyyy/MM/dd HH:mm:ss");
                row++;
            }

            MemoryStream itemFile = new MemoryStream();
            ep.SaveAs(itemFile);
            ep.Dispose();  // 關閉檔案、釋放資源           
            itemFile.Position = 0;
            return (itemFile);
        }
    }
}
