

namespace Application.Response
{
    public class APIResponse
    {

        public APIResponse() { }

        public object APISuccessResponse(object data, object status_code,string message)
        {
            var successResponse = new
            {
                data,
                status_code,
                message = message
            };
            return successResponse;
        }

        public object ApiErrorResponse(string message, string error_type, int status_code)
        {
            var errorResponse = new
            {
                error_type,
                status_code,
                errors = new Dictionary<string, string[]>
                            {
                                { "message", new[] { message } }
                            }
            };
            return errorResponse;
        }
    }
}

