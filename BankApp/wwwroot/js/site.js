document.addEventListener('DOMContentLoaded', function () {
    // Load Register form dynamically
    document.getElementById('load-register').addEventListener('click', function () {
        fetch('/api/home/loadregisterform')
            .then(response => response.text())
            .then(html => {
                document.getElementById('content-placeholder').innerHTML = html;
                attachRegisterFormHandler(); // Attach handler after loading the form
            })
            .catch(error => console.error('Error loading the register form:', error));
    });

    // Load Login form dynamically
    document.getElementById('load-login').addEventListener('click', function () {
        fetch('/api/home/loadloginform')
            .then(response => response.text())
            .then(html => {
                document.getElementById('content-placeholder').innerHTML = html;
                attachLoginFormHandler();
            })
            .catch(error => console.error('Error loading the login form:', error));
    });

    attachLoginFormHandler();
    document.getElementById('load-user-dashboard').addEventListener('click', loadUserDashboard);




    function handleSuccessfulLogin(userData) {
        // Clear any existing user data
        sessionStorage.clear();

        // Set new user data
        sessionStorage.setItem('user_id', userData.user_id);
        sessionStorage.setItem('username', userData.username);

        console.log('Logged in user:', userData.username, 'with ID:', userData.user_id);

        // Update UI for logged-in state
        const initialButtons = document.getElementById('initial-buttons');
        if (initialButtons) {
            initialButtons.style.display = 'none';
        }

        loadUserDashboard();
    }
    // Function to handle register form submission
    function attachRegisterFormHandler() {
        document.getElementById('register-form').addEventListener('submit', function (event) {
            event.preventDefault();
            let userData = {
                username: document.getElementById('username').value,
                password: document.getElementById('password').value,
                email: document.getElementById('email').value,
                address: document.getElementById('address').value,
                phone: document.getElementById('phone').value
            };

            // Updated URL with correct API call to LocalDBWebAPI
            fetch('http://localhost:5082/api/user/createuser', { // Replace with your actual API URL if different
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(userData)
            })
                .then(response => {
                    if (response.ok) {
                        alert('Account created successfully!');
                        document.getElementById('content-placeholder').innerHTML = '';  // Clear form after registration
                    } else {
                        response.json().then(data => alert(`Error creating account: ${data.message}`));
                    }
                })
                .catch(error => console.error('Error creating user:', error));
        });
    }

    // Function to handle login form submission
    function attachLoginFormHandler() {
        console.log('Attaching login form handler');
        const loginForm = document.getElementById('login-form');
        if (loginForm) {
            loginForm.addEventListener('submit', function (event) {
                event.preventDefault();
                console.log('Login form submitted');

                const usernameInput = document.getElementById('username');
                const passwordInput = document.getElementById('password');

                if (!usernameInput || !passwordInput) {
                    console.error('Username or password input not found');
                    showErrorMessage('Login form is incomplete. Please try again.');
                    return;
                }

                let loginData = {
                    username: usernameInput.value,
                    password: passwordInput.value
                };

                console.log('Attempting login for user:', loginData.username);

                fetch(`http://localhost:5082/api/user/isvalidauth/${loginData.username}/${loginData.password}`)
                    .then(response => response.json())
                    .then(isValid => {
                        if (isValid) {
                            console.log('Login successful');
                            return fetch(`http://localhost:5082/api/user/getuser/${loginData.username}`);
                        } else {
                            throw new Error('Invalid credentials.');
                        }
                    })
                    .then(response => response.json())
                    .then(userData => {
                        console.log('User data retrieved:', userData);
                        handleSuccessfulLogin(userData);
                    })
                    .catch(error => {
                        console.error('Error during login:', error);
                        showErrorMessage(error.message || 'An error occurred during login.');
                    });
            });
        } else {
            console.error('Login form not found');
        }
    }

    function showErrorMessage(message) {
        console.error('Error:', message);
        alert(message); // You can replace this with a more user-friendly error display method
    }

    // Make sure to call attachLoginFormHandler when the DOM is loaded
    document.addEventListener('DOMContentLoaded', function () {
        attachLoginFormHandler();
    });


    function promptCreateAccount() {
        let createAccountHtml = `
        <h2>Create Your First Account</h2>
        <p>You don't have any accounts yet. Would you like to create one?</p>
        <button id="create-account-btn">Create Account</button>
    `;
        document.getElementById('content-placeholder').innerHTML = createAccountHtml;

        document.getElementById('create-account-btn').addEventListener('click', function () {
            // Use the username stored in localStorage
            const username = localStorage.getItem('username');
            if (!username) {
                console.error('Username not found in localStorage');
                alert('Error: Username not found. Please try logging in again.');
                return;
            }

            fetch(`http://localhost:5082/api/user/getuser/${username}`)
                .then(response => response.json())
                .then(userData => {
                    return fetch(`http://localhost:5082/api/account/createaccount/${userData.user_id}`, {
                        method: 'POST'
                    });
                })
                .then(response => {
                    if (response.ok) {
                        alert('Account created successfully!');
                        document.getElementById('initial-buttons').style.display = 'none';
                        document.getElementById('dashboard-buttons').style.display = 'block';
                        loadUserDashboard();
                    } else {
                        throw new Error('Failed to create account');
                    }
                })
                .catch(error => {
                    console.error('Error creating account:', error);
                    alert('Error creating account: ' + error.message);
                });
        });
    }

    // Load User Dashboard dynamically
    document.getElementById('load-user-dashboard').addEventListener('click', function () {
        loadUserDashboard();
    });
    function loadUserDashboard() {
        const userId = sessionStorage.getItem('user_id');
        const username = sessionStorage.getItem('username');
        if (!userId || !username) {
            console.error('No user data found. Redirecting to login.');
            showLoginForm();
            return;
        }

        if (userId === '1') {

            loadAdminDashboard();

        } else {
            console.log(`Loading dashboard for user: ${username} (ID: ${userId})`);
            fetch('/api/home/loaduserdashboard')
                .then(response => response.text())
                .then(html => {
                    document.getElementById('content-placeholder').innerHTML = html;
                    displayCurrentUser();
                    attachUserDashboardHandlers();
                    loadUserAccounts();
                    addTransferButton(); // Add this line to create the transfer button
                })
                .catch(error => console.error('Error loading user dashboard:', error));
        }

        
    }
    function loadUserAccounts() {
        const userId = sessionStorage.getItem('user_id');
        if (!userId) {
            console.error('No user ID found. User might not be logged in.');
            return;
        }
        console.log(`Fetching accounts for user: ${sessionStorage.getItem('username')} (ID: ${userId})`);
        fetch(`http://localhost:5082/api/account/getaccountsofuser/${userId}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json();
            })
            .then(accounts => {
                console.log(`Received ${accounts.length} accounts for user ${sessionStorage.getItem('username')}`);
                const accountsList = document.getElementById('accounts-list');
                accountsList.innerHTML = accounts.map(account => `
                <div class="account-box" data-account-num="${account.account_num}" data-balance="${account.balance.toFixed(2)}">
                    <div class="account-number">Account ${account.account_num}</div>
                    <div class="account-balance">$${account.balance.toFixed(2)}</div>
                </div>
            `).join('');
                attachAccountClickHandlers();
            })
            .catch(error => {
                console.error('Error loading accounts:', error);
                alert('Error loading accounts: ' + error.message);
            });
    }

    function attachAccountClickHandlers() {
        const accountBoxes = document.querySelectorAll('.account-box');
        console.log(`Attaching click handlers to ${accountBoxes.length} account boxes`);
        accountBoxes.forEach(box => {
            box.addEventListener('click', function () {
                const accountNum = this.dataset.accountNum;
                const balance = this.dataset.balance;
                console.log(`Clicked on account ${accountNum} with balance ${balance}`);
                loadAccountTransactions(accountNum, balance);
            });
        });
    }


    function loadAccountTransactions(accountNum, balance) {
        console.log(`Loading transactions for account ${accountNum}`);
        fetch('/api/home/loadaccounttransactions')
            .then(response => response.text())
            .then(html => {
                const contentPlaceholder = document.getElementById('content-placeholder');
                if (contentPlaceholder) {
                    contentPlaceholder.innerHTML = html;

                    // Update account number and balance
                    const accountNumberElement = document.getElementById('current-account-number');
                    const accountBalanceElement = document.getElementById('current-account-balance');
                    if (accountNumberElement) accountNumberElement.textContent = accountNum;
                    if (accountBalanceElement) accountBalanceElement.textContent = balance;

                    // Set default date range
                    const endDate = new Date().toISOString().split('T')[0];
                    const startDate = new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0];
                    const startDateInput = document.getElementById('start-date');
                    const endDateInput = document.getElementById('end-date');
                    if (startDateInput) startDateInput.value = startDate;
                    if (endDateInput) endDateInput.value = endDate;

                    // Fetch initial transactions
                    fetchTransactions(accountNum, startDate, endDate);

                    // Attach event handlers
                    attachTransactionHandlers(accountNum);
                } else {
                    console.error('content-placeholder element not found');
                }
            })
            .catch(error => console.error('Error loading account transactions view:', error));
    }

    function fetchTransactions(accountNum, startDate = null, endDate = null) {
        let url = `http://localhost:5082/api/transaction/getacctrans/${accountNum}/${startDate || '1900-01-01'}/${endDate || '2100-12-31'}`;

        console.log('Fetching transactions from URL:', url);

        fetch(url)
            .then(response => response.json())
            .then(transactions => {
                console.log('Received transactions:', transactions);
                displayTransactions(transactions);
            })
            .catch(error => console.error('Error fetching transactions:', error));
    }
    function displayTransactions(transactions) {
        const transactionsList = document.getElementById('transactions-list');
        if (transactionsList) {
            if (transactions.length === 0) {
                transactionsList.innerHTML = '<tr><td colspan="4">No transactions found for this period.</td></tr>';
            } else {
                transactions.sort((a, b) => new Date(b.date) - new Date(a.date));
                transactionsList.innerHTML = transactions.map(transaction => `
                <tr>
                    <td>${transaction.date}</td>
                    <td>${transaction.counterparty || '-'}</td>
                    <td>$${transaction.amount.toFixed(2)}</td>
                    <td>${transaction.description || '-'}</td>
                </tr>
            `).join('');
            }
        } else {
            console.error('transactions-list element not found');
        }
    }

    function attachTransactionHandlers(accountNum) {
        const filterButton = document.getElementById('filter-transactions');
        const startDateInput = document.getElementById('start-date');
        const endDateInput = document.getElementById('end-date');
        const viewAllButton = document.getElementById('view-all-transactions');
        const backButton = document.getElementById('back-to-accounts');

        if (filterButton && startDateInput && endDateInput && viewAllButton && backButton) {
            console.log('All transaction handler elements found');

            filterButton.addEventListener('click', function () {
                console.log('Filter button clicked');
                if (startDateInput.value && endDateInput.value) {
                    fetchTransactions(accountNum, startDateInput.value, endDateInput.value);
                } else {
                    alert('Please select both start and end dates to filter.');
                }
            });

            viewAllButton.addEventListener('click', function () {
                console.log('View all button clicked');
                startDateInput.value = '';
                endDateInput.value = '';
                fetchTransactions(accountNum);
            });

            backButton.addEventListener('click', function () {
                console.log('Back button clicked');
                loadUserDashboard();
            });

            function validateDates() {
                const startDate = new Date(startDateInput.value);
                const endDate = new Date(endDateInput.value);
                filterButton.disabled = !startDateInput.value || !endDateInput.value || startDate > endDate;
            }

            startDateInput.addEventListener('change', validateDates);
            endDateInput.addEventListener('change', validateDates);

            validateDates();
        } else {
            console.error('One or more transaction handler elements not found');
            console.log('Filter button:', filterButton);
            console.log('Start date input:', startDateInput);
            console.log('End date input:', endDateInput);
            console.log('View all button:', viewAllButton);
            console.log('Back button:', backButton);
        }
    }

    function addAccount() {
        const userId = sessionStorage.getItem('user_id');
        if (!userId) {
            console.error('No user ID found. User might not be logged in.');
            alert('Error: User not logged in.');
            return;
        }

        // Make sure this URL matches your API route
        fetch(`http://localhost:5082/api/account/createaccount/${userId}`, {
            method: 'POST'
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json(); // Change this to .json() since the response is JSON
            })
            .then(data => {
                console.log('Server response:', data);
                if (data.account_num) {
                    alert(`New account added successfully. Account number: ${data.account_num}`);
                    loadUserAccounts(); // Refresh the account list
                } else {
                    throw new Error('Unexpected server response');
                }
            })
            .catch(error => {
                console.error('Error creating account:', error);
                alert('Error creating new account: ' + error.message);
            });
    }

    // Load Admin Dashboard dynamically
    document.getElementById('load-admin-dashboard').addEventListener('click', function () {
        loadAdminDashboard();
    });

    function loadAdminDashboard() {
        const userId = sessionStorage.getItem('user_id');
        if (userId !== '1') {
            console.error('Unauthorized access to admin dashboard');
            alert('You do not have permission to access the admin dashboard.');
            return;
        }

        fetch('/api/home/loadadmindashboard')
            .then(response => response.text())
            .then(html => {
                document.getElementById('content-placeholder').innerHTML = html;
                attachAdminDashboardHandlers();
            })
            .catch(error => console.error('Error loading admin dashboard:', error));
    }

    //Transfer code

    function loadTransferForm() {
        fetch('/api/home/loadtransferform')
            .then(response => response.text())
            .then(html => {
                document.getElementById('content-placeholder').innerHTML = html;
                populateUserAccounts();
                attachTransferHandlers();
            })
            .catch(error => console.error('Error loading transfer form:', error));
    }

    function populateUserAccounts() {
        const userId = sessionStorage.getItem('user_id');
        if (!userId) {
            console.error('No user ID found. User might not be logged in.');
            return;
        }
        fetch(`http://localhost:5082/api/account/getaccountsofuser/${userId}`)
            .then(response => response.json())
            .then(accounts => {
                const select = document.getElementById('transferring-from');
                select.innerHTML = '<option value="">Transferring from</option>'; // Clear existing options
                accounts.forEach(account => {
                    const option = document.createElement('option');
                    option.value = account.account_num;
                    option.textContent = `Account ${account.account_num} - Balance: $${account.balance.toFixed(2)}`;
                    select.appendChild(option);
                });
            })
            .catch(error => console.error('Error loading user accounts:', error));
    }

    function attachTransferHandlers() {
        document.getElementById('back-to-dashboard').addEventListener('click', loadUserDashboard);
        document.getElementById('submit-transfer').addEventListener('click', submitTransfer);
    }

    function submitTransfer() {
        const fromAccount = document.getElementById('transferring-from').value;
        const toAccount = document.getElementById('transferring-to').value;
        const amount = document.getElementById('transfer-amount').value;
        const description = document.getElementById('transfer-description').value;

        console.log(`Attempting transfer: From ${fromAccount} to ${toAccount}, Amount: ${amount}`);

        fetch(`http://localhost:5082/api/transaction/createtransaction/${fromAccount}/${toAccount}/${amount}/${description}/transfer/${new Date().toISOString()}`, {
            method: 'POST',
            headers: addUserHeader() // Use the addUserHeader function to add the user ID to the request
        })
            .then(response => response.json())
            .then(data => {
                console.log('Transfer response:', data);
                if (data.message.includes("Successfully created transaction")) {
                    alert('Transfer successful!');
                    loadUserDashboard();
                } else {
                    alert(`Error: ${data.message}`);
                }
            })
            .catch(error => {
                console.error('Transfer error:', error);
                alert(`Error: ${error.message}`);
            });
    }

    function addTransferButton() {
        const transferButton = document.createElement('button');
        transferButton.textContent = 'Transfer';
        transferButton.className = 'btn btn-primary mt-3';
        transferButton.addEventListener('click', loadTransferForm);
        document.getElementById('user-dashboard').appendChild(transferButton);
    }





    function editProfile() {
        // Implement edit profile functionality
        //alert('Edit profile functionality to be implemented');
        fetch('api/home/loadedituserform').then(response => response.text())
            .then(html => {
                document.getElementById('content-placeholder').innerHTML = html;
                attachEditProfileHandlers();
            })
            .catch(error => console.error('Error loading admin dashboard', error));
    }


    function updateProfile() {
        let userData = {
            username: document.getElementById('new-username').value,
            password: document.getElementById('new-password').value,
            email: document.getElementById('new-email').value,
            address: document.getElementById('new-address').value,
            phone: document.getElementById('new-phone').value,
            user_id: sessionStorage.getItem('user_id')
        };

        // Sending a POST request
        fetch('http://localhost:5082/api/user/updateuser', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(userData)
        })
            .then(response => {
                if (!response.ok) {
                    // Throw error if the response is not successful (e.g., status 400)
                    return response.json().then(errorData => {
                        throw new Error(errorData.error);
                    });
                }
                return response.json();  // Return JSON if successful
            })
            .then(data => {
                console.log(data.message);
                loadUserDashboard();
            })
            .catch(error => {
                console.error('Saving error:', error);
                alert(`Error: ${error.message}`);
            });
    }


    function attachEditProfileHandlers() {
        document.getElementById('save-details').addEventListener('click', updateProfile);
    }

    // Attach handlers for user dashboard actions (e.g., money transfer)
    function attachUserDashboardHandlers() {
        const editProfileButton = document.getElementById('edit-profile');
        if (editProfileButton) {
            editProfileButton.addEventListener('click', editProfile);
        }

        const addAccountButton = document.getElementById('add-account');
        if (addAccountButton) {
            addAccountButton.addEventListener('click', addAccount);
        }

        const refreshAccountsButton = document.getElementById('refresh-accounts');
        if (refreshAccountsButton) {
            refreshAccountsButton.addEventListener('click', loadUserAccounts);
        }

        const depositButton = document.getElementById('deposit');
        if (depositButton) {
            depositButton.addEventListener('click', deposit);
        }

        const withdrawButton = document.getElementById('withdraw');
        if (withdrawButton) {
            withdrawButton.addEventListener('click', withdraw);
        }

        const logoutButton = document.getElementById('logout');
        if (logoutButton) {
            logoutButton.addEventListener('click', logout);
        } else {
            console.error('Logout button not found');
        }
    }
    function refreshAccounts() {
        loadUserAccounts();
    }

    // Attach handlers for admin dashboard actions (e.g., view all users, transactions)
    function attachAdminDashboardHandlers() {
        const searchInput = document.getElementById('user-search');
        const viewUserBtn = document.getElementById('search-users');
        const editMyAccountBtn = document.getElementById('edit-admin-profile');

        if (viewUserBtn) {
            viewUserBtn.addEventListener('click', () => searchUser(searchInput.value));
        }

        if (editMyAccountBtn) {
            editMyAccountBtn.addEventListener('click', editAdminProfile);
        }

        // Use event delegation for dynamically added edit buttons
        document.addEventListener('click', function (event) {
            if (event.target && event.target.classList.contains('edit-user-btn')) {
                const userId = event.target.getAttribute('data-user-id');
                editUser(userId);
            }
        });
    }


    // Logout button
    document.getElementById('logout').addEventListener('click', logout);

    function logout() {
        sessionStorage.clear();
        console.log('User logged out. All session data cleared.');
        showLoginForm();
    }

    function searchUser(username) {
        if (!username) {
            alert('Please enter a username to search.');
            return;
        }

        fetch(`http://localhost:5082/api/user/getuser/${username}`)
            .then(response => response.json())
            .then(user => {
                if (user) {
                    displayUserDetails(user);
                    fetchUserAccounts(user.user_id);
                } else {
                    alert('User not found.');
                }
            })
            .catch(error => console.error('Error searching user:', error));
    }

    function displayUserDetails(user) {
        const userInfoDiv = document.getElementById('user-info');
        userInfoDiv.innerHTML = `
        <h3>User Details</h3>
        <p>Username: ${user.username}</p>
        <p>Email: ${user.email}</p>
        <p>Address: ${user.address}</p>
        <p>Phone: ${user.phone}</p>
        <button onclick="editUser(${user.user_id})">Edit User</button>
    `;
    }

    function fetchUserAccounts(userId) {
        fetch(`http://localhost:5082/api/account/getaccountsofuser/${userId}`)
            .then(response => response.json())
            .then(accounts => displayUserAccounts(accounts))
            .catch(error => console.error('Error fetching user accounts:', error));
    }

    function displayUserAccounts(accounts) {
        const accountsListDiv = document.getElementById('accounts-list');
        accountsListDiv.innerHTML = '<h3>User Accounts</h3>';
        accounts.forEach(account => {
            const accountDiv = document.createElement('div');
            accountDiv.className = 'account-box';
            accountDiv.innerHTML = `
            <p>Account ${account.account_num}</p>
            <p>$${account.balance.toFixed(2)}</p>
        `;
            accountDiv.addEventListener('click', () => viewAccountTransactions(account.account_num));
            accountsListDiv.appendChild(accountDiv);
        });
    }

    function editAdminProfile() {
        console.log('Editing admin profile');
        fetch('/api/home/loadedituserform')
            .then(response => response.text())
            .then(html => {
                document.getElementById('content-placeholder').innerHTML = html;
                attachAdminEditProfileHandlers();
            })
            .catch(error => console.error('Error loading edit admin profile form:', error));
    }

    function attachAdminEditProfileHandlers() {
        const updateButton = document.getElementById('save-details');
        if (updateButton) {
            updateButton.addEventListener('click', updateAdminProfile);
        } else {
            console.error('Update profile button not found');
        }
    }

    function updateAdminProfile() {
        const userId = sessionStorage.getItem('user_id');
        let userData = {
            user_id: userId,
            username: document.getElementById('new-username').value,
            password: document.getElementById('new-password').value,
            email: document.getElementById('new-email').value,
            address: document.getElementById('new-address').value,
            phone: document.getElementById('new-phone').value
        };

        fetch('http://localhost:5082/api/user/updateuser', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(userData)
        })
            .then(response => {
                if (!response.ok) {
                    return response.json().then(errorData => {
                        throw new Error(errorData.error || 'Failed to update profile');
                    });
                }
                return response.json();
            })
            .then(data => {
                console.log(data.message);
                alert('Profile updated successfully');
                loadAdminDashboard(); // Reload admin dashboard after update
            })
            .catch(error => {
                console.error('Saving error:', error);
                alert(`Error: ${error.message}`);
            });
    }


    function viewAccountTransactions(accountNum) {
        // Reuse the existing loadAccountTransactions function
        loadAccountTransactions(accountNum);
    }

    function editUser(userId) {
        console.log(`Editing user with ID: ${userId}`);
        fetch(`/api/home/loadedituserform?userId=${userId}`)
            .then(response => response.text())
            .then(html => {
                document.getElementById('content-placeholder').innerHTML = html;
                // Fetch user data and populate the form
                fetch(`http://localhost:5082/api/user/getuser/${userId}`)
                    .then(response => response.json())
                    .then(userData => {
                        populateEditUserForm(userData);
                        attachEditUserFormHandlers(userId);
                    })
                    .catch(error => console.error('Error fetching user data:', error));
            })
            .catch(error => console.error('Error loading edit user form:', error));
    }

    function populateEditUserForm(userData) {
        document.getElementById('new-username').value = userData.username;
        document.getElementById('new-email').value = userData.email;
        document.getElementById('new-address').value = userData.address;
        document.getElementById('new-phone').value = userData.phone;
        // Don't populate the password field for security reasons
    }

    function attachEditUserFormHandlers(userId) {
        const saveButton = document.getElementById('save-details');
        if (saveButton) {
            saveButton.addEventListener('click', () => updateUserProfile(userId));
        } else {
            console.error('Save details button not found');
        }
    }

    function updateUserProfile(userId) {
        console.log(`Updating profile for user ID: ${userId}`);
        let userData = {
            user_id: userId,
            username: document.getElementById('new-username').value,
            password: document.getElementById('new-password').value,
            email: document.getElementById('new-email').value,
            address: document.getElementById('new-address').value,
            phone: document.getElementById('new-phone').value
        };

        fetch('http://localhost:5082/api/user/updateuser', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(userData)
        })
            .then(response => {
                if (!response.ok) {
                    return response.json().then(errorData => {
                        throw new Error(errorData.error || 'Failed to update profile');
                    });
                }
                return response.json();
            })
            .then(data => {
                console.log(data.message);
                alert('User profile updated successfully');
                loadAdminDashboard(); // Reload admin dashboard after update
            })
            .catch(error => {
                console.error('Saving error:', error);
                alert(`Error: ${error.message}`);
            });
    }


    // Update the displayUserDetails function
    function displayUserDetails(user) {
        const userInfoDiv = document.getElementById('user-info');
        userInfoDiv.innerHTML = `
        <h3>User Details</h3>
        <p>Username: ${user.username}</p>
        <p>Email: ${user.email}</p>
        <p>Address: ${user.address}</p>
        <p>Phone: ${user.phone}</p>
        <button class="btn btn-primary edit-user-btn" data-user-id="${user.user_id}">Edit User</button>
    `;
    }

    function showLoginForm() {
        fetch('/api/home/loadloginform')
            .then(response => response.text())
            .then(html => {
                document.getElementById('content-placeholder').innerHTML = html;
                const initialButtons = document.getElementById('initial-buttons');
                if (initialButtons) {
                    initialButtons.style.display = 'block';
                }
                attachLoginFormHandler();
            })
            .catch(error => console.error('Error loading login form:', error));
    }
    function displayCurrentUser() {
        const username = sessionStorage.getItem('username');
        const userNameElement = document.getElementById('user-name');
        if (userNameElement) {
            userNameElement.textContent = username;
        }
        const welcomeMessageElement = document.getElementById('welcome-message');
        if (welcomeMessageElement) {
            welcomeMessageElement.textContent = `Welcome, ${username}!`;
        }
    }

    function withdraw() {
        fetch('api/home/loadwithdrawform').then(response => response.text())
            .then(html => {
                document.getElementById('content-placeholder').innerHTML = html;
                populateUserAccountsWithdraw();
                attachWithdrawHandlers();

            })
            .catch(error => console.error('Error loading admin dashboard', error));
    }

    function submitWithdrawal() {
        const accountNo = document.getElementById('withdraw-from').value;
        const amount = document.getElementById('withdraw-amount').value;
        const descrption = '-';
        const receiver = '';
        //const balancecheck;

        fetch(`http://localhost:5082/api/account/getaccount/${accountNo}`, {
            method: 'GET', // Specify the method as GET
            headers: {
                'Content-Type': 'application/json',
                // You can include any other headers you might need here
            }
        })
            .then(response => {
                if (!response.ok) {
                    // If the response is not OK, throw an error with the status text
                    return response.json().then(data => {
                        throw new Error(data.message || 'Error retrieving account details');
                    });
                }
                return response.json(); // Parse the JSON response if successful
            })
            .then(accountDetails => {
                console.log('Account details:', accountDetails);
                const balance = accountDetails.balance;
               
            })
            .catch(error => {
                console.error('Error fetching account details:', error.message);
                alert(`Error: ${error.message}`);
            });

        
        

        if (amount < 0) {
            alert('Cannot withdraw a negative amount');
        }
        
        else {
            fetch(`http://localhost:5082/api/account/accountwithdraw/${accountNo}/${amount}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                }
            })
                .then(response => {
                    if (!response.ok) {
                        return response.text().then(errorMessage => {
                            throw new Error(errorMessage);
                        });
                    }
                    return response.text();  // Parse the text response if successful
                })
                .then(data => {
                    console.log('Withdrawal successful:', data);
                    //alert('Withdrawal successful!');
                })
                .catch(error => {
                    console.error('Error during Withdrawal:', error.message);
                    alert(`Error: ${error.message}`);
                });

                /*
            fetch(`http://localhost:5082/api/transaction/createtransaction/${accountNo}/null/${amount}/${descrption}/withdrawal/${new Date().toISOString()}`, {
                method: 'POST',
                headers: addUserHeader() 
            })
                
                .then(response => {
                    if (!response.ok) {
                        return response.json().then(data => {
                            throw new Error(data.message || 'Unknown error occurred');
                        });
                    }
                    return response.json(); // Parse the JSON response
                })
                .then(data => {
                    console.log('Transaction response:', data);
                    if (data.message.includes("Successfully created transaction")) {
                        alert('Withdrawal and transaction recorded successfully!');
                        loadUserDashboard(); // Reload the user dashboard
                    } else {
                        alert(`Error: ${data.message}`);
                    }
                })
                .catch(error => {
                    console.error('Transaction error:', error);
                    alert(`Error: ${error.message}`);
                });*/
            loadUserDashboard();
        }


    }

    function attachWithdrawHandlers() {
        document.getElementById('submit-withdrawal').addEventListener('click', submitWithdrawal);
        document.getElementById('back-to-dashboard').addEventListener('click', loadUserDashboard);
    }

    function deposit() {
        fetch('api/home/loaddepositform').then(response => response.text())
            .then(html => {
                document.getElementById('content-placeholder').innerHTML = html;
                populateUserAccountsDeposit();
                attachDepositHandlers();

            })
            .catch(error => console.error('Error loading admin dashboard', error));
    }

    function attachDepositHandlers() {
        document.getElementById('submit-deposit').addEventListener('click', submitDeposit);
        document.getElementById('back-to-dashboard').addEventListener('click', loadUserDashboard);
    }

    function submitDeposit() {
        const accountNo = document.getElementById('deposit-to').value;
        const amount = document.getElementById('deposit-amount').value;

        if (amount < 0) {
            alert('Cannot deposit a negative amount');
        }
        else {
            fetch(`http://localhost:5082/api/account/accountdeposit/${accountNo}/${amount}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                }
            })
                .then(response => {
                    if (!response.ok) {
                        return response.text().then(errorMessage => {
                            throw new Error(errorMessage);
                        });
                    }
                    return response.text();  // Parse the text response if successful
                })
                .then(data => {
                    console.log('Deposit successful:', data);
                    alert('Deposit successful!');
                })
                .catch(error => {
                    console.error('Error during deposit:', error.message);
                    alert(`Error: ${error.message}`);
                });
            loadUserDashboard();
        }
    }

    function populateUserAccountsWithdraw() {
        const userId = sessionStorage.getItem('user_id');
        if (!userId) {
            console.error('No user ID found. User might not be logged in.');
            return;
        }
        fetch(`http://localhost:5082/api/account/getaccountsofuser/${userId}`)
            .then(response => response.json())
            .then(accounts => {
                const select = document.getElementById('withdraw-from');
                select.innerHTML = '<option value="">Withdrawing from</option>'; // Clear existing options
                accounts.forEach(account => {
                    const option = document.createElement('option');
                    option.value = account.account_num;
                    option.textContent = `Account ${account.account_num} - Balance: $${account.balance.toFixed(2)}`;
                    select.appendChild(option);
                });
            })
            .catch(error => console.error('Error loading user accounts:', error));
    }

    function populateUserAccountsDeposit() {
        const userId = sessionStorage.getItem('user_id');
        if (!userId) {
            console.error('No user ID found. User might not be logged in.');
            return;
        }
        fetch(`http://localhost:5082/api/account/getaccountsofuser/${userId}`)
            .then(response => response.json())
            .then(accounts => {
                const select = document.getElementById('deposit-to');
                select.innerHTML = '<option value="">Depositing to</option>'; // Clear existing options
                accounts.forEach(account => {
                    const option = document.createElement('option');
                    option.value = account.account_num;
                    option.textContent = `Account ${account.account_num} - Balance: $${account.balance.toFixed(2)}`;
                    select.appendChild(option);
                });
            })
            .catch(error => console.error('Error loading user accounts:', error));
    }

    function addUserHeader(headers = {}) {
        const userId = sessionStorage.getItem('user_id');
        return { ...headers, 'X-User-ID': userId };
    }




});

