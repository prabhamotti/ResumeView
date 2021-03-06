﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketBase;
using SuperSocket.Common;

namespace SuperSocket.QuickStart.CustomProtocol
{
    class MyCommandReader : CommandReaderBase<BinaryCommandInfo>
    {
        public MyCommandReader(IAppServer appServer)
            : base(appServer)
        {

        }

        /// <summary>
        /// Finds the command.
        /// "SEND 0008 xg^89W(v"
        /// Read 10 chars as command name and command data length
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="readBuffer">The read buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="isReusableBuffer">if set to <c>true</c> [is reusable buffer].</param>
        /// <returns></returns>
        public override BinaryCommandInfo FindCommandInfo(IAppSession session, byte[] readBuffer, int offset, int length, bool isReusableBuffer, out int left)
        {
            left = 0;

            int leftLength = 10 - this.BufferSegments.Count;

            if (length < leftLength)
            {
                AddArraySegment(readBuffer, offset, length, isReusableBuffer);
                NextCommandReader = this;
                return null;
            }

            AddArraySegment(readBuffer, offset, leftLength, isReusableBuffer);
            byte[] source = BufferSegments.ToArrayData();

            string commandName = Encoding.ASCII.GetString(source, 0, 4);
            int commandDataLength = Convert.ToInt32(Encoding.ASCII.GetString(source, 5, 4).TrimStart('0'));

            if (length > leftLength)
            {
                int leftDataLength = length - leftLength;
                if (leftDataLength >= commandDataLength)
                {
                    byte[] commandData = readBuffer.CloneRange(offset + leftLength, commandDataLength);
                    BufferSegments.ClearSegements();
                    NextCommandReader = this;
                    var commandInfo = new BinaryCommandInfo(commandName, commandData);

                    //The next commandInfo is comming
                    if (leftDataLength > commandDataLength)
                        left = leftDataLength - commandDataLength;

                    return commandInfo;
                }
                else// if (leftDataLength < commandDataLength)
                {
                    //Save left data
                    AddArraySegment(readBuffer, offset + leftLength, length - leftLength, isReusableBuffer);
                    NextCommandReader = new MyCommandDataReader(commandName, commandDataLength, this);
                    return null;
                }
            }
            else
            {
                NextCommandReader = new MyCommandDataReader(commandName, commandDataLength, this);
                return null;
            }
        }
    }
}
