# Issue 437 Profile Lookup

> Lookup profiles by handle. Check the local data set and fallback to webfinger if we don't have it.

Given these profiles

* ben@letterbook.social
* ben@mastodon.social

You can search for either and they will be returned.

<table border="1">
  <thead>
    <tr>
      <td>URL</td>
      <td>Status</td>
      <td>Result</td>
    </tr>
  </thead>
  <thead>
    <tr>
      <td>GET /lb/v1/search_profiles?q=ben@letterbook.social</td>
      <td>200</td>
      <td>ben@letterbook.social</td>
    </tr>
    <tr>
      <td>GET /lb/v1/search_profiles?q=ben</td>
      <td>200</td>
      <td>ben@letterbook.social</td>
    </tr>
    <tr>
      <td>GET /lb/v1/search_profiles?q=ben@mastodon.social</td>
      <td>200</td>
      <td>ben@mastodon.social</td>
    </tr>
    <tr>
      <td>GET /lb/v1/search_profiles?q=xxx@xxx.xxx</td>
      <td>404</td>
      <td>NONE</td>
    </tr>
  </thead>
</table>

* `GET /lb/v1/search_profiles?q=ben@letterbook.social` invokes the correct domain use case
* Whether it uses webfinger or not can be established elsewhere

## Problems

### Missing API endpoint fails weirdly

Looks like it is handling 404 this way. I have not added the controller yet.

```
Xunit.Sdk.EqualException
Assert.Equal() Failure: Values differ
Expected: OK
Actual:   InternalServerError
   at Letterbook.IntegrationTests.ProfileLookupTests.InvokeCorrectSearchMethod() in C:\Users\BenBiddington\sauce\Letterbook\Tests\Letterbook.IntegrationTests\ProfileLookupTests.cs:line 20
   at Xunit.Sdk.TestInvoker`1.<>c__DisplayClass47_0.<<InvokeTestMethodAsync>b__1>d.MoveNext() in /_/src/xunit.execution/Sdk/Frameworks/Runners/TestInvoker.cs:line 259
--- End of stack trace from previous location ---
   at Xunit.Sdk.ExecutionTimer.AggregateAsync(Func`1 asyncAction) in /_/src/xunit.execution/Sdk/Frameworks/ExecutionTimer.cs:line 48
   at Xunit.Sdk.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.core/Sdk/ExceptionAggregator.cs:line 90



Moq.MockException: IAuthorizationService.List<ModerationReport>(ClaimsPrincipal.<get_Claims>d__22) invocation failed with mock behavior Strict.
All invocations on the mock must have a corresponding setup.
   at Moq.FailForStrictMock.Handle(Invocation invocation, Mock mock) in C:\projects\moq4\src\Moq\Interception\InterceptionAspects.cs:line 182
   at Moq.Mock.Moq.IInterceptor.Intercept(Invocation invocation) in C:\projects\moq4\src\Moq\Interception\Mock.cs:line 27
   at Moq.CastleProxyFactory.Interceptor.Intercept(IInvocation underlying) in C:\projects\moq4\src\Moq\Interception\CastleProxyFactory.cs:line 107
   at Castle.DynamicProxy.AbstractInvocation.Proceed()
   at Castle.Proxies.IAuthorizationServiceProxy.List[T](IEnumerable`1 claims)
   at Letterbook.Web.Pages.Shared.Pages_Shared__Layout.<ExecuteAsync>b__26_1() in C:\Users\BenBiddington\sauce\Letterbook\Source\Letterbook.Web\Pages\Shared\_Layout.cshtml:line 28
   at Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext.SetOutputContentAsync()
   at Letterbook.Web.Pages.Shared.Pages_Shared__Layout.ExecuteAsync()
   at Microsoft.AspNetCore.Mvc.Razor.RazorView.RenderPageCoreAsync(IRazorPage page, ViewContext context)
   at Microsoft.AspNetCore.Mvc.Razor.RazorView.RenderPageAsync(IRazorPage page, ViewContext context, Boolean invokeViewStarts)
   at Microsoft.AspNetCore.Mvc.Razor.RazorView.RenderLayoutAsync(ViewContext context, ViewBufferTextWriter bodyWriter)
   at Microsoft.AspNetCore.Mvc.Razor.RazorView.RenderAsync(ViewContext context)
   at Microsoft.AspNetCore.Mvc.ViewFeatures.ViewExecutor.ExecuteAsync(ViewContext viewContext, String contentType, Nullable`1 statusCode)
   at Microsoft.AspNetCore.Mvc.ViewFeatures.ViewExecutor.ExecuteAsync(ViewContext viewContext, String contentType, Nullable`1 statusCode)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeResultAsync>g__Logged|22_0(ResourceInvoker invoker, IActionResult result)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeNextResultFilterAsync>g__Awaited|30_0[TFilter,TFilterAsync](ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.Rethrow(ResultExecutedContextSealed context)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.ResultNext[TFilter,TFilterAsync](State& next, Scope& scope, Object& state, Boolean& isCompleted)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.InvokeResultFilters()
--- End of stack trace from previous location ---
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeNextResourceFilter>g__Awaited|25_0(ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.Rethrow(ResourceExecutedContextSealed context)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.InvokeFilterPipelineAsync()
--- End of stack trace from previous location ---
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Logged|17_1(ResourceInvoker invoker)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Logged|17_1(ResourceInvoker invoker)
   at Serilog.AspNetCore.RequestLoggingMiddleware.Invoke(HttpContext httpContext)
   at Letterbook.AspNet.ProfileIdentityMiddleware.InvokeAsync(HttpContext context, IAccountService accounts) in C:\Users\BenBiddington\sauce\Letterbook\Source\Letterbook.AspNet\ProfileIdentityMiddleware.cs:line 69
   at Microsoft.AspNetCore.Authorization.AuthorizationMiddleware.Invoke(HttpContext context)
   at Microsoft.AspNetCore.Authentication.AuthenticationMiddleware.Invoke(HttpContext context)
   at Letterbook.Api.Authentication.HttpSignature.Infrastructure.HttpSignatureVerificationMiddleware.InvokeAsync(HttpContext context, RequestDelegate next) in C:\Users\BenBiddington\sauce\Letterbook\Source\Letterbook.Api.Authentication.HttpSignature\Infrastructure\HttpSignatureVerificationMiddleware.cs:line 43
   at Microsoft.AspNetCore.Builder.UseMiddlewareExtensions.InterfaceMiddlewareBinder.<>c__DisplayClass2_0.<<CreateMiddleware>b__0>d.MoveNext()
--- End of stack trace from previous location ---
   at Microsoft.AspNetCore.Builder.StatusCodePagesExtensions.<>c__DisplayClass7_0.<<CreateHandler>b__0>d.MoveNext()
--- End of stack trace from previous location ---
   at Microsoft.AspNetCore.Diagnostics.StatusCodePagesMiddleware.Invoke(HttpContext context)
   at Swashbuckle.AspNetCore.SwaggerUI.SwaggerUIMiddleware.Invoke(HttpContext httpContext)
   at Swashbuckle.AspNetCore.Swagger.SwaggerMiddleware.Invoke(HttpContext httpContext, ISwaggerProvider swaggerProvider)
   at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddlewareImpl.Invoke(HttpContext context)

HEADERS
=======
Authorization: Test 86b77ec9-20d8-45b7-bd5d-d33cf8f6336c
Host: localhost

```