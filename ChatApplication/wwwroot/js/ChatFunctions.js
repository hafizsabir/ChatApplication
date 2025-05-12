// chat-functions.js

export function sendMessage(tokenKey, senderId, selectedReceiverId) {
    const messageText = document.getElementById("messageInput").value.trim();
    console.log("the data inside mesg funtion is ", tokenKey, senderId, selectedReceiverId);
    if (!messageText) {
        alert("Please type a message");
        return;
    }

    const token = localStorage.getItem(tokenKey);
    const messageData = {
        ReceiverId: selectedReceiverId,
        MessageText: messageText,
        SenderId: senderId
    };

    console.log("The message data is:", messageData);
    console.log("Token being sent:", token);

    $.ajax({
        url: '/api/v1.0/chat/sendchat',
        method: 'POST',
        headers: {
            "Authorization": "Bearer " + token
        },
        contentType: "application/json",
        data: JSON.stringify(messageData),
        success: function (response) {
            if (response.success) {
                document.getElementById("messageInput").value = "";
                console.log("The response of msg is", response);
                appendMessageToChatBox(response.data, senderId);
            } else {
                alert("Failed to send message");
            }
        },
        error: function (xhr, status, error) {
            console.error("Error sending message", error);
        }
    });
}
function appendMessageToChatBox(message, currentUserId) {
    const chatBox = document.getElementById("chatMessages");
    const msgElement = document.createElement("div");

    const isSender = message.senderId === currentUserId;

    msgElement.style.alignSelf = isSender ? "flex-end" : "flex-start";
    msgElement.style.background = isSender ? "#dcf8c6" : "#fff";
    msgElement.style.padding = "10px";
    msgElement.style.margin = "5px";
    msgElement.style.borderRadius = "10px";
    msgElement.style.maxWidth = "60%";
    msgElement.style.boxShadow = "0 1px 3px rgba(0,0,0,0.1)";

    msgElement.innerHTML = `
                    <div style="font-size: 16px;">${message.messageText}</div>
                    <div style="font-size: 12px; text-align: right; color: gray;">
                        ${new Date(message.timestamp).toLocaleTimeString()}
                    </div>
                `;

    chatBox.appendChild(msgElement);
    chatBox.scrollTop = chatBox.scrollHeight; // scroll to bottom
}
//-------------------------load user messages-----------------------
export function loadMessagesWith(userId, senderId, tokenKey) {
    const token = localStorage.getItem(tokenKey);
    const selectedReceiverId = userId;

    const payload = {
        SenderId: senderId,
        ReceiverId: userId
    };
    console.log("the payload in chat funtions is ", payload);

    $.ajax({
        url: `/api/v1.0/chat/GetAllMessages`,
        type: "POST",
        contentType: "application/json",
        headers: {
            "Authorization": "Bearer " + token
        },
        data: JSON.stringify(payload),
        success: function (response) {
            if (response) {
                console.log("User Messages response is", response);
                const chatMessages = $("#chatMessages");
                chatMessages.empty(); // Clear previous messages

                if (response.success && Array.isArray(response.data)) {
                    response.data.forEach(msg => {
                        const isSender = msg.senderId === senderId;
                        const messageHtml = `
                            <div style="align-self: ${isSender ? 'flex-end' : 'flex-start'}; background: ${isSender ? '#dcf8c6' : '#fff'}; padding: 10px; border-radius: 8px; margin: 5px 0; max-width: 70%;">
                                ${msg.messageText}
                            </div>
                        `;
                        chatMessages.append(messageHtml);
                    });
                } else {
                    chatMessages.html(`<p class="text-muted">No messages found.</p>`);
                }

                chatMessages.scrollTop(chatMessages[0].scrollHeight);
            }
        },
        error: function () {
            console.error("Failed to load chat messages.");
        }
    });
}
