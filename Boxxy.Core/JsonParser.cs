using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Boxxy.Core
{
    public interface JsonValue<T>
    {
        T Value { get; set; }
    }

    public class InvalidToRepUsage : Exception
    {
    }

    public abstract class JsonValue
    {
        // A workaround for not having either dependent names
        public abstract T ToRep<T>();
    }

    public class JsonString : JsonValue, JsonValue<string>
    {
        public string Value { get; set; }

        public override T ToRep<T>() {
            if (typeof (T) != typeof (string)) throw new InvalidToRepUsage();
            return (T) (object) Value;
        }
    }

    public class JsonNumber : JsonValue, JsonValue<double>
    {
        public double Value { get; set; }

        public override T ToRep<T>() {
            if (typeof (T) != typeof (double)) throw new InvalidToRepUsage();
            return (T) (object) Value;
        }
    }

    public class JsonObject : JsonValue, JsonValue<Dictionary<string, JsonValue>>
    {
        public Dictionary<string, JsonValue> Value { get; set; }

        public JsonObject() {
            Value = new Dictionary<string, JsonValue>();
        }

        public override T ToRep<T>() {
            if (typeof (T) != typeof (Dictionary<string, JsonValue>)) throw new InvalidToRepUsage();
            return (T) (object) Value;
        }
    }

    public class JsonArray : JsonValue, JsonValue<List<JsonValue>>
    {
        public List<JsonValue> Value { get; set; }

        public JsonArray() {
            Value = new List<JsonValue>();
        }

        public override T ToRep<T>() {
            if (typeof (T) != typeof (List<JsonValue>)) throw new InvalidToRepUsage();
            return (T) (object) Value;
        }
    }

    public class Json
    {
        internal static Parser<JsonValue> AnyValue() {
            return
                Array().Cast<JsonValue>() |
                String().Cast<JsonValue>() |
                Number().Cast<JsonValue>() |
                Object().Cast<JsonValue>();
        }

        internal static Parser<JsonString> String() {
            var parser = Parser.EscapedString('"');

            return parser.Then(s => Parser.Result(new JsonString {Value = s}));
        }

        internal static Parser<JsonNumber> Number() {
            return Parser.Or(Parser.Double(), Parser.Number().Then(x => Parser.Result((double) x)))
                .Then(x => Parser.Result(new JsonNumber {Value = x}));
        }

        internal static Parser<JsonObject> Object() {
            return Parser.Bracket(
                Parser.Char('{').IgnoreWhitespace(),
                Parser.SepBy(KeyValuePairParser().IgnoreWhitespace(), Parser.Char(',').IgnoreWhitespace()),
                Parser.Char('}').IgnoreWhitespace()).Then(
                    pairs => {
                        var obj = new JsonObject();

                        foreach (var pair in pairs) {
                            obj.Value[pair.Item1] = pair.Item2;
                        }

                        return Parser.Result(obj);
                    });
        }

        internal static Parser<JsonArray> Array()
        {
            return Parser.Bracket(
                Parser.Char('['),
                Parser.SepBy(AnyValue(), Parser.Char(',').IgnoreWhitespace()),
                Parser.Char(']')).Then(
                    items => Parser.Result(new JsonArray { Value = items }));
        }

        private static Parser<Tuple<string, JsonValue>> KeyValuePairParser() {
            return String().Then<Tuple<string, JsonValue>>(
                key => Parser.Char(':').IgnoreWhitespace().Then<Tuple<string, JsonValue>>(
                    _ => {
                        return AnyValue().Then(
                            value => Parser.Result(Tuple.Create(key.Value, value)));
                    }));
        }

        public static ParseResult<JsonObject> Parse(string s) {
            return Object().Run(s);
        }
    }
}