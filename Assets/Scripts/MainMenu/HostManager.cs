using System;
using UnityEngine;

namespace Evade.MainMenu {
    public class HostManager : ClientManager {
        private TcpServerCommunicator _server;
        const string DEFAULT_SERVER_IP_ADDRESS = "0.0.0.0";
        const string LOCAL_HOST_IP_ADDRESS = "127.0.0.1";

        private void Start() {
            IPInputField.text = DEFAULT_SERVER_IP_ADDRESS;
            _nickname = "PanCHocK2";
        }

        protected override void InitializeCommunicator() {
            _clientCommunicator = new TcpClientCommunicator(LOCAL_HOST_IP_ADDRESS, int.Parse(PortInputField.text));
            _clientCommunicator.Start();
        }

        public override void OnClickConnect() {
            try {
                Debug.Log("Starting Server");
                _server = new TcpServerCommunicator(IPInputField.text, Int32.Parse(PortInputField.text));
                _server.Start();

                base.OnClickConnect();
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }

        protected override void OnDestroy() {
            Debug.Log("HostManager destroyed");
            if (_server != null && _server.IsAlive) {
                Debug.Log("Destrotying HostCommunicator Thread");
                _server.Stop();
            }
            base.OnDestroy();
        }

        public void OnClickStartGame() {
            if (AreAllClientsReady()) {
                Debug.Log("Starting Game");
            } else {
                Debug.Log("Not all clients ready");
            }
        }

        private bool AreAllClientsReady() {
            foreach (ClientDetails clientDetails in _clientCommunicator.Clients) {
                if (!clientDetails.IsReady) {
                    return false;
                }
            }
            return true;
        }
    }
}