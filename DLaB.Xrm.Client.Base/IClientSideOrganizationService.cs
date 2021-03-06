﻿using Microsoft.Xrm.Sdk;
using System;
using DLaB.Xrm;

namespace DLaB.Xrm.Client
{
    /// <summary>
    /// A Disposible service that allows for getting the Service Uri.
    /// </summary>
    public interface IClientSideOrganizationService : IOrganizationService, IDisposable
    {
        /// <summary>
        /// Returns an Uri for the Organization Service
        /// </summary>
        /// <returns></returns>
        Uri GetServiceUri();
    }
}
