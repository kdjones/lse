using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Reporting
{
    public static class ReportGenerator
    {
        public static string CreateCsvReport(IEnumerable<ICsvReportable> records)
        {
            StringBuilder report = new StringBuilder();
            if (records.Count() > 0)
            {
                report.Append(records.First().CsvHeader);
            }
            foreach (ICsvReportable record in records)
            {
                report.Append(record.ToCsvLineString());
            }
            return report.ToString();
        }
    }
}
