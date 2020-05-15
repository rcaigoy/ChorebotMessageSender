using System;
using System.Collections.Generic;
using System.Text;
//using System.Threading;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.SqlServer;
using System.Linq;
using System.Net;
using System.Net.Http;

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

//added project
using ChorebotMessageSender.Entities;

namespace ChorebotMessageSender
{
    public class Scheduler
    {
        bool DBInUse;
        System.Timers.Timer ScheduleTimer;
        System.Timers.Timer CallTimer;

        TimeSpan ScheduleInterval;
        TimeSpan SenderInterval;

        //private ChorebotSchedulerContext db;

        public Scheduler()
        {
            Console.WriteLine("Starting Program:  " + DateTime.Now);
            
            DBInUse = false;

            ScheduleInterval = new TimeSpan(0, 10, 0);

            //run once at beginning of start
            SetSchedule(null, null);
            //make scheduler
            //schedule every 30 minutes
            //this.ScheduleTimer = new Timer(SetSchedule, null, 0, (int)ScheduleInterval.TotalMilliseconds/2);
            this.ScheduleTimer = new System.Timers.Timer((int)ScheduleInterval.TotalMilliseconds / 2);
            this.ScheduleTimer.Elapsed += new ElapsedEventHandler(SetSchedule);
            this.ScheduleTimer.Start();
            this.ScheduleTimer.AutoReset = true;

            //initialize sender interval
            SenderInterval = new TimeSpan(0, 1, 0);

            //wait one minute before processing calls after schedule initially starts
            Thread.Sleep((int)SenderInterval.TotalMilliseconds);

            //send once at beginning
            SendCalls(null, null);

            //make caller every 30 seconds
            //this.CallTimer = new System.Timers.Timer(SendCalls, null, 0, 60000);
            this.CallTimer = new System.Timers.Timer((int)SenderInterval.TotalMilliseconds);
            this.CallTimer.Elapsed += new ElapsedEventHandler(SendCalls);
            this.CallTimer.Start();
            this.CallTimer.AutoReset = true;
        }


        private async void SetSchedule(Object o, ElapsedEventArgs e)
        {
            using (var db = new ChorebotSchedulerContext())
            {
                Console.WriteLine("Set Schedule " + DateTime.Now);
                var ActiveList = db.Calls.Where(x => x.IsActive && x.DateStart < DateTime.Now).ToList();

                ////minute and hour interval type case
                //foreach (var call in ActiveList.Where(x => x.IntervalTypeId <= 2))
                //{
                //    //check if active call exists for after right now already
                //    if (db.ScheduledTasks.Count(x => x.CallId == call.CallId && x.DateScheduled > DateTime.Now) == 0)
                //    {
                //        DateTime TempDateTime = DateTime.Now;

                //        //check if now's time is after datestart time
                //        if (DateTime.Now.TimeOfDay > call.DateStart.TimeOfDay)
                //        {
                //            TempDateTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, call.DateStart.Hour, call.DateStart.Minute, 0);

                //        } //end if if (DateTime.Now.TimeOfDay > call.DateStart.TimeOfDay)

                //        else
                //        {
                //            TempDateTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.AddDays(-1).Day, call.DateStart.Hour, call.DateStart.Minute, 0);
                //        }


                //        //find each scheduled task for each next 30 minutes
                //        while (TempDateTime <= DateTime.Now + ScheduleInterval)
                //        {
                //            if (TempDateTime > DateTime.Now)
                //            {
                //                ScheduledTasks TempScheduledTask = new ScheduledTasks();

                //                //public int CallId { get; set; }
                //                TempScheduledTask.CallId = call.CallId;

                //                TempScheduledTask.DateScheduled = TempDateTime;
                //                TempScheduledTask.IsSent = false;

                //                while (DBInUse)
                //                {
                //                    Console.WriteLine("DB In Use " + DateTime.Now);
                //                    Thread.Sleep(1000);
                //                }
                //                Console.WriteLine("Setting Schedule " + DateTime.Now);
                //                DBInUse = true;
                //                db.ScheduledTasks.Add(TempScheduledTask);
                //                db.SaveChanges();
                //                this.DBInUse = false;
                //            }

                //                //choose adding algorithm
                //                if (call.IntervalTypeId == 1)
                //                {
                //                    TempDateTime = TempDateTime.AddMinutes(call.Interval);
                //                }

                //                if (call.IntervalTypeId == 2)
                //                {
                //                    TempDateTime = TempDateTime.AddHours(call.Interval);
                //                }

                //                //if (call.IntervalTypeId == 3)
                //                //{
                //                //    TempDateTime = TempDateTime.AddDays(call.Interval);
                //                //}

                //                //if (call.IntervalTypeId == 4)
                //                //{
                //                //    TempDateTime = TempDateTime.AddMonths(call.Interval);
                //                //}
                            

                //        }// while (TempDateTime <= DateTime.Now + ScheduleInterval)

                //    }//end if (db.ScheduledTasks.Count(x => x.CallId == call.Interval && x.DateScheduled > DateTime.Now) == 0)

                //}
                ////end minute or hour condition

                //day contidion
                foreach (var call in ActiveList)
                {
                    //check if active call exists for after right now already
                    if (db.ScheduledTasks.Count(x => x.CallId == call.CallId && x.DateScheduled > DateTime.Now) == 0)
                    {
                        
                        DateTime TempDateTime = DateTime.Now + ScheduleInterval;
                        
                        //find starting date to start looking
                        var TempDateTimeQuery = db.ScheduledTasks.OrderByDescending(x => x.DateScheduled).FirstOrDefault(x => x.CallId == call.CallId);

                        if (TempDateTimeQuery == null)
                        {
                            TempDateTime = call.DateStart;
                        }
                        else
                        {
                            TempDateTime = TempDateTimeQuery.DateScheduled;
                        }

                        //find each scheduled task for each next 30 minutes
                        while (TempDateTime <= DateTime.Now + ScheduleInterval)
                        {
                            if (TempDateTime > DateTime.Now)
                            {
                                ScheduledTasks TempScheduledTask = new ScheduledTasks();

                                //public int CallId { get; set; }
                                TempScheduledTask.CallId = call.CallId;

                                TempScheduledTask.DateScheduled = TempDateTime;
                                TempScheduledTask.IsSent = false;

                                while (DBInUse)
                                {
                                    Console.WriteLine("DB In Use " + DateTime.Now);
                                    Thread.Sleep(1000);
                                }
                                Console.WriteLine("Setting Schedule " + DateTime.Now);
                                DBInUse = true;
                                db.ScheduledTasks.Add(TempScheduledTask);
                                db.SaveChanges();
                                this.DBInUse = false;
                            }

                            //choose adding algorithm
                            if (call.IntervalTypeId == 1)
                            {
                                TempDateTime = TempDateTime.AddMinutes(call.Interval);
                            }

                            if (call.IntervalTypeId == 2)
                            {
                                TempDateTime = TempDateTime.AddHours(call.Interval);
                            }

                            if (call.IntervalTypeId == 3)
                            {
                                TempDateTime = TempDateTime.AddDays(call.Interval);
                            }

                            if (call.IntervalTypeId == 4)
                            {
                                TempDateTime = TempDateTime.AddMonths(call.Interval);
                            }


                        }// while (TempDateTime <= DateTime.Now + ScheduleInterval)

                    }
                }


            }

        }


        private async void SendCalls(Object o, ElapsedEventArgs e)
        {
            Console.WriteLine("Sending call " + DateTime.Now);
            try
            {
                using (var db = new ChorebotSchedulerContext())
                {
                    // Display the date/time when this method got called.
                    Console.WriteLine("Sending Calls: " + DateTime.Now);

                    //get each scheduled task that isn't complete
                    foreach (var ScheduledTask in db.ScheduledTasks.Where(x => !x.IsSent && x.DateScheduled <= DateTime.Now))
                    {
                        try
                        {
                            Calls Call = db.Calls.FirstOrDefault(x => x.CallId == ScheduledTask.CallId);
                            SendCall(ScheduledTask);
                        }
                        catch (Exception ex)
                        {
                            AlertAdmin("Level 1 for scheduled task " + ScheduledTask.ScheduledTaskId, ex);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                AlertAdmin("Level 2", ex);
            }

            // Force a garbage collection to occur for this demo.
            GC.Collect();
        }


        private void SendCall(ScheduledTasks ScheduledTask)
        {
            try
            {
                using (var db = new ChorebotSchedulerContext())
                {
                    var Call = db.Calls.FirstOrDefault(x => x.CallId == ScheduledTask.CallId);

                    string UrlRequest = Call.Url;

                    var QueryStringsQuery = db.QueryStrings.Where(x => x.CallId == Call.CallId && x.IsActive);

                    if (QueryStringsQuery != null && QueryStringsQuery.Count() > 0)
                    {
                        var QueryStringList = QueryStringsQuery.ToList();
                        UrlRequest += "?";

                        //get first
                        UrlRequest += QueryStringList[0].KeyString + "=" + QueryStringList[0].ValueString;

                        for (int i = 1; i < QueryStringList.Count; i++)
                        {
                            UrlRequest += "&";
                            UrlRequest += QueryStringList[i] + "=" + QueryStringList[i].ValueString;
                        }
                    }

                    var client = new HttpClient();
                    client.BaseAddress = new Uri(UrlRequest);

                    //Task<HttpResponseMessage> response = null;
                    HttpResponseMessage response = null;

                    switch (Call.CallType)
                    {
                        case "get":
                            Console.WriteLine("Starting:  " + Call.Url);
                            Task.Run(async () =>
                            {
                            response = await client.GetAsync(UrlRequest);
                                if (response == null)
                                {
                                    AlertAdmin("null response", new Exception(Call.Url));
                                }
                                else if (response.StatusCode != HttpStatusCode.OK)
                                {
                                    AlertAdmin(response.StatusCode.ToString(), new Exception(response.Content.ToString()));
                                }
                                else
                                {
                                    CompleteScheduledTask(ScheduledTask.ScheduledTaskId);
                                }
                            }); //end Task.Run(()

                            break;

                        case "post":
                            Console.WriteLine("Starting:  " + Call.Url);
                            Task.Run(async () =>
                            {
                                response = await client.PostAsync(UrlRequest, null);
                                if (response == null)
                                {
                                    AlertAdmin("null response", new Exception(Call.Url));
                                }
                                else if (response.StatusCode != HttpStatusCode.OK)
                                {
                                    AlertAdmin(response.StatusCode.ToString(), new Exception(response.Content.ToString()));
                                }
                                else
                                {
                                    CompleteScheduledTask(ScheduledTask.ScheduledTaskId);
                                }
                            }); //end Task.Run(()

                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                AlertAdmin("Send call issue", ex, ScheduledTask.ScheduledTaskId);
            }
        }


        private void CompleteScheduledTask(int ScheduledTaskId)
        {
            try
            {
                using (var db = new ChorebotSchedulerContext())
                {
                    while (DBInUse)
                    {
                        Thread.Sleep(1000);
                        Console.WriteLine("Db in use " + DateTime.Now);
                    }
                    this.DBInUse = true;
                    var ScheduledTask = db.ScheduledTasks.FirstOrDefault(x => x.ScheduledTaskId == ScheduledTaskId);
                    ScheduledTask.IsSent = true;
                    ScheduledTask.DateSent = DateTime.Now;
                    db.SaveChanges();

                    var Call = db.Calls.FirstOrDefault(x => x.CallId == ScheduledTask.CallId);

                    Console.WriteLine("Completed Scheduled Task " + ScheduledTaskId + ":  " +  Call.Url);

                    this.DBInUse = false;
                }
            }
            catch (Exception ex)
            {
                AlertAdmin("database write issue", ex);
            }

        }




        private void AlertAdmin(string Level, Exception ex = null,int? ScheduledTaskId = null)
        {
            try
            {
                using (var db = new ChorebotSchedulerContext())
                {
                    string Message = string.Empty;

                    if (ex != null)
                        Message = ex.Message + ex.StackTrace;

                    Console.WriteLine(Level + ":  " + Message);

                    if (ScheduledTaskId != null)
                    {
                        while (DBInUse)
                        {
                            Console.WriteLine("Db in use " + DateTime.Now);
                            Thread.Sleep(1000);
                        }
                        Console.WriteLine("Starting db change " + DateTime.Now);

                        DBInUse = true;
                        var ScheduledTask = db.ScheduledTasks.FirstOrDefault(x => x.ScheduledTaskId == ScheduledTaskId);
                        ScheduledTask.ErrorLog = Level + ":  " + Message;
                        db.SaveChanges();

                        DBInUse = false;
                    }
                }
            }
            catch (Exception ex2)
            {
                //log?
            }
        }

    }
}
