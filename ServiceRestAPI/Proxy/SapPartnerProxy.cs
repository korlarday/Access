
using Allprimetech.GeneralUtils;
using Allprimetech.ServiceRestAPI.Metadatas;
using Newtonsoft.Json;
using RestSharp;
using System;

namespace Allprimetech.ServiceRestAPI.Proxy
{
    public class SapPartnerProxy
    {
        public string _BaseResource { get; set; }

        public SapPartnerProxy()
        {

        }
        public SapPartnerProxy(string url)
        {
            _BaseResource = AppConfigUtil.BaseResource(url);
            //+"partners/";
        }

        public ReadDisc GetClientDisc(DiscAndGroupInfoResource NewData)
        {
            try
            {
                var client = new RestClient(_BaseResource);
                var request = new RestRequest($"{NewData.CustomerId}/{NewData.TotalGroup}/{NewData.UniqueGroup}", Method.POST) { RequestFormat = DataFormat.Json };

                var json = JsonConvert.SerializeObject(NewData, AppConfigUtil.Jsonsettings);
                request.AddParameter("application/json", json, null, ParameterType.RequestBody);
                //request = AppConfigUtil.AddCustomHeader(request);

                IRestResponse<ReadDisc> obj = client.Execute<ReadDisc>(request);
                if (obj.StatusCode == System.Net.HttpStatusCode.OK)
                    return obj.Data;
                else
                    return null;
            }
            catch (Exception ex)
            {
                Logs.logError("SapPartnerProxy", ex.Message, "GetClientDisc", ex);
                return null;
            }
        }


    }
}
