using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Class model yang menyimpan informasi user powerapps
    /// </summary>
    public class UserModel
    {
        /// <summary>
        ///  Login Status, 0 is Failed, 1 is Success
        /// </summary>
        public int LoginStatus { get; set; }

        /// <summary>
        /// ID user
        /// </summary>
        public Guid ID { get; set; }        

        /// <summary>
        /// Nama user
        /// </summary>
        public string Name { get; set; }   
        
        /// <summary>
        /// NRP user
        /// </summary>
        public string EmployeeNumber { get; set; }

        /// <summary>
        /// ID Role user
        /// </summary>
        public int RoleID { get; set; }

        /// <summary>
        /// ID grup proses dari group leader
        /// </summary>
        public Nullable<int> ProcessGroupID { get; set; }       
        
        /// <summary>
        /// ID parent dari user
        /// </summary>
        public Nullable<Guid> ParentUserID { get; set; }

        /// <summary>
        /// Email user
        /// </summary>
        public string Email { get; set; }        

        /// <summary>
        /// Status user
        /// </summary>
        public bool IsActive { get; set; }


        /// <summary>
        /// status operator apakah sudah ditugaskan atau belum
        /// </summary>
        public bool IsAssign { get; set; }

        /// <summary>
        /// Person ID user
        /// </summary>
        public string PersonID { get; set; }

        /// <summary>
        /// Nama grup proses user
        /// </summary>
        public string ProcessGroupName { get; set; }

        /// <summary>
        /// Nama role
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// PIN user
        /// </summary>
        public string PIN { get; set; }

        /// <summary>
        /// username user
        /// </summary>
        public string UserName { get; set; }
        //public ICollection<UserImage> UserImages { get; set; }
    }
}