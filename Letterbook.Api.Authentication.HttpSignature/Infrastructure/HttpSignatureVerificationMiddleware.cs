using System.Diagnostics;
using Letterbook.Api.Authentication.HttpSignature.Verification;
using Letterbook.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Constants = NSign.Constants;

namespace Letterbook.Api.Authentication.HttpSignature.Infrastructure;

/// <summary>
/// Implements HTTP signature verification.
///
/// If no signature header is present, the request is allowed to proceed.
///
/// If a signature header is present, the signature is validated.
///
/// If validation succeeds, the validated actor ID URIs are stored using <see cref="HttpSignatureFeature"/>.
///
/// If validation fails, the request terminates with HTTP 400 Bad Request.
/// </summary>
public class HttpSignatureVerificationMiddleware : IMiddleware
{
	private readonly ILogger<HttpSignatureVerificationMiddleware> _logger;
	private readonly IFederatedActorHttpSignatureVerifier _verifier;
	private readonly ActivitySource _instrumentation;

	public HttpSignatureVerificationMiddleware(
		ILogger<HttpSignatureVerificationMiddleware> logger,
		IFederatedActorHttpSignatureVerifier verifier,
		Instrumentation instrumentation
		)
	{
		_logger = logger;
		_verifier = verifier;
		_instrumentation = instrumentation.ActivitySource;
	}

	public HttpSignatureVerificationMiddleware(
		ILogger<HttpSignatureVerificationMiddleware> logger,
		IFederatedActorHttpSignatureVerifier verifier
	)
	{
		_logger = logger;
		_verifier = verifier;
		_instrumentation = Activity.Current?.Source ?? new ActivitySource(GetType().Name);
	}

	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		using var activity = _instrumentation.StartActivity(GetType().Name);
		var signatureFeature = new HttpSignatureFeature();
		context.Features.Set(signatureFeature);

		if (!context.Request.Headers.ContainsKey(Constants.Headers.Signature))
		{
			// No signatures present. Allow the request to proceed. Authorization requirements
			// may deny the request at a later stage.
			await next(context);
			return;
		}

		try
		{
			var verifiedSignatures = await _verifier
				.VerifyAsync(context, context.RequestAborted)
				.ToListAsync();

			if (!verifiedSignatures.Any())
			{
				// A signature header was present, but validation failed: either the signature
				// itself was invalid or the public key could not be fetched. Regardless of whether
				// signature authentication is actually requested, we terminate here just to be safe.
				await WriteInvalidSignatureResponse(context);
				return;
			}

			// At least one valid signature was found, so we add the validated identities to the collection.
			// At this point, we don't have an authenticated principal--that happens at a later stage, when
			// the authentication handler gets called.
			signatureFeature.Add(verifiedSignatures);
			await next(context);
		}
		catch (Exception ex)
		{
			_logger.LogError("Unhandled exception during request signature validation: {Exception}", ex);
			throw;
		}

		static async Task WriteInvalidSignatureResponse(HttpContext context)
		{
			var badRequest = new BadRequestObjectResult(new
			{
				ErrorMessage = "Request signature validation failed"
			});

			await badRequest.ExecuteResultAsync(new ActionContext()
			{
				HttpContext = context
			});
		}
	}
}