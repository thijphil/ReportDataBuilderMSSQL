using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportDataBuilder.Controllers
{
    public interface IController
    {
        Task BuildDataAsync();
    }
}
