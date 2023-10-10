using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAuthAPI.Models.DTO
{
    public class SecurityRequestQandADTO
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public string UserEmail { get; set; }

    }
}