import { jwtDecrypt } from 'https://cdn.jsdelivr.net/npm/jose@latest/dist/browser/index.min.js';
import Swal from 'sweetalert2'; // no need to write full URL

import { sendMessage } from '/js/ChatFunctions.js';
import { sessionState } from '/js/Sessionvariables.js';
import { loadProfileImage } from '/js/LoggedUserData.js';
import { loadUsersForAdminPanel, editUser, cancelEdit, saveUser } from '/js/AdminFunctions.js';
import { confirmLogout, confirmBeforeEdit } from '/js/AlertFunctions.js';
import { refreshToken, handleSessionExpiry, resetVerification, fetchContacts } from '/js/FuntionsofMainChat.js';
export async function showUserData(tokenKey, secretKey) {
    const token = localStorage.getItem(tokenKey);
    if (!token) return;

    try {
        const { payload } = await jwtDecrypt(token, secretKey);

        document.getElementById("userNameEdit").value = payload.username || "";
        document.getElementById("FullNameEdit").value = payload.fullname || "";
        document.getElementById("userEmailedit").value = payload.email || "";

        // Optionally use backend image or fallback to current one
        const profilePicture = document.getElementById('profilePicture').src;
        document.getElementById('modalProfilePicture').src = profilePicture;

        document.getElementById("profileModal").style.display = "flex";
    } catch (error) {
        console.error("Failed to populate modal:", error);
    }
}
// profileManager.js

export async function saveProfileChanges({
    tokenKey,
    jwtDecrypt,
    AppConfig,
    startSessionCycle,
    resetVerification, // if needed elsewhere
    Swal
}) {
    try {
        const token = localStorage.getItem(tokenKey);
        if (!token) {
            Swal.fire({
                icon: 'error',
                title: 'Unauthorized',
                text: 'Please log in again.'
            });
            return;
        }

        const { payload } = await jwtDecrypt(token, AppConfig.secretKey);

        const expiryTime = new Date(payload.exp * 1000);
        const currentTime = new Date();
        const remainingMilliseconds = expiryTime - currentTime;
        const remainingSeconds = Math.floor(remainingMilliseconds / 1000);
        const remainingMinutes = Math.floor(remainingMilliseconds / 60000);

        console.log("Remaining time in seconds:", remainingSeconds);
        console.log("the time remaining is", remainingSeconds);
        console.log("Remaining time: " + remainingMinutes + " minutes and " + remainingSeconds + " seconds.");

        const username = document.getElementById('userNameEdit').value;
        const email = document.getElementById('userEmailedit').value;
        const fullname = document.getElementById('FullNameEdit').value;
        const file = document.getElementById('newProfilePictureInput').files[0];

        const formData = new FormData();
        if (file) formData.append("profilePicture", file);
        formData.append("email", email);
        formData.append("FullName", fullname);
        formData.append("UserName", username);
        formData.append("TimeRemaining", remainingSeconds);

        $.ajax({
            url: "/api/v1.0/auth/UpdateUserInfo",
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + token
            },
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.success) {
                    const newToken = response.data.regeneratedToken;
                    localStorage.setItem(tokenKey, newToken);
                    console.log("The new generated token is:", newToken);

                    document.getElementById("FullName").innerText = fullname;
                    document.getElementById("userName").innerText = username;
                    startSessionCycle();

                    if (file) {
                        const reader = new FileReader();
                        reader.onload = function (e) {
                            document.getElementById("profilePicture").src = e.target.result;
                        };
                        reader.readAsDataURL(file);
                    }

                    Swal.fire({
                        icon: 'success',
                        title: 'Profile Updated',
                        showConfirmButton: false,
                        timer: 2000
                    });

                    document.getElementById("profileModal").style.display = "none";
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: response.message || 'Profile update failed.'
                    });
                }
            },
            error: function (xhr, status, error) {
                Swal.fire({
                    icon: 'error',
                    title: 'Request Failed',
                    text: 'Something went wrong while updating profile.'
                });
                console.error("Upload failed:", error);
            }
        });

    } catch (error) {
        console.error("Decryption or update error:", error);
        Swal.fire({
            icon: 'error',
            title: 'Something went wrong',
            text: 'Failed to update profile. Please try again later.'
        });
    }
}

