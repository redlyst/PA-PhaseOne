using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Constants
{
    /// <summary>
    /// Merupakan sebuah class untuk menjadi konstanta nama role dari user
    /// </summary>
    public class RoleNames
    {
        public const string Administrator = "Administrator";
        public const string SuperAdmin = "Super Administrator";
        public const string PPC = "Production Planning Control";
        public const string PE = "Production Engineering";
        public const string GroupLeader = "Group Leader Produksi";
        public const string Operator = "Operator";
        public const string Inspector = "Inspector";
        public const string Supervisor = "Supervisor";
        public const string Foreman = "Foreman";
        public const string GroupLeaderPB = "Group Leader PB";

        //public static string Administrator { get { return ConfigurationManager.AppSettings["RoleName_Administrator"]; } }
        //public static string SuperAdmin { get { return ConfigurationManager.AppSettings["RoleName_SuperAdmin"]; } }
        //public static string PPC { get { return ConfigurationManager.AppSettings["RoleName_PPC"]; } }
        //public static string PE { get { return ConfigurationManager.AppSettings["RoleName_PE"]; } }
        //public static string GroupLeader { get { return ConfigurationManager.AppSettings["RoleName_GroupLeader"]; } }
        //public static string Operator { get { return ConfigurationManager.AppSettings["RoleName_Operator"]; } }
        //public static string Inspector { get { return ConfigurationManager.AppSettings["RoleName_Inspector"]; } }
        //public static string Supervisor { get { return ConfigurationManager.AppSettings["RoleName_Supervisor"]; } }
        //public static string Foreman { get { return ConfigurationManager.AppSettings["RoleName_Foreman"]; } }
    }
}