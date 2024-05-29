namespace Identity
{
    public class ApiResponse
    {
        public string? Message { get; set; }
        public object? Data { get; set; }
        public bool IsOk { get; set; }

        public static ApiResponse OkMessage(object? data, string? message)
        {
            return new ApiResponse
            {
                Message = message,
                Data = data,
                IsOk = true
            };
        }

        public static ApiResponse OkMessage(string message)
        {
            return new ApiResponse
            {
                Message = message,
                Data = null,
                IsOk = true
            };
        }

        public static ApiResponse Ok(object? data)
        {
            return new ApiResponse
            {
                Message = null,
                Data = data,
                IsOk = true
            };
        }

        public static ApiResponse Ok()
        {
            return new ApiResponse
            {
                IsOk = true
            };
        }
        public static ApiResponse Error()
        {
            return new ApiResponse
            {
                IsOk = false
            };
        }


        public static ApiResponse Error(object? data)
        {
            return new ApiResponse
            {
                Message = null,
                Data = data,
                IsOk = false
            };
        }

        public static ApiResponse ErrorMessage(string message)
        {
            return new ApiResponse
            {
                Message = message,
                Data = null,
                IsOk = false
            };
        }

        public static ApiResponse ErrorMessage(object? data, string? message)
        {
            return new ApiResponse
            {
                Message = message,
                Data = data,
                IsOk = false
            };
        }



    }
}