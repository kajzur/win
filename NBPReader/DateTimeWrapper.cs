using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBPReader
{
    class DateTimeWrapper
    {
        private string dateFormat = "{0:d/M/yyyy}";
        private DateTime dateTime;
        private string filename;
        public DateTimeWrapper(DateTime dt, string rawFileName)
        {
            this.dateTime = dt;
            this.filename = rawFileName;
        }

        public String DateTimeString
        {
            get
            {
                return String.Format(dateFormat, dateTime);       
            }
        }

        public String RawFileName
        {
            get
            {
                return filename;
            }
        }
    }
}
