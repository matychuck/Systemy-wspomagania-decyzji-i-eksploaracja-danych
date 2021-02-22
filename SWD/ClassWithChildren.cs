using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD
{
    class ClassWithChildren
    {
        public string name { get; set; }
        public Dictionary<string, double> colorsWithNames= null;

        public ClassWithChildren(string name, Dictionary<string,double> colorsWithNames)
        {
            this.name = name;
            this.colorsWithNames = colorsWithNames;
        }
    }
}
