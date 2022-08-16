using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BradsBank.Views.Home
{
    public class WithdrawModel : PageModel
    {
        // Global Variable
        private string m_User;

        // Receive all the variables you pass in
        public WithdrawModel(string? username = "none")
        {
            // Assign them to global variables
            m_User = username;
        }

        public string Username
        {
            get
            {
                return m_User;
            }
        }

        public void OnGet()
        {
        }
    }
}
