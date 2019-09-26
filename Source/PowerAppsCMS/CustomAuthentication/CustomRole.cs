//using CustomAuthenticationMVC.DataAccess;
using PowerAppsCMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace PowerAppsCMS.CustomAuthentication
{
    /// <summary>
    /// Custom role Provider untuk security login User
    /// </summary>
    public class CustomRole : RoleProvider
    {
        /// <summary>
        /// Method IsUserInRole, method untuk pengecekan apakah sebuah Username termasuk ke dalam sebuah Role
        /// </summary>
        /// <param name="username">Parameter username dengan tipe data string</param>
        /// <param name="roleName">Parameter roleName dengan tipe data string</param>
        /// <returns>Return 'True' jika sebuah Username termasuk ke dalam sebuah Role, 'False' jika tidak</returns>
        public override bool IsUserInRole(string username, string roleName)
        {
            //var userRoles = GetRolesForUser(username.Split('\\')[1]);
            var userRoles = GetRolesForUser(username);
            return userRoles.Contains(roleName);
        }

        /// <summary>
        /// Method GetRolesForUser, method untuk mengambil rangkaian/beberapa Role yang dimiliki oleh sebuah Username
        /// </summary>
        /// <param name="username">Parameter username dengan tipe data string</param>
        /// <returns>Array/rangakaian string Role</returns>
        public override string[] GetRolesForUser(string username)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return null;
            }

            var userRoles = new string[] { };

            using (PowerAppsCMSEntities dbContext = new PowerAppsCMSEntities())
            {
                var selectedUser = (from us in dbContext.Users.Include("Role")
                                    where string.Compare(us.Username, username, StringComparison.OrdinalIgnoreCase) == 0
                                    select us).FirstOrDefault();


                if (selectedUser != null)
                {
                    userRoles = new[] { selectedUser.Role.Name };
                }

                return userRoles.ToArray();
            }


        }

        #region MyRegion

        #region Overrides of Role Provider

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }


        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

    }
}