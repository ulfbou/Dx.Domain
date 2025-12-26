// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DeterministicHash.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;

namespace Dx.Domain.Generators.Common
{
    public static class DeterministicHash
    {
        public static string Compute(string payload)
        {
            // Normalize to Unix Line Endings for cross-platform stability
            var normalized = payload.Replace("\r\n", "\n");
            var bytes = Encoding.UTF8.GetBytes(normalized);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}
