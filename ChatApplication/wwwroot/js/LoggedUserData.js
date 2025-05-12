export function loadProfileImage(imageElementId) {
     
    const token = localStorage.getItem("jwtToken");
    if (!token) {
        console.error("Token missing");
        return;
    }

    $.ajax({
        url: "/api/v1.0/auth/GetProfilePicture",
        method: 'GET',
        headers: { 'Authorization': 'Bearer ' + token },
        success: function (data) {
            console.log("getprofileimage chala hai");
            const base64 = data?.data?.profilePicture;

            if (!base64) {
                console.warn("No image found");
                return;
            }

            let mimeType = "image/jpeg"; // default
            if (base64.startsWith("iVBORw0KGgo")) mimeType = "image/png";
            else if (base64.startsWith("/9j/")) mimeType = "image/jpeg";
            else if (base64.startsWith("R0lGOD")) mimeType = "image/gif";

            const src = `data:${mimeType};base64,${base64}`;
            $('#' + imageElementId).attr('src', src);
        },
        error: function (xhr, status, error) {
            console.error('Error fetching image:', error);
        }
    });
}
window.loadProfileImage = loadProfileImage;