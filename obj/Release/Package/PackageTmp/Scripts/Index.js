$(document).ready(function () {
    $("#CommentsEnabled").toggleSwitch().change(function () {
        alert("Changed!!");
    });
});

function reload() {
    window.location.reload();
}