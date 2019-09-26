using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Partial class model user
    /// </summary>
    [MetadataType(typeof(UserDataAnotation))]
    public partial class User
    {
        /// <summary>
        /// Menampilkan data nama user beserta employee number
        /// </summary>
        public string UserDetail
        {
            get
            {
                return this.Name + " " + "-" + " " + this.EmployeeNumber;
            }
            set
            {
                return;
            }
        }
    }

    /// <summary>
    /// Data anotation user
    /// </summary>
    public class UserDataAnotation
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public string EmployeeNumber { get; set; }
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        public string PIN { get; set; }
        [Required(ErrorMessage = "Role must be selected")]
        public int RoleID { get; set; }
        public Nullable<int> ProcessGroupID { get; set; }
        public Nullable<System.Guid> ParentUserID { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        
    }
}