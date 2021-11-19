using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.STS.Interface
{
    public interface ISmsService
    {
        Task SendSms(string phone, string text);
    }
}
