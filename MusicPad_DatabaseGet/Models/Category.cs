using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models
{
    public class Category
    {
        public int Categoryid { get; set; }
        public string CategoryName { get; set; }
        public string Discription { get; set; }
    }

    public class Chat
    {
        public Chat()
        {
            request = "0";
            groupType = "";
            newMessageCount = "0";
            newrequest = false;
            GroupUser = new List<JoinGroupUserList>();
        }
        public int groupId { get; set; }
        public bool isadmin { get; set; }
        public string groupName { get; set; }
        public string tagname { get; set; }
        public string imagepath { get; set; }
        public string groupType { get; set; }
        public string request { get; set; }
        public string newMessageCount { get; set; }
        public bool newrequest { get; set; }
        public List<JoinGroupUserList> GroupUser { get; set; }

    }

    public class ChatDetail
    {
        public ChatDetail()
        {
            eventName = "";
            isEvent = false;
            eventId = 0;
        }
        public int groupId { get; set; }
        public int userId { get; set; }
        public int chatID { get; set; }
        public string chattext { get; set; }
        public string createdDate { get; set; }
        public string userName { get; set; }
        public string imagepath { get; set; }
        public string createdTime { get; set; }
        public int eventId { get; set; }
        public bool isEvent { get; set; }
        public string eventName { get; set; }


    }

    public class GroupUserList
    {
        public int groupId { get; set; }
        public int userId { get; set; }
        public string userName { get; set; }
        public string imagePath { get; set; }
        public bool isnew { get; set; }
        public bool ISApprove { get; set; }

    }

    public class JoinGroupUserList
    {
        public int groupId { get; set; }
        public int userId { get; set; }
        public string userName { get; set; }
        public string imagePath { get; set; }
        public bool isAdmin { get; set; }
        public bool ISApprove { get; set; }

    }


}