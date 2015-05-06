using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBPReader
{
    
    public class ChartValue
    {
        private string date;
        private double value;
        private string kod_waluty;
        public string Date
        {
            get
            {
                return date;
            }

            set
            {
                this.date = value;
            }
        }

        public string KodWaluty
        {
            get
            {
                return kod_waluty;
            }

            set
            {
                this.kod_waluty = value;
            }
        }

        public string Value
        {
            get
            {
                return value.ToString();
            }

            set
            {
                this.value = Double.Parse(value.Replace(',', '.'));
            }
        }

        public ChartValue(string name, string val)
        {
            this.date = name;
            this.value = Double.Parse(val.Replace(',', '.')); ;
        }

        public ChartValue()
        {

        }
    }
}
