using Allprimetech.GeneralUtils;
using Allprimetech.ServiceRestAPI.Metadatas;
using Newtonsoft.Json;
using RestSharp;
using System;

namespace Allprimetech.ServiceRestAPI.Proxy
{
    public class CustomersProxy
    {
        public string _BaseResource { get; set; }

        public CustomersProxy()
        {

        }
        public CustomersProxy(string url)
        {
            _BaseResource = AppConfigUtil.BaseResource(url) + "/api/customers/";
        }

        public bool CreateIACustomer(CreateCustomer NewData)
        {
            try
            {
                var client = new RestClient(_BaseResource);
                var request = new RestRequest($"CreateSapCustomer", Method.POST) { RequestFormat = DataFormat.Json };

                var json = JsonConvert.SerializeObject(NewData, AppConfigUtil.Jsonsettings);
                request.AddParameter("application/json", json, null, ParameterType.RequestBody);
                request = AppConfigUtil.AddIntegraAdminHeader(request);

                IRestResponse<bool> obj = client.Execute<bool>(request);
                if (obj.StatusCode == System.Net.HttpStatusCode.OK)
                    return obj.Data;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Logs.logError("CustomerProxy", ex.Message, "CreateIACustomer", ex);
                return false;
            }
        }

        public bool OrderConfirmationEmail(SendOrderConfirmation data)
        {
            try
            {
                var client = new RestClient(_BaseResource);
                var request = new RestRequest($"SendCustomerOrderConfirmation", Method.POST) { RequestFormat = DataFormat.Json };

                var json = JsonConvert.SerializeObject(data, AppConfigUtil.Jsonsettings);
                request.AddParameter("application/json", json, null, ParameterType.RequestBody);
                request = AppConfigUtil.AddIntegraAdminHeader(request);

                IRestResponse<bool> obj = client.Execute<bool>(request);
                if (obj.StatusCode == System.Net.HttpStatusCode.OK)
                    return obj.Data;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Logs.logError("CustomerProxy", ex.Message, "OrderConfirmationEmail", ex);
                return false;
            }
        }

        public bool SendEmailReadyForPickUp(SendOrderConfirmation data)
        {
            try
            {
                var client = new RestClient(_BaseResource);
                var request = new RestRequest($"SendEmailReadyForPickUp", Method.POST) { RequestFormat = DataFormat.Json };

                var json = JsonConvert.SerializeObject(data, AppConfigUtil.Jsonsettings);
                request.AddParameter("application/json", json, null, ParameterType.RequestBody);
                request = AppConfigUtil.AddIntegraAdminHeader(request);

                IRestResponse<bool> obj = client.Execute<bool>(request);
                if (obj.StatusCode == System.Net.HttpStatusCode.OK)
                    return obj.Data;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Logs.logError("CustomerProxy", ex.Message, "SendEmailReadyForPickUp", ex);
                return false;
            }
        }

    }
}
