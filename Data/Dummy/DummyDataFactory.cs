using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uul_api.Models;

namespace uul_api.Data.Dummy {
    public class DummyDataFactory {
        public static void CreateDummyData(UULContext context) {
       
            for (int i = 0; i < 20; i++) {
                var user = new NewUserDTO() { Login = "Login" + i, AvatarSrc = "Default", Name = "Inhabitant_" + i, ApartmentCode = "T" + i, Pwd = "12345" };
                UserDao.AddFromDto(context, user);
            }
            context.SaveChanges();
            List<TimeSlot> timeSlots = new();
            var today = DateTime.UtcNow;

            for (int i = 0; i < 3; i++) {
                var slots = TimeSlotsFactory.CreateTimeSlotsForDateUTC(context, new DateTime(today.Year, today.Month, today.Day - i).ToUniversalTime(), 5);
                slots.Wait();
                timeSlots.AddRange(slots.Result);
            }
            var habitants = context.Habitants.ToList();
            var size = habitants.Count;
            var rnd = new Random();
            foreach (TimeSlot timeSlot in timeSlots) {
                for(int i = 0; i < rnd.Next(4); i++) {
                    if (timeSlot.OccupiedBy == null) { timeSlot.OccupiedBy = new List<Habitant>(); }
                    timeSlot.OccupiedBy.Add(habitants.ElementAt(rnd.Next(size)));
                }
            }
            context.TimeSlots.AddRange(timeSlots);

            var newsList = new List<News>();
            for (int i = 0; i < 5; i ++) {
                newsList.Add(new News() { Title = "Title " + i, Content = " Content " + i + dummyContent, Author = "Dummy data generator", CreatedAt = DateTime.UtcNow, Auditory = Auditory.GUESTS, NewsType = NewsType.INFO });
            }
            newsList.Add(new News() { Title = "Title Registered", Content = " Content Registered", Author = "Dummy data generator", CreatedAt = DateTime.UtcNow, Auditory = Auditory.REGISTERED, NewsType = NewsType.CALL_TO_ACTION });
            newsList.Add(new News() { Title = "Title Activated", Content = " Content Activated", Author = "Dummy data generator", CreatedAt = DateTime.UtcNow, Auditory = Auditory.ACTIVATED, NewsType = NewsType.ALERT });
            context.News.AddRange(newsList);
            context.SaveChanges();

        }

        private static string dummyContent = @"'
Call us 3325937557 +523325937557
https://google.com
uul-web.exe' (CoreCLR: clrhost): Loaded 'C:\Program Files\dotnet\shared\Microsoft.NETCore.App\5.0.3\System.Web.HttpUtility.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
System.Net.Http.HttpClient.UsersClient.LogicalHandler: Information: Start processing HTTP request POST https://192.168.100.8:5001/api/webusers/update
System.Net.Http.HttpClient.UsersClient.ClientHandler: Information: Sending HTTP request POST https://192.168.100.8:5001/api/webusers/update
System.Net.Http.HttpClient.UsersClient.ClientHandler: Information: Received HTTP response headers after 17.2348ms - 200
System.Net.Http.HttpClient.UsersClient.LogicalHandler: Information: End processing HTTP request after 20.7377ms - 200
System.Net.Http.HttpClient.UsersClient.LogicalHandler: Information: Start processing HTTP request GET https://192.168.100.8:5001/api/webusers/list
System.Net.Http.HttpClient.UsersClient.ClientHandler: Information: Sending HTTP request GET https://192.168.100.8:5001/api/webusers/list";
    }
}
