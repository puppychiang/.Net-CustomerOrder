using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace CustomerOrder2.Models
{
    public partial class CustomerInfo
    {
        public long Id { get; set; }
        [DisplayName("姓名")]
        [Required(ErrorMessage = "必要欄位, 請輸入")]
        [StringLength(10, MinimumLength = 2)]
        public string Name { get; set; }
        [DisplayName("E-Mail")]
        [Required(ErrorMessage = "必要欄位, 請輸入")]
        [EmailAddress]
        public string Mail { get; set; }
        [DisplayName("寄送地址")]
        [Required(ErrorMessage = "必要欄位, 請輸入")]
        public string Address { get; set; }
        [DisplayName("新增資料時間")]
        public DateTime CreateTime { get; set; }
        [DisplayName("修改資料時間")]
        public DateTime? UpdateTime { get; set; }
        [DisplayName("顯示狀態")]
        public int? Visible { get; set; }
    }
}