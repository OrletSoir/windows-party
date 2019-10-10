using System;

namespace OrletSoir.JSON
{
    /**
	 * A tuple of two objects implementing the IJsonVariable interface. One acting as a Key, other as Value.
	 * Returns Value by default.
	 */
    public class JsonTuple : IJsonVariable
    {
        public IJsonVariable Key { get; private set; }
        public IJsonVariable Value { get; private set; }

        #region constructors
        public JsonTuple(IJsonVariable key, IJsonVariable value)
        {
            Key = key;
            Value = value;
        }
        #endregion

        #region implementation of IJsonVariable
        public JsonType Type
        {
            get { return JsonType.Tuple; }
        }

        public string AsString()
        {
            return Value.AsString();
        }

        public int AsInteger()
        {
            return Value.AsInteger();
        }

        public double AsFloat()
        {
            return Value.AsFloat();
        }

        public bool AsBoolean()
        {
            return Value.AsBoolean();
        }

        public DateTime? AsDateTime()
        {
            return Value.AsDateTime();
        }

        public JsonArray AsArray()
        {
            return Value.AsArray();
        }

        public JsonSet AsSet()
        {
            return Value.AsSet();
        }

        public string ToJsonString()
        {
            return string.Format("{0}:{1}", Key.ToJsonString(), Value.ToJsonString());
        }
        #endregion

        #region additional methods
        public override string ToString()
        {
            return string.Format("{0} ({1}, {2})", GetType().Name, Key, Value);
        }
        #endregion
    }
}
