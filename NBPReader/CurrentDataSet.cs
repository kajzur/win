using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBPReader
{
    class CurrentDataSet
    {
        private List<DataRetrieverItem> _items = new List<DataRetrieverItem>();
        private DateTime _date = new DateTime();

        public List<DataRetrieverItem> items
        {

            get
            {
                return _items;
            }
            set
            {
                _items = value;
            }
        
        }

        public DateTime date
        {
            get
            {
                return _date;
            }

            set
            {
                _date = value;
            }
        }
        public CurrentDataSet()
        {

        }


    }
}
