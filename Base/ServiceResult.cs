namespace Base
{
    public class ServiceResult
    {
        public string Message { get; private init; } = null!;
        public bool IsOk { get; private init; } 
        

        public static ServiceResult Ok(string message)
        {
            return new ServiceResult{Message = message, IsOk = true};
        }
        
        public static ServiceResult Error(string message)
        {
            return new ServiceResult{Message = message, IsOk = false};
        }
        
        public static ServiceResult Ok()
        {
            return new ServiceResult{Message = null!, IsOk = true};
        }
    }
}