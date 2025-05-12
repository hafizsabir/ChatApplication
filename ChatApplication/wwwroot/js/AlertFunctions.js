// logout-handler.js
export function confirmLogout(tokenKey, resetVerification) {
    Swal.fire({
        title: 'Are you sure?',
        text: 'Do you really want to log out?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, log out',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            localStorage.removeItem(tokenKey);
            if (typeof resetVerification === 'function') {
                resetVerification();
            }
            window.location.href = '/ChatPages/Login';
        }
    });
}
//-------------------------------------confirm Before Edit---------------------
export function confirmBeforeEdit(tokenKey) {
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
                // Hide confirm modal
                document.getElementById("confirmModal").style.display = "none";
                localStorage.setItem("userVerified", "true");

                // Show profile modal
                const modal = document.getElementById("profileModal");
                modal.style.display = "flex";
                void modal.offsetWidth;
                modal.classList.add("show");

                // Autofill
                document.getElementById('userNameEdit').value = document.getElementById('userName').innerText;
                document.getElementById('FullNameEdit').value = document.getElementById('FullName').innerText;
                document.getElementById('userEmailedit').value = document.getElementById('userEmail').innerText;
                document.getElementById('modalProfilePicture').src = document.getElementById('profilePicture').src;

                // Reset verification inputs
                document.getElementById("verifyEmail").value = "";
                document.getElementById("verifyPassword").value = "";
            } else {
                Swal.fire("Unauthorized", response.message || "Invalid credentials", "error");
            }
        },
        error: function () {
            Swal.fire("Error", "Verification failed.", "error");
        }
    });
}
