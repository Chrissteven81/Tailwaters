using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.TwiML;
using System.Net.Http;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace TailWaters.MicroServices.Actions
{
    public static class Subscribe
    {
        [FunctionName("Subscribe")]
        //[return: TwilioSms(AccountSidSetting = "AC88d3904a49278c9b1d316cf637e93825", AuthTokenSetting = "TwilioAuthToken", From = "+15012296047")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log, ExecutionContext context)
        {
            var application = new Application(context);

            var data = await req.ReadAsStringAsync();
            var message = Models.TwilioSMSMessage.Parse(data);

            string responseText;
            using (var dbContext = new TailWaters.Data.Models.TailWatersContext(application.ContextOptions))
            {
                //Lookup Subscriber
                var subscriber = await dbContext.Subscribers.Where(t => t.Number == message.From).SingleOrDefaultAsync();

                //Create Subscriber if they do not already exist
                if (subscriber == null)
                {
                    subscriber = new Data.Models.Subscriber()
                    {
                        City = message.FromCity,
                        State = message.FromState,
                        Country = message.FromCountry,
                        Number = message.From,
                        Zip = message.FromZip,
                        DateCreated = DateTime.UtcNow
                    };
                    
                    dbContext.Subscribers.Add(subscriber);
                    await dbContext.SaveChangesAsync();

                    responseText = "You are now subscribed to Tail Waters";
                }
                else
                    responseText = "You have already subscribed.";
            }

            var response = new MessagingResponse()
            .Message(responseText);

            var twiml = response.ToString();
            twiml = twiml.Replace("utf-16", "utf-8");

            return new HttpResponseMessage
            {
                Content = new StringContent(twiml, Encoding.UTF8, "application/xml")
            };
        }
    }
}
