using LocalDBWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // This method loads the index page
        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }

        // Load the Register Form Partial View
        [HttpGet("loadregisterform")]
        public IActionResult LoadRegisterForm()
        {
            return PartialView("_RegisterForm");
        }

        // Load the Login Form Partial View
        [HttpGet("loadloginform")]
        public IActionResult LoadLoginForm()
        {
            return PartialView("_LoginForm");
        }

        // Load the User Dashboard Partial View
        [HttpGet("loaduserdashboard")]
        public IActionResult LoadUserDashboard()
        {
            return PartialView("_UserDashboard");
        }

        // Load the Admin Dashboard Partial View
        [HttpGet("loadadmindashboard")]
        public IActionResult LoadAdminDashboard()
        {
            return PartialView("_AdminDashboard");
        }
        //load Transform code
        [HttpGet("loadtransferform")]
        public IActionResult LoadTransferForm()
        {
            return PartialView("_TransferForm");
        }
        //loading the transactions of the account
        [HttpGet("loadaccounttransactions")]
        public IActionResult LoadAccountTransactions()
        {
            return PartialView("_AccountTransactions");
        }

        [HttpGet("loadedituserform")]
        public IActionResult LoadEditUserForm()
        {
            return PartialView("_EditUserForm");
        }


        [HttpGet("loaddepositform")]
        public IActionResult loadDepositForm()
        {
            return PartialView("_DepositForm");
        }

        [HttpGet("loadwithdrawform")]
        public IActionResult loadWithdrawForm()
        {
            return PartialView("_WithdrawForm");
        }


        // Error handling (optional, you can add this to handle errors)
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
