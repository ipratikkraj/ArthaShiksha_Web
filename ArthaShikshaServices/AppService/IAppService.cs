using ArthaShikshaShared.UtilitiesModel;
using Microsoft.AspNetCore.Components.Forms;

namespace LearniFyWeb.Services.AppService
{
    public interface IAppService
    {
        Task<Response> GetHttpCall(string url);
        Task<APIMessage> GetHttpCall1(string url);
        Task<Response> PostHttpCall(string url, object data);
        Task<Response> DeleteHttpCall(string url);
        Task<byte[]> GetfileAsync(string url);
        Task<ResponseV1> LogInPostHttpCall(string email, string password);
        Task<string> GetStringAsync(string url);
        Task<Response> UploadFile(IBrowserFile file, string url);
        Task<Response> UploadFilePDF(HttpContent content, string apiurl);
       // Task PostHttpCall<T>(string v, AddCourseMappingModel addCourseMapping);
    }
}
