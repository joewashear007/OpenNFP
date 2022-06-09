using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFP.Shared.Models
{
    public class CycleIndex<T> where T: class
    {
        public T? Item { get; set; }
        public int Index { get; set; }
    }
}
