// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Dx.Domain.Facts;

/// <summary>
/// Extensions for working with domain facts.
/// </summary>
public static class DomainFactExtensions
{
    /// <summary>
    /// Tries to extract a strongly typed payload from the specified fact.
    /// </summary>
    /// <typeparam name="TPayload">The expected payload type.</typeparam>
    /// <param name="fact">The fact to inspect.</param>
    /// <param name="payload">When this method returns, contains the payload if found; otherwise, the default value.</param>
    /// <returns><see langword="true"/> if the payload was extracted; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetPayload<TPayload>(this IDomainFact fact, out TPayload? payload)
    {
        ArgumentNullException.ThrowIfNull(fact);

        if (fact is IDomainFact<TPayload> typed)
        {
            payload = typed.GetPayload();
            return true;
        }

        if (fact.GetPayload() is TPayload value)
        {
            payload = value;
            return true;
        }

        payload = default;
        return false;
    }
}
