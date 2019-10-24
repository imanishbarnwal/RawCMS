﻿//******************************************************************************
// <copyright file="license.md" company="RawCMS project  (https://github.com/arduosoft/RawCMS)">
// Copyright (c) 2019 RawCMS project  (https://github.com/arduosoft/RawCMS)
// RawCMS project is released under GPL3 terms, see LICENSE file on repository root at  https://github.com/arduosoft/RawCMS .
// </copyright>
// <author>Daniele Fontani, Emanuele Bucarelli, Francesco Mina'</author>
// <autogenerated>true</autogenerated>
//******************************************************************************
using System.Collections.Generic;

namespace RawCMS.Plugins.ApiGateway.Classes.Settings
{
    public class BalancerData
    {
        public Dictionary<int, long> Scores { get; set; } = new Dictionary<int, long>();
        public long LastServed { get; set; }
    }
}