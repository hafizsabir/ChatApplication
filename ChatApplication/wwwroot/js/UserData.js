//-----------------------------User related Data Who is logged in-----------------------------
export function fetchUserInfo(tokenKey, handleSessionExpiry, resetVerification) {
    const token = localStorage.getItem(tokenKey);

    if (!token) {
        console.error("Token not found in localStorage.");
        handleSessionExpiry(tokenKey, resetVerification);
        return;
    }

    $.ajax({
        url: "/api/v1.0/auth/getuserinfo",
        type: "GET",
        headers: {
            "Authorization": "Bearer " + token
        },
        dataType: "json",
        success: function (response) {
            console.log("User Info:", response);

            if (response && response.data) {
                const username = response.data.username || "N/A";
                const fullname = response.data.fullname || "N/A";
                const email = response.data.email || "N/A";

                console.log("✅ fetchUserInfo ran");

                $("#userName").text(username);
                $("#userEmail").text(email);
                $("#FullName").text(fullname);

                // You can optionally use remainingTimeInSeconds or expirationTime here if needed
            } else {
                console.error("No user data found in the response.");
                handleSessionExpiry(tokenKey, resetVerification);
            }
        },
        error: function (xhr, status, error) {
            console.error("❌ Error fetching user info:", error);
            if (xhr.status === 401) {
                handleSessionExpiry(tokenKey, resetVerification);
            }
        }
    });
}

//------------------------------------------------------------------------
export function confirmBeforeEdit() {
    const token = localStorage.getItem(tokenKey);
    const email = document.getElementById("verifyEmail").value;
    const password = document.getElementById("verifyPassword").value;

    if (!token || !email || !password) {
        Swal.fire("Error", "Email and password are required.", "error");
        return;
    }

    $.ajax({
        url: "/api/v1.0/auth/VerifyForEditProfile",
        method: "POST",
        headers: {
            "Authorization": "Bearer " + token
        },
        contentType: "application/json",
        data: JSON.stringify({ email, password }),
        success: function (response) {
            if (response.success) {
                document.getElementById("confirmModal").style.display = "none";
                localStorage.setItem("userVerified", "true");

                const modal = document.getElementById("profileModal");
                modal.style.display = "flex";
                void modal.offsetWidth;
                modal.classList.add("show");

                const username = document.getElementById('userName').innerText;
                const fullname = document.getElementById('FullName').innerText;
                const useremail = document.getElementById('userEmail').innerText;
                const profilePicture = document.getElementById('profilePicture').src;

                document.getElementById('userNameEdit').value = username;
                document.getElementById('FullNameEdit').value = fullname;
                document.getElementById('userEmailedit').value = useremail;
                document.getElementById('modalProfilePicture').src = profilePicture;

                document.getElementById("verifyEmail").value = "";
                document.getElementById("verifyPassword").value = "";
            } else {
                Swal.fire("Unauthorized", response.message || "Invalid credentials", "error");
            }
        },
        error: function (xhr) {
            Swal.fire("Error", "Verification failed.", "error");
        }
    });
}



