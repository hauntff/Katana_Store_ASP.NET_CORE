using System;
using System.Collections.Generic;
using System.Text;

namespace Web.Application.Domain.Configs
{
    public class KkbConfig
    {
        public string KkbLogonUrl { get; set; }
        public string KkbStatusUrl { get; set; }
        public string KkbApproveUrl { get; set; }
        public string BackLink { get; set; }
        public string PostLink { get; set; }
        public string KkbCaFile { get; set; }
        public string KkbPfxFile { get; set; }
        public string KkbPfxPass { get; set; }
        public string KkbCertId { get; set; }
        public string KkbShopName { get; set; }
        public string KkbCurrency { get; set; } // 398 !!!
        public string KkbMerchantId { get; set; }
    }
}
