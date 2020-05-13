using hw_4_6.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hw_4_6.Models
{
    public class SimchaViewModel
    {
        public List<Simcha> Simchos { get; set; }
        public string Message { get; set; }
        public int TotalContributors { get; set; }
    }
}
