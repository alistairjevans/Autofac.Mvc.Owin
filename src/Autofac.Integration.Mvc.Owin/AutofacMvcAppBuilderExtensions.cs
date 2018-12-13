﻿// This software is part of the Autofac IoC container
// Copyright © 2014 Autofac Contributors
// https://autofac.org
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using Autofac;
using Autofac.Integration.Owin;

namespace Owin
{
    /// <summary>
    /// Extension methods for configuring the OWIN pipeline.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class AutofacMvcAppBuilderExtensions
    {
        internal static Func<HttpContextBase> CurrentHttpContext = () => new HttpContextWrapper(HttpContext.Current);

        /// <summary>
        /// Extends the Autofac lifetime scope added from the OWIN pipeline through to the MVC request lifetime scope.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <returns>The application builder.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static IAppBuilder UseAutofacMvc(this IAppBuilder app)
        {
            return app.Use(async (context, next) =>
            {
                var lifetimeScope = context.GetAutofacLifetimeScope();
                var httpContext = CurrentHttpContext();

                if (lifetimeScope != null && httpContext != null)
                    httpContext.Items[typeof(ILifetimeScope)] = lifetimeScope;

                await next();
            });
        }
    }
}
