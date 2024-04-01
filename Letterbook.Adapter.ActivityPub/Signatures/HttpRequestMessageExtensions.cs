// Copyright (c) 2021, Unisys
//
// Adapted from NSign, used under the terms of the MIT license
// https://github.com/Unisys/NSign/commit/660b2412cd523ed175d387cf32f549065b3cc56f

using Letterbook.Adapter.ActivityPub.Exceptions;
using NSign.Signatures;
using static NSign.Constants;



namespace Letterbook.Adapter.ActivityPub.Signatures;

internal static class HttpRequestMessageExtensions
{
	public static string GetDerivedComponentValue(this HttpRequestMessage request, DerivedComponent derivedComponent)
	{
		if (request.RequestUri is not { } uri)
			throw ClientException.SignatureError();

		return derivedComponent.ComponentName switch
		{
			DerivedComponents.SignatureParams =>
				throw new NotSupportedException("The '@signature-params' component cannot be included explicitly."),
			DerivedComponents.Method => request.Method.Method,
			DerivedComponents.TargetUri => uri.OriginalString,
			DerivedComponents.Authority => uri.Authority.ToLower(),
			DerivedComponents.Scheme => uri.Scheme.ToLower(),
			DerivedComponents.RequestTarget => uri.PathAndQuery,
			DerivedComponents.Path => uri.AbsolutePath,
			DerivedComponents.Query =>
				String.IsNullOrWhiteSpace(uri.Query) ? "?" : uri.Query,
			DerivedComponents.QueryParam =>
				throw new NotSupportedException("The '@query-param' component must have the 'name' parameter set."),
			DerivedComponents.Status =>
				throw new NotSupportedException("The '@status' component cannot be included in request signatures."),

			_ =>
				throw new NotSupportedException(
					$"Non-standard derived signature component '{derivedComponent.ComponentName}' cannot be retrieved."),
		};
	}
}