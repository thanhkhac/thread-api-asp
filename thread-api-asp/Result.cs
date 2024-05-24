namespace thread_api_asp
{
    public class Result
    {
        public string? Message { get; set; }
        public object? Data { get; set; }
        public bool IsOk { get; set; }

        public static Result OkMessage(object? data, string message)
        {
            return new Result
            {
                Message = message,
                Data = data,
                IsOk = true
            };
        }
        public static Result Ok(object? data)
        {
            return new Result
            {
                Message = null,
                Data = data,
                IsOk = true
            };
        }

        public static Result Ok()
        {
            return new Result
            {
                IsOk = true
            };
        }
        public static Result Error()
        {
            return new Result
            {
                IsOk = false
            };
        }


        public static Result Error(object? data)
        {
            return new Result
            {
                Message = null,
                Data = data,
                IsOk = false
            };
        }

        public static Result ErrorMessage(string message)
        {
            return new Result
            {
                Message = message,
                Data = null,
                IsOk = false
            };
        }
        
        public static Result ErrorMessage(object? data, string message)
        {
            return new Result
            {
                Message = message,
                Data = data,
                IsOk = false
            };
        }


    }
}