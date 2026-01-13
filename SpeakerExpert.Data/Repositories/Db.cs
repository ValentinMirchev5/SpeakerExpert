using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace SpeakerExpert.Data.Repositories
{
    public class Db
    {
        public string ConnectionString { get; }

        public Db(IConfiguration config)
        {
            ConnectionString = config.GetConnectionString("SpeakerExpertDb")
                ?? throw new Exception("Missing connection string: SpeakerExpertDb");
        }
    }
}

