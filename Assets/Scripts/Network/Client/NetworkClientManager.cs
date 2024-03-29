using System.Collections.Generic;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Utils.Messages;
using Google.Protobuf;

namespace Assets.Scripts.Network.Client {
    public class NetworkClientManager : IClientManager {
        private readonly NetworkClientCommunicatorReceiverThread _networkClientCommunicatorReceiverThread;
        private readonly NetworkClientCommunicatorSenderThread _networkClientCommunicatorSenderThread;

        public NetworkClientManager(IMessageBasedClient messageBasedNetworkClient) {
            Communicator = new QueueCommunicator();
            NetworkClient = messageBasedNetworkClient;
            _networkClientCommunicatorSenderThread = new NetworkClientCommunicatorSenderThread(this);
            _networkClientCommunicatorSenderThread.Start();
            _networkClientCommunicatorReceiverThread = new NetworkClientCommunicatorReceiverThread(this);
            _networkClientCommunicatorReceiverThread.Start();
        }

        public QueueCommunicator Communicator { get; }

        public IMessageBasedClient NetworkClient { get; }

        public void Dispose() {
            _networkClientCommunicatorReceiverThread?.Stop();
            _networkClientCommunicatorSenderThread?.Stop();
            NetworkClient?.Dispose();
        }

        public MessageToReceive Receive() {
            return Communicator.Receive();
        }

        public List<MessageToReceive> ReceiveAll() {
            return Communicator.ReceiveAll();
        }

        public void Send(IMessage message) {
            Communicator.Send(message);
        }
    }
}