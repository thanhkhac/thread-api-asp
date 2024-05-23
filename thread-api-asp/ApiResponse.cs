namespace thread_api_asp
{
    public class ApiResponse
    {
        public string? Message { get; set; }
        public object? Object { get; set; } 
        public bool IsOk { get; set; } 
        

        public static ApiResponse Ok(object? @object)
        {
            return new ApiResponse
            {
                Message = null,
                Object = @object,
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
        
        public static ApiResponse OkMessage(string message)
        {
            return new ApiResponse
            {
                Message = message,
                Object = null,
                IsOk = true
            };
        }

        public static ApiResponse Error(object? @object)
        {
            return new ApiResponse
            {
                Message = null,
                Object = @object,
                IsOk = false
            };
        }
        
        public static ApiResponse ErrorMessage(string message)
        {
            return new ApiResponse
            {
                Message = message,
                Object = null,
                IsOk = false
            };
        }
        

    }
}