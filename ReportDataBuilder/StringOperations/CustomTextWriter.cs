using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportDataBuilder.StringOperations
{
    public class CustomTextWriter : TextWriter
    {
        //public override void Write(char value)
        //{
        //    //Do something, like write to a file or something
        //}

        //public override void Write(string value)
        //{
        //    //Do something, like write to a file or something
        //}
        private readonly TextWriter originalOut;

        public CustomTextWriter(TextWriter originalOut)
        {
            this.originalOut = originalOut;
        }

        public override void WriteLine(string? value)
        {
            DateTime currentDateTime = DateTime.Now;
            string currentDateTimeString = $"{currentDateTime.ToString("yyyy-MM-dd HH:mm:ss")}";
            //Console.WriteLine($"{currentDateTimeString}: {value}");
            originalOut.WriteLine($"{currentDateTimeString}: {value}");
            //Do something, like write to a file or something
        }
        public override Encoding Encoding => originalOut.Encoding;

    }
}
