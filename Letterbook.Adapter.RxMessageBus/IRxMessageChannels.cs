using System.Reactive.Subjects;
using CloudNative.CloudEvents;

namespace Letterbook.Adapter.RxMessageBus;

public interface IRxMessageChannels
{
    public Subject<CloudEvent> GetSubject(string type);
}