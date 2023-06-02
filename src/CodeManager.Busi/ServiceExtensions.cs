using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeManager.Busi
{
    public static class ServiceExtensions
    {
        public static void AddBusiService(this IServiceCollection service)
        {
            service.AddTransient<BllSysDb>();
        }
    }
}
