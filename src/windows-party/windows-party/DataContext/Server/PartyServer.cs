using System;
using System.Collections.Generic;
using windows_party.DataContext.Factories;
using windows_party.DataContext.Parsers;
using windows_party.DataContext.Web;
using windows_party.Properties;

namespace windows_party.DataContext.Server
{
    public class PartyServer : IServer
    {
        #region private fields
        private readonly string _url;
        #endregion

        #region constructor/destructor
        public PartyServer()
        {
            _url = Resources.ServersUrl;
        }
        #endregion

        #region interface methods
        public IServerResult FetchServerData(string token)
        {
            HttpResult result = Http.Get(_url, ServersRequestFactory.MakeRequestHeaders(token));

            ServerResult serverResult = new ServerResult { Success = result.Success };

            // error?
            if (result.Success)
            {
                if (!string.IsNullOrEmpty(result.Response))
                    serverResult.Servers = PartyServersListParser.ParseListItems(result.Response);
            }
            else
            {
                if (!string.IsNullOrEmpty(result.Response))
                    serverResult.Message = ErrorMessageParser.ParseErrorMessage(result.Response);
            }

            return serverResult;
        }
        #endregion

        #region private helpers
        private ServerResult GetRandomItemsResult(int count)
        {
            Random rnd = new Random();
            List<ServerItem> items = new List<ServerItem>();

            for (int i = 0; i < count; i++)
                items.Add(new ServerItem { Name = Guid.NewGuid().ToString(), Distance = rnd.Next(0, 2500).ToString() });

            return new ServerResult { Success = true, Servers = items };
        }
        #endregion
    }
}
