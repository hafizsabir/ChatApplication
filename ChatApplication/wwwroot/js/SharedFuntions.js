function confirmBeforeEdit() {
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
window.confirmBeforeEdit = confirmBeforeEdit;
