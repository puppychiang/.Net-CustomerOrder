using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace CustomerOrder2.Models
{
    public partial class OrderInfo
    {
        [DisplayName("訂單編號")]
        public long OrderId { get; set; }
        [DisplayName("訂單成立時間")]
        public DateTime OrderTime { get; set; }
        [DisplayName("生乳捲數量")]
        [Range(0, 100)]
        public int Product1 { get; set; }
        [DisplayName("戚風蛋糕數量")]
        [Range(0, 100)]
        public int Product2 { get; set; }
        [DisplayName("金額")]
        public int Price { get; set; }
        [DisplayName("客戶編號")]
        public long CustomerId { get; set; }
        [DisplayName("顯示狀態")]
        public int? Visible { get; set; }
    }
}