﻿using OrletSoir.JSON;
using System.Collections.Generic;
using System.Linq;
using windows_party.DataContext.Server;

namespace windows_party.DataContext.Parsers
{
    public static class PartyServersListParser
    {
        #region Logger
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        #endregion

        #region private fields
        public const string _nameField = @"name";
        public const string _distanceField = @"distance";
        public const string _km = @" km";
        #endregion

        #region public methods
        public static List<ServerItem> ParseListItems(string result)
        {
            // parse json
            IJsonVariable jsonResult = Json.Parse(result);

            // is it a set?
            if (jsonResult.Type != JsonType.Array)
            {
                Logger.Info("Invalid JSON query format: data is not a JSON array");

                return null;
            }

            // convert
            JsonArray jsonArray = jsonResult.AsArray();

            // check every item and parse where appropriate
            List<ServerItem> servers = new List<ServerItem>();
            foreach (IJsonVariable item in jsonArray)
            {
                if (item.Type != JsonType.Set)
                    continue;

                ServerItem serverItem = ParseServerItem(item.AsSet());

                // verify both fields are set
                if (string.IsNullOrEmpty(serverItem.Name) || string.IsNullOrEmpty(serverItem.Distance))
                    continue;

                servers.Add(serverItem);
            }

            Logger.Debug("JSON query parsed successfully: {number} items returned", servers.Count);

            // return the parsed items
            return servers;
        }
        #endregion

        #region private helpers
        private static ServerItem ParseServerItem(JsonSet item)
        {
            ServerItem serverItem = new ServerItem();

            // check for [name] field and return its value
            if (item.Keys.Contains(_nameField) && item[_nameField] != null && item[_nameField].Type == JsonType.String)
                serverItem.Name = item[_nameField].AsString();
            else
                Logger.Info("Invalid JSON query format: field {field} was not found in the element or is invalid", _nameField);

            // check for [distance] field and return its value (accepted types are int, float, and string)
            if (item.Keys.Contains(_distanceField) && item[_distanceField] != null
                && (item[_distanceField].Type == JsonType.Int || item[_distanceField].Type == JsonType.Float || item[_distanceField].Type == JsonType.String))
                serverItem.Distance = item[_distanceField].AsString() + _km;
            else
                Logger.Info("Invalid JSON query format: field {field} was not found in the element or is invalid", _distanceField);

            return serverItem;
        }
        #endregion
    }
}
