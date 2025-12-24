// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="StructuralComparer.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System.Collections;

namespace Dx.Domain.Generators.Core
{
    public static class StructuralComparer
    {
        public static bool StructurallyEqual(object? x, object? y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (x is null || y is null)
                return false;

            if (x is IEnumerable ex && y is IEnumerable ey)
            {
                if (x.GetType() != y.GetType())
                    return false;

                var ix = ex.GetEnumerator();
                var iy = ey.GetEnumerator();

                while (true)
                {
                    var mx = ix.MoveNext();
                    var my = iy.MoveNext();

                    if (mx != my)
                        return false;
                    if (!mx)
                        return true;

                    if (!StructurallyEqual(ix.Current, iy.Current))
                        return false;
                }
            }

            return x.Equals(y);
        }
    }
}
