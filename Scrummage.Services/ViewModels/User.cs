﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Scrummage.Services.ViewModels
{
    public class User
    {
        #region Properties

        public int Id { get; set; }
        
        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "The {0} must be between {2} and {1} characters long.")]
        public string Name { get; set; }
        
        [Required]
        [StringLength(3, MinimumLength = 2, ErrorMessage = "The {0} must be between {2} and {1} characters long.")]
        [Display(Name = "Short Name")]
        public string ShortName { get; set; }
        
        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "The {0} must be between {2} and {1} characters long.")]
        [Display(Name = "UserViewModel Name")]
        public string Username { get; set; }
        
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "The email address is invalid.")]
        public string Password { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Email { get; set; }

        #endregion

        #region Navigation

        public virtual ICollection<Role> Roles { get; set; }
        public virtual Avatar Avatar { get; set; }

        #endregion
    }
}