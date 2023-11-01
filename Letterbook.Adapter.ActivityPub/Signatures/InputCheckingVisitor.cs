
using System.Diagnostics.CodeAnalysis;
using NSign;
using static NSign.Constants;
using NSign.Signatures;
using StructuredFieldValues;

namespace Letterbook.Adapter.ActivityPub.Signatures;

internal sealed class InputCheckingVisitor : ISignatureComponentVisitor
{
    private readonly HttpRequestMessage _context;
    
    public InputCheckingVisitor(HttpRequestMessage context)
    {
        _context = context;
    }
    
    public bool Found { get; private set; } = true;

    public void Visit(SignatureComponent component)
    {
        /* Intentionally left blank because signature components can't be unavailable */
    }
    
    public void Visit(HttpHeaderComponent httpHeader)
    {
        Found &= TryGetHeaderValues(_context, httpHeader.ComponentName, out _);
    }
    
    public void Visit(HttpHeaderDictionaryStructuredComponent httpHeaderDictionary)
    {
        Found &= TryGetHeaderValues(_context, httpHeaderDictionary.ComponentName, out var values)
                 && HasKey(values, httpHeaderDictionary.Key);
    }
    
    public void Visit(HttpHeaderStructuredFieldComponent httpHeaderStructuredField)
    {
        // Assume that the header value is a proper structured field, so we can leave the check to the normal
        // check for HttpHeaderComponent.
        Visit((HttpHeaderComponent)httpHeaderStructuredField);
    }
    
    public void Visit(DerivedComponent derived)
    {
        switch (derived.ComponentName)
        {
            case DerivedComponents.Method:
            case DerivedComponents.TargetUri:
            case DerivedComponents.Authority:
            case DerivedComponents.Scheme:
            case DerivedComponents.RequestTarget:
            case DerivedComponents.Path:
            case DerivedComponents.Query:
                break;
            case DerivedComponents.Status:
                Found = false;
                break;
            case DerivedComponents.SignatureParams:
            case DerivedComponents.QueryParam:
                throw new NotSupportedException(
                    $"Derived component '{derived.ComponentName}' must be added through the corresponding class.");

            default:
                Found = false;
                break;
        }
    }

    
    public void Visit(SignatureParamsComponent signatureParamsComponent)
    {
        // Intentionally left blank: the @signature-params component is always present.
    }

    
    public void Visit(QueryParamComponent queryParam)
    {
        Found &= _context.RequestUri?.Query.Contains(queryParam.Name) ?? false;
    }

    private static bool HasKey(IEnumerable<string> structuredDictValues, string key)
    {
        foreach (string value in structuredDictValues)
        {
            if (null == SfvParser.ParseDictionary(value, out IReadOnlyDictionary<string, ParsedItem> actualDict) &&
                actualDict.TryGetValue(key, out _))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryGetHeaderValues(HttpRequestMessage message, string header,
        [NotNullWhen(true)] out IEnumerable<string>? values)
    {
        return message.Headers.TryGetValues(header, out values)
               || (message.Content != null && message.Content.Headers.TryGetValues(header, out values));
    }
}