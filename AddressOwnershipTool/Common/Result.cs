﻿namespace AddressOwnershipTool.Common
{
    public enum ResultStatus
    {
        Success = 0,
        Error
    }

    public class Result
    {
        public Result()
        {
        }

        public int StatusCode { get; set; }

        public ResultStatus Status { get; set; }
        public bool Success => this.Status == ResultStatus.Success;

        public string Message { get; set; }

        public bool Failure => !Success;

        protected Result(ResultStatus status, string message = null)
        {
            this.Status = status;
            this.Message = message;
        }

        public static Result Fail(string message)
        {
            return new Result(ResultStatus.Error, message);
        }

        public static Result<T> Fail<T>(string message)
        {
            return new Result<T>(default(T), ResultStatus.Error, message);
        }

        public static Result Ok()
        {
            return new Result(ResultStatus.Success, string.Empty);
        }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value, ResultStatus.Success, string.Empty);
        }

        public static Result Combine(params Result[] results)
        {
            foreach (Result result in results)
            {
                if (result.Failure)
                    return result;
            }

            return Ok();
        }
    }


    public class Result<T> : Result
    {
        public Result()
        {

        }

        private T _value;

        public T Value { get; set; }

        protected internal Result(T value, ResultStatus status, string message = null)
            : base(status, message)
        {
            Value = value;
        }
    }
}
