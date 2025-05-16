export async function sendMessagetcp(currentUserId, selectedReceiverId,) {
    const text = document.getElementById("messageInput").value;

    if (!text.trim()) {
        alert("Message cannot be empty!");
        return;
    }

    if (!selectedReceiverId) {
        alert("Please select a contact to chat with.");
        return;
    }

    const response = await fetch("/api/v1.0/TcpChat/send", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            senderId: currentUserId,
            receiverId: selectedReceiverId,
            messageText: text
        })
    });

    const result = await response.json();

    if (result.success) {
        appendMessageToChat(text, true);
        document.getElementById("messageInput").value = "";
    } else {
        alert(result.message || "Failed to send message.");
    }
}

export function appendMessageToChat(message, isSender) {
    const chatContainer = document.getElementById("chatMessages");

    const msgDiv = document.createElement("div");
    msgDiv.textContent = message;
    msgDiv.style.marginBottom = "10px";
    msgDiv.style.padding = "10px";
    msgDiv.style.borderRadius = "8px";
    msgDiv.style.maxWidth = "60%";
    msgDiv.style.alignSelf = isSender ? "flex-end" : "flex-start";
    msgDiv.style.background = isSender ? "#DCF8C6" : "#E5E5EA";
    msgDiv.style.color = "#000";

    chatContainer.appendChild(msgDiv);
    chatContainer.scrollTop = chatContainer.scrollHeight;
}

export function selectReceiver(receiverId, receiverName) {
    selectedReceiverId = receiverId;
    document.getElementById("chatUserName").innerText = receiverName;
    document.getElementById("chatMessages").innerHTML = "";
}
window.sendMessagetcp = sendMessagetcp;

export async function loadMessages() {
    const response = await fetch("/api/v1.0/TcpChat/GetAllmessages");
    const data = await response.json();

    const ul = document.getElementById("messages");
    ul.innerHTML = "";
    data.data.forEach(m => {
        const li = document.createElement("li");
        li.innerText = `[${m.timestamp}] ${m.senderId} → ${m.receiverId}: ${m.messageText}`;
        ul.appendChild(li);
    });
}
