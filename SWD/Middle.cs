using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD
{
    public class Middle
    {
        public int number { get; set; }
        public List<double> values { get; set; }

        public Middle(int number, List<double> values)
        {
            this.number = number;
            this.values = values;
        }
    }
}
