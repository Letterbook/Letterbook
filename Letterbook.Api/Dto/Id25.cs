using System.Diagnostics.CodeAnalysis;
using Medo;

namespace Letterbook.Api.Dto;

public struct Id25
{
    private string _id;
    public override string ToString() => _id;

    public static implicit operator Id25(Uuid7 v) => new() { _id = v.ToId25String() };
    public static implicit operator Id25(string v) => new() { _id = v };
    public static implicit operator string(Id25 v) => v._id;
    
    public bool TryAsUuid7(out Uuid7 id)
    {
        try
        {
            id = Uuid7.FromId25String(_id);
            return true;
        }
        catch (Exception)
        {
            id = Uuid7.Empty;
            return false;
        }
    }
}