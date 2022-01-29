using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoDomain.Sql.Context;

namespace ToDoWebApp.Api.Common
{
    public class BaseController: ControllerBase
    {
        internal readonly ToDoDbContext ToDoDbContext;

        public BaseController(ToDoDbContext dbContext)
        {
            ToDoDbContext = dbContext;

        }
    }
}
