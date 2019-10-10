using System;
using System.Collections.Generic;

namespace OrletSoir.JSON
{
    /**
	 * A set/dictionary/object implementation. Key can only be a string value,
	 * the Value can be any object implementing the IJsonVariable interface.
	 * Converting to Array results in losing the Key values.
	 */
    public class JsonSet : Dictionary<string, IJsonVariable>, IJsonVariable
    {
        #region constructors
        public JsonSet()
        {
            //
        }

        public JsonSet(IEnumerable<IJsonVariable> collection)
        {
            int i = 0;
            foreach (IJsonVariable item in collection)
            {
                if (item.Type == JsonType.Tuple)
                {
                    JsonTuple jt = (JsonTuple)item;

                    Add(jt.Key.ToString(), jt.Value);
                }
                else
                {
                    while (ContainsKey(i.ToString()))
                        i++;

                    Add(i.ToString(), item);
                }
            }
        }
        #endregion

        #region implementation of IJsonVariable
        public JsonType Type
        {
            get { return JsonType.Set; }
        }

        public string AsString()
        {
            throw new NotSupportedException();
        }

        public int AsInteger()
        {
            throw new NotSupportedException();
        }

        public double AsFloat()
        {
            throw new NotSupportedException();
        }

        public bool AsBoolean()
        {
            throw new NotSupportedException();
        }

        public DateTime? AsDateTime()
        {
            throw new NotSupportedException();
        }

        public JsonArray AsArray()
        {
            return new JsonArray(Values);
        }

        public JsonSet AsSet()
        {
            return this;
        }

        public string ToJsonString()
        {
            List<string> sl = new List<string>();

            foreach (var kvp in this)
                sl.Add(string.Format("\"{0}\":{1}", kvp.Key, kvp.Value.ToJsonString()));

            return string.Format("{{{0}}}", string.Join(",", sl.ToArray()));
        }
        #endregion

        #region additional methods
        public void Add(string key, int value)
        {
            this.Add(key, new JsonVariable(value));
        }

        public void Add(string key, double value)
        {
            this.Add(key, new JsonVariable(value));
        }

        public void Add(string key, decimal value)
        {
            this.Add(key, new JsonVariable(value));
        }

        public void Add(string key, bool value)
        {
            this.Add(key, new JsonVariable(value));
        }

        public void Add(string key, DateTime value)
        {
            this.Add(key, new JsonVariable(value));
        }

        public void Add(string key, string value)
        {
            this.Add(key, new JsonVariable(value));
        }

        public override string ToString()
        {
            return string.Format("{0} (Count = {1})", GetType().Name, Count);
        }
        #endregion
    }
}
