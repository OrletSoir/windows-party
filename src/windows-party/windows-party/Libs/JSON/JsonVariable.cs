using System;
using System.Globalization;

namespace OrletSoir.JSON
{
    /**
	 * A standard variable.
	 * Can be Integer, Boolean, String, Double or DateTime.
	 * String is the default type.
	 */
    public class JsonVariable : IJsonVariable, IComparable
    {
        // type - cannot be JsonType.Set or JsonType.Array
        private readonly JsonType _type;

        // value holder fields
        private readonly string _strValue;
        private readonly int _intValue;
        private readonly double _dblValue;
        private readonly bool _boolValue;
        private DateTime? _dtValue;

        #region typed constructors and likes
        public JsonVariable(int value)
        {
            _type = JsonType.Int;
            _intValue = value;
        }

        public JsonVariable(double value)
        {
            _type = JsonType.Float;
            _dblValue = value;
        }

        public JsonVariable(decimal value)
        {
            _type = JsonType.Float;
            _dblValue = (double)value;
        }

        public JsonVariable(bool value)
        {
            _type = JsonType.Bool;
            _boolValue = value;
        }

        public JsonVariable(DateTime value)
        {
            _type = JsonType.DateTime;
            _dtValue = value;
        }

        public JsonVariable(string value)
        {
            _type = JsonType.String;
            _strValue = value;
        }

        // default constructor
        public JsonVariable()
        {
            _type = JsonType.String;
        }

        public static JsonVariable FromString(string value)
        {
            if (value.IsInt())
                return new JsonVariable(value.ToInt());

            if (value.IsNumeric())
                return new JsonVariable(value.ToDouble());

            if (value.ToLowerInvariant() == "true")
                return new JsonVariable(true);

            if (value.ToLowerInvariant() == "false")
                return new JsonVariable(false);

            return new JsonVariable(value);
        }
        #endregion

        #region implicit conversion operators
        public static implicit operator JsonVariable(int value)
        {
            return new JsonVariable(value);
        }

        public static implicit operator JsonVariable(double value)
        {
            return new JsonVariable(value);
        }

        public static implicit operator JsonVariable(bool value)
        {
            return new JsonVariable(value);
        }

        public static implicit operator JsonVariable(DateTime value)
        {
            return new JsonVariable(value);
        }

        public static implicit operator JsonVariable(string value)
        {
            return new JsonVariable(value);
        }
        #endregion

        #region comparison operators
        public static bool operator <(JsonVariable jvar1, JsonVariable jvar2)
        {
            return jvar1.CompareTo(jvar2) < 0;
        }

        public static bool operator <=(JsonVariable jvar1, JsonVariable jvar2)
        {
            return jvar1.CompareTo(jvar2) <= 0;
        }

        public static bool operator >(JsonVariable jvar1, JsonVariable jvar2)
        {
            return jvar1.CompareTo(jvar2) > 0;
        }

        public static bool operator >=(JsonVariable jvar1, JsonVariable jvar2)
        {
            return jvar1.CompareTo(jvar2) >= 0;
        }

        public static bool operator ==(JsonVariable jvar1, JsonVariable jvar2)
        {
            return jvar1.CompareTo(jvar2) == 0;
        }

        public static bool operator !=(JsonVariable jvar1, JsonVariable jvar2)
        {
            return jvar1.CompareTo(jvar2) != 0;
        }
        #endregion

        #region equality methods
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != typeof(JsonVariable))
                return false;

            return Equals((JsonVariable)obj);
        }

        public bool Equals(JsonVariable other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._type, _type)
                   && Equals(other._strValue, _strValue)
                   && other._intValue == _intValue
                   && other._dblValue.Equals(_dblValue)
                   && other._boolValue.Equals(_boolValue)
                   && other._dtValue.Equals(_dtValue);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = _type.GetHashCode();
                result = (result * 397) ^ (_strValue != null ? _strValue.GetHashCode() : 0);
                result = (result * 397) ^ _intValue;
                result = (result * 397) ^ _dblValue.GetHashCode();
                result = (result * 397) ^ _boolValue.GetHashCode();
                result = (result * 397) ^ _dtValue.GetHashCode();
                return result;
            }
        }
        #endregion

        #region implementation of IJsonVariable
        public JsonType Type
        {
            get { return _type; }
        }

        public string AsString()
        {
            switch (_type)
            {
                case JsonType.Int:
                    return _intValue.ToString(CultureInfo.InvariantCulture);

                case JsonType.Bool:
                    return _boolValue ? "true" : "false";

                case JsonType.Float:
                    return _dblValue.ToString(CultureInfo.InvariantCulture);

                case JsonType.DateTime:
                    return _dtValue != null ? _dtValue.Value.ToString("yyyy-MM-dd HH':'mm':'ss") : null;

                case JsonType.String:
                    return _strValue;
            }

            return null;
        }

        public int AsInteger()
        {
            switch (_type)
            {
                case JsonType.Int:
                    return _intValue;

                case JsonType.Bool:
                    return _boolValue ? 1 : 0;

                case JsonType.Float:
                    return (int)_dblValue;

                case JsonType.DateTime:
                    return _dtValue != null ? (int)_dtValue.Value.ToUnixTimestamp() : 0;

                case JsonType.String:
                    return _strValue.ToInt();
            }

            return 0;
        }

        public double AsFloat()
        {
            switch (_type)
            {
                case JsonType.Int:
                    return _intValue;

                case JsonType.Bool:
                    return _boolValue ? 1.0 : 0.0;

                case JsonType.Float:
                    return _dblValue;

                case JsonType.DateTime:
                    return _dtValue != null ? (int)_dtValue.Value.ToUnixTimestamp() : 0.0;

                case JsonType.String:
                    return _strValue.ToDouble();
            }

            return 0.0;
        }

        public bool AsBoolean()
        {
            switch (_type)
            {
                case JsonType.Int:
                    return _intValue > 0;

                case JsonType.Bool:
                    return _boolValue;

                case JsonType.Float:
                    return _dblValue > 0;

                case JsonType.DateTime:
                    return _dtValue != null;

                case JsonType.String:
                    return _strValue.ToInt() > 0;
            }

            return false;
        }

        public DateTime? AsDateTime()
        {
            switch (_type)
            {
                case JsonType.Int:
                    return Tools.UnixTimestampToDate((uint)_intValue);

                case JsonType.Bool:
                    return null;

                case JsonType.Float:
                    return Tools.UnixTimestampToDate((uint)_dblValue);

                case JsonType.DateTime:
                    return _dtValue;

                case JsonType.String:
                    return _strValue == null ? (DateTime?)null : _strValue.ToDateTime(DateTime.Now);
            }

            return null;
        }

        public JsonArray AsArray()
        {
            return new JsonArray { this };
        }

        public JsonSet AsSet()
        {
            return new JsonSet { { _type.ToString(), this } };
        }

        public string ToJsonString()
        {
            switch (_type)
            {
                case JsonType.Int:
                    return _intValue.ToString(CultureInfo.InvariantCulture);

                case JsonType.Bool:
                    return _boolValue ? "true" : "false";

                case JsonType.Float:
                    return _dblValue.ToString(CultureInfo.InvariantCulture);

                case JsonType.DateTime:
                    return string.Format("\"{0}\"", _dtValue != null ? _dtValue.Value.ToString("yyyy-MM-dd HH':'mm':'ss") : string.Empty);

                case JsonType.String:
                    return _strValue.ToLiteral();
            }

            return string.Format("\"{0}\"", AsString());
        }
        #endregion

        #region implementation of IComparable
        public int CompareTo(object obj)
        {
            //  null? well, in this case we're always greater =]
            if (obj == null) return 1;

            // comparing to another inst of same object? awesome!
            if (obj is JsonVariable)
            {
                JsonVariable v = (JsonVariable)obj;

                // actual comparison!
                switch (v.Type)
                {
                    case JsonType.Int:
                        return _intValue.CompareTo(v.AsInteger());

                    case JsonType.Bool:
                        return _boolValue.CompareTo(v.AsInteger());

                    case JsonType.Float:
                        return _dblValue.CompareTo(v.AsFloat());

                    case JsonType.DateTime:
                        return _dtValue != null ? _dtValue.Value.CompareTo(v.AsDateTime()) : 1;

                    case JsonType.String:
                        return _strValue.CompareTo(v.AsDateTime());

                    case JsonType.Array:
                        throw new Exception("Cannot compare to an array!");

                    case JsonType.Set:
                        throw new Exception("Cannot compare to a set!");
                }
            }

            // does the object implement IComparable? also fine
            if (obj is IComparable)
            {
                IComparable o = (IComparable)obj;

                switch (_type)
                {
                    case JsonType.Int:
                        return o.CompareTo(_intValue) * -1; // by multiplying the result by -1 we're inverting the comparison sides to restore the original order

                    case JsonType.Bool:
                        return o.CompareTo(_boolValue) * -1;

                    case JsonType.Float:
                        return o.CompareTo(_dblValue) * -1;

                    case JsonType.DateTime:
                        return o.CompareTo(_dtValue) * -1;

                    case JsonType.String:
                        return o.CompareTo(_strValue) * -1;
                }
            }

            return 0;
        }
        #endregion

        #region additional methods
        public override string ToString()
        {
            return AsString();
        }
        #endregion
    }
}
