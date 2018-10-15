using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Myvas.Extensions.Configuration.EntityFrameworkCore
{
    public class EfConfigurationSource: IConfigurationSource
    {
        public Action<DbContextOptionsBuilder> OptionsAction { get; }
        public IDictionary<string, string> InitialData { get; set; }

        public EfConfigurationSource(Action<DbContextOptionsBuilder> optionsAction)
        {
            OptionsAction = optionsAction;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new EfConfigurationProvider(this);
        }
    }
}
