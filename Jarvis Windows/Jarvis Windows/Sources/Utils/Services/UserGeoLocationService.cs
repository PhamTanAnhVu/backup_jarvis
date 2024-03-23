using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

public static class UserGeolocationService
{
    private static async Task<IPAddress?> GetExternalIpAddress()
    {
        try
        {
            var httpClient = new HttpClient();
            var externalIpString = await httpClient.GetStringAsync("http://icanhazip.com");
            externalIpString = externalIpString.Replace("\n", "").Trim();
            if (!IPAddress.TryParse(externalIpString, out var ipAddress))
                return null;
            return ipAddress;
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public static async Task<dynamic> GetUserGeoLocation()
    {
        try
        {
            IPAddress externalIp = (await GetExternalIpAddress()) ?? IPAddress.Loopback;

            var Ip_Api_Url = $"http://ip-api.com/json/{externalIp}";

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.BaseAddress = new Uri(Ip_Api_Url);

                HttpResponseMessage response = await httpClient.GetAsync(Ip_Api_Url);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    dynamic responseObject = JsonConvert.DeserializeObject(responseContent);
                    return responseObject;
                }
                else
                {
                    return null;
                }
            }
        }
        catch (Exception)
        {
            return null;
        }
    }
}
