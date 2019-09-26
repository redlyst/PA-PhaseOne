using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PowerAppsCMS.Models;

namespace PowerAppsCMS.ViewModel
{
    /// <summary>
    /// View model dari user yang berisikan object User, ProcessGroup, dan list object UserImages
    /// </summary>
    public class UserViewModel
    {
        public User User { get; set; }
        public int RoleID { get; set; }
        public int? ProcessGroupID { get; set; }
        public Guid? ParentUserID { get; set; }
        public ProcessGroup ProcessGroup { get; set; }
        public List<UserImage> UserImages { get; set; }
        

        public int[] SelectedProcessGroup { get; set; }
        public List<ProcessGroup> ProcessGroupList { get; set; }
        public List<UserProcessGroup> UserProcessGroupList { get; set; }

        public UserViewModel()
        {
            ProcessGroupList = new List<ProcessGroup>();
            UserImages = new List<UserImage>();
            UserProcessGroupList = new List<UserProcessGroup>();
        }
    }
}