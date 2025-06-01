setTimeout(() => {
    const successAlert = document.getElementById("success-alert");
    const errorAlert = document.getElementById("error-alert");

    if (successAlert) {
        successAlert.classList.remove("show");
        successAlert.classList.add("fade");
        setTimeout(() => successAlert.remove(), 500);
    }

    if (errorAlert) {
        errorAlert.classList.remove("show");
        errorAlert.classList.add("fade");
        setTimeout(() => errorAlert.remove(), 500);
    }
}, 3000);
