using System;

namespace Boxxy.Core
{
    public static class ParseResult
    {
        public static ParseResult<T> Failed<T>(string why) {
            return new ParseResult<T> {Success = false, Error = why};
        }

        public static ParseResult<T> Passed<T>(T value, string remains) {
            return new ParseResult<T> {Success = true, Result = Tuple.Create(value, remains)};
        }
    }

    /// Represents a sum type of parse result, which can either contain a valid result,
    /// or an error message containing the failure reason.            
    public class ParseResult<T>
    {
        public bool Success { get; set; }
        public Tuple<T, string> Result { get; set; }
        public string Error { get; set; }

        internal ParseResult() {
        }
    }
}