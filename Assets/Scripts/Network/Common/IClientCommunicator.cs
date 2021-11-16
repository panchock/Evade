using System;
using Assets.Scripts.Utils;
using Google.Protobuf;

namespace Assets.Scripts.Network.Common {
    public interface IClientCommunicator : IDisposable {
        void Send(IMessage message);

        Message GetMessage();
    }
}