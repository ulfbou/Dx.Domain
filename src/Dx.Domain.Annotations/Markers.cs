// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Markers.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain.Annotations
{
    public interface IAggregateRoot { }

    public interface IEntity { }

    public interface IValueObject { }

    public interface IDomainEvent { }

    public interface IDomainPolicy { }

    public interface IDomainFactory { }

    /// <summary>
    /// Semantic marker interface for Dx.Domain identity vocabulary (pure metadata).
    /// </summary>
    /// <remarks>
    /// This interface imposes no runtime semantics. It carries no members or default implementations;
    /// used by analyzers and Kernel to classify identity intent.
    /// SEE: Kernel Specification → Identity Primitives.
    /// </remarks>
    public interface IIdentity { }
}
