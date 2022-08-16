using BradsBank.Models;
using BradsBank.Views.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace BradsBank.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            this.configuration = config;
        }

        public string hashingin256(string value)
        {
            StringBuilder Sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

        public string GetSalt()
        {
            var listofChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            var gettingStringOfChars = new char[3];

            var randomVar = new Random();

            for (int i = 0; i < gettingStringOfChars.Length; i++)
            {
                gettingStringOfChars[i] = listofChars[randomVar.Next(listofChars.Length)];
            }

            var stringResult = new String(gettingStringOfChars);

            return stringResult;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SignInValidation(string username, string password)
        {
            if(username != null)
            {
                // Query to see if the user exists
                string connectionString = configuration.GetConnectionString("DefaultConnectionString");

                SqlConnection connection = new SqlConnection(connectionString);

                connection.Open();
                string checkUserExistsQuery = String.Format("SELECT COUNT(*) FROM Users WHERE Username = '{0}' ", username);
                SqlCommand db = new SqlCommand(checkUserExistsQuery, connection);
                var countUser = (int)db.ExecuteScalar();

                // If they do exist, return
                if (countUser == 0)
                {
                    connection.Close();
                    return RedirectToAction("SignIn", "Home", "Username");
                }

                // If not, get a salt
                string getSaltQuery = String.Format("SELECT Salt FROM Users WHERE Username = '{0}' ", username);
                db = new SqlCommand(getSaltQuery, connection);
                var getSalt = (string)db.ExecuteScalar();

                // Add it to the password
                var saltedPassword = password + getSalt;

                // Hash the password
                var hashedPassword = hashingin256(saltedPassword);

                // Insert the username, hashed password, salt
                string checkForUser = String.Format("SELECT COUNT(*) FROM Users WHERE Username = '{0}' AND HashedPassword = '{1}' ", username, hashedPassword);
                db = new SqlCommand(checkForUser, connection);
                var checkResult = (int)db.ExecuteScalar();

                if (checkResult > 0)
                {
                    // Close the database
                    connection.Close();

                    return RedirectToAction("AccountActions", "Home", username);
                }
                else
                {
                    connection.Close();
                    return RedirectToAction("SignIn", "Home", "Password");
                }
            }
            return RedirectToAction("SignIn", "Home", "Username");
        }

        public IActionResult ValidateRegistration(string username, string password, string confirmed, string first, string last, string email)
        {
            if(username != null)
            {
                if (password != confirmed)
                {
                    return RedirectToAction("Register", "Home", "MistmatchedPassword");
                }

                // Query to see if the user exists
                string connectionString = configuration.GetConnectionString("DefaultConnectionString");

                SqlConnection connection = new SqlConnection(connectionString);

                connection.Open();
                string checkUserExistsQuery = String.Format("SELECT COUNT(*) FROM Users WHERE Username = '{0}' ", username);
                SqlCommand db = new SqlCommand(checkUserExistsQuery, connection);
                var countUser = (int)db.ExecuteScalar();

                // If they do exist, return
                if (countUser > 0)
                {
                    connection.Close();
                    return RedirectToAction("Register", "Home", "UserExists");
                }

                // If not, get a salt
                string getSalt = GetSalt();

                // Add it to the password
                var saltedPassword = password + getSalt;

                // Hash the password
                var hashedPassword = hashingin256(saltedPassword);

                // Insert the username, hashed password, salt
                string checkForUser = String.Format("INSERT INTO users (username, hashedpassword, salt, firstname, lastname, email) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}') ", username, hashedPassword, getSalt, first, last, email);
                db = new SqlCommand(checkForUser, connection);
                var tryit = db.ExecuteScalar();

                connection.Close();

                return RedirectToAction("AccountActions", "Home", username);
            }

            return RedirectToAction("Register", "Home", "UserExists");
        }

        public IActionResult Register()
        {
            return View(new RegisterModel());
        }

        public IActionResult SignIn()
        {
            return View(new SignInModel());
        }

        public IActionResult WithdrawMoney (string username, string accountFrom, int amount)
        {
            if(username != null)
            {
                //Abdul start:

                //make amount in pennies
                amount /= 100;


                // create connection to the database
                string connetionString;
                SqlConnection cnn;
                connetionString = @"Data Source=137.190.19.13;Initial Catalog=AmandaShow;User ID=AmandaShow;Password=+his!$TheP@$$w0rd";
                cnn = new SqlConnection(connetionString);
                cnn.Open();
                cnn.Close();

                //sql statement to get the balance in accountFrom and save it in accoutFrom
                //string sql = "Select currentbalance from account where account = ACCOUNTNUMBER";

                //exec.sql

                double fromAmount = 0 / 100;

                //check if the account we are drawing money from has enough funds
                if (fromAmount < amount)
                {
                    Console.WriteLine("account does not have enough funds");
                    return RedirectToAction("AccountActions", "Home", username);
                }

                string sql = "insert into Transactions (account, amount, tranDesc) values (account number, amount, withdraw)";

                //sql statement to send this new accoutFrom and update the balance avaliable on that specific account


                Console.WriteLine($"Withdrawal of ${amount} was successful");


                return RedirectToAction("AccountActions", "Home", username);
            }

            return RedirectToAction("AccountActions", "Home", username);
        }

        public IActionResult TransferMoney (string username, string accountFrom, string accountTo, double amount)
        {
            if(username != null)
            {

                //Abdul started writing:
                string sql;
                sql = "insert into Transactions (account, amount, tranDesc) values (account number, amount, transfer from accout no)";
                sql = "insert into Transactions (account, amount, tranDesc) values (account number, -amount, transfer from accout no)";

                Console.WriteLine("Transfered successfully");

                //Abdul's code ends here
                return RedirectToAction("AccountActions", "Home", username);
            }

            return RedirectToAction("AccountActions", "Home", username);

        }

        public IActionResult DepositMoney(string username, string account, double amount)
        {
            if(username != null)
            {
                // Abdul:

                //insert into the database

                string sql;
                sql = "insert into Transactions (account, amount, tranDesc) values (account number, amount, deposit)";

                //Abdul: update the amount into the database

                // Add it by amount passed in
                double new_amount = 0;

                // Make the query
                sql = "";

                sql += String.Format("UPDATE accounts SET amount = {0}", new_amount);

                // Run the query
                return RedirectToAction("AccountActions", "Home", username);
            }

            return RedirectToAction("AccountActions", "Home", username);

        }

        public IActionResult AccountActions(string? username)
        {
            if(username != null)
            {
                return View(new AccountActionsModel(username));
            }

            // Pass the variable into the model
            return View(new AccountActionsModel(username));
        }

        public IActionResult Deposit(string? username)
        {
            if (username != null)
            {
                return View(new DepositModel(username));
            }

            return View(new DepositModel(username));
        }

        public IActionResult Withdraw(string? username)
        {
            if (username != null)
            {
                return View(new WithdrawModel(username));
            }

            return View(new WithdrawModel(username));
        }

        public IActionResult Transfer(string? username)
        {
            if (username != null)
            {
                return View(new TransferModel(username));
            }

            return View(new TransferModel(username));
        }

        public IActionResult Transactions(string? username)
        {
            if (username != null)
            {
                return View(new TransactionsModel(username));
            }

            return View(new TransactionsModel(username));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
