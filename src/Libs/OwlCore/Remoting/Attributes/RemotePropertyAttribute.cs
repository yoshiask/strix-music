﻿using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Cauldron.Interception;
using OwlCore.Remoting.EventArgs;

namespace OwlCore.Remoting.Attributes
{
    /// <summary>
    /// Attribute used in conjuction with <see cref="MemberRemote"/>.
    /// Mark a property with this attribute to opt into remote method changes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class RemotePropertyAttribute : Attribute, IPropertySetterInterceptor
    {
        /// <inheritdoc/>
        public bool OnException(Exception e)
        {
            ExceptionRaised?.Invoke(this, e);
            return true;
        }

        /// <inheritdoc/>
        public void OnExit()
        {
            Exited?.Invoke(this, System.EventArgs.Empty);
        }

        /// <inheritdoc/>
        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            // Check if the setter was the OwlCore.Remoting library.
            // Don't re-emit "set" to the library if so.
            lock (MemberRemote.MemberHandleExpectancyMap)
            {
                if (MemberRemote.MemberHandleExpectancyMap.TryGetValue(Thread.CurrentThread.ManagedThreadId, out var expectedInstance) && expectedInstance == propertyInterceptionInfo.Instance)
                {
                    MemberRemote.MemberHandleExpectancyMap.Remove(Thread.CurrentThread.ManagedThreadId);
                    return false;
                }
            }

            var args = new PropertySetEnteredEventArgs(propertyInterceptionInfo, oldValue, newValue);
            SetEntered?.Invoke(this, args);

            return false;
        }

        /// <summary>
        /// Raised when the property is set.
        /// </summary>
        /// <remarks>
        /// This static needed because the <see cref="IPropertySetterInterceptor"/> weaver removes the attribute from the method in IL, making it innaccessible through normal means.
        /// Since this event emits the same instance we pass to <see cref="MemberRemote"/>, we can still use this.
        /// </remarks>
        public static event EventHandler<PropertySetEnteredEventArgs>? SetEntered;

        /// <summary>
        /// Raised when the property is finished setting and is about to exit.
        /// </summary>
        /// <remarks>
        /// This static needed because the <see cref="IPropertySetterInterceptor"/> weaver removes the attribute from the method in IL, making it innaccessible through normal means.
        /// Since this event emits the same instance we pass to <see cref="MemberRemote"/>, we can still use this.
        /// </remarks>
        public static event EventHandler? Exited;

        /// <summary>
        /// Raised when an exception occurs while setting the property.
        /// </summary>
        /// <remarks>
        /// This static needed because the <see cref="IPropertySetterInterceptor"/> weaver removes the attribute from the method in IL, making it innaccessible through normal means.
        /// Since this event emits the same instance we pass to <see cref="MemberRemote"/>, we can still use this.
        /// </remarks>
        public static event EventHandler<Exception>? ExceptionRaised;
    }
}
