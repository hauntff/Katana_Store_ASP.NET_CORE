using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.STS.Base
{
    public abstract class BaseEntity
    {
        public string Code { get; set; } = Guid.NewGuid().ToString();
    }
}
