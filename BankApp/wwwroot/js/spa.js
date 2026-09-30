// General site-wide JavaScript functions

// Function to show loading animation while content is being loaded
function showLoading() {
    let contentPlaceholder = document.getElementById('content-placeholder');
    contentPlaceholder.innerHTML = '<div class="loading">Loading...</div>';
}

// Function to hide the loading animation after content is loaded
function hideLoading() {
    let loadingElement = document.querySelector('.loading');
    if (loadingElement) {
        loadingElement.remove();
    }
}

// Function to display a success message
function showSuccessMessage(message) {
    const messageElement = document.createElement('div');
    messageElement.textContent = message;
    messageElement.className = 'alert alert-success mt-3';
    document.getElementById('user-dashboard').appendChild(messageElement);
    setTimeout(() => messageElement.remove(), 3000);
}

// Function to display an error message
function showErrorMessage(message) {
    const messageElement = document.createElement('div');
    messageElement.textContent = message;
    messageElement.className = 'alert alert-danger mt-3';
    document.getElementById('user-dashboard').appendChild(messageElement);
    setTimeout(() => messageElement.remove(), 3000);
}

// Utility function to handle form submission
function handleFormSubmit(event, apiUrl, formData, successCallback, errorCallback) {
    event.preventDefault(); // Prevent default form submission behavior
    showLoading();

    // Send a POST request to the specified API endpoint
    fetch(apiUrl, {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            hideLoading();
            if (data.success) {
                showSuccessMessage(data.message);
                if (successCallback) successCallback(data);
            } else {
                showErrorMessage(data.message);
                if (errorCallback) errorCallback(data);
            }
        })
        .catch(error => {
            hideLoading();
            showErrorMessage('An error occurred. Please try again.');
            console.error('Error submitting form:', error);
        });
}

// Attach event listeners when the DOM content is fully loaded
document.addEventListener('DOMContentLoaded', function () {
    // Event listener for the Register form
    let registerForm = document.getElementById('register-form');
    if (registerForm) {
        registerForm.addEventListener('submit', function (event) {
            let formData = new FormData(registerForm);
            handleFormSubmit(event, '/api/user/createuser', formData);
        });
    }

    // Event listener for the Login form
    let loginForm = document.getElementById('login-form');
    if (loginForm) {
        loginForm.addEventListener('submit', function (event) {
            let formData = new FormData(loginForm);
            handleFormSubmit(event, '/api/user/login', formData);
        });
    }
});
