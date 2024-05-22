namespace thread_api_asp
{
    public class ApiResponse
    {
        public object? Object { get; set; }
        public bool IsOk { get; set; }

        public ApiResponse(object? @object, bool isOk)
        {
            this.Object = @object;
            IsOk = isOk;
        }

        public static ApiResponse Ok(object? @object)
        {
            return new ApiResponse(@object, true);
        }

        public static ApiResponse Error(object? @object)
        {
            return new ApiResponse(@object, false);
        }
    }
}
