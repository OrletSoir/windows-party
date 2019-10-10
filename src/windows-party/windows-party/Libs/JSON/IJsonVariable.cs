using System;

namespace OrletSoir.JSON
{
    /**
	 * Interface for accessing the value in various forms.
	 */
    public interface IJsonVariable
    {
        JsonType Type { get; }

        string AsString();
        int AsInteger();
        double AsFloat();
        bool AsBoolean();
        DateTime? AsDateTime();
        JsonArray AsArray();
        JsonSet AsSet();

        string ToJsonString();
    }
}
