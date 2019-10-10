using System;
using System.Collections.Generic;

namespace OrletSoir.JSON
{
    /**
	 * Array type. Is an array of objects implementing the IJsonVariable interface.
	 * Converting to Set will make Keys out of indices.
	 */
    public class JsonArray : List<IJsonVariable>, IJsonVariable
    {
        #region constructors
        public JsonArray()
        {
            //
        }

        public JsonArray(IEnumerable<IJsonVariable> collection)
        {
            AddRange(collection);
        }
        #endregion

        #region implementation of IJsonVariable
        public JsonType Type
        {
            get { return JsonType.Array; }
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
            return this;
        }

        public JsonSet AsSet()
        {
            JsonSet ns = new JsonSet();

            for (int i = 0; i < Count; i++)
                ns.Add(i.ToString(), this[i]);

            return ns;
        }

        public string ToJsonString()
        {
            List<string> sl = new List<string>();

            foreach (IJsonVariable jval in this)
                sl.Add(jval.ToJsonString());

            return string.Format("[{0}]", string.Join(",", sl.ToArray()));
        }
        #endregion

        #region additional methods
        public override string ToString()
        {
            return string.Format("{0} (Count = {1})", GetType().Name, Count);
        }
        #endregion
    }
}
