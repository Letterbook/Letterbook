using Microsoft.Extensions.Logging;
using NSign;
using NSign.Signatures;
using static NSign.MessageSigningOptions;

namespace Letterbook.Adapter.ActivityPub.Signatures;

/// <summary>
/// ClientMessageSigner is mostly duplicated from NSign, with a few small tweaks to produce signatures that mastodon
/// will understand
/// </summary>
public class ClientMessageSigner : IMessageSigner
{
    /// <summary>
    /// The ILogger to use.
    /// </summary>
    private readonly ILogger<ClientMessageSigner> logger;

    /// <summary>
    /// The <see cref="ISigner"/> to use to sign messages.
    /// </summary>
    private readonly ISigner signer;

    /// <summary>
    /// Initializes a new instance of DefaultMessageSigner.
    /// </summary>
    /// <param name="logger">
    /// The ILogger to use.
    /// </param>
    /// <param name="signer">
    /// The <see cref="ISigner"/> to use to sign messages.
    /// </param>
    public ClientMessageSigner(ILogger<ClientMessageSigner> logger, ISigner signer)
    {
        this.logger = logger;
        this.signer = signer;
    }

    #region IMessageSigner Implementation

    /// <inheritdoc/>
    public async Task SignMessageAsync(MessageContext context)
    {
        if (null == context.SigningOptions)
        {
            throw new InvalidOperationException("The message context does not have signing options.");
        }

        MessageSigningOptions options = context.SigningOptions;

        if (String.IsNullOrWhiteSpace(options.SignatureName))
        {
            throw new InvalidOperationException("The SignatureName must be set to a non-blank string. Signing failed.");
        }

        SignatureInputSpec inputSpec = new SignatureInputSpec(options.SignatureName);
        options.SetParameters?.Invoke(inputSpec.SignatureParameters);

        foreach (ComponentSpec componentSpec in options.ComponentsToInclude)
        {
            // Add only fields which are mandatory, or refer to components that exist on the request.
            if (componentSpec.Mandatory || context.HasSignatureComponent(componentSpec.Component))
            {
                inputSpec.SignatureParameters.AddComponent(componentSpec.Component);
            }
        }

        if (options.UseUpdateSignatureParams)
        {
            signer.UpdateSignatureParams(inputSpec.SignatureParameters);
        }

        ReadOnlyMemory<byte> signature = await signer.SignAsync(
            context.GetSignatureInput(inputSpec.SignatureParameters, out string sigInput, new MastodonComponentBuilder(context)),
            context.Aborted);

        if (logger.IsEnabled(LogLevel.Debug))
        {
            string messageType = context.HasResponse ? "response to request" : "request";
            string? method = context.GetDerivedComponentValue(SignatureComponent.Method);
            string? url = context.GetDerivedComponentValue(SignatureComponent.RequestTarget);

            logger.LogDebug("Using signature-input '{input}' for signature '{sig}' of {type} '{method} {url}'.",
                sigInput, inputSpec.Name, messageType, method, url);
        }

        // It's time to add the 'signature-input' and 'signature' headers.
        context.AddHeader(Constants.Headers.SignatureInput, $"{inputSpec.Name}={sigInput}");
        context.AddHeader(Constants.Headers.Signature, $"{inputSpec.Name}=:{Convert.ToBase64String(signature.Span)}:");
    }

    #endregion
}