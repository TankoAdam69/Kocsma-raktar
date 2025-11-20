using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kocsma_raktár
{
    public class DrinkItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public bool InStock { get; set; }
        public DateTime LastRestock { get; set; }
    }
}
