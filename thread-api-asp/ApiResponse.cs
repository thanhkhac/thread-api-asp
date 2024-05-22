namespace thread_api_asp
{
    public class ApiResponse(object? @object, bool isOk)
    {
        public object? Object { get; set; } = @object;
        public bool IsOk { get; set; } = isOk;

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
