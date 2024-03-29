using System;
using Assets.Scripts.General;
using Assets.Scripts.Network.Server;
using Assets.Scripts.Utils.Messages;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Game {
    public class ServerGame : MonoBehaviour {
        private void Awake() {
            GameManager.Instance.NetworkManagers.UdpServerManager =
                new UdpServerManager(GameConsts.DefaultUdpServerPort);
        }

        private void FixedUpdate() {
            // All the game logic is here
            CheckGameEnd();

            var messages = GameManager.Instance.NetworkManagers.UdpServerManager.Communicator.ReceiveAll();
            if (messages.Count > 0) {
                foreach (var message in messages) {
                    HandleMessage(message);
                }
            }

            SendGlobalStateToAllClients();
        }

        private void CheckGameEnd() {
            if (GameManager.Instance.IsGameEnded) {
                var gameEndedMessage = new GameEndedMessage {
                    WinnerNickname = GameManager.Instance.WinnerNickname
                };
                GameManager.Instance.NetworkManagers.TcpServerManager.Communicator.Send(gameEndedMessage);
                SceneManager.LoadScene("GameEnd");
                Destroy(this);
            }
        }

        private void SendGlobalStateToAllClients() {
            var globalState = GetGlobalState();
            GameManager.Instance.NetworkManagers.UdpServerManager.Communicator.Send(globalState);
        }

        private IMessage GetGlobalState() {
            var globalStateMessage = new GlobalStateMessage();
            foreach (var serverGameObject in GameManager.Instance.ServerGameObjects) {
                var objectId = serverGameObject.Key;
                var realGameObject = serverGameObject.Value.Item1;
                var ownerClient = serverGameObject.Value.Item2;

                var messageToPack = realGameObject.GetComponentInChildren<NetworkBehaviour>().SerializeState();
                var stateMessage = new ObjectStateMessage {
                    ObjectId = objectId,
                    OwnerNickname = ownerClient.Details.Nickname,
                    State = Any.Pack(messageToPack)
                };
                globalStateMessage.ObjectsState.Add(stateMessage);
            }

            return globalStateMessage;
        }

        private void HandleMessage(MessageToReceive messageToReceive) {
            var anyMessage = messageToReceive.AnyMessage;
            // Clients send in the beginning ClientReadyMessage in order to subscribe to UdpServer. Just ignore this.
            if (anyMessage.Is(ClientReadyMessage.Descriptor)) {
                return;
            }

            if (!anyMessage.Is(ObjectInputMessage.Descriptor)) {
                throw new Exception("Got unsupported message from client");
            }

            var objectInputMessage = anyMessage.Unpack<ObjectInputMessage>();

            if (!GameManager.Instance.ServerGameObjects.ContainsKey(objectInputMessage.ObjectId)) {
                return;
            }
            
            var gameObjectIdentifier = GameManager.Instance.ServerGameObjects[objectInputMessage.ObjectId];
            if (gameObjectIdentifier.Item2.Id != objectInputMessage.ClientId) {
                throw new Exception(
                    $"Client: {objectInputMessage.ClientId} tries to change object which is not owned be him");
            }

            gameObjectIdentifier.Item1.GetComponentInChildren<NetworkBehaviour>()
                .ServerUpdateFromClient(objectInputMessage.Input);
        }
    }
}