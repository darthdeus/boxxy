using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Boxxy.Core.Tests")]

namespace Boxxy.Core
{
    public class ParseResult<T>
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public Tuple<T, string> Result { get; set; }
    }

    public static class ParseResult
    {
        public static ParseResult<T> Failed<T>(string why) {
            return new ParseResult<T> {Success = false, Error = why};
        }

        public static ParseResult<T> Passed<T>(T parsed, string rest) {
            return new ParseResult<T> {Success = true, Result = Tuple.Create(parsed, rest)};
        }
    }

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
        public static Parser<char> Char(char s) {
            return AnyChar().Then<char>(
                c => {
                    if (c == s) {
                        return Result(c);
                    } else {
                        return Failed<char>(string.Format("Unexpected char '{0}', expecting '{1}' instead.", c, s));
                    }
                });
        }

        /// Builds a parser that accepts only a given string <code>str</code>.
        public static Parser<string> String(string str) {
            if (str.Length == 0) {
                return Result("");
            } else {
                var combined = Combine(Char(str[0]), String(str.Substring(1)));
                return Combine(combined, Result(str));
            }
        }

        /// Parses a single digit
        public static Parser<char> Digit() {
            return Sat(x => x >= '0' && x <= '9');
        }

        public static Parser<T> Or<T>(Parser<T> a, Parser<T> b) {
            return new Parser<T>(
                s => {
                    var res = a.Run(s);
                    if (res.Success) {
                        return res;
                    } else {
                        return b.Run(s);
                    }
                });
        }

        /// Builds a parser that accepts any (zero to infinity) number of matches of the given parser.
        public static Parser<List<T>> Many<T>(Parser<T> p) {
            return Many1(p) | Result(new List<T>());
        }

        public static Parser<List<T>> SepBy<T, S>(Parser<T> p, Parser<S> sep) {
            return SepBy1(p, sep) | Result(new List<T>());
        }

        public static Parser<List<T>> SepBy1<T, S>(Parser<T> p, Parser<S> sep) {
            return p.Then(
                x => Many1(sep.Drop(p)).Then(
                    xs => {
                        var list = new List<T>();
                        list.Add(x);
                        list.AddRange(xs);
                        return Result(list);
                    }));
        }

        public static Parser<List<T>> Many1<T>(Parser<T> p) {
            return p.Then(
                x => {
                    return Many(p).Then(
                        xs => {
                            var res = new List<T> {x};
                            res.AddRange(xs);
                            return Result(res);
                        });
                });
        }

        public static Parser<string> ToCharString(Parser<char> c) {
            return Many(c).Then(s => Result(string.Concat(s)));
        }

        public static Parser<int> Number() {
            return Many1(Digit()).Then(x => Result(int.Parse(string.Concat(x))));
        }

        public static Parser<double> Double() {
            return Number().Then(
                pre => { return Char('.').Then(_ => Number().Then(dec => Result(double.Parse(pre + "." + dec)))); });
        }

        public static Parser<T> Bracket<L, T, R>(Parser<L> l, Parser<T> t, Parser<R> r) {
            return l.Then(_ll => t.Then(res => r.Then(_rr => Result(res))));
        }

        public static Parser<char> OneOf(string s) {
            if (s.Length == 0) {
                return Failed<char>("none of the characters matched");
            } else {
                return Or(Char(s[0]), OneOf(s.Substring(1)));
            }
        }

        public static Parser<char> NoneOf(string s) {
            return Sat(c => !s.Contains(c));
        }

        public static Parser<string> Whitespace() {
            return ToCharString(Sat(char.IsWhiteSpace));
        }

        public static Parser<string> EscapedString(char quote) {
            return Char(quote).Drop(EscapedString(quote, false));
        }        

        private static Parser<string> EscapedString(char quote, bool escaped) {
            return AnyChar().Then(
                c => {
                    // Escaped quotes are ignored, while non-escaped one terminate the parser
                    if (c == quote) {
                        if (escaped) {
                            // If we were previously in escape mode, hitting a quote simply ignores it
                            // and switches back to non-escaped mode.
                            return Concat(Result(c), EscapedString(quote, false));
                        } else {
                            return Result("");
                        }
                    } else if (c == '\\') {
                        if (escaped) {
                            return Concat(Result(c), EscapedString(quote, false));
                        } else {
                            // A backslash turns on the escaping "mode", which will ignore the next quote character,
                            // but only as long as its a quote.
                            return Concat(Result(c), EscapedString(quote, true));
                        }
                    } else {
                        return Concat(Result(c), EscapedString(quote, false));
                    }
                }) | Result("");
        }

        /// Builds a parser that doesn't consume any input and always returns the given result.
        public static Parser<T> Result<T>(T value) {
            return new Parser<T>(s => ParseResult.Passed(value, s));
        }

        /// Builds a parser that fails for any input with a given message.
        public static Parser<T> Failed<T>(string why) {
            return new Parser<T>(s => ParseResult.Failed<T>(why));
        }

        public static Parser<string> Concat(Parser<char> c, Parser<string> s) {
            return c.Then(x => s.Then(xs => Result(string.Format("{0}{1}", x, xs))));
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

        /// Runs the first parser, drops the result and runs the second parser.
        public Parser<U> Drop<U>(Parser<U> p) {
            return Then(_ => p);
        }

        public Parser<T> IgnoreWhitespace() {
            return Parser.Whitespace().Then(
                _wl =>
                    Then(
                        x => Parser.Result(x)));
        }

        public static Parser<T> operator |(Parser<T> lhs, Parser<T> rhs) {
            return Parser.Or(lhs, rhs);
        }

        private ParseResult<U> FailedResult<U>(string error) {
            return new ParseResult<U> {Success = false, Error = error};
        }

        /// Dangerous cast through <code>object</code>, use with care.
        public Parser<U> Cast<U>() {
            return Then(x => Parser.Result((U) (object) x));
        }
    }
}