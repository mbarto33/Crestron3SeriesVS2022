﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrestronModule.Core
{
    public interface IOutput<T>
    {
        T Value { get; set; }
    }
}
