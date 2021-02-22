using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD
{
    class Element
    {
        public string className { get; set; }
        public double value { get; set; }
        public int originalTableIndex { get; set; }
        public Element()
        {

        }
        public Element(string className, double value, int index)
        {
            this.className = className;
            this.value = value;
            this.originalTableIndex = index;
        }
    }
}
