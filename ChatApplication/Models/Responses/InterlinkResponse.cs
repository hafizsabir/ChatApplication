using ChatApplication.Models.ChatMessageModel.UserInfo;
using ChatApplication.Models.Responses.Common;

namespace ChatApplication.Models.Responses
{
    public class InterlinkResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public ErrorCodes StatusCode { get; set; }

        public InterlinkResponse(bool success, T data = default, string message = null, ErrorCodes statusCode = ErrorCodes.OK)
        {
            Success = success;
            Data = data;
            Message = message ?? statusCode.ToString();  // Default to the status code message if not provided
            StatusCode = statusCode;
        }

        
        public static InterlinkResponse<T> SuccessResponse(T data, string message, ErrorCodes statusCode = ErrorCodes.OK)
        {
            return new InterlinkResponse<T>(true, data, message, statusCode);
        }

        
        public static InterlinkResponse<T> FailResponse(T data=default, string message=default, ErrorCodes statusCode = ErrorCodes.BadRequest)
        {
            return new InterlinkResponse<T>(false, data, message, statusCode);
        }

       
    }
}