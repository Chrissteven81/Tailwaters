using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using HtmlAgilityPack;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace TailWaters.MicroServices.Operators
{
    public static class SouthwesternPowerAdministration
    {
        private const string _url = "https://swpa.gov/gen";

        [FunctionName("SouthwesternPowerAdministration")]
        public static async Task Run([TimerTrigger("0 0 0 * * *", RunOnStartup = true)]TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            var configuration = new Application(context)
                                .Configuration;

            //Get Day of the Week
            var day = DateTime.Today.DayOfWeek.ToString();

            //Retrieve HTM Doc
            var html = new HtmlDocument();
            html.LoadHtml(new WebClient().DownloadString($"{_url}/{day.Substring(0, 3)}.htm"));

            //Find Pertenant Node
            var grid =  html.DocumentNode.Descendants("Pre").Single();
            var lines = grid.InnerHtml.Split("\n").Where(t=> t.Trim() != string.Empty).Skip(3).Take(25).ToArray();

            //Build Context Options
            var optionBuilder = new DbContextOptionsBuilder<Data.Models.TailWatersContext>();
            optionBuilder.UseSqlServer(configuration.GetConnectionString("TailWaters"));

            using (var dbContext = new TailWaters.Data.Models.TailWatersContext(optionBuilder.Options))
            {
                //Build Cache so we only have to make 2 queries
                var tailWatersCache = await dbContext.TailWaters.ToListAsync();
                var scheduleCache = await dbContext.Schedules.Where(t=> t.DateCreated.Date == DateTime.UtcNow.Date).ToListAsync();

                //Parse Header Data
                var headers = lines.Take(1).Single().Split(" ").Where(t=> t.Trim() != string.Empty).ToArray();
                for (int i = 1; i < headers.Count(); i++)
                {
                   
                    var header = headers[i];
                    var tailWater = tailWatersCache.SingleOrDefault(t => t.Acronym == header);

                    //Create Tailwater if it does not already exist(they may add a new one)
                    if(tailWater == null)
                    {
                        tailWater = new Data.Models.TailWater()
                        {
                            OperatorId = 1,
                            Acronym = header,
                            Name = header,
                            MaxFlow = 0
                        };
                        await dbContext.TailWaters.AddAsync(tailWater);
                    }

                    //Pivot data and push to database
                    foreach (var line in lines.Skip(1))
                    {
                        var cols = line.Split(" ").Where(t => t.Trim() != string.Empty).ToArray();

                        var key = int.Parse(cols[0]);
                        var value = double.Parse(cols[i]);

                        //Check for existing Schedule
                        var schedule = scheduleCache.Where(t => t.TailWater.Acronym == header && t.Hour == key).SingleOrDefault();
                        if(schedule == null)
                        {
                            tailWater.Schedules.Add(new Data.Models.Schedule()
                            {
                                Hour = key,
                                ProjectedFlow = value,
                                DateCreated = DateTime.UtcNow
                            });
                        }
                        else
                            schedule.ProjectedFlow = value;

                        //Update Max Flow. for now I am going to try to do it like this, later i want to make this a machine learning variable
                        if (value > tailWater.MaxFlow)
                            tailWater.MaxFlow = value;
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
