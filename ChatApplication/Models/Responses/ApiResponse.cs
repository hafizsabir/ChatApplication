using ChatApplication.Models.Responses.Common;
using System.Net;

namespace ChatApplication.Models.Responses
{
    public class ApiResponse<T>
    {
        private bool v1;
        private object userInfo;
        private string v2;
        private HttpStatusCode oK;

        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public ErrorCodes StatusCode { get; set; }

        public ApiResponse(bool success, T data = default, string message = null, ErrorCodes statusCode = ErrorCodes.OK)
        {
            Success = success;
            Data = data;
            Message = message ?? statusCode.ToString();
            StatusCode = statusCode;
        }

      

        public static ApiResponse<T> SuccessResponse(T data, string message)
        {
            return new ApiResponse<T>(true, data, message.ToString(), ErrorCodes.OK);
        }

        // Failure Response with customizable error message and status code
        public static ApiResponse<T> FailResponse(string message, ErrorCodes statusCode = ErrorCodes.BadRequest)
        {
            return new ApiResponse<T>(false, default, message.ToString(), statusCode);
        }

    }
}
