using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportDataBuilder.objects
{
    public class ParamValue
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public ParamValue()
        {
                
        }

        public ParamValue(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}
