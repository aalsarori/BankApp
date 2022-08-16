using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BradsBank.Views.Home
{
    public class SignInModel : PageModel
    {
        string m_Error = "none";

        public SignInModel(string? error = "none")
        {
            m_Error = error;
        }

        public string GetError
        {
            get
            {
                if(m_Error != null)
                {
                    return m_Error;
                }
                else
                {
                    return "none";
                }
            }
        }

        public void OnGet()
        {
        }
    }
}
