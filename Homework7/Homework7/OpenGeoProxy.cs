using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Homework7
{
    class OpenGeoProxy
    {
        public async static Task<RootObject> GetGeoInformation(String str) {
            var http = new HttpClient();
            var response = await http.GetAsync(String.Format("http://gc.ditu.aliyun.com/geocoding?a={0}", str));
            //var response = await http.GetAsync("http://gc.ditu.aliyun.com/geocoding?a=苏州市");
            var result = await response.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(RootObject)); // 序列化

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (RootObject)serializer.ReadObject(ms);

            return data;
        }
    }

    [DataContract]
    public class RootObject
    {
        [DataMember]
        public double lon { get; set; }
        [DataMember]
        public int level { get; set; }
        [DataMember]
        public string address { get; set; }
        [DataMember]
        public string cityName { get; set; }
        [DataMember]
        public int alevel { get; set; }
        [DataMember]
        public double lat { get; set; }
    }
}
