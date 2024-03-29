﻿using System;
using System.Collections.Generic;

namespace Trident.IoC
{
    /// <summary>
    /// Configuration adapter for <see cref="IoCChildLifetimeScopeServiceProviderFactory" />.
    /// </summary>
    public class IoCChildLifetimeScopeConfigurationAdapter
    {
        private readonly List<Action<IIoCProvider>> _configurationActions = new List<Action<IIoCProvider>>();

        /// <summary>
        /// Adds a configuration action that will be executed when the child <see cref="ILifetimeScope"/> is created.
        /// </summary>
        /// <param name="configurationAction">Action on a <see cref="ContainerBuilder"/> that adds component registrations to the container.</param>
        /// <exception cref="ArgumentNullException">Throws when the passed configuration-action is null.</exception>
        public void Add(Action<IIoCProvider> configurationAction)
        {
            if (configurationAction == null) throw new ArgumentNullException(nameof(configurationAction));

            _configurationActions.Add(configurationAction);
        }

        /// <summary>
        /// Gets the list of configuration actions to be executed on the <see cref="ContainerBuilder"/> for the child <see cref="ILifetimeScope"/>.
        /// </summary>
        public IReadOnlyList<Action<IIoCProvider>> ConfigurationActions => _configurationActions;
    }
}
