using System;


namespace PlayerScripts {
    [Serializable]
    public class ChatMessage {
        public int SenderId;
        public string SenderName;
        public DateTime SendTimeUTC;
        public string Message;

        public ChatMessage() {
            SenderId = -1;
            SenderName = "Error";
            SendTimeUTC = default;
            Message = "";
        }

        public ChatMessage(int senderId, string senderName, string message) {
            SenderId = senderId;
            SenderName = senderName;
            Message = message;
            SendTimeUTC = DateTime.UtcNow;
        }
    }
}