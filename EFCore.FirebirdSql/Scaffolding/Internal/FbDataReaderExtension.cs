// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data.Common;

namespace Microsoft.EntityFrameworkCore.Scaffolding.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public static class FbDataReaderExtension
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static T GetValueOrDefault<T>(this DbDataReader reader,string name)
        {
            var idx = reader.GetOrdinal(name);
            return reader.IsDBNull(idx)
                ? default
                : (T)GetValue<T>(reader.GetValue(idx));
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static T GetValueOrDefault<T>(this DbDataRecord record,string name)
        {
            var idx = record.GetOrdinal(name);
            return record.IsDBNull(idx)
                ? default
                : (T)GetValue<T>(record.GetValue(idx));
        }

        private static object GetValue<T>(object valueRecord)
        {
            switch (typeof(T).Name)
            {
                case nameof(Int32):
                    return Convert.ToInt32(valueRecord);
                case nameof(Boolean):
                    return Convert.ToBoolean(valueRecord);
                default:
                    return valueRecord;
            }
        }
    }
}
