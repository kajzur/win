using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;

namespace NBPReader
{
    class CacheController
    {
		private StorageFolder cacheFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        private string waluta;
        public string Waluta
        {
            get
            {
                return waluta;
            }
        }
        private string fromDate;
        private string toDate;
        public CacheController(string waluta, DateTime dateFrom, DateTime dateTo)
        {
            this.waluta = waluta;
            fromDate = dateFrom.Date.ToString();
            toDate = dateTo.Date.ToString();
        }
		private string generateFileName(){
			string extension = ".xml";
            return waluta + "-" + fromDate.Split(' ')[0].Replace('/', '_') + "-" + toDate.Split(' ')[0].Replace('/', '_') + extension;
		}
        private async Task<bool> checkIsExists()
        {
			bool exists = await FileExistsAsync(cacheFolder, generateFileName());
			return exists;
        }
		public async Task<bool> FileExistsAsync(StorageFolder folder, string fileName)
		{
            try
            {
                var isNull = await folder.GetItemAsync(fileName);
                return isNull==null?false:true;
            }
            catch (Exception x)
            {
                return false;
            }
		}

        public async Task<bool> IsCacheAvailable()
        {
            return await checkIsExists();
        }
		public async Task<List<ChartValue>> GetCachedValues(){
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(List<ChartValue>));
            Stream file = await cacheFolder.OpenStreamForReadAsync(generateFileName());
            List<ChartValue> list = (List<ChartValue>)x.Deserialize(file);
            file.Dispose();
            return list;
		}

		public async Task<int> CreateCache(List<ChartValue> values){
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(List<ChartValue>));
            StorageFile file = await cacheFolder.CreateFileAsync(generateFileName());
            var stream = await file.OpenStreamForWriteAsync();
            x.Serialize(stream, values);
            stream.Dispose();
            return 1;
		}
    }
}
