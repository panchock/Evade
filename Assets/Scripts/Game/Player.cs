using Assets.Scripts.Utils;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

namespace Assets.Scripts.Game {
    public class Player : AbstractNetworkMonoBehaviour {
        private int _index = 1;
        public Rigidbody PlayerRigidbody;
        public float Speed;

        private void Awake() {
            Debug.Log($"Game framerate: {Application.targetFrameRate}");
        }

        protected override IMessage ClientUpdate() {
            var moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            if (moveDirection == Vector3.zero) {
                return null;
            }

            var input = new PlayerInputMessage {
                KeyboardInput = MessagesHelpers.CreateVector3Message(moveDirection),
                Index = _index
            };
            Debug.Log($"Send player input with index: {_index}");
            _index++;
            return input;
        }

        public override void ServerUpdate(Any message) {
            var playerInput = message.Unpack<PlayerInputMessage>().KeyboardInput;
            Debug.Log($"Got player input with index: {message.Unpack<PlayerInputMessage>().Index}");
            var moveDirection = MessagesHelpers.CreateVector3FromMessage(playerInput);
            PlayerRigidbody.velocity = moveDirection * Speed;
        }

        public override void DeserializeState(Any message) {
            var position = message.Unpack<PlayerStateMessage>().Position;
            PlayerRigidbody.position = MessagesHelpers.CreateVector3FromMessage(position);
        }

        public override IMessage SerializeState() {
            return new PlayerStateMessage {
                Position = MessagesHelpers.CreateVector3Message(PlayerRigidbody.position)
            };
        }
    }
}