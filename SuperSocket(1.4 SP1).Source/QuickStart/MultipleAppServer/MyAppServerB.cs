﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;

namespace SuperSocket.QuickStart.MultipleAppServer
{
    public class MyAppServerB : AppServer, IDespatchServer
    {
        public void DispatchMessage(string sessionKey, string message)
        {
            var session = GetAppSessionByIndentityKey(sessionKey);
            if (session == null)
                return;

            session.SendResponse(message);
        }
    }
}
