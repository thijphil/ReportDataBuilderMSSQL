using ReportDataBuilder.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportDataBuilder.StringOperations
{
    public static class StringBuilder
    {
        public static string CreateValueList(ViewObject viewObject)
        {
            string values = "";
            foreach (var para in viewObject.paras)
            {
                switch (para.Value.Type)
                {
                    case "System.Int64":
                        values += $"{para.Value.Value}, ";
                        break;
                    case "System.Int32":
                        values += $"{para.Value.Value}, ";
                        break;
                    case "System.Decimal":
                        values += $"{para.Value.Value}, ";
                        break;
                    case "System.DateTime":
                        values += $"'{para.Value.Value}', ";
                        break;

                    default:
                        break;
                }
            }
            values = values.Substring(0, values.Length - 2);
            values += " ";
            return values;
        }
        public static string CreateColumnList(ViewObject viewObject)
        {
            string columns = "";
            foreach (var para in viewObject.paras)
            {
                columns += $"`{para.Key}`, ";
            }
            columns = columns.Substring(0, columns.Length - 2);
            columns += " ";
            return columns;
        }
        public static string CreateColumnListMySql(List<string> cols)
        {
            string columns = "";
            foreach (var para in cols)
            {
                columns += $"`{para}`, ";
            }
            columns = columns.Substring(0, columns.Length - 2);
            columns += " ";
            return columns;
        }

        public static string CreateColumnListMsSql(List<string> cols)
        {
            string columns = "";
            foreach (var para in cols)
            {
                columns += $"[{para}], ";
            }
            columns = columns.Substring(0, columns.Length - 2);
            columns += " ";
            return columns;
        }

        public static string CreateColumnParamsList(ViewObject viewObject)
        {
            string columns = "";
            foreach (var para in viewObject.paras)
            {
                columns += $"@{para.Key}, ";
            }
            columns = columns.Substring(0, columns.Length - 2);
            columns += " ";
            return columns;
        }
    }
}
