using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using TailWaters.Data.Models;

namespace TailWaters.MicroServices
{
    public class Application
    {
        public IConfiguration Configuration { get; }

        public DbContextOptions<TailWatersContext> ContextOptions { get; set; }

        public Application(ExecutionContext context)
        {
            //Initialize Configuration for ASP.net Core v2
            Configuration = new ConfigurationBuilder()
                        .SetBasePath(context.FunctionAppDirectory)
                        .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .Build();

            //Build Context Options
            var optionBuilder = new DbContextOptionsBuilder<Data.Models.TailWatersContext>();
            optionBuilder.UseSqlServer(Configuration.GetConnectionString("TailWaters"));
            ContextOptions = optionBuilder.Options;
        }
    }
}
