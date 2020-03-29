using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Skribbl_Website.Client.Extensions
{
    public static class HttpClientExtensions
    {
    //    public static async Task ExecuteJsonWExceptionAsync(
    //         Func<Task<HttpResponseMessage>> httpClientCallAsync)
    //    {
    //        HttpResponseMessage response = await httpClientCallAsync();
    //        if (response.IsSuccessStatusCode)
    //        {
    //            //var json = JsonSerializer.CreateDefault();
    //            //return Newtonsoft.Json.JsonConvert.DeserializeObject
    //            return System.Text.Json.JsonSerializer.Deserialize
    //                (
    //               (await response.Content.ReadAsStringAsync()), new JsonSerializerOptions
    //               {
    //                   PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    //               });
    //        }
    //        else
    //        {
    //            JsonSerializerSettings settings = new JsonSerializerSettings();
    //            settings.TypeNameHandling = TypeNameHandling.All;
    //            throw JsonConvert.DeserializeObject(
    //                await response.Content.ReadAsStringAsync(), settings) as Exception;
    //        }
    //    }

    //    public static async Task PostJsonWithExceptionAsync(
    // this HttpClient httpClient,
    // string url,
    // HttpContent httpContent = null)
    //    {
    //        await ExecuteJsonWExceptionAsync(
    //             () => httpClient.PostAsync(url, httpContent));
    //    }

    //    public static async Task GetJsonWithExceptionAsync(
    //           this HttpClient httpClient,
    //           string url)
    //    {
    //         await ExecuteJsonWExceptionAsync(() => httpClient.GetAsync(url));
    //    }
    }
}
