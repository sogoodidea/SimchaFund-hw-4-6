using hw_4_6.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hw_4_6.Models
{
    public class ContributionViewModel
    {
        public List<Contributor> Contributors { get; set; }
        public Simcha Simcha { get; set; }
        public List<int> IdsContributed { get; set; }
    }
}
