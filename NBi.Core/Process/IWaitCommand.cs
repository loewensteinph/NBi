﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBi.Core.Process;

namespace NBi.Core.Process
{
    public interface IWaitCommand : IProcessCommand
    {
        int MilliSeconds { get; set; }
    }
}
