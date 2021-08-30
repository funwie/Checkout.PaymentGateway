using System;

namespace Checkout.Functional
{
    public class Result<TSuccess, TError>
    {
        private readonly TSuccess _success;
        private readonly TError _error;

        public bool IsSuccess { get; private set; }
        public TSuccess Success
        {
            get
            {
                if (!IsSuccess)
                {
                    throw new InvalidOperationException();
                }

                return _success;
            }
        }

        public TError Error
        {
            get
            {
                if (IsSuccess)
                {
                    throw new InvalidOperationException();
                }

                return _error;
            }
        }

        public Result(TSuccess success)
        {
            IsSuccess = true;
            _success = success;
            _error = default;
        }

        public Result(TError error)
        {
            IsSuccess = false;
            _success = default;
            _error = error;
        }

        public static implicit operator Result<TSuccess, TError>(TSuccess success)
        {
            return new Result<TSuccess, TError>(success);
        }

        public static implicit operator Result<TSuccess, TError>(TError error)
        {
            return new Result<TSuccess, TError>(error);
        }
    }
}