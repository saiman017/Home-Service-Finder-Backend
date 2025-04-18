using System.Net;

namespace Home_Service_Finder
{
    public class APIResponse
    {
        public bool Success { get; set; }
        public HttpStatusCode Code { get; set; }
        public dynamic? Data { get; set; }

        public string Message { get; set; } = "";

        public APIResponse(bool success, HttpStatusCode code, dynamic data, string message)
        {
            Success = success;
            Code = code;
            Data = data;
            Message = message;
        }

    }  
}
