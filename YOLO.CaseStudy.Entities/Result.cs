namespace YOLO.CaseStudy.Entities
{
    public abstract class Result
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        protected Result(string message = null)
        {
            this.Message = message;
        }
    }

    public abstract class Result<T> : Result
    {
        protected Result(T data, string message = null) : base(message)
        {
            this.Data = data;
        }
    }

    public class SuccessResult<T> : Result<T>
    {
        public SuccessResult(T data, string message = null) : base(data, message)
        {
            this.Success = true;
        }
    }

    public class ErrorResult : Result
    {
        public ErrorResult(string message) : base(message)
        {
            this.Success = false;
        }
    }
}
