using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boxxy.Core
{
    public interface JsonValue<T>
    {
        T Value { get; set; }
    }

    public abstract class JsonValue
    {
    }

    public class JsonString : JsonValue, JsonValue<string>
    {
        public string Value { get; set; }
    }

    public class JsonNumber : JsonValue, JsonValue<Double>
    {
        public double Value { get; set; }
    }

    public class JsonObject : JsonValue, JsonValue<Dictionary<String, JsonValue>>
    {
        public Dictionary<string, JsonValue> Value { get; set; }

        public JsonObject() {
            Value = new Dictionary<string, JsonValue>();
        }
    }

    public class Json
    {
        internal static Parser<JsonValue> AnyValue() {
            return
                String().Cast<JsonValue>() |
                Number().Cast<JsonValue>() |
                Object().Cast<JsonValue>();
        }

        internal static Parser<JsonString> String() {
            var parser = Parser.Bracket(
                Parser.Char('"'),
                //Parser.EscapedString('"'),
                Parser.ToCharString(Parser.NoneOf("\"")),
                Parser.Char('"'));

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

        private static Parser<Tuple<string, JsonValue>> KeyValuePairParser() {
            return String().Then<Tuple<string, JsonValue>>(
                key => Parser.Char(':').IgnoreWhitespace().Then<Tuple<string, JsonValue>>(
                    _ => {
                        return AnyValue().Then(
                            value => Parser.Result(Tuple.Create(key.Value, value)));
                    }));
        }

        public static JsonValue Parse(string s) {
            // TODO
            return null;
        }
    }
}