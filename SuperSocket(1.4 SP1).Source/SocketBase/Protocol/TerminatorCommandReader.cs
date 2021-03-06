﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using SuperSocket.Common;
using SuperSocket.SocketBase.Command;

namespace SuperSocket.SocketBase.Protocol
{
    public enum SearhTokenStatus
    {
        None,
        Found,
        FoundStart
    }

    public class SearhTokenResult
    {
        public SearhTokenStatus Status { get; set; }
        public int Value { get; set; }
    }

    public class TerminatorCommandReader : CommandReaderBase<StringCommandInfo>
    {
        public byte[] Terminator { get; private set; }
        protected Encoding Encoding { get; private set; }
        private ICommandParser m_CommandParser;

        public TerminatorCommandReader(IAppServer appServer)
            : base(appServer)
        {

        }

        public TerminatorCommandReader(CommandReaderBase<StringCommandInfo> previousCommandReader)
            : base(previousCommandReader)
        {

        }

        public TerminatorCommandReader(IAppServer appServer, Encoding encoding, string terminator, ICommandParser commandParser)
            : this(appServer)
        {
            Encoding = encoding;
            Terminator = encoding.GetBytes(terminator);
            m_CommandParser = commandParser;
        }

        public override StringCommandInfo FindCommandInfo(IAppSession session, byte[] readBuffer, int offset, int length, bool isReusableBuffer, out int left)
        {
            NextCommandReader = this;

            string command;

            if (!FindCommandInfoDirectly(readBuffer, offset, length, isReusableBuffer, out command, out left))
                return null;

            return m_CommandParser.ParseCommand(command);
        }        

        protected bool FindCommandInfoDirectly(byte[] readBuffer, int offset, int length, bool isReusableBuffer, out string command, out int left)
        {
            left = 0;
            ArraySegment<byte> currentSegment;

            if (isReusableBuffer)
            {
                //Next received data also will be saved in this buffer, so we should create a new byte[] to persistent current received data
                currentSegment = new ArraySegment<byte>(readBuffer.CloneRange(offset, length));
            }
            else
            {
                currentSegment = new ArraySegment<byte>(readBuffer, offset, length);
            }

            BufferSegments.AddSegment(currentSegment);

            int? result = BufferSegments.SearchMark(Terminator);

            if (!result.HasValue)
            {
                command = string.Empty;
                return false;
            }

            if (result.Value < 0)
            {
                command = string.Empty;
                return false;
            }

            int findLen = result.Value + Terminator.Length;
            int total = BufferSegments.Count;
            command = Encoding.GetString(BufferSegments.ToArrayData(0, result.Value));

            ClearBufferSegments();

            if (findLen < total)
            {
                left = total - findLen;
            }

            return true;
        }
    }
}
