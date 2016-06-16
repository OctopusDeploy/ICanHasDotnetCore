using System;
using System.Collections.Generic;
using System.Linq;

namespace ICanHasDotnetCore.Magic
{
    public interface IResult
    {
        bool WasSuccessful { get; }
        string[] Errors { get; }
        bool WasFailure { get; }
        string ErrorString { get; }
    }

    public class Result : IResult
    {
        private string[] _errors;

        private Result()
        {

        }

        public bool WasSuccessful { get; private set; }
        public bool WasFailure => !WasSuccessful;

        public string[] Errors => _errors.ToArray();

        public string ErrorString => string.Join(Environment.NewLine, _errors);

        public static Result Failed(params string[] errors)
        {
            return new Result() { _errors = errors.ToArray() };
        }

        public static Result<T> Failed<T>(params string[] errors)
        {
            return Result<T>.Failed(errors);
        }

        public static Result Failed(params IResult[] becauseOf)
        {
            return new Result() { _errors = becauseOf.SelectMany(b => b.Errors).ToArray() };
        }

        public static Result Success()
        {
            return new Result() { WasSuccessful = true };
        }

        public static Result<T> Success<T>(T value)
        {
            return Result<T>.Success(value);
        }


        public static Result From(params IResult[] results)
        {
            var failed = results.Where(r => !r.WasSuccessful).ToArray();
            if (failed.Length == 0)
                return Result.Success();
            return Result.Failed(failed.SelectMany(f => f.Errors).ToArray());
        }
    }

    public class Result<T> : IResult
    {
        private T _value;
        private string[] _errors;

        private Result()
        {

        }

        public bool WasSuccessful { get; private set; }

        public string[] Errors => _errors?.ToArray();

        public string ErrorString => _errors != null ? string.Join(Environment.NewLine, _errors) : null;

        public T Value
        {
            get
            {
                if (!WasSuccessful)
                    throw new Exception("No value as the operation was not successful");
                return _value;
            }
        }

        public bool WasFailure => !WasSuccessful;

        public static Result<T> Failed()
        {
            return new Result<T>() { _errors = new string[0] };
        }

        public static Result<T> Failed(params string[] errors)
        {
            return new Result<T>() { _errors = errors.ToArray() };
        }

        public static Result<T> Failed(params IResult[] becauseOf)
        {
            return new Result<T>() { _errors = becauseOf.Where(s => s.WasFailure).SelectMany(b => b.Errors).ToArray() };
        }

        public static Result<T> Failed(IReadOnlyCollection<IResult> becauseOf)
        {
            return new Result<T>() { _errors = becauseOf.Where(s => s.WasFailure).SelectMany(b => b.Errors).ToArray() };
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>() { WasSuccessful = true, _value = value };
        }

        public static implicit operator Result<T>(T value)
        {
            return Success(value);
        }

        public static implicit operator T(Result<T> result)
        {
            return result.Value;
        }

        public T ValueOr(T def)
        {
            return WasSuccessful ? Value : def;
        }

        public override string ToString()
        {
            return WasSuccessful
                ? "" + Value
                : ErrorString;
        }

        public static Result<T> From<TIn>(Result<TIn> result) where TIn : T
        {
            return result.WasSuccessful
                ? Success(result.Value)
                : Failed(result);
        }
    }

    public static class ResultExtensions
    {
        public static Result<TOut> Then<TIn, TOut>(
            this IReadOnlyCollection<Result<TIn>> results,
            Func<IEnumerable<TIn>, TOut> ifAllSuccessful
            )
        {
            if (results.All(r => r.WasSuccessful))
                return ifAllSuccessful(results.Select(r => r.Value));

            return Result<TOut>.Failed(results);
        }

        public static Result<TOut> Then<TIn, TOut>(
            this Result<TIn> result,
            Func<TIn, Result<TOut>> ifSuccessful
            )
        {
            return result.WasSuccessful ? ifSuccessful(result.Value) : Result<TOut>.Failed(result);
        }

        public static Result<TOut> Then<TIn, TOut>(
            this Result<TIn> result,
            Func<TIn, TOut> ifSuccessful
            )
        {
            return result.WasSuccessful ? Result<TOut>.Success(ifSuccessful(result.Value)) : Result<TOut>.Failed(result);
        }

        public static Result<IReadOnlyList<T>> InvertToList<T>(this IEnumerable<Result<T>> results)
        {
            IReadOnlyList<Result<T>> resultsArr = results.ToArray();
            if (resultsArr.All(r => r.WasSuccessful))
                return Result<IReadOnlyList<T>>.Success(resultsArr.Select(r => r.Value).ToArray());
            return Result<IReadOnlyList<T>>.Failed(resultsArr);
        }

        public static Result<TOut> Combine<TA, TB, TOut>(this Result<TA> a, Result<TB> b, Func<TA, TB, TOut> transform)
        {
            if (a.WasSuccessful && b.WasSuccessful)
                return transform(a.Value, b.Value);
            return Result<TOut>.Failed(a, b);
        }

        public static Result<T> If<T>(this Result<T> a, IResult b)
        {
            if (a.WasSuccessful && b.WasSuccessful)
                return a.Value;
            return Result<T>.Failed(a, b);
        }

        public static Result<T> If<T>(this IResult a, Result<T> b)
        {
            if (a.WasSuccessful && b.WasSuccessful)
                return b.Value;
            return Result<T>.Failed(a, b);
        }

        public static Result<T> ToResult<T>(this Option<T> option, string errorIfNone)
        {
            return option.Some ? Result<T>.Success(option.Value) : Result<T>.Failed(errorIfNone);
        }

        public static Result<T> AsSuccess<T>(this T value)
        {
            return Result<T>.Success(value);
        }
    }

}