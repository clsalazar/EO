using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using EO.Core.Data;

namespace EO.Core.Utilities.RateParser
{
    public static class ExampleREP
    {
        private static readonly string uri = "https://www.lifeenergy.com/api/rpm/v2/utilities/";
        private static HttpClient _httpClient = new HttpClient();

        //Get provider id(s) needed to request rates for given zip code
        public static Dictionary<string, string> GetIds(string zipCode)
        {
            var providerIds = new Dictionary<string, string>();

            //Prevent SSL/TLS protocol exception
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var requestIds = _httpClient.GetStringAsync(uri + zipCode);
            var resultIds = JArray.Parse(requestIds.Result);

            foreach(var id in resultIds)
            {
                providerIds.Add(id["id"].ToString(), id["name"].ToString());
            }

            return providerIds;
        }

        public static List<Rates> GetRates(string zipCode, string providerId)
        {
            var rates = new List<Rates>();

            //Prevent SSL/TLS protocol exception
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var requestRates = _httpClient.GetStringAsync(string.Format("{0}{1}/{2}/products", uri, zipCode, providerId));
            var resultRates = JArray.Parse(requestRates.Result);

            foreach(var rate in resultRates)
            {
                //LifeEnergy only displays rates with fixed pricing
                if (rate["price_type"].ToString() != "Fixed")
                    continue;

                rates.Add(new Rates
                {
                    Product = rate["name"].ToString(),
                    kwh500 = rate["latest_price_500"].ToObject<decimal>(),
                    kwh1000 = rate["latest_price_1000"].ToObject<decimal>(),
                    kwh2000 = rate["latest_price_2000"].ToObject<decimal>(),
                    PromotionDesc = rate["promotional_incentive"].ToString(),
                    Renewable = rate["renewable_percent"].ToObject<decimal>(),
                    TermValue = rate["term_months"].ToObject<int?>(),
                    FactsURL = rate["documents"].Where(x => x["name"].ToString() == "electricity_facts_label").First()["public_url"].ToString(),
                    TermsURL = rate["documents"].Where(x => x["name"].ToString() == "terms_of_service").First()["public_url"].ToString(),
                    RightsURL = rate["documents"].Where(x => x["name"].ToString() == "yrac").First()["public_url"].ToString()
                });
            }

            return rates;
        }
    }
}