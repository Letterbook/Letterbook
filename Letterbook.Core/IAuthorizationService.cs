using System.Security.Claims;
using Letterbook.Core.Authorization;
using Letterbook.Core.Models;
using Medo;

namespace Letterbook.Core;

public interface IAuthorizationService
{
    public Decision Create<T>(IEnumerable<Claim> claims, T target, Uuid7 profile) where T : IFederated;
    public Decision Update<T>(IEnumerable<Claim> claims, T target, Uuid7 profile) where T : IFederated;
    public Decision Delete<T>(IEnumerable<Claim> claims, T target, Uuid7 profile) where T : IFederated;
    public Decision Attribute<T>(IEnumerable<Claim> claims, T target, Uuid7 attributeTo, Uuid7 profile) where T : IFederated;
    public Decision Publish<T>(IEnumerable<Claim> claims, T target, Uuid7 profile) where T : IFederated;
    public Decision View<T>(IEnumerable<Claim> claims, T target, Uuid7 profile) where T : IFederated;
    public Decision Report<T>(IEnumerable<Claim> claims, T target, Uuid7 profile) where T : IFederated;

    // TODO: collections/tags/whatever we call them
    // TODO: account stuff
    // TODO: profile stuff (notifications, follows, blocks, etc)
    // TODO: thread stuff (manage replies, maybe more)
}