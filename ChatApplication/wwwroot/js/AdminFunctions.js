
import { refreshToken, handleSessionExpiry, resetVerification, fetchContacts } from '/js/FuntionsofMainChat.js';
let istoken = null;
export function loadUsersForAdminPanel(tokenKey) {
    istoken = tokenKey;
    const token = localStorage.getItem(tokenKey);
    console.log("The token inside load user for admin is ", tokenKey);
    $.ajax({
        url: "/api/v1.0/auth/getallusers",
        type: "GET",
        headers: {
            "Authorization": "Bearer " + token
        },
        success: function (response) {
            if (response.success && response.data) {
                const userTableBody = $("#userTableBody");
                userTableBody.empty();

                response.data.forEach(user => {
                    const row = `
<tr data-id="${user.id}" data-email="${user.email}">
    <td style="padding: 10px;">${user.fullName}</td>
    <td style="padding: 10px;">${user.userName || '-'}</td>
    <td style="padding: 10px;">${user.email}</td>
    <td style="padding: 10px; text-align: center;">
        <button onclick="editUser('${user.id}')" style="padding: 5px 10px; background: #3498db; color: white; border: none; border-radius: 4px;">Edit</button>
        <button onclick="deleteUserFromRow(this, '${tokenKey}')" style="padding: 5px 10px; background: #e74c3c; color: white; border: none; border-radius: 4px; margin-left: 5px;">Delete</button>
    </td>
</tr>`;
                    userTableBody.append(row);
                });
            }
        },
        error: function () {
            console.error("Failed to load users.");
        }
    });
}

export function deleteUserFromRow(button, tokenKey) {
    const row = $(button).closest("tr");
    const userId = row.data("id");
    const email = row.data("email");

    const selected = {
       
        email: email
    };

    deleteUser(selected, tokenKey); // Call deleteUser here
}
window.deleteUserFromRow = deleteUserFromRow;
export function deleteUser(selected, tokenKey) {
    const token = localStorage.getItem(tokenKey);

    $.ajax({
        url: "/api/v1.0/auth/delete-user",
        type: "POST",
        contentType: "application/json",
        headers: {
            "Authorization": `Bearer ${token}`
        },
        data: JSON.stringify({
            email: selected.email
        }),
        success: function (response) {
            if (response.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Deleted!',
                    text: 'User deleted successfully!',
                    timer: 2000,
                    showConfirmButton: false
                });

                $(`tr[data-email="${selected.email}"]`).remove();
                loadUsersForAdminPanel(tokenKey);
                fetchContacts(tokenKey);
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Failed',
                    text: 'Failed to delete user: ' + response.message
                });
            }
        },
        error: function (error) {
            console.error('Error:', error);
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Error occurred while deleting the user.'
            });
        }
    });
}
export function editUser(userId) {
    const row = document.querySelector(`tr[data-id='${userId}']`);
    const fullName = row.children[0].innerText;
    const userName = row.children[1].innerText;
    const email = row.children[2].innerText;
    
    row.innerHTML = `
        <td><input type="text" value="${fullName}" /></td>
        <td><input type="text" value="${userName === '-' ? '' : userName}" /></td>
        <td><input type="text" value="${email}" disabled /></td>
        <td style="text-align: center;">
            <button onclick="saveUser('${userId}', '${localStorage.getItem('jwtToken')}')" style="padding: 5px 10px; background: green; color: white; border: none; border-radius: 4px;">Save</button>
            <button onclick="cancelEdit('${userId}', '${fullName}', '${userName}', '${email}')" style="padding: 5px 10px; background: gray; color: white; border: none; border-radius: 4px; margin-left: 5px;">Cancel</button>
        </td>`;
}
window.editUser = editUser;

export function saveUser(userId, tokenKey) {
    const token = localStorage.getItem(istoken);
    console.log(tokenKey,  "The Token Innside save is ", token);
    const row = document.querySelector(`tr[data-id='${userId}']`);
    const fullName = row.querySelectorAll('input')[0].value;
    const userName = row.querySelectorAll('input')[1].value;
    //const admin = isAdmin;
    //console.log("The ahhhhhhhhhhhhh", admin);
    $.ajax({
        url: `/api/v1.0/auth/update-user`, // URL me userId nahi, sirf updateuser
        type: "PUT",
        contentType: "application/json",
        headers: {
            "Authorization": "Bearer " + token,
          //  "Role": admin?'Admin':'user'  // Custom header
        },
        data: JSON.stringify({ userId, fullName, userName }), // Body mein userId
        success: function (response) {
            if (response.success) {
                loadUsersForAdminPanel(tokenKey); // Success ke baad list update karein
            } else {
                alert("Failed to update user.");
            }
        },
        error: function () {
            alert("Error while updating user.");
        }
    });
}


window.saveUser = saveUser;

export function cancelEdit(userId, fullName, userName, email, tokenKey) {
    const row = document.querySelector(`tr[data-id='${userId}']`);
    row.innerHTML = `
        <td style="padding: 10px;">${fullName}</td>
        <td style="padding: 10px;">${userName || '-'}</td>
        <td style="padding: 10px;">${email}</td>
        <td style="padding: 10px; text-align: center;">
            <button onclick="editUser('${userId}')" style="padding: 5px 10px; background: #3498db; color: white; border: none; border-radius: 4px;">Edit</button>
            <button onclick="deleteUserFromRow(this, '${tokenKey}')" style="padding: 5px 10px; background: #e74c3c; color: white; border: none; border-radius: 4px; margin-left: 5px;">Delete</button>
        </td>`;
}
window.cancelEdit = cancelEdit;


