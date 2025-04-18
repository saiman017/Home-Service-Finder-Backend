using System.Net;

namespace Home_Service_Finder
{
    public class ResponseHandler
    {

        public static APIResponse GetSuccessResponse(dynamic data, string message)
        {
            return new APIResponse(true, HttpStatusCode.OK, data, message);
        }

        public static APIResponse GetSuccessResponse(dynamic data)
        {
            return new APIResponse(true, HttpStatusCode.OK, data, Message.OK);
        }

        public static APIResponse GetBadRequestResponse( string message)
        {
            return new APIResponse(false, HttpStatusCode.BadRequest, message, Message.ERROR);
        }

        public static APIResponse GetNotFoundResponse(string message)
        {
            return new APIResponse(false, HttpStatusCode.NotFound, message, Message.ERROR);
        }

        public static APIResponse GetUnauthorizedResponse( string message)
        {
            return new APIResponse(false, HttpStatusCode.Unauthorized, message, Message.ERROR);
        }

        public static APIResponse GetNoContentResponse( string message)
        {
            return new APIResponse(false, HttpStatusCode.NoContent, message, Message.ERROR);
        }



    }
}
