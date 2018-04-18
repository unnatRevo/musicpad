using MusicEventAPI.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models
{
    public class User
    {
        public User()
        {
            password = "";
        }

        public string password { get; set; }
        public int userId { get; set; }
        public string fbId { get; set; }
        public string name { get; set; }
        public string userName { get; set; }
        public string countryCode { get; set; }
        public string phone { get; set; }
        public string emailId { get; set; }
        public string userImagePath { get; set; }
        public bool isEnterprice { get; set; }
        public bool applyEnterprice { get; set; }
        public string profileType { get; set; }
        public string accessToken { get; set; }
        public string deviceToken { get; set; }
        public string deviceType { get; set; }
        public string groupPeople { get; set; }
        public bool isBusinessUser { get; set; }
        public string businessLocation { get; set; }
        public string bussinesslatitude { get; set; }
        public string businesslongitude { get; set; }

        public bool isFlySolo { get; set; }
        public bool isGroup { get; set; }
        public bool isCouple { get; set; }



    }

    public class NearMe
    {
        public int eventId { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string categoryName { get; set; }
        public string eventName { get; set; }
        public string imagepath { get; set; }
        public string location { get; set; }


    }

    public class Event
    {

        public Event()
        {
            ratings = "";
            nearMe = "";
            ratingCount = "";
            ticketCount = "";
            price = "";
            addToCalender = false;
            isadvertisement = false;
            eventUser = new List<EventUser>();
            isFlySolo = "";
            isGroup = "";
            isCouple = "";
            nearMeEvents = new List<NearMe>();
            alredyRating = false;
            isTmaster = false;
        }
        public int eventId { get; set; }
        //  public int userId { get; set; }
        public int categoryId { get; set; }
        public string eventName { get; set; }
        public string location { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string eventDescription { get; set; }
        public string imagepath { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public int activityCount { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string dealtext { get; set; }
        public string percentage { get; set; }
        public string categoryName { get; set; }
        public bool isOwner { get; set; }
        public string ratings { get; set; }

        public string ratingCount { get; set; }
        public string price { get; set; }
        public string ticketCount { get; set; }
        public string nearMe { get; set; }
        public bool addToCalender { get; set; }
        public bool isadvertisement { get; set; }
        public bool alredyRating { get; set; }
        public bool isTmaster { get; set; }
        public List<EventUser> eventUser { get; set; }

        public List<NearMe> nearMeEvents { get; set; }

        public string isFlySolo { get; set; }
        public string isGroup { get; set; }
        public string isCouple { get; set; }
        public string ticketUrl { get; set; }
    }



    public class NearME
    {
        public NearME()
        {
            isadvertisement = false;
        }

        public int eventId { get; set; }
        //  public int userId { get; set; }
        public string eventName { get; set; }
        public string location { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string eventDescription { get; set; }
        public string imagepath { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string categoryName { get; set; }
        public bool isOwner { get; set; }
        public string ratings { get; set; }
        public string nearMe { get; set; }
        public bool addToCalender { get; set; }
        public bool isadvertisement { get; set; }

    }


    public class EventAdvertisement
    {
        public EventAdvertisement()
        {
            eventUser = new List<EventUser>();
            addToCalender = false;

        }

        public int eventId { get; set; }
        //  public int userId { get; set; }
        public int categoryId { get; set; }
        public string eventName { get; set; }
        public string location { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string eventDescription { get; set; }
        public string imagepath { get; set; }
        public int activityCount { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string dealtext { get; set; }
        public string percentage { get; set; }
        public bool isadvertisement { get; set; }
        public string PhoneNo { get; set; }
        public string categoryName { get; set; }
        public bool isOwner { get; set; }
        public string ratings { get; set; }
        public string nearMe { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public List<EventUser> eventUser { get; set; }
        public bool addToCalender { get; set; }

    }

    public class Activity
    {
        public int eventId { get; set; }
        public int activityId { get; set; }
        public string activityName { get; set; }
        public string location { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string activityDescription { get; set; }
        public string imagepath { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public bool isOwner { get; set; }

    }

    public class Group
    {
        public int adminId { get; set; }
        public int groupId { get; set; }
        public string groupName { get; set; }
        public string image { get; set; }
        public string tagName { get; set; }
        public string groupType { get; set; }
        public bool isActive { get; set; }
    }

    public class Advertisement
    {
        public int advertisementId { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string PhoneNo { get; set; }
        public string Discription { get; set; }
        public string Discount { get; set; }
        public string Address { get; set; }
    }

    public class EventUser
    {
        public string userId { get; set; }
        public int eventID { get; set; }
        public string userName { get; set; }
        public string imagePath { get; set; }
    }


    public class EvntPushNotification
    {
        public string eventId { get; set; }
        public string eventName { get; set; }
        public string DeviceToken { get; set; }
        public string DeviceType { get; set; }
        public string appname { get; set; }
    }


    public class NewChatPushNotification
    {
        public NewChatPushNotification()
        {
            Groupstatus = "";
            groupType = "";
            newrequest = false;
        }
        public bool isadmin { get; set; }
        public string tagname { get; set; }
        public string imagepath { get; set; }
        public string groupType { get; set; }
        public string Groupstatus { get; set; }

        public string groupId { get; set; }
        public string groupName { get; set; }
        public string DeviceToken { get; set; }
        public string DeviceType { get; set; }
        public string EventImage { get; set; }
        public string EventName { get; set; }
        public bool newrequest { get; set; }
        public string chattext { get; set; }
        public bool isevent { get; set; }
        public string appname { get; set; }
    }

    public static class convertdatatableToObject
    {
        public static User DatatabletoUser(System.Data.DataTable dt)
        {
            User _user = new User
            {
                userId = Convert.ToInt32(dt.Rows[0]["UserID"]),
                fbId = Convert.ToString(dt.Rows[0]["FbID"]),
                name = Convert.ToString(dt.Rows[0]["Name"]),
                userName = Convert.ToString(dt.Rows[0]["UserName"]),
                countryCode = Convert.ToString(dt.Rows[0]["CountryCode"]),
                phone = Convert.ToString(dt.Rows[0]["Phone"]),
                emailId = Convert.ToString(dt.Rows[0]["Email"]),
                //userImagePath = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/") + (string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["UserImagePath"])) ? "/content/Image/NoImage.png" : Convert.ToString(dt.Rows[0]["UserImagePath"])),
                userImagePath = ImagePath.GetimagePath(Convert.ToString(dt.Rows[0]["UserImagePath"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                isEnterprice = Convert.ToBoolean(dt.Rows[0]["IsEnterprice"]),
                applyEnterprice = Convert.ToBoolean(dt.Rows[0]["ApplyEnterprice"]),
                profileType = Convert.ToString(dt.Rows[0]["GroupType"]),
                isBusinessUser = Convert.ToBoolean(dt.Rows[0]["IsBusinessUser"]),
                accessToken = Convert.ToString(dt.Rows[0]["AccessToken"]),
                deviceToken = Convert.ToString(dt.Rows[0]["DeviceToken"]),
                deviceType = Convert.ToString(dt.Rows[0]["DeviceType"]),
                groupPeople = Convert.ToString(dt.Rows[0]["GroupPeople"]),
                businessLocation = Convert.ToString(dt.Rows[0]["BusinessLocation"]),
                bussinesslatitude = Convert.ToString(dt.Rows[0]["Bussinesslatitude"]),
                businesslongitude = Convert.ToString(dt.Rows[0]["Businesslongitude"]),
                isCouple = Convert.ToBoolean(dt.Rows[0]["IsCouple"]),
                isFlySolo = Convert.ToBoolean(dt.Rows[0]["IsFlySolo"]),
                isGroup = Convert.ToBoolean(dt.Rows[0]["ISGroup"]),

            };
            return _user;
        }

        public static Event DatatabletoEvent(System.Data.DataTable dt, int userid = 0)
        {
            Event _event = new Event
            {
                eventId = Convert.ToInt32(dt.Rows[0]["EventID"]),
                isOwner = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["UserID"])) ? false : Convert.ToInt32(dt.Rows[0]["UserID"]) == userid ? true : false,
                categoryId = Convert.ToInt32(dt.Rows[0]["CategoryID"]),
                eventName = Convert.ToString(dt.Rows[0]["EventName"]),
                location = Convert.ToString(dt.Rows[0]["location"]),
                startDate = Convert.ToDateTime(dt.Rows[0]["StartDate"]).ToString("MM-dd-yyyy"),
                endDate = Convert.ToDateTime(dt.Rows[0]["EndDate"]).ToString("MM-dd-yyyy"),
                eventDescription = Convert.ToString(dt.Rows[0]["EventDiscription"]),
                //imagepath = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/") + (string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["Image"])) ? "/content/Image/NoImage.png" : Convert.ToString(dt.Rows[0]["Image"])),
                imagepath = ImagePath.GetimagePath(Convert.ToString(dt.Rows[0]["Image"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                latitude = Convert.ToString(dt.Rows[0]["latitude"]),
                longitude = Convert.ToString(dt.Rows[0]["longitude"]),
                activityCount = Convert.ToInt32(dt.Rows[0]["SubEventCount"]),
                startTime = Convert.ToString(dt.Rows[0]["EventStartTime"]),
                endTime = Convert.ToString(dt.Rows[0]["EventEndTime"]),
                dealtext = Convert.ToString(dt.Rows[0]["Dealtext"]),
                percentage = Convert.ToString(dt.Rows[0]["Percentage"]),
                categoryName = Convert.ToString(dt.Rows[0]["CategoryName"]),

            };
            return _event;
        }

        public static Activity DatatabletoActivity(System.Data.DataTable dt, int userid = 0)
        {
            Activity _activity = new Activity
            {
                eventId = Convert.ToInt32(dt.Rows[0]["EventID"]),
                isOwner = Convert.ToInt32(dt.Rows[0]["UserID"]) == userid ? true : false,
                activityId = Convert.ToInt32(dt.Rows[0]["SubEventID"]),
                activityName = Convert.ToString(dt.Rows[0]["SubEventName"]),
                location = Convert.ToString(dt.Rows[0]["Location"]),
                startDate = Convert.ToDateTime(dt.Rows[0]["SubStartDate"]).ToString("MM-dd-yyyy"),
                endDate = Convert.ToDateTime(dt.Rows[0]["SubEndDate"]).ToString("MM-dd-yyyy"),
                activityDescription = Convert.ToString(dt.Rows[0]["SubEventDiscription"]),
                //imagepath = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/") + (string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["Image"])) ? "/content/Image/NoImage.png" : Convert.ToString(dt.Rows[0]["Image"])),
                imagepath = ImagePath.GetimagePath(Convert.ToString(dt.Rows[0]["Image"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                startTime = Convert.ToString(dt.Rows[0]["ActivityStartTime"]),
                endTime = Convert.ToString(dt.Rows[0]["ActivityEndTime"]),
                latitude = Convert.ToString(dt.Rows[0]["latitude"]),
                longitude = Convert.ToString(dt.Rows[0]["longitude"])

            };
            return _activity;
        }
    }
}