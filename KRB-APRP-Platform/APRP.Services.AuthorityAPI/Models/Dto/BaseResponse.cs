namespace APRP.Services.AuthorityAPI.Models.Dto
{
    public abstract class BaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? StatusCode { get; set; }

        public BaseResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public BaseResponse(bool success, string message, string statusCode)
        {
            Success = success;
            Message = message;
            StatusCode = statusCode;
        }

    }
}
