using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace MovieCatalog.Models
{

    public class ChangePasswordModel
    {
        [Required]
        [DataType( DataType.Password )]
        [Display( Name = "Current password" )]
        public string OldPassword { get; set; }

        [Required]
        [StringLength( 100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6 )]
        [DataType( DataType.Password )]
        [Display( Name = "New password" )]
        public string NewPassword { get; set; }

        [DataType( DataType.Password )]
        [Display( Name = "Confirm new password" )]
        [Compare( "NewPassword", ErrorMessage = "The new password and confirmation password do not match." )]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display( Name = "User name" )]
        public string UserName { get; set; }

        [Required]
        [DataType( DataType.Password )]
        [Display( Name = "Password" )]
        public string Password { get; set; }

        [Display( Name = "Remember me?" )]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        static RegisterModel()
        {
            GenderList = GetGenderSelectList();
        }

        [Required]
        [Display( Name = "User name" )]
        public string UserName { get; set; }

        [Required]
        [DataType( DataType.EmailAddress )]
        [Display( Name = "Email address" )]
        public string Email { get; set; }

        [DefaultValue( 0 )]
        [Range( 6, 300, ErrorMessage = "You specify wrong value of age." )]
        [Display( Name = "Age" )]
        public int Age { get; set; }

        [Display( Name = "Gender" )]
        public Gender Gender { get; set; }

        public static SelectList GenderList { get; set; }

        [Required]
        [StringLength( 100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6 )]
        [DataType( DataType.Password )]
        [Display( Name = "Password" )]
        public string Password { get; set; }

        [DataType( DataType.Password )]
        [Display( Name = "Confirm password" )]
        [Compare( "Password", ErrorMessage = "The password and confirmation password do not match." )]
        public string ConfirmPassword { get; set; }

        private static SelectList GetGenderSelectList()
        {
            var enumValues =
                Enum.GetValues( typeof( Gender ) ).Cast<Gender>().Select(
                    e => new { Value = e.ToString(), Text = e.ToString() } ).ToList();

            return new SelectList( enumValues, "Value", "Text", Gender.NotDefined.ToString() );
        }
    }
}
