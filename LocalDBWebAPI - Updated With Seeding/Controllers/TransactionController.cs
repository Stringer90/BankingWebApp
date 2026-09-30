using Microsoft.AspNetCore.Mvc;
using LocalDBWebAPI.Models;
using System.Diagnostics;
using LocalDBWebAPI.Data;
using Microsoft.AspNetCore.Http;
using System.Transactions;

namespace LocalDBWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : Controller
    {
        // ===== FOR TRANSACTION MANAGEMENT PART OF TASK DESCRIPTION FOR TUTORIAL 7 =====

        // Deposit and withdrawal operations already in AccountController (AccountDeposit and AccountWithdraw)

        // Checking that balances are updated correctly:
        //     In the Web App: checks that account has enough funds to withdraw from (if withdrawing).
        //     Then: Call AccountController (AccountDeposit / AccountWithdraw) in the Web App.

        // Store transaction (create transaction) (updated by avis cos wasnt working properly)
        [HttpPost]
        [Route("createtransaction/{sender_num}/{receiver_num}/{amount}/{description}/{type}/{date}")]
        public IActionResult CreateTransaction(int? sender_num, int? receiver_num, double amount, string description, string type, string date)
        {
            try
            {
                // Log the incoming parameters
                Console.WriteLine($"Attempting transfer: From {sender_num} to {receiver_num}, Amount: {amount}, Type: {type}");

                // Check sender account
                if (sender_num.HasValue)
                {
                    var senderAccount = DBManager.GetAccount(sender_num.Value);
                    Console.WriteLine($"Sender account balance: {senderAccount?.balance}");
                    if (senderAccount == null || senderAccount.balance < amount)
                    {
                        return Json(new { message = $"Error: Insufficient funds or invalid sender account. Balance: {senderAccount?.balance}, Amount: {amount}" });
                    }
                }

                // Proceed with transaction creation
                if (DBManager.CreateTransaction(sender_num, receiver_num, amount, description, type, date))
                {
                    // Update account balances
                    if (sender_num.HasValue)
                    {
                        var senderAccount = DBManager.GetAccount(sender_num.Value);
                        DBManager.UpdateAccount(sender_num.Value, senderAccount.balance - amount);
                    }
                    if (receiver_num.HasValue)
                    {
                        var receiverAccount = DBManager.GetAccount(receiver_num.Value);
                        DBManager.UpdateAccount(receiver_num.Value, receiverAccount.balance + amount);
                    }

                    return Json(new { message = "Successfully created transaction and updated balances" });
                }
                return Json(new { message = "Error when creating transaction" });
            }
            catch (Exception ex)
            {
                return Json(new { message = $"Error: {ex.Message}" });
            }
        }


        // ===== OTHER METHODS FOR USE IN TUTORIAL 8 APP =====

        // Get transactions involving an account, for display in a table.
        [HttpGet]
        [Route("getacctrans/{account_num}/{start_date}/{end_date}")]
        public IEnumerable<TransactionWithAccount> GetAccountTransactions(int account_num, string start_date, string end_date)
        {
            List<TransactionWithAccount> transactions = DBManager.GetAccountTransactions(account_num, start_date, end_date);

            // if account not found, account will be null, checked when getting request results
            return transactions;
        }

        // get all transactions (date descending and ascending) (for admin)

        [HttpGet]
        [Route("getalldatedesc/{searchInput}/{startDate}/{endDate}")]
        public IEnumerable<Models.Transaction> GetAllTransactionsDateDesc(string searchInput, string startDate, string endDate)
        {
            List<Models.Transaction> transactions = DBManager.GetAllTransactionsDateDesc(searchInput, startDate, endDate);

            // if account not found, account will be null, checked when getting request results
            return transactions;
        }

        [HttpGet]
        [Route("getalldateasc/{searchInput}/{startDate}/{endDate}")]
        public IEnumerable<Models.Transaction> GetAllTransactionsDateAsc(string searchInput, string startDate, string endDate)
        {
            List<Models.Transaction> transactions = DBManager.GetAllTransactionsDateAsc(searchInput, startDate, endDate);

            // if account not found, account will be null, checked when getting request results
            return transactions;
        }
    }
}
