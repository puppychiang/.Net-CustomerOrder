using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerOrder2.Models.Dto
{
    public class OrderInfoDto
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
        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("E-Mail")]
        [EmailAddress]
        public string Mail { get; set; }
        [DisplayName("寄送地址")]
        public string Address { get; set; }
    }
}
