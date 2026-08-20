namespace MiniSiniestros.Common.Responses
{
    public class ValidationError
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public ValidationError() { }

        public ValidationError(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }

    public class ServiceResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<ValidationError> Errors { get; set; } = new();

        public static ServiceResponse<T> Ok(T data, string message = "")
        {
            return new ServiceResponse<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        public static ServiceResponse<T> Fail(ValidationError error)
        {
            return new ServiceResponse<T>
            {
                Success = false,
                Errors = new List<ValidationError> { error }
            };
        }

        public static ServiceResponse<T> Fail(ValidationError error, string customMessage)
        {
            return new ServiceResponse<T>
            {
                Success = false,
                Errors = new List<ValidationError> { new ValidationError(error.Code, customMessage) }
            };
        }

        public static ServiceResponse<T> Fail(string code, string message)
        {
            return new ServiceResponse<T>
            {
                Success = false,
                Errors = new List<ValidationError> { new ValidationError(code, message) }
            };
        }

        public static ServiceResponse<T> Fail(string message)
        {
            return Fail("ERROR", message);
        }

        public static ServiceResponse<T> Fail(List<ValidationError> errors)
        {
            return new ServiceResponse<T>
            {
                Success = false,
                Errors = errors
            };
        }
    }
}
