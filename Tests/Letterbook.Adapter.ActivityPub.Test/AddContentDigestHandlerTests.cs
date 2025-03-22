using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using NSign.Client;
using static NSign.Client.AddContentDigestOptions;
using AddContentDigestHandler = Letterbook.Adapter.ActivityPub.Signatures.AddContentDigestHandler;

namespace Letterbook.Adapter.ActivityPub.Test
{
	public sealed class AddContentDigestHandlerTests
	{
		private readonly Mock<HttpMessageHandler> _mockInnerHandler = new(MockBehavior.Strict);
		private readonly HttpRequestMessage _request = new(HttpMethod.Get, "http://localhost:8080/UnitTests/");
		private readonly HttpResponseMessage _response = new();
		private readonly AddContentDigestOptions _options = new();
		private readonly AddContentDigestHandler _handler;

		public AddContentDigestHandlerTests()
		{
			_mockInnerHandler.Protected().Setup("Dispose", ItExpr.Is<bool>(d => d == true));

			_options.WithHash(Hash.Sha256).WithHash(Hash.Sha512);

			_handler = new AddContentDigestHandler(new OptionsWrapper<AddContentDigestOptions>(_options))
			{
				InnerHandler = _mockInnerHandler.Object,
			};
		}

		[Fact(DisplayName = "Should not add digest for empty body")]
		public async Task SendAsyncDoesNotAddContentDigestIfRequestDoesNotHaveContent()
		{
			using var invoker = new HttpMessageInvoker(_handler);

			_request.Method = HttpMethod.Get;

			_mockInnerHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
					ItExpr.Is<HttpRequestMessage>(r =>
						r == _request && !r.Headers.Contains("digest") && !r.Headers.Contains("content-digest")),
					ItExpr.Is<CancellationToken>(c => c == CancellationToken.None))
				.ReturnsAsync(_response);

			Assert.Same(_response, await invoker.SendAsync(_request, CancellationToken.None));

			_mockInnerHandler.Protected().Verify<Task<HttpResponseMessage>>(
				"SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
		}

		[Theory(DisplayName = "Should add configured digest")]
		[InlineData(
			"stream",
			"hello world",
			Hash.Sha256,
			"sha-256=:uU0nuZNNPgilLlLX2n2r+sSE7+N6U4DukIj3rOLvzek=:",
			"sha-256=uU0nuZNNPgilLlLX2n2r+sSE7+N6U4DukIj3rOLvzek="
		)]
		[InlineData(
			"string",
			"hello",
			Hash.Sha256,
			"sha-256=:LPJNul+wow4m6DsqxbninhsWHlwfp0JecwQzYpOLmCQ=:",
			"sha-256=LPJNul+wow4m6DsqxbninhsWHlwfp0JecwQzYpOLmCQ="
		)]
		[InlineData(
			"json",
			"hello",
			Hash.Sha256,
			"sha-256=:Wqdirjg/u3J688ejbUlApbjECpiUUtIwT8lY/z81Tno=:",
			"sha-256=Wqdirjg/u3J688ejbUlApbjECpiUUtIwT8lY/z81Tno="
		)]
		[InlineData(
			"stream",
			"hello world",
			Hash.Sha512,
			"sha-512=:MJ7MSJwS1utMxA9QyQLytNDtd+5RGnx6m808qG1M2G+YndNbxf9JlnDaNCVbRbDP2DDoH2Bdz33FVC6TrpzXbw==:",
			"sha-512=MJ7MSJwS1utMxA9QyQLytNDtd+5RGnx6m808qG1M2G+YndNbxf9JlnDaNCVbRbDP2DDoH2Bdz33FVC6TrpzXbw=="
		)]
		[InlineData(
			"string",
			"hello",
			Hash.Sha512,
			"sha-512=:m3HSJL1i83hdltRq0+o9czGb+8KJDKra4t/3JRlnPKcjI8PZm6XBHXx6zG4UuMXaDEZjR1wuXDre9G9zvN7AQw==:",
			"sha-512=m3HSJL1i83hdltRq0+o9czGb+8KJDKra4t/3JRlnPKcjI8PZm6XBHXx6zG4UuMXaDEZjR1wuXDre9G9zvN7AQw=="
		)]
		[InlineData(
			"json",
			"hello",
			Hash.Sha512,
			"sha-512=:A8pplr4vsk4xdLkJruCXWp6+i+dy/3pSW5HW5ke1jDWS70Dv6Fstf1jS+XEcLqEVhW3i925IPlf/4tnpnvAQDw==:",
			"sha-512=A8pplr4vsk4xdLkJruCXWp6+i+dy/3pSW5HW5ke1jDWS70Dv6Fstf1jS+XEcLqEVhW3i925IPlf/4tnpnvAQDw=="
		)]
		public async Task SendAsyncAddsOnlyConfiguredHashes(string httpContentType, string content, Hash hash, string expectedContentDigest,
			string expectedDigest)
		{
			using var invoker = new HttpMessageInvoker(_handler);

			_request.Method = HttpMethod.Post;
			_request.Content = CreateHttpContent(httpContentType, content);
			_options.Hashes.Clear();
			_options.WithHash(hash);

			_mockInnerHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
					ItExpr.Is<HttpRequestMessage>(r =>
						r == _request && VerifyDigestHeader(NSign.Constants.Headers.ContentDigest, r, expectedContentDigest)
						             && VerifyDigestHeader(AddContentDigestHandler.DigestHeader, r, expectedDigest)),
					ItExpr.Is<CancellationToken>(c => c == CancellationToken.None))
				.ReturnsAsync(_response);

			Assert.Same(_response, await invoker.SendAsync(_request, default));

			_mockInnerHandler.Protected().Verify<Task<HttpResponseMessage>>(
				"SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
		}

		[Theory(DisplayName = "Should add all configured digests")]
		[InlineData(
			"stream",
			"test",
			"sha-256=:n4bQgYhMfWWaL+qgxVrQFaO/TxsrC4Is0V1sFbDwCgg=:",
			"sha-512=:7iaw3Ur350mqGo7jwQrpkj9hiYB3Lkc/iBml1JQODbJ6wYX4oOHV+E+IvIh/1nsUNzLDBMxfqa2Ob1f1ACio/w==:"
		)]
		[InlineData(
			"string",
			"test",
			"sha-256=:n4bQgYhMfWWaL+qgxVrQFaO/TxsrC4Is0V1sFbDwCgg=:",
			"sha-512=:7iaw3Ur350mqGo7jwQrpkj9hiYB3Lkc/iBml1JQODbJ6wYX4oOHV+E+IvIh/1nsUNzLDBMxfqa2Ob1f1ACio/w==:"
		)]
		[InlineData(
			"json",
			"test",
			"sha-256=:TZZ6MBEb8p8OugHESLN1wWKbL+0BzfzDrtkfG1fV3V4=:",
			"sha-512=:ceemix/T1umsPeT9f/DEUNpccmguTGuGcqp+SAhBhz9oTwjX49sBWRNNam2cvhm51qgMV+NsXMm/Fg6JsKjgJQ==:"
		)]
		public async Task SendAsyncAddsMultipleHashes(string httpContentType, string content, string expectedValue1, string expectedValue2)
		{
			using var invoker = new HttpMessageInvoker(_handler);

			_request.Method = HttpMethod.Post;
			_request.Content = CreateHttpContent(httpContentType, content);

			_mockInnerHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
					ItExpr.Is<HttpRequestMessage>(r =>
						r == _request && VerifyDigestHeader(NSign.Constants.Headers.ContentDigest, r, expectedValue1, expectedValue2)),
					ItExpr.Is<CancellationToken>(c => c == CancellationToken.None))
				.ReturnsAsync(_response);

			Assert.Same(_response, await invoker.SendAsync(_request, default));

			_mockInnerHandler.Protected().Verify<Task<HttpResponseMessage>>(
				"SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
		}

		[Fact(DisplayName = "Should only add a single value in the Digest header")]
		public async Task SingleDigestValue()
		{
			using var invoker = new HttpMessageInvoker(_handler);

			_request.Method = HttpMethod.Post;
			_request.Content = CreateHttpContent("string", "hello world");
			_options.Hashes.Clear();
			_options.WithHash(Hash.Sha256);
			_options.WithHash(Hash.Sha512);

			_mockInnerHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(_response);
			Assert.Same(_response, await invoker.SendAsync(_request, default));

			string[] expectedDigest = ["sha-256=uU0nuZNNPgilLlLX2n2r+sSE7+N6U4DukIj3rOLvzek="];
			_mockInnerHandler.Protected()
				.Verify<Task<HttpResponseMessage>>("SendAsync", Times.Once(),
					ItExpr.Is<HttpRequestMessage>(msg =>
						// Content-Digest should use both configured hashes
						msg.Content!.Headers.GetValues("content-digest").Count() == 2 &&
						// Digest should use only the first configured hash
						VerifyDigestHeader("Digest", msg, expectedDigest)),
					ItExpr.IsAny<CancellationToken>());
		}

		[Theory(DisplayName = "Should throw on unknown hash algorithm")]
		[InlineData(Hash.Unknown)]
		[InlineData((Hash)999)]
		public async Task SendAsyncThrowsOnUnsupportedHash(Hash hash)
		{
			using var invoker = new HttpMessageInvoker(_handler);

			_request.Method = HttpMethod.Post;
			_request.Content = CreateHttpContent("string", "blah");

			_options.Hashes.Clear();
			_options.WithHash(hash);

			var ex = await Assert.ThrowsAsync<NotSupportedException>(() => invoker.SendAsync(_request, default));
			Assert.Equal($"Hash algorithm '{hash}' is not supported.", ex.Message);
		}

		private static HttpContent CreateHttpContent(string type, string content) => type switch
		{
			"stream" => new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(content))),
			"string" => new StringContent(content),
			"json" => JsonContent.Create(content),
			_ => throw new NotSupportedException()
		};

		private static bool VerifyDigestHeader(string header, HttpRequestMessage request, params string[] expected)
		{
			if (!request.Content!.Headers.TryGetValues(header, out var values))
			{
				return false;
			}

			var actual = new HashSet<string>(values);
			return actual.Count == expected.Length && expected.All(actual.Contains);
		}
	}
}