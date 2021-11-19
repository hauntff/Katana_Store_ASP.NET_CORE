
using System;
using Web.Application.Domain.Base;

namespace Web.Application.Domain
{
    public class Payment : BaseDateEntity
    {
        public string Command { get; set; }
        public string Account { get; set; }
        public string TxnId { get; set; }
        public string TxnDate { get; set; }
        public decimal Sum { get; set; }
    }
}
