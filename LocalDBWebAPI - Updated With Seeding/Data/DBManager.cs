using LocalDBWebAPI.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Buffers.Text;
using System.ComponentModel;
using System;
using System.Data.SQLite;
using static System.Data.Entity.Infrastructure.Design.Executor;
using System.Net;

namespace LocalDBWebAPI.Data
{
    public class DBManager
    {
        private static string connectionString = "Data Source=mydatabase.db;Version=3;";

        //updated by avis
        public static Account CreateAccount(int user_id)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                INSERT INTO Account (user_id)
                VALUES (@user_id);
                SELECT last_insert_rowid();";

                        command.Parameters.AddWithValue("@user_id", user_id);

                        int newAccountId = Convert.ToInt32(command.ExecuteScalar());

                        if (newAccountId > 0)
                        {
                            return new Account { account_num = newAccountId, user_id = user_id, balance = 0 };
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return null;
        }
        //updated by avis
        public static Account GetAccount(int account_num)
        {
            Account account = null;

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT * FROM Account WHERE account_id = @AccountNum";
                        command.Parameters.AddWithValue("@AccountNum", account_num);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                account = new Account
                                {
                                    account_num = Convert.ToInt32(reader["account_id"]),
                                    user_id = Convert.ToInt32(reader["user_id"]),
                                    balance = Convert.ToDouble(reader["balance"])
                                };
                                Console.WriteLine($"Retrieved account: {account.account_num}, Balance: {account.balance}");
                            }
                            else
                            {
                                Console.WriteLine($"No account found with number: {account_num}");
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAccount: {ex.Message}");
            }

            return account;
        }
        //updated by avis wasnt working properly
        public static bool UpdateAccount(int account_num, double newBalance)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"UPDATE Account SET balance = @Balance WHERE account_id = @AccountID";
                        command.Parameters.AddWithValue("@Balance", newBalance);
                        command.Parameters.AddWithValue("@AccountID", account_num);

                        int rowsUpdated = command.ExecuteNonQuery();
                        connection.Close();

                        return rowsUpdated > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating account: " + ex.Message);
                return false;
            }
        }

        public static bool DeleteAccount(int account_num)
        {
            try
            {
                // Create a new SQLite connection
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Create a new SQLite command to execute SQL
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        // Build the SQL command
                        command.CommandText = @"DELETE FROM Account WHERE account_id = @AccountID";
                        command.Parameters.AddWithValue("@AccountID", account_num);

                        // Execute the SQL command
                        int rowsDeleted = command.ExecuteNonQuery();

                        connection.Close();
                        if (rowsDeleted > 0)
                        {
                            return true;
                        }
                    }
                    connection.Close();
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        public static bool AccountWithdraw(int account_num, double amount)
        {
            try
            {
                // Create a new SQLite connection
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Create a new SQLite command to execute SQL
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        // Build the SQL command
                        command.CommandText = @"UPDATE Account SET balance = balance - @Amount WHERE account_id = @AccountID";
                        command.Parameters.AddWithValue("@Amount", amount);
                        command.Parameters.AddWithValue("@AccountID", account_num);

                        // Execute the SQL command
                        int rowsUpdated = command.ExecuteNonQuery();
                        connection.Close();

                        if (rowsUpdated > 0)
                        {
                            return true;
                        }
                    }
                    connection.Close();
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        public static bool AccountDeposit(int account_num, double amount)
        {
            try
            {
                // Create a new SQLite connection
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Create a new SQLite command to execute SQL
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        // Build the SQL command
                        command.CommandText = @"UPDATE Account SET balance = balance + @Amount WHERE account_id = @AccountID";
                        command.Parameters.AddWithValue("@Amount", amount);
                        command.Parameters.AddWithValue("@AccountID", account_num);

                        // Execute the SQL command
                        int rowsUpdated = command.ExecuteNonQuery();
                        connection.Close();

                        if (rowsUpdated > 0)
                        {
                            return true;
                        }
                    }
                    connection.Close();
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        public static List<Account> GetAccountsOfUser(int user_id)
        {
            List<Account> accountList = new List<Account>();

            try
            {
                // Create a new SQLite connection
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Create a new SQLite command to execute SQL
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        // Build the SQL command
                        command.CommandText = @"SELECT * FROM Account WHERE user_id = @UserID";
                        command.Parameters.AddWithValue("@UserID", user_id);

                        // Execute the SQL command and retrieve data
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Account account = new Account();
                                account.account_num = Convert.ToInt32(reader["account_id"]);
                                account.user_id = Convert.ToInt32(reader["user_id"]);
                                account.balance = Convert.ToDouble(reader["balance"]);

                                accountList.Add(account);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return accountList;
        }

        public static bool CreateTransaction(int? sender_num, int? receiver_num, double amount, string description, string type, string date)
        {
            try
            {
                // Create a new SQLite connection
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Create a new SQLite command to execute SQL
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        // SQL command to insert data
                        command.CommandText = @"INSERT INTO Transactions (sender_num, receiver_num, amount, description, type, date) VALUES (@SenderNum, @ReceiverNum, @Amount, @Description, @Type, @Date)";

                        // Define parameters for the query
                        command.Parameters.AddWithValue("@SenderNum", sender_num);
                        command.Parameters.AddWithValue("@ReceiverNum", receiver_num);
                        command.Parameters.AddWithValue("@Amount", amount);
                        command.Parameters.AddWithValue("@Description", description);
                        command.Parameters.AddWithValue("@Type", type);
                        command.Parameters.AddWithValue("@Date", date);

                        // Execute the SQL command
                        int rowsInserted = command.ExecuteNonQuery();

                        connection.Close();
                        if (rowsInserted > 0)
                        {
                            return true;
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return false;
        }

        public static List<TransactionWithAccount> GetAccountTransactions(int account_num, string start_date, string end_date)
        {
            List<TransactionWithAccount> transactionList = new List<TransactionWithAccount>();

            try
            {
                // Create a new SQLite connection
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Create a new SQLite command to execute SQL
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        // Build the SQL command
                        command.CommandText = @"
                            SELECT * FROM (
                                SELECT 
                                    t.date,
                                    t.type,
                                    'from: ' || u.username || ' (' || a.account_id || ')' AS counterparty,
                                    t.amount AS amount,
                                    t.description AS description
                                FROM 
                                    Transactions t
                                    JOIN Account a ON t.sender_num = a.account_id
                                    JOIN User u ON a.user_id = u.user_id
                                WHERE 
                                    t.receiver_num = @AccountID AND t.type = 'transfer' AND t.date BETWEEN @StartDate AND @EndDate

                                UNION ALL

                                SELECT 
                                    t.date,
                                    t.type,
                                    'to: ' || u.username || ' (' || a.account_id || ')' AS counterparty,
                                    -t.amount AS amount,
                                    t.description AS description
                                FROM 
                                    Transactions t
                                    JOIN Account a ON t.receiver_num = a.account_id
                                    JOIN User u ON a.user_id = u.user_id
                                WHERE 
                                    t.sender_num = @AccountID AND t.type = 'transfer' AND t.date BETWEEN @StartDate AND @EndDate

                                UNION ALL

                                SELECT 
                                    t.date,
                                    t.type,
                                    '-' AS counterparty,
                                    t.amount AS amount,
                                    t.description AS description
                                FROM 
                                    Transactions t
                                WHERE 
                                    t.receiver_num = @AccountID AND t.type = 'deposit' AND t.date BETWEEN @StartDate AND @EndDate

                                UNION ALL

                                SELECT 
                                    t.date,
                                    t.type,
                                    '-' AS counterparty,
                                    -t.amount AS amount,
                                    t.description AS description
                                FROM 
                                    Transactions t
                                WHERE 
                                    t.sender_num = @AccountID AND t.type = 'withdrawal' AND t.date BETWEEN @StartDate AND @EndDate
                            )
                            AS result
                            ORDER BY date DESC";

                        command.Parameters.AddWithValue("@AccountID", account_num);
                        command.Parameters.AddWithValue("@StartDate", start_date);
                        command.Parameters.AddWithValue("@EndDate", end_date);

                        // Execute the SQL command and retrieve data
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TransactionWithAccount transaction = new TransactionWithAccount();

                                transaction.date = reader.GetDateTime(reader.GetOrdinal("date")).ToString("yyyy-MM-dd");
                                transaction.type = reader["type"].ToString();
                                transaction.counterparty = reader["counterparty"].ToString();
                                transaction.amount = Convert.ToDouble(reader["amount"]);
                                transaction.description = reader["description"].ToString();

                                transactionList.Add(transaction);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return transactionList;
        }

        public static List<Transaction> GetAllTransactionsDateDesc(string searchInput, string startDate, string endDate)
        {
            List<Transaction> transactionList = new List<Transaction>();

            try
            {
                // Create a new SQLite connection
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Create a new SQLite command to execute SQL
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        // Build the SQL command
                        /*
                        command.CommandText = @"
                            SELECT * FROM Transactions t
                            WHERE 
                                (t.date BETWEEN @StartDate AND @EndDate)
                                AND
                                (t.description LIKE '%' || @SearchInput || '%' OR t.type LIKE '%' || @SearchInput || '%')
                            ORDER BY date DESC";
                        */
                        command.CommandText = @"
                            SELECT 
                                t.date,
                                    (SELECT u.username || ' (' || a.account_id || ')' 
                                    FROM Account a 
                                        JOIN User u ON a.user_id = u.user_id 
                                    WHERE a.account_id = t.sender_num) 
                                AS sender,
                                    (SELECT u.username || ' (' || a.account_id || ')' 
                                    FROM Account a 
                                        JOIN User u ON a.user_id = u.user_id 
                                    WHERE a.account_id = t.receiver_num) 
                                AS receiver,
                                t.type,
                                t.amount,
                                t.description
                            FROM Transactions t
                            WHERE 
                                (t.date BETWEEN @StartDate AND @EndDate)
                                AND
                                (t.description LIKE '%' || @SearchInput || '%' 
                                OR t.type LIKE '%' || @SearchInput || '%' 
                                OR (SELECT u.username || ' (' || a.account_id || ')' 
                                    FROM Account a 
                                    JOIN User u ON a.user_id = u.user_id 
                                    WHERE a.account_id = t.sender_num) LIKE '%' || @SearchInput || '%' 
                                OR (SELECT u.username || ' (' || a.account_id || ')' 
                                    FROM Account a 
                                    JOIN User u ON a.user_id = u.user_id 
                                    WHERE a.account_id = t.receiver_num) LIKE '%' || @SearchInput || '%' 
                            ORDER BY t.date DESC";

                        command.Parameters.AddWithValue("@SearchInput", searchInput);
                        command.Parameters.AddWithValue("@StartDate", startDate);
                        command.Parameters.AddWithValue("@EndDate", endDate);

                        // Execute the SQL command and retrieve data
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Transaction transaction = new Transaction();

                                transaction.date = reader.GetDateTime(reader.GetOrdinal("date")).ToString("yyyy-MM-dd");
                                transaction.sender = reader["sender"]?.ToString();
                                transaction.receiver = reader["receiver"]?.ToString();
                                transaction.type = reader["type"].ToString();
                                transaction.amount = Convert.ToDouble(reader["amount"]);
                                transaction.description = reader["description"].ToString();

                                transactionList.Add(transaction);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return transactionList;
        }

        public static List<Transaction> GetAllTransactionsDateAsc(string searchInput, string startDate, string endDate)
        {
            List<Transaction> transactionList = new List<Transaction>();

            try
            {
                // Create a new SQLite connection
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Create a new SQLite command to execute SQL
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        // Build the SQL command
                        /*
                        command.CommandText = @"
                            SELECT * FROM Transactions t
                            WHERE 
                                (t.date BETWEEN @StartDate AND @EndDate)
                                AND
                                (t.description LIKE '%' || @SearchInput || '%' OR t.type LIKE '%' || @SearchInput || '%')
                            ORDER BY date ASC";
                        */

                        command.CommandText = @"
                            SELECT 
                                t.date,
                                (SELECT u.username || ' (' || a.account_id || ')' 
                                FROM Account a 
                                    JOIN User u ON a.user_id = u.user_id 
                                WHERE a.account_id = t.sender_num) 
                                AS sender,
                                (SELECT u.username || ' (' || a.account_id || ')' 
                                FROM Account a 
                                    JOIN User u ON a.user_id = u.user_id 
                                WHERE a.account_id = t.receiver_num) 
                                AS receiver,
                                t.type,
                                t.amount,
                                t.description
                            FROM Transactions t
                            WHERE 
                                (t.date BETWEEN @StartDate AND @EndDate)
                                AND
                                (t.description LIKE '%' || @SearchInput || '%' 
                                OR t.type LIKE '%' || @SearchInput || '%' 
                                OR (SELECT u.username || ' (' || a.account_id || ')' 
                                    FROM Account a 
                                    JOIN User u ON a.user_id = u.user_id 
                                    WHERE a.account_id = t.sender_num) LIKE '%' || @SearchInput || '%' 
                                OR (SELECT u.username || ' (' || a.account_id || ')' 
                                    FROM Account a 
                                    JOIN User u ON a.user_id = u.user_id 
                                    WHERE a.account_id = t.receiver_num) LIKE '%' || @SearchInput || '%' 
                            ORDER BY t.date ASC";


                        command.Parameters.AddWithValue("@SearchInput", searchInput);
                        command.Parameters.AddWithValue("@StartDate", startDate);
                        command.Parameters.AddWithValue("@EndDate", endDate);

                        // Execute the SQL command and retrieve data
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Transaction transaction = new Transaction();

                                transaction.date = reader.GetDateTime(reader.GetOrdinal("date")).ToString("yyyy-MM-dd");
                                transaction.sender = reader["sender"]?.ToString();
                                transaction.receiver = reader["receiver"]?.ToString();
                                transaction.type = reader["type"]?.ToString();
                                transaction.amount = Convert.ToDouble(reader["amount"]);
                                transaction.description = reader["description"]?.ToString();

                                transactionList.Add(transaction);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return transactionList;
        }

        public static bool CreateUser(string username, string password, string email, string address, string phone)
        {
            try
            {
                // Create a new SQLite connection
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Create a new SQLite command to execute SQL
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        // SQL command to insert data
                        command.CommandText = @"INSERT INTO User (username, password, email, address, phone) VALUES (@Username, @Password, @Email, @Address, @Phone)";

                        // Define parameters for the query
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Address", address);
                        command.Parameters.AddWithValue("@Phone", phone);

                        // Execute the SQL command
                        int rowsInserted = command.ExecuteNonQuery();

                        connection.Close();
                        if (rowsInserted > 0)
                        {
                            return true;
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return false;
        }

        public static User GetUser(string searchString)
        {
            User user = null;

            try
            {
                // Create a new SQLite connection
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Create a new SQLite command to execute SQL
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        // Build the SQL command to select a student by ID
                        command.CommandText = "SELECT * FROM User WHERE username = @Username OR email = @Email";
                        command.Parameters.AddWithValue("@Username", searchString);
                        command.Parameters.AddWithValue("@Email", searchString);

                        // Execute the SQL command and retrieve data
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user = new User();
                                user.user_id = Convert.ToInt32(reader["user_id"]);
                                user.username = reader["username"]?.ToString();
                                user.password = reader["password"]?.ToString();
                                user.email = reader["email"].ToString();
                                user.address = reader["address"].ToString();
                                user.phone = reader["phone"].ToString();
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return user;
        }

        public static bool UpdateUser(string username, string password, string email, string address, string phone, int user_id)
        {
            try
            {
                // Create a new SQLite connection
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Create a new SQLite command to execute SQL
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        // Build the SQL command
                        command.CommandText = @"UPDATE User SET username = @Username, password = @Password, email = @Email, address = @Address, phone = @Phone WHERE user_id = @UserID";
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Address", address);
                        command.Parameters.AddWithValue("@Phone", phone);
                        command.Parameters.AddWithValue("@UserID", user_id);

                        // Execute the SQL command
                        int rowsUpdated = command.ExecuteNonQuery();
                        connection.Close();

                        if (rowsUpdated > 0)
                        {
                            return true;
                        }
                    }
                    connection.Close();
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }


        public static bool DeleteUser(int user_id)
        {
            try
            {
                // Create a new SQLite connection
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Create a new SQLite command to execute SQL
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        // Build the SQL command
                        command.CommandText = @"DELETE FROM User WHERE user_id = @UserID";
                        command.Parameters.AddWithValue("@UserID", user_id);

                        // Execute the SQL command
                        int rowsDeleted = command.ExecuteNonQuery();

                        connection.Close();
                        if (rowsDeleted > 0)
                        {
                            return true;
                        }
                    }
                    connection.Close();
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        public static bool IsValidAuth(string un_email, string password)
        {
            try
            {
                // Create a new SQLite connection
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Create a new SQLite command to execute SQL
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        // Build the SQL command
                        command.CommandText = @"SELECT * FROM User WHERE (username = @Username AND password = @Password) OR (email = @Email AND password = @Password);";
                        command.Parameters.AddWithValue("@Username", un_email);
                        command.Parameters.AddWithValue("@Email", un_email);
                        command.Parameters.AddWithValue("@Password", password);

                        // Execute the SQL command
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return true;
                            }
                        }
                    }
                    connection.Close();
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        public static void DBInitialise()
        {
            CreateUserTable();
            CreateAccountTable();
            CreateTransactionTable();

            // Insert Admin user, user_id will be 1
            CreateUser("admin", "admin", "admin@email.com", "1 Admin St", "0412345678");

            // Data Seeding
            SeedData();
        }

        // Create user table
        public static bool CreateUserTable()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    // Create a new SQLite command to execute SQL
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"DROP TABLE IF EXISTS User";
                        command.ExecuteNonQuery();

                        // SQL command to create a table
                        command.CommandText = @"
                        CREATE TABLE User (
                            user_id INTEGER PRIMARY KEY AUTOINCREMENT,
                            username VARCHAR(20) NOT NULL,
                            password VARCHAR(20) NOT NULL,
                            email VARCHAR(50) NOT NULL,
                            address VARCHAR(100) NOT NULL,
                            phone VARCHAR(10) NOT NULL
                        )";

                        // Execute the SQL command to create the table
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                Console.WriteLine("Table created successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return false; // Create table failed
        }

        // Create account table
        public static bool CreateAccountTable()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    // Create a new SQLite command to execute SQL
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"DROP TABLE IF EXISTS Account";
                        command.ExecuteNonQuery();

                        // SQL command to create a table
                        command.CommandText = @"
                        CREATE TABLE Account (
                            account_id INTEGER PRIMARY KEY AUTOINCREMENT,
                            user_id INTEGER NOT NULL,
                            balance DECIMAL(10, 2) NOT NULL DEFAULT 0,
                            FOREIGN KEY (user_id) REFERENCES User(user_id) ON DELETE CASCADE
                        )";

                        // Execute the SQL command to create the table
                        command.ExecuteNonQuery();

                        // Initialise Account table to make account number start at 10,000
                        command.CommandText = @"
                        INSERT INTO sqlite_sequence (name, seq) 
                        VALUES ('Account', 9999);";

                        command.ExecuteNonQuery();

                        connection.Close();
                    }
                }
                Console.WriteLine("Table created successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return false; // Create table failed
        }

        // Create tansactions table
        public static bool CreateTransactionTable()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    // Create a new SQLite command to execute SQL
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"DROP TABLE IF EXISTS Transactions";
                        command.ExecuteNonQuery();

                        // SQL command to create a table
                        command.CommandText = @"
                        CREATE TABLE Transactions (
                            transaction_id INTEGER PRIMARY KEY AUTOINCREMENT,
                            sender_num INTEGER,
                            receiver_num INTEGER,
                            amount DECIMAL(10, 2) NOT NULL,
                            description VARCHAR(100) NOT NULL DEFAULT '-',
                            type VARCHAR(10) NOT NULL CHECK(type IN ('deposit', 'withdrawal', 'transfer')),
                            date DATE NOT NULL,
                            FOREIGN KEY (sender_num) REFERENCES Account(account_id),
                            FOREIGN KEY (receiver_num) REFERENCES Account(account_id)
                        )";

                        // Execute the SQL command to create the table
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                Console.WriteLine("Table created successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return false; // Create table failed
        }

        // Seed data into database
        public static void SeedData()
        {
            Random random = new Random();

            // Create list to store created user ids
            List<int> userIds = new List<int>();

            // Create list to store created account ids
            List<int> accountIds = new List<int>();

            // Create 5-10 users
            int numUsers = random.Next(5, 11);
            for (int i = 0; i < numUsers; i++)
            {
                string username = $"user{i + 1}";
                string password = $"password{i + 1}";
                string email = $"{username}@email.com";
                string address = $"{i + 1} Bank Street";
                string phone = $"04{random.Next(10000000, 99999999)}";
                CreateUser(username, password, email, address, phone);

                // Get the user ID and store them
                User user = GetUser(username);
                userIds.Add(user.user_id);
            }

            // Create 1-3 accounts for each user createdd
            foreach (int userId in userIds)
            {
                int numAccounts = random.Next(1, 4);
                for (int i = 0; i < numAccounts; i++)
                {
                    CreateAccount(userId);
                    // Get the id of the account just created above
                    // (get the account number of the most recently inserted account for the current user)
                    Account account = GetAccount(GetAccountsOfUser(userId).Last().account_num);
                    accountIds.Add(account.account_num);
                    // Set the account balance to amount between 5000-10000
                    double balance = random.NextDouble() * 5000 + 5000;
                    UpdateAccount(account.account_num, balance);
                }
            }

            // Create 3 transactions for each account, a deposit, a withdrawal and transfer
            foreach (int accountId in accountIds)
            {
                // Deposit
                double depositAmount = random.NextDouble() * 50 + 50;
                DateTime depositDate = new DateTime(2024, 9, random.Next(1, 31));
                CreateTransaction(null, accountId, depositAmount, "Deposit description", "deposit", depositDate.ToString("yyyy-MM-dd"));

                // Withdrawal
                double withdrawalAmount = random.NextDouble() * 50 + 50;
                DateTime withdrawalDate = new DateTime(2024, 9, random.Next(1, 31));
                CreateTransaction(accountId, null, withdrawalAmount, "Withdrawal description", "withdrawal", withdrawalDate.ToString("yyyy-MM-dd"));

                // Transfer
                int transferAccountId = accountIds[random.Next(accountIds.Count)];
                // Assure an account is not transferring to itself
                while (transferAccountId == accountId)
                {
                    transferAccountId = accountIds[random.Next(accountIds.Count)];
                }
                double transferAmount = random.NextDouble() * 50 + 50;
                DateTime transferDate = new DateTime(2024, 9, random.Next(1, 31));
                CreateTransaction(accountId, transferAccountId, transferAmount, "Transfer description", "transfer", transferDate.ToString("yyyy-MM-dd"));
            }
        }
    }
}


