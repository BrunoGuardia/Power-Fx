﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.PowerFx
{
    /// <summary>
    /// Provide settings to FormulaValues in order to serialize values in different situations.
    /// </summary>
    public class FormulaValueSerializerSettings
    {
        /// <summary>
        /// Provides a human-friendly representation of a FormulaValue value.
        /// </summary>
        public bool UseCompactRepresentation { get; set; }
    }
}
