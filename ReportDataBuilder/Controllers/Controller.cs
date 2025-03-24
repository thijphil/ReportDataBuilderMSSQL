using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportDataBuilder.Controllers
{
    public abstract class Controller : IController
    {
        public abstract Task BuildDataAsync();
        public static bool HasFilter(List<string> columnNames) => columnNames.Contains("CreatedDatetimeFilter");
    }
}
