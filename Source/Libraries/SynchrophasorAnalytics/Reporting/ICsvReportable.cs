using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Reporting
{
    public interface ICsvReportable
    {
        string CsvHeader
        {
            get;
        }

        string ToCsvLineString();
    }
}
