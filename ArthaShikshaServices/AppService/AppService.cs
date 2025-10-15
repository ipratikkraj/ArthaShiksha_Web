using ArthaShikshaShared.Model;
using ArthaShikshaShared.UtilitiesModel;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace LearniFyWeb.Services.AppService
{

	public class AppService : IAppService
	{

		private readonly HttpClient _httpClient;

        // Add a private readonly ILogger<AppService> field to the class
        private readonly ILogger<AppService> _logger;
        public AppService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            httpClient.Timeout = TimeSpan.FromSeconds(600);
        }
        public async Task<Response> GetHttpCall(string url)
        {
            Response response = new Response();
            try
            {
                var output = await _httpClient.GetAsync(url);
                response = await output.Content.ReadFromJsonAsync<Response>();
            }
            catch (Exception ex)
            {
                response.StatusMessage = ex.Message;
            }
            return response;
        }
        public async Task<APIMessage> GetHttpCall1(string url)
        {
            APIMessage response = new APIMessage();
            try
            {
                var output = await _httpClient.GetAsync(url);
                response = await output.Content.ReadFromJsonAsync<APIMessage>();
            }
            catch (Exception ex)
            {
                response.StatusMessage = ex.Message;
            }
            return response;
        }
        public async Task<Response> PostHttpCall(string url, object data)
        {
            Response response = new Response();
            try
            {
                var output = await _httpClient.PostAsJsonAsync(url, data);
                response = await output.Content.ReadFromJsonAsync<Response>();
            }
            catch (Exception ex)
            {
                response.StatusMessage = ex.Message;
            }

            return response;
        }
        public async Task<Response> DeleteHttpCall(string url)
        {
            Response response = new Response();
            try
            {
                var output = await _httpClient.DeleteAsync(url);
                response = await output.Content.ReadFromJsonAsync<Response>();
            }
            catch (Exception ex)
            {
                response.StatusMessage = ex.Message;
            }
            return response;
        }
        public async Task<byte[]> GetfileAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                else
                {
                    throw new HttpRequestException($"Failed to fetch data: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
        public async Task<ResponseV1> LogInPostHttpCall(string emailId, string password)
        {
            try
            {
                var loginModel = new PortalLoginModel
                {
                    EmailId = emailId,
                    Password = password
                };

                var response = await _httpClient.PostAsJsonAsync("api/Account/PortalLogin", loginModel);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Response Status: {response.StatusCode}");
                _logger.LogInformation($"Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var apiMessage = JsonConvert.DeserializeObject<APIMessage>(responseContent);
                    return new ResponseV1
                    {
                        Success = true,
                        StatusCode = apiMessage.StatusCode,
                        Message = apiMessage.StatusMessage,
                        Data = apiMessage.Data
                    };
                }

                return new ResponseV1
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Message = responseContent
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for email: {EmailId}", emailId);
                return new ResponseV1
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "An error occurred while trying to login"
                };
            }
        }
        public async Task<string> GetStringAsync(string url)
        {
            string sasToken = "";
            try
            {
                //if (!url.Contains("GetSASToken"))
                //await AddOrUpdateHeader();
                //string url1 = "GetSASToken";
                var output = await _httpClient.GetAsync(url);
                sasToken = await output.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {

            }
            return sasToken;
        }

        public async Task<Response> UploadFile(IBrowserFile file,string apiurl)
        {
            
                Response response = new Response();
                using (var ms = file.OpenReadStream(file.Size))
                {
                    var content = new MultipartFormDataContent();
                    content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
                    content.Add(new StreamContent(ms, Convert.ToInt32(file.Size)), "files", file.Name);

                    var result = await _httpClient.PostAsync(apiurl, content);
                    response = await result.Content.ReadFromJsonAsync<Response>();
                }
            return response;
        }
        public async Task<Response> UploadFilePDF(HttpContent content, string apiurl)
        {
            Response response = new Response();
            try
            {
                // Perform the API POST request with MultipartFormDataContent or ByteArrayContent
                var result = await _httpClient.PostAsync(apiurl, content);

                // Parse the response into the Response model
                response = await result.Content.ReadFromJsonAsync<Response>();
            }
            catch (Exception ex)
            {
                // Handle exceptions
                response.StatusMessage = $"Error: {ex.Message}";
            }
            return response;
        }

        // Update the constructor to accept ILogger<AppService> and assign it
        public AppService(HttpClient httpClient, ILogger<AppService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            httpClient.Timeout = TimeSpan.FromSeconds(600);
        }
    }
}

