﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationWS.Utils.Interfaces
{
    public interface IReadGpTables
    {
        Task Run();
        Task ReadTables();
    }
}
