using Assets.Scripts.General;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

namespace Assets.Scripts.Game {
    public abstract class ClientNetworkBehaviour : MonoBehaviour, ISerializableNetworkObject {
        public bool IsLocal { get; set; }

        public abstract void DeserializeState(Any message);
        public abstract IMessage SerializeState();

        /// <summary>
        ///     FixedUpdate used in order to sync the input speed from clients.
        ///     If one client will run on higher fps we don't want it to send more input to the server.
        /// </summary>
        protected virtual void FixedUpdate() {
            if (IsLocal) {
                var updateMessage = ClientUpdate();
                if (updateMessage != null) {
                    SendUpdate(updateMessage);
                }
            }
        }

        /// <summary>
        ///     Create input message and send it to server
        /// </summary>
        /// <param name="message">The inner message to send to server</param>
        private void SendUpdate(IMessage message) {
            var inputMessage = new ObjectInputMessage {
                ObjectId = name,
                ClientId = GameManager.Instance.ClientId,
                Input = Any.Pack(message)
            };
            GameManager.Instance.NetworkManagers.UnreliableClientManager.Send(inputMessage);
        }

        /// <summary>
        ///     The FixedUpdate logic on the server
        /// </summary>
        /// <param name="message">The input message from client</param>
        public abstract void ServerUpdate(Any message);

        /// <summary>
        ///     The FixedUpdate logic on the client
        /// </summary>
        /// <returns>Input message to send to server</returns>
        protected abstract IMessage ClientUpdate();
    }
}