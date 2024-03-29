using System;
using System.Collections.Generic;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Network.Server;
using Assets.Scripts.Utils.Messages;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace Assets.Scripts.Network.Host {
    public class HostClientManager : IClientManager {
        private readonly IServerCommunicatorForHost _serverCommunicator;

        public HostClientManager(IServerCommunicatorForHost serverCommunicator, string nickname) {
            _serverCommunicator = serverCommunicator;
            _serverCommunicator.HostConnect(nickname);
        }

        public List<MessageToReceive> ReceiveAll() {
            throw new NotImplementedException();
        }

        public void Send(IMessage message) {
            var messageToInsert = new MessageToReceive(HostClient.GetHostClientEndpoint(), Any.Pack(message));
            _serverCommunicator.AddMessageToReceive(messageToInsert);
        }

        public MessageToReceive Receive() {
            throw new NotImplementedException();
        }

        public void Dispose() { }
    }
}