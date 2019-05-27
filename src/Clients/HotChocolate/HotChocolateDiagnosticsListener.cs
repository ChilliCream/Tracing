﻿using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Execution.Instrumentation;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DiagnosticAdapter;

namespace Thor.Extensions.HotChocolate
{
    public class HotChocolateDiagnosticsListener
        : IDiagnosticObserver
    {
        public HotChocolateDiagnosticsListener()
        {

        }

        [DiagnosticName("HotChocolate.Execution.Query")]
        public void QueryExecute()
        {
            // This method is required to enable recording "Query.Start" and
            // "Query.Stop" diagnostic events. Do not write code in here.
        }

        [DiagnosticName("HotChocolate.Execution.Query.Start")]
        public void BeginQueryExecute(IQueryContext context)
        {
            HotChocolateRequest request = new HotChocolateRequest
            {
                Query = context.Request.Query,
                OperationName = context.Request.OperationName,
                VariableValues = context.Request.VariableValues
            };

            context.ContextData[nameof(HotChocolateRequest)] = request;

            HttpContext httpContext = context.GetHttpContext();
            HotChocolateActivity activity =
                HotChocolateActivity.Create(request);

            httpContext.Features.Set(activity);
        }

        [DiagnosticName("HotChocolate.Execution.Query.Stop")]
        public void EndQueryExecute(
            IQueryContext context,
            IExecutionResult result)
        {
            context
                .GetHttpContext()
                .Features
                .Get<HotChocolateActivity>()
                ?.Dispose();
        }

        [DiagnosticName("HotChocolate.Execution.Query.Error")]
        public virtual void OnQueryError(
            IQueryContext context,
            Exception exception)
        {
            context
                .GetHttpContext()
                .Features
                .Get<HotChocolateActivity>()
                ?.HandleQueryError(exception);
        }

        [DiagnosticName("HotChocolate.Execution.Resolver.Error")]
        public virtual void OnResolverError(
            IResolverContext context,
            IEnumerable<IError> errors)
        {
            context
                .GetHttpContext()
                .Features
                .Get<HotChocolateActivity>()
                ?.HandlesResolverErrors(
                    TryGetRequest(context.ContextData),
                    errors.ToList());
        }

        [DiagnosticName("HotChocolate.Execution.Validation.Error")]
        public virtual void OnValidationError(
            IQueryContext context,
            IReadOnlyCollection<IError> errors)
        {
            context
                .GetHttpContext()
                .Features
                .Get<HotChocolateActivity>()
                ?.HandleValidationError(
                    TryGetRequest(context.ContextData),
                    errors.ToList());
        }

        private static HotChocolateRequest TryGetRequest(
            IDictionary<string, object> contextData)
        {
            if (contextData.TryGetValue(nameof(HotChocolateRequest),
                out object request)
                && request is HotChocolateRequest r)
            {
                return r;
            }
            return null;
        }
    }
}
