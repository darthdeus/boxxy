using System;

namespace Boxxy.Core
{
    public delegate ParseResult<T> ParserFunc<T>(string input);

    public static class Parser
    {
        private static Parser<char> AnyChar() {
            return new Parser<char>(
                s => {
                    if (s.Length == 0) {
                        return ParseResult.Failed<char>("Expected char, empty string given.");
                    } else {
                        return ParseResult.Passed(s[0], s.Substring(1));
                    }
                });
        }

        private static Parser<char> Sat(Func<char, bool> f) {
            return AnyChar().Then<char>(
                c => {
                    if (f(c)) {
                        return Result(c);
                    } else {
                        return Failed<char>(string.Format("Unexpected char '{0}'.", c));
                    }
                });
        }

        public static Parser<B> Combine<A, B>(Parser<A> a, Parser<B> b) {
            return a.Then(x => b);
        }

        /// Builds a parser for a given char <code>c</code>.
        public static Parser<char> Char(char c) {
            return Sat(x => x == c);
        }

        /// Builds a parser for a given string.
        public static Parser<string> String(string str) {
            if (str.Length == 0) {
                return Result("");
            } else {
                var combined = Combine(Char(str[0]), String(str.Substring(1)));
                return Combine(combined, Result(str));
            }
        }

        public static Parser<T> Result<T>(T value) {
            return new Parser<T>(s => ParseResult.Passed(value, s));
        }

        public static Parser<T> Failed<T>(string why) {
            return new Parser<T>(s => ParseResult.Failed<T>(why));
        }
    }

    public class Parser<T>
    {
        public Parser(ParserFunc<T> f) {
            _f = f;
        }

        private ParserFunc<T> _f;

        public ParseResult<T> Run(string input) {
            return _f(input);
        }

        public Parser<U> Then<U>(Func<T, Parser<U>> next) {
            return new Parser<U>(
                s => {
                    var res = Run(s);

                    if (res.Success) {
                        var parserU = next(res.Result.Item1);
                        return parserU.Run(res.Result.Item2);
                    } else {
                        return FailedResult<U>(res.Error);
                    }
                });
        }

        private ParseResult<U> FailedResult<U>(string error) {
            return new ParseResult<U> {Success = false, Error = error};
        }
    }
}