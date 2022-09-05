﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Microsoft.PowerFx.Core.IR;

namespace Microsoft.PowerFx.Types
{
    // Represent record backed by known list of values. 
    internal class InMemoryRecordValue : RecordValue
    {
        private readonly IReadOnlyDictionary<string, FormulaValue> _fields;
        private readonly IDictionary<string, FormulaValue> _mutableFields;

        public InMemoryRecordValue(IRContext irContext, IEnumerable<NamedValue> fields)
          : this(irContext, ToDict(fields))
        {
        }

        public InMemoryRecordValue(IRContext irContext, IReadOnlyDictionary<string, FormulaValue> fields)
            : base(irContext)
        {
            Contract.Assert(IRContext.ResultType is RecordType);

            _fields = fields;
            _mutableFields = fields as IDictionary<string, FormulaValue>;

            if (_mutableFields.IsReadOnly)
            {
                _mutableFields = null;
            }
        }

        private static IReadOnlyDictionary<string, FormulaValue> ToDict(IEnumerable<NamedValue> fields)
        {
            var dict = new Dictionary<string, FormulaValue>(StringComparer.Ordinal);
            foreach (var field in fields)
            {
                dict[field.Name] = field.Value;
            }

            return dict;
        }

        protected override bool TryGetField(FormulaType fieldType, string fieldName, out FormulaValue result)
        {
            return _fields.TryGetValue(fieldName, out result);
        }

        public override async Task<DValue<RecordValue>> UpdateFieldsAsync(RecordValue record)
        {
            if (_mutableFields == null)
            {
                return await base.UpdateFieldsAsync(record);
            }

            var fields = new List<NamedValue>();

            foreach (var field in record.Fields)
            {
                _mutableFields[field.Name] = field.Value;
            }

            foreach (var kvp in _fields)
            {
                fields.Add(new NamedValue(kvp.Key, kvp.Value));
            }

            return DValue<RecordValue>.Of(NewRecordFromFields(fields));
        }
    }
}
