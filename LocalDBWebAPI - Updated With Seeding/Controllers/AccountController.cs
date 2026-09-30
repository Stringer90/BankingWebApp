using Microsoft.AspNetCore.Mvc;
using LocalDBWebAPI.Models;
using System.Diagnostics;
using LocalDBWebAPI.Data;
using Microsoft.AspNetCore.Http;

namespace LocalDBWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        // ===== FOR ACCOUNT MANAGEMENT PART OF TASK DESCRIPTION FOR TUTORIAL 7 =====

        // create account
        [HttpPost]
        [Route("createaccount/{user_id}")]
        public IActionResult CreateAccount(int user_id)
        {
            Account newAccount = DBManager.CreateAccount(user_id);
            if (newAccount != null)
            {
                return Ok(newAccount); // Return the new account object
            }
            return BadRequest("Error when creating account");
        }
        // retrieve account details by account number
        [HttpGet]
        [Route("getaccount/{account_num}")]
        public IActionResult GetAccount(int account_num)
        {
            Account account = DBManager.GetAccount(account_num);

            // if account not found, account will be null, checked when getting request results
            return Json(account);
        }

        // update account details (amount / balance)
        [HttpPost]
        [Route("updateaccount/{account_num}/{newBalance}")]
        public IActionResult UpdateAccount(int account_num, double newBalance)
        {
            if (DBManager.UpdateAccount(account_num, newBalance))
            {
                return Ok("Successfully updated account balance");
            }
            return BadRequest("Error when updating account balance");
        }

        // delete an account
        [HttpPost]
        [Route("deleteaccount/{account_num}")]
        public IActionResult DeleteAccount(int account_num)
        {
            if (DBManager.DeleteAccount(account_num))
            {
                return Ok("Successfully deleted account");
            }
            return BadRequest("Error when deleting account");
        }

        // ===== OTHER METHODS FOR USE IN TUTORIAL 8 APP =====

        // remove from balance (for use in transaction operations)
        [HttpPost]
        [Route("accountwithdraw/{account_num}/{amount}")]
        public IActionResult AccountWithdraw(int account_num, double amount)
        {
            if (DBManager.AccountWithdraw(account_num, amount))
            {
                return Ok("Successfully withdrew from account");
            }
            return BadRequest("Error when withdrawing from account");
        }


        // add to balance (for use in transaction operations)
        [HttpPost]
        [Route("accountdeposit/{account_num}/{amount}")]
        public IActionResult AccountDeposit(int account_num, double amount)
        {
            if (DBManager.AccountDeposit(account_num, amount))
            {
                return Ok("Successfully deposited into account");
            }
            return BadRequest("Error when withdrawing from account");
        }

        // get only accounts of a user
        // returns list of account objects
        // (for use in a list selection) (to select which to view)
        [HttpGet]
        [Route("getaccountsofuser/{user_id}")]
        public IEnumerable<Account> GetAccountsOfUser(int user_id)
        {
            List<Account> accounts = DBManager.GetAccountsOfUser(user_id);

            // if account not found, account will be null, checked when getting request results
            return accounts;
        }

    }
}
