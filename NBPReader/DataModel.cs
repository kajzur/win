using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBPReader
{
    public class DataModel
    {
        private ObservableCollection<ChartValue> chartValues = new ObservableCollection<ChartValue>();
        public ObservableCollection<ChartValue> ChartValues
        {
            get
            {
                return chartValues;
            }
            set
            {
                chartValues = value;
            }
        }

        private string currentValue;
        public string CurrentValue
        {
            get
            {
                return currentValue;
            }

            set
            {
                currentValue = value;
            }
        }

        public DataModel()
        {

        }
    }
}
