using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportDataBuilder.StringOperations
{
    public class CustomTextWriter(TextWriter originalOut) : TextWriter
    {
        public override void WriteLine(string? value)
        {
            DateTime currentDateTime = DateTime.Now;
            string currentDateTimeString = $"{currentDateTime.ToString("yyyy-MM-dd HH:mm:ss")}";
            originalOut.WriteLine($"{currentDateTimeString}: {value}");
        }
        public override Encoding Encoding => originalOut.Encoding;

    }
}
