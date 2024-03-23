using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class ReportModel
    {
        public DateTime date { get; set; }

        public double? t9 { get; set; }
        public double? t13 { get; set; }
        public double? t16 { get; set; }
        public double? m9 { get; set; }
        public double? m13 { get; set; }
        public double? m16 { get; set; }
        public double? maxt { get; set; }
        public double? mint { get; set; }
        public double? maxm { get; set; }
        public double? minm { get; set; }

    }
}
