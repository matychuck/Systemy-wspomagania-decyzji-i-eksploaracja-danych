using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD
{
    class ColorsTable
    {
        public static List<ClassWithChildren> colors = new List<ClassWithChildren>();
        public static Dictionary<string, double> getColorsByClassName(string className)
        {
            var dictionary = colors.Where(x => x.name == className).FirstOrDefault();
            return dictionary == null ? null : dictionary.colorsWithNames;
        }
    }
}
