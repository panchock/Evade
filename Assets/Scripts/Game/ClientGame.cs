using System.Net;
using Assets.Scripts.General;
using Assets.Scripts.Network.Client;
using Assets.Scripts.Utils.Network.UDP;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Game {
    public class ClientGame : MonoBehaviour {
        private Player _player;

        private void Awake() {
            _player = GameObject.FindWithTag("Player").GetComponent<Player>();
            var endpoint =
                new IPEndPoint(NetworkManager.Instance.ServerIpAddress, GameConsts.DefaultUdpServerPort);
            var udpClient = new UdpClientMessageBasedClient(new UdpClient(endpoint));
            NetworkManager.Instance.Communicators.UnreliableClientCommunicator =
                new NetworkClientCommunicator(udpClient);
        }

        protected virtual void Update() {
            // Messages from server
            var messages = NetworkManager.Instance.Communicators.UnreliableClientCommunicator.ReceiveAll();
            if (messages == null) {
                return;
            }

            foreach (var message in messages) {
                if (message.AnyMessage.Is(ServerDisconnectMessage.Descriptor)) {
                    SceneManager.LoadScene("MainMenu");
                } else {
                    HandleGlobalState(message.AnyMessage.Unpack<GlobalStateMessage>());
                }
            }
        }

        private void HandleGlobalState(GlobalStateMessage globalStateMessage) {
            foreach (var state in globalStateMessage.ObjectsState) {
                HandleState(state);
            }
        }

        private void HandleState(Any state) {
            _player.DeserializeState(state);
        }

        private void OnDestroy() {
            NetworkManager.Instance.Reset();
        }
    }
}