namespace Evade.MainMenu {
    public class ClientDetails {
        public string Nickname { get; set; }
        public bool IsReady { get; set; } = false;

        public ClientDetails() { }
        public ClientDetails(string nickname, bool isReady) {
            Nickname = nickname;
            IsReady = isReady;
        }
        public void ToggleReady() => IsReady = !IsReady;

        public override string ToString() {
            return $"Nickname: {Nickname} | Ready: {IsReady}";
        }
    }
}