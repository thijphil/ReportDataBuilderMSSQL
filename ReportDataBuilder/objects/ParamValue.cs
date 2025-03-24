using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportDataBuilder.objects
{
    public class ParamValue(string type, string value)
    {
        public string Type { get; set; } = type;
        public string Value { get; set; } = value;

    }
}
