





//----------------------------------------------
//----------------------------- fetch all contacts ---------------------------------
export function fetchContacts(tokenKey) {
    const token = localStorage.getItem(tokenKey);

    $.ajax({
        url: "/api/v1.0/auth/getallusers",
        type: "GET",
        headers: {
            "Authorization": "Bearer " + token
        },
        success: function (response) {
            if (response.success && response.data) {
                const contactList = $("#contactList");
                contactList.empty();
                console.log("all user data ", response.data);

                response.data.forEach(user => {
                    const isOnline = user.isOnline;

                    // Render the contact with proper attributes
                    const contact = `
<div class="contact"
    data-user-id="${user.id}" 
    data-full-name="${user.fullName}" 
    data-is-online="${isOnline}" 
    onclick="startChat(this)"
    style="display: flex; align-items: center; padding: 10px; margin-bottom: 10px; border-radius: 4px; cursor: pointer; background: ${isOnline ? '#4CAF50' : '#f1f1f1'}; position: relative; border: 2px solid transparent;">
    <img src="${user.profilePicture ? `data:image/png;base64,${user.profilePicture}` : '/Images/default.png'}"
        alt="${user.profilePicture}"
        style="width: 45px; height: 45px; border-radius: 50%; margin-right: 10px; object-fit: cover;">
    <div style="flex: 1;">
        <div style="font-weight: bold; color: black;">${user.fullName}</div>
        <div style="color: black; font-size: 12px; opacity: 0.8;">${user.email || 'Hey there!'}</div>
    </div>
    <span style="position: absolute; right: 10px; top: 50%; transform: translateY(-50%); width: 10px; height: 10px; border-radius: 50%; background: ${isOnline ? '#4CAF50' : '#ccc'};"></span>
</div>`;
                    contactList.append(contact);
                });
            }
        },
        error: function () {
            console.error("Failed to load users.");
        }
    });
}

//------------------------------------------------------------------------------------------------------------
// rset user for profile edit 
export const resetVerification = () => {
    localStorage.removeItem("userVerified");
};
// refreshToken.js
export async function refreshToken(tokenKey, sessionState) {
    const oldToken = localStorage.getItem(tokenKey);
    if (!oldToken) return;

    sessionState.isRefreshing = true;

    try {
        const response = await fetch('/api/v1.0/auth/refresh-token', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${oldToken}`
            },
            body: JSON.stringify({ token: oldToken })
        });

        const data = await response.json();
        if (response.ok && data.success) {
            localStorage.setItem(tokenKey, data.data);
            console.log("🔁 Token refreshed", data.data);
        } else {
            console.warn("❌ Token refresh failed:", data.message);
        }
    } catch (error) {
        console.error("⚠️ Error while refreshing token:", error);
    } finally {
        sessionState.isRefreshing = false;
    }
}

///------------------------Handle session expiry-------------------------
export function handleSessionExpiry(tokenKey, resetVerification) {
    document.getElementById("countdownTimer").textContent = "Session expired";

    Swal.fire({
        title: 'Session Expired',
        text: 'Your session has expired. Please log in again.',
        icon: 'warning',
        confirmButtonText: 'Login Again'
    }).then(() => {
        localStorage.removeItem(tokenKey);
        resetVerification();
        window.location.href = '/ChatPages/Login';
    });
}

