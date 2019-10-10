using System;
using System.Collections.Generic;

namespace OrletSoir.JSON
{
    /**
     * The static JSON parser class.
     * Parses a given JSON string into IJsonVariable object(s)
     * 
     * This is my own library that I wrote for a personal project
     * a while ago. It's been tested to work with JSON data
     * and should be sufficient for the purposes of this
     * application.
     * 
     * Project on GitHub: https://github.com/OrletSoir/JSON
     * 
     */
    public static class Json
    {
        #region private types
        private struct JsonToken
        {
            /**
			 * Open and Close set tokens.
			 * Create a JsonSet from objects in the stack starting by last OpenSet token.
			 */
            public const char OpenSet = '{';
            public const char CloseSet = '}';

            /**
			 * Key-Value separator.
			 * Specifies that during a new object creation two values should be popped and made into a KVP.
			 */
            public const char KeyValueSeparator = ':';

            /**
			 * Open and close array tokens.
			 * Create a JsonArray from objects in the stack starting by last OpenArray token.
			 */
            public const char OpenArray = '[';
            public const char CloseArray = ']';

            /**
			 * Item delimeter.
			 * Pushes current value into the stack.
			 */
            public const char ItemDelimeter = ',';

            /**
			 * Escape character.
			 * Escapes the following literal to create a specific symbol.
			 * Used only within strings.
			 */
            public const char Escape = '\\';

            /**
			 * String delimiter.
			 * Returns a string starting from next symbol and ending at first non-escaped string delimiter.
			 */
            public const char StringDelimiter = '"';
        }

        /**
		 * Same stack item type
		 */
        private enum JsonStackItemType
        {
            OpenSetMarker,
            CloseSetMarker,
            OpenArrayMarker,
            CloseArrayMarker,
            TupleMarker,
            Value
        }

        /**
		 * stack item - can be either a token (specified by type) or a value
		 */
        private struct JsonStackItem
        {
            public JsonStackItemType Type;
            public IJsonVariable Value;

            public new string ToString()
            {
                if (Type == JsonStackItemType.Value)
                    return Value.ToString();

                return Type.ToString();
            }
        }
        #endregion

        #region Parser and Pre-parser
        /**
		 * Parse the pre-parsed result into full json object.
		 */
        public static IJsonVariable Parse(string jsonString)
        {
            Queue<JsonStackItem> q = PreParse(jsonString);

            Stack<JsonStackItem> jsonStack = new Stack<JsonStackItem>();
            Stack<int> tupleDepth = new Stack<int>();
            int depth = 0;

            while (q.Count > 0)
            {
                JsonStackItem si = q.Dequeue();

                // if it's a value, push it into the jsonStack
                if (si.Type == JsonStackItemType.Value)
                    jsonStack.Push(si);

                // if it's an open array/set marker, push it into the jsonStack and increment depth counter so we won't accidentally a tuple
                if (si.Type == JsonStackItemType.OpenArrayMarker || si.Type == JsonStackItemType.OpenSetMarker)
                {
                    jsonStack.Push(si);
                    depth++;
                }

                // if it's a close array marker, pop all the items from jsonStack into a temporary stack until first open array marker, then create an array of popped values
                // (temp stack is required to maintain the original item sequencing)
                if (si.Type == JsonStackItemType.CloseArrayMarker)
                {
                    Stack<JsonStackItem> tempStack = new Stack<JsonStackItem>();

                    // pup items into new stack
                    JsonStackItem ti = jsonStack.Pop();
                    while (ti.Type == JsonStackItemType.Value)
                    {
                        tempStack.Push(ti);
                        ti = jsonStack.Pop();
                    }

                    // error check
                    if (ti.Type != JsonStackItemType.OpenArrayMarker)
                        throw new Exception(string.Format("Unexpected stack item! Expected {0} got {1}.", JsonStackItemType.OpenArrayMarker, ti.Type));

                    // create the array
                    JsonArray ja = new JsonArray();
                    while (tempStack.Count > 0)
                        ja.Add(tempStack.Pop().Value);

                    // push the value back into jsonStack
                    jsonStack.Push(new JsonStackItem { Type = JsonStackItemType.Value, Value = ja });

                    // decrement the depth counter
                    depth--;
                }

                // if it's a close set marker, pop all the items from jsonStack into a temporary stack until first open set marker, then create a set of popped value tuples
                // (temp stack is required to maintain the original item sequencing)
                if (si.Type == JsonStackItemType.CloseSetMarker)
                {
                    Stack<JsonStackItem> tempStack = new Stack<JsonStackItem>();

                    // pup items into new stack
                    JsonStackItem ti = jsonStack.Pop();
                    while (ti.Type == JsonStackItemType.Value)
                    {
                        tempStack.Push(ti);
                        ti = jsonStack.Pop();
                    }

                    // error check
                    if (ti.Type != JsonStackItemType.OpenSetMarker)
                        throw new Exception(string.Format("Unexpected stack item! Expected {0} got {1}.", JsonStackItemType.OpenSetMarker, ti.Type));

                    // create the set
                    JsonSet js = new JsonSet();
                    while (tempStack.Count > 0)
                    {
                        JsonStackItem ji = tempStack.Pop();
                        if (ji.Value.Type == JsonType.Tuple)
                        {
                            JsonTuple jt = (JsonTuple)ji.Value;

                            js.Add(jt.Key.ToString(), jt.Value);
                        }
                    }

                    // push the value back into jsonStack
                    jsonStack.Push(new JsonStackItem { Type = JsonStackItemType.Value, Value = js });

                    // decrement the depth counter
                    depth--;
                }

                // if tupleDepth top element points at current tuple level, pop two items from jsonStack and combine them into a tuple, then push the tuple back into jsonStack;
                if (tupleDepth.Count > 0 && tupleDepth.Peek() == depth)
                {
                    IJsonVariable i2 = jsonStack.Pop().Value;
                    IJsonVariable i1 = jsonStack.Pop().Value;

                    jsonStack.Push(new JsonStackItem { Type = JsonStackItemType.Value, Value = new JsonTuple(i1, i2) });

                    // remove the tuple item for current depth from tuple stack
                    tupleDepth.Pop();
                }

                // push current tuple depth into tuple stack
                if (si.Type == JsonStackItemType.TupleMarker)
                    tupleDepth.Push(depth);
            }

            if (jsonStack.Count > 0)
                return jsonStack.Pop().Value;

            return new JsonVariable();
        }

        /**
		 * Parsing tokens and analysis:
		 * { - set start
		 * } - set end
		 * : - key-value delimiter in set
		 * , - item delimiter in set/array
		 * " - string delimiter (start then end)
		 * \ - escape character for next symbol (in-string only, regular unescaping rules)
		 * [ - array start
		 * ] - array end
		 * 
		 */

        /**
		 * Walk the string and parse it into queue of stack items for further processing.
		 * Convert values into variables and put them into the queue as well.
		 */
        private static Queue<JsonStackItem> PreParse(string jsonString)
        {
            List<char> whiteSpace = new List<char> { ' ', '\n', '\r', '\t', '\0' };
            Queue<JsonStackItem> q = new Queue<JsonStackItem>();
            int i = 0;
            bool valueCollectingMode = false;
            int valueStart = 0;

            while (i < jsonString.Length)
            {
                switch (jsonString[i])
                {
                    /* array and set tokens */
                    case JsonToken.OpenSet:
                        // if we're collecting value, add it first
                        if (valueCollectingMode)
                        {
                            q.Enqueue(new JsonStackItem { Type = JsonStackItemType.Value, Value = JsonVariable.FromString(jsonString.Substring(valueStart, i - valueStart)) });
                            valueCollectingMode = false;
                        }

                        q.Enqueue(new JsonStackItem { Type = JsonStackItemType.OpenSetMarker });
                        break;

                    case JsonToken.CloseSet:
                        // if we're collecting value, add it first
                        if (valueCollectingMode)
                        {
                            q.Enqueue(new JsonStackItem { Type = JsonStackItemType.Value, Value = JsonVariable.FromString(jsonString.Substring(valueStart, i - valueStart)) });
                            valueCollectingMode = false;
                        }

                        q.Enqueue(new JsonStackItem { Type = JsonStackItemType.CloseSetMarker });
                        break;

                    case JsonToken.OpenArray:
                        // if we're collecting value, add it first
                        if (valueCollectingMode)
                        {
                            q.Enqueue(new JsonStackItem { Type = JsonStackItemType.Value, Value = JsonVariable.FromString(jsonString.Substring(valueStart, i - valueStart)) });
                            valueCollectingMode = false;
                        }

                        q.Enqueue(new JsonStackItem { Type = JsonStackItemType.OpenArrayMarker });
                        break;

                    case JsonToken.CloseArray:
                        // if we're collecting value, add it first
                        if (valueCollectingMode)
                        {
                            q.Enqueue(new JsonStackItem { Type = JsonStackItemType.Value, Value = JsonVariable.FromString(jsonString.Substring(valueStart, i - valueStart)) });
                            valueCollectingMode = false;
                        }

                        q.Enqueue(new JsonStackItem { Type = JsonStackItemType.CloseArrayMarker });
                        break;

                    case JsonToken.KeyValueSeparator:
                        // if we're collecting value, add it first
                        if (valueCollectingMode)
                        {
                            q.Enqueue(new JsonStackItem { Type = JsonStackItemType.Value, Value = JsonVariable.FromString(jsonString.Substring(valueStart, i - valueStart)) });
                            valueCollectingMode = false;
                        }

                        q.Enqueue(new JsonStackItem { Type = JsonStackItemType.TupleMarker });
                        break;

                    case JsonToken.ItemDelimeter:
                        // if we're collecting value, add it first
                        if (valueCollectingMode)
                        {
                            q.Enqueue(new JsonStackItem { Type = JsonStackItemType.Value, Value = JsonVariable.FromString(jsonString.Substring(valueStart, i - valueStart)) });
                            valueCollectingMode = false;
                        }
                        break;

                    case JsonToken.StringDelimiter:
                        // if we're collecting value, drop it
                        if (valueCollectingMode)
                            valueCollectingMode = false;

                        string value = ExtractStringValue(jsonString, i + 1);
                        q.Enqueue(new JsonStackItem { Type = JsonStackItemType.Value, Value = new JsonVariable(value.Unescape()) });
                        i += value.Length + 1;
                        break;

                    default:
                        if (whiteSpace.Contains(jsonString[i]))
                            break;

                        if (!valueCollectingMode)
                            valueStart = i;

                        valueCollectingMode = true;
                        break;
                }

                i++;
            }

            if (valueCollectingMode)
            {
                q.Enqueue(new JsonStackItem { Type = JsonStackItemType.Value, Value = new JsonVariable(jsonString.Substring(valueStart, i - valueStart)) });
            }

            return q;
        }

        /**
		 * Extract string value starting from position till first unescaped string separator literal.
		 * Also unescape any escaped characters found there.
		 */
        private static string ExtractStringValue(string jsonString, int startIndex)
        {
            bool prevEsc = false; // flag if previous character was the escape character

            for (int i = startIndex; i < jsonString.Length; i++)
            {
                if (jsonString[i] == JsonToken.StringDelimiter && !prevEsc)
                    //return System.Text.RegularExpressions.Regex.Unescape(jsonString.Substring(startIndex, i - startIndex));
                    //return Microsoft.JScript.GlobalObject.unescape(jsonString.Substring(startIndex, i - startIndex));
                    return jsonString.Substring(startIndex, i - startIndex);

                prevEsc = jsonString[i] == JsonToken.Escape;
            }

            return jsonString.Substring(startIndex);
        }
        #endregion
    }
}
