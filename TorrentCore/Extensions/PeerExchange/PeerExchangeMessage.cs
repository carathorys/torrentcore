﻿// This file is part of TorrentCore.
//     https://torrentcore.org
// Copyright (c) 2017 Samuel Fisher.
// 
// TorrentCore is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation, version 3.
// 
// TorrentCore is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with TorrentCore.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using BencodeNET.Objects;
using BencodeNET.Parsing;
using TorrentCore.Extensions.ExtensionProtocol;
using TorrentCore.Serialization;
using TorrentCore.Transport;

namespace TorrentCore.Extensions.PeerExchange
{
    class PeerExchangeMessage : IExtensionProtocolMessage
    {
        public const string MessageType = "ut_pex";

        string IExtensionProtocolMessage.MessageType => MessageType;

        public IList<IPEndPoint> Added { get; set; }

        public byte[] Serialize()
        {
            throw new NotImplementedException();
        }

        public void Deserialize(byte[] data)
        {
            var dictParser = new BDictionaryParser(new BencodeParser());
            var dict = dictParser.Parse(data);

            if (dict.TryGetValue("added", out IBObject added))
                Added = ParseEndPoints((BString)added).ToList();
        }

        private IEnumerable<IPEndPoint> ParseEndPoints(BString input)
        {
            using (var ms = new MemoryStream(input.Value.ToArray()))
            {
                var reader = new BigEndianBinaryReader(ms);
                while (ms.Position < ms.Length)
                {
                    yield return reader.ReadIpV4EndPoint();
                }
            }
        }
    }
}
