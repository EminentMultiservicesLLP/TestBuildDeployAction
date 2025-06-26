using CommonLayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using CommonLayer;

namespace CGHSBilling.Common
{
    public class AsyncWebCalls
    {
        static readonly ILogger Logger = CommonLayer.Logger.Register(typeof(AsyncWebCalls));

        
        [AsyncTimeout(300)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        public static async Task<T> GetAsync<T>(HttpClient client, string url, CancellationToken cancellationToken) where T : new()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                  
                    httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "deflate,gzip");
                    var response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        var contentEncoding = response.Content.Headers.ContentEncoding;
                        if (!contentEncoding.Any())
                        {
                            return response.Content.ReadAsAsync<T>().Result;
                        }

                        var encoding = contentEncoding.Single().ToLower();
                        if (String.Compare(encoding, "gzip", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var responseStream = response.Content.ReadAsStreamAsync().Result;
                            var result = GZipCompressionHelper.DeCompressResponse(responseStream);
                            return JsonConvert.DeserializeObject<T>(result);
                        }
                    }

                    var exception =
                        new Exception("Resource server returned an error. StatusCode : {response.StatusCode}");
                    exception.Data.Add("StatusCode", response.StatusCode);
                    throw exception;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in AsyncWebCalls method GetAsync :" + ex.Message + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }

        [AsyncTimeout(300)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        public static async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken) where T : new()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {

                    var weburl = CGHSBilling.Common.Constants.WebAPIAddress + url + Common.Constants.JsonTypeResult;
                    httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "deflate,gzip");
                    var response = await httpClient.GetAsync(weburl, cancellationToken).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        var contentEncoding = response.Content.Headers.ContentEncoding;
                        if (!contentEncoding.Any())
                        {
                            return response.Content.ReadAsAsync<T>().Result;
                        }

                        var encoding = contentEncoding.Single().ToLower();
                        if (String.Compare(encoding, "gzip", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var responseStream = response.Content.ReadAsStreamAsync().Result;
                            var result = GZipCompressionHelper.DeCompressResponse(responseStream);
                            return JsonConvert.DeserializeObject<T>(result);
                        }
                    }

                    var exception =
                        new Exception("Resource server returned an error. StatusCode : {response.StatusCode}");
                    exception.Data.Add("StatusCode", response.StatusCode);
                    throw exception;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in AsyncWebCalls method GetAsync :" + ex.Message + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }

        public static async Task<T> PostAsync<T>(string url, T data, CancellationToken cancellationToken) where T : class
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var weburl = CGHSBilling.Common.Constants.WebAPIAddress + url + Common.Constants.JsonTypeResult;

                    var responseMessage = await client.PostAsJsonAsync(weburl, data, cancellationToken).ConfigureAwait(false);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        var contentEncoding = responseMessage.Content.Headers.ContentEncoding;
                        if (contentEncoding.Any())
                        {
                            var encoding = contentEncoding.Single().ToLower();
                            if (String.Compare(encoding, "gzip", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                var responseStream = responseMessage.Content.ReadAsStreamAsync().Result;
                                var result = GZipCompressionHelper.DeCompressResponse(responseStream);
                                return JsonConvert.DeserializeObject<T>(result);
                            }
                        }

                        return JsonConvert.DeserializeObject<T>(responseMessage.Content.ReadAsStringAsync().Result);
                    }
                    var exp = JsonConvert.DeserializeObject<T>(responseMessage.Content.ReadAsStringAsync().Result);
                    TryCatch.RunSilent(() =>
                    {
                        var httpResponseMessage = exp as HttpResponseMessage;
                        if (httpResponseMessage != null)
                        {
                            httpResponseMessage.StatusCode = responseMessage.StatusCode;
                            httpResponseMessage.ReasonPhrase = responseMessage.ReasonPhrase;
                        }
                    });
                    return exp;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in AsyncWebCalls method PostAsync :" + ex.Message + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }

        public static async Task<T> PostBigDataAsync<T>(string url, T data, CancellationToken cancellationToken) where T : class
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var weburl = CGHSBilling.Common.Constants.WebAPIAddress + url + Common.Constants.JsonTypeResult;
                    client.DefaultRequestHeaders.Add("Accept-Encoding", "deflate,gzip");

                    var responseMessage = await client.PostAsJsonAsync(weburl, data, cancellationToken).ConfigureAwait(false);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        var contentEncoding = responseMessage.Content.Headers.ContentEncoding;
                        if (contentEncoding.Any())
                        {
                            var encoding = contentEncoding.Single().ToLower();
                            if (String.Compare(encoding, "gzip", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                var responseStream = responseMessage.Content.ReadAsStreamAsync().Result;
                                var result = GZipCompressionHelper.DeCompressResponse(responseStream);
                                return JsonConvert.DeserializeObject<T>(result);
                            }
                        }

                        return JsonConvert.DeserializeObject<T>(responseMessage.Content.ReadAsStringAsync().Result);
                    }

                    var exp = JsonConvert.DeserializeObject<T>(responseMessage.Content.ReadAsStringAsync().Result);
                    TryCatch.RunSilent(() =>
                    {
                        var httpResponseMessage = exp as HttpResponseMessage;
                        if (httpResponseMessage != null)
                        {
                            httpResponseMessage.StatusCode = responseMessage.StatusCode;
                            httpResponseMessage.ReasonPhrase = responseMessage.ReasonPhrase;
                        }
                    });
                    return exp;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in AsyncWebCalls method PostAsync :" + ex.Message + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }

        public static async Task<HttpResponseMessage> PutAsync<T>(string url, T data, CancellationToken cancellationToken) where T : class
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var weburl = CGHSBilling.Common.Constants.WebAPIAddress + url + Common.Constants.JsonTypeResult;
                    var responseMessage = await client.PutAsJsonAsync(weburl, data, cancellationToken).ConfigureAwait(false);

                    if (responseMessage == null)
                        responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                        {
                            ReasonPhrase = "Internal Server Error, Please contact system Administrator"
                        };

                    return responseMessage;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in AsyncWebCalls method PutAsync :" + ex.Message + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }

        public static async Task<T> PutAsync_ModelResult<T>(string url, T data, CancellationToken cancellationToken) where T : class
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var weburl = CGHSBilling.Common.Constants.WebAPIAddress + url + Common.Constants.JsonTypeResult;
                    var responseMessage = await client.PutAsJsonAsync(weburl, data, cancellationToken).ConfigureAwait(false);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        var contentEncoding = responseMessage.Content.Headers.ContentEncoding;
                        if (contentEncoding.Any())
                        {
                            var encoding = contentEncoding.Single().ToLower();
                            if (String.Compare(encoding, "gzip", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                var responseStream = responseMessage.Content.ReadAsStreamAsync().Result;
                                var result = GZipCompressionHelper.DeCompressResponse(responseStream);
                                return JsonConvert.DeserializeObject<T>(result);
                            }
                        }
                        return JsonConvert.DeserializeObject<T>(responseMessage.Content.ReadAsStringAsync().Result);
                    }

                    var exception = new Exception("Resource server returned an error. StatusCode : {responseMessage.StatusCode}");
                    exception.Data.Add("StatusCode", responseMessage.StatusCode);
                    exception.Data.Add("ReasonPhrase", responseMessage.ReasonPhrase);
                    throw exception;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in AsyncWebCalls method PutAsync :" + ex.Message + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }

        public static async Task<T> PutBigDataAsync<T>(string url, T data, CancellationToken cancellationToken) where T : class
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var weburl = CGHSBilling.Common.Constants.WebAPIAddress + url + Common.Constants.JsonTypeResult;
                    client.DefaultRequestHeaders.Add("Accept-Encoding", "deflate,gzip");

                    var responseMessage = await client.PutAsJsonAsync(weburl, data, cancellationToken).ConfigureAwait(false);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        var contentEncoding = responseMessage.Content.Headers.ContentEncoding;
                        var encoding = contentEncoding.Single().ToLower();
                        if (String.Compare(encoding, "gzip", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var responseStream = responseMessage.Content.ReadAsStreamAsync().Result;
                            var result = GZipCompressionHelper.DeCompressResponse(responseStream);
                            return JsonConvert.DeserializeObject<T>(result);
                        }

                        return JsonConvert.DeserializeObject<T>(responseMessage.Content.ReadAsStringAsync().Result);
                    }

                    var exception = new Exception("Resource server returned an error. StatusCode : {responseMessage.StatusCode}");
                    exception.Data.Add("StatusCode", responseMessage.StatusCode);
                    exception.Data.Add("ReasonPhrase", responseMessage.ReasonPhrase);
                    throw exception;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in AsyncWebCalls method PutAsync :" + ex.Message + Environment.NewLine + ex.StackTrace);
                throw;

            }
        }

        public static async Task<bool> DeleteAsync(string url, CancellationToken cancellationToken)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var weburl = CGHSBilling.Common.Constants.WebAPIAddress + url + Common.Constants.JsonTypeResult;
                    var responseMessage = await client.DeleteAsync(weburl, cancellationToken).ConfigureAwait(false);
                    if (responseMessage.IsSuccessStatusCode)
                        return true;

                    var exception = new Exception("Resource server returned an error. StatusCode : {responseMessage.StatusCode}");
                    exception.Data.Add("StatusCode", responseMessage.StatusCode);
                    throw exception;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in AsyncWebCalls method GetAsync :" + ex.Message + Environment.NewLine + ex.StackTrace);
                throw ;

            }
        }
    }

    public class GZipCompressionHelper
    {
        public static string DeCompressResponse(Stream responseStream)
        {
            var gzipStream = new GZipStream(responseStream, CompressionMode.Decompress);
            var destinationStream = new MemoryStream();
            gzipStream.CopyTo(destinationStream);
            return Encoding.ASCII.GetString(destinationStream.ToArray());
        }
    }
}