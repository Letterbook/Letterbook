using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CloudNative.CloudEvents;
using Letterbook.Core.Tests.Fakes;
using Microsoft.Extensions.Options;
using Moq;

namespace Letterbook.Core.Tests;

public class ActivityEventServiceTest : WithMocks
{
    private ActivityEventService _service;
    private Subject<CloudEvent> _subject;
    private FakeNote _fakeNote;
    private FakeProfile _fakeProfile;

    public ActivityEventServiceTest()
    {
        _subject = new Subject<CloudEvent>();
        MessageBusAdapterMock.Setup(m => m.OpenChannel<It.IsAnyType>())
            .Returns(_subject.AsObserver());
        _fakeNote = new FakeNote();
        _fakeProfile = new FakeProfile();
        
        _service = new ActivityEventService(CoreOptionsMock, MessageBusAdapterMock.Object);
    }
    
    [Fact]
    public void Exists()
    {
        Assert.NotNull(_service);
    }

    [Fact]
    public void CanMock()
    {
        _subject.Subscribe(c => Assert.NotNull(c));
        _service.Approved(_fakeNote.Generate());
    }

    [Fact]
    public void PublishesCloudEvents()
    {
        var emitted = false;
        var note = _fakeNote.Generate();
        _subject.Subscribe(c =>
        {
            emitted = true;
            Assert.Equal(note.Id.ToString(), c.Subject);
            Assert.True(c.IsValid);
        });
        _service.Approved(note);
        Assert.True(emitted);
    }
    
    [Fact]
    public void PublishesApprovedEvent()
    {
        var note = _fakeNote.Generate();
        _subject.Subscribe(c =>
        {
            var action = c.Type!.Split(".").Last();
            Assert.Equal("Approved", action);
        });
        _service.Approved(note);
    }
    
    [Fact]
    public void PublishesBoostedEvent()
    {
        var note = _fakeNote.Generate();
        _subject.Subscribe(c =>
        {
            var action = c.Type!.Split(".").Last();
            Assert.Equal("Boosted", action);
        });
        _service.Boosted(note);
    }
    
    [Fact]
    public void PublishesCreatedEvent()
    {
        var note = _fakeNote.Generate();
        _subject.Subscribe(c =>
        {
            var action = c.Type!.Split(".").Last();
            Assert.Equal("Created", action);
        });
        _service.Created(note);
    }
    
    [Fact]
    public void PublishesUpdatedEvent()
    {
        var note = _fakeNote.Generate();
        _subject.Subscribe(c =>
        {
            var action = c.Type!.Split(".").Last();
            Assert.Equal("Updated", action);
        });
        _service.Updated(note);
    }
    
    [Fact]
    public void PublishesDeletedEvent()
    {
        var note = _fakeNote.Generate();
        _subject.Subscribe(c =>
        {
            var action = c.Type!.Split(".").Last();
            Assert.Equal("Deleted", action);
        });
        _service.Deleted(note);
    }
    
    [Fact]
    public void PublishesFlaggedEvent()
    {
        var note = _fakeNote.Generate();
        _subject.Subscribe(c =>
        {
            var action = c.Type!.Split(".").Last();
            Assert.Equal("Flagged", action);
        });
        _service.Flagged(note);
    }
    
    [Fact]
    public void PublishesLikedEvent()
    {
        var note = _fakeNote.Generate();
        _subject.Subscribe(c =>
        {
            var action = c.Type!.Split(".").Last();
            Assert.Equal("Liked", action);
        });
        _service.Liked(note);
    }
    
    [Fact]
    public void PublishesRejectedEvent()
    {
        var note = _fakeNote.Generate();
        _subject.Subscribe(c =>
        {
            var action = c.Type!.Split(".").Last();
            Assert.Equal("Rejected", action);
        });
        _service.Rejected(note);
    }
    
    [Fact]
    public void PublishesRequestedEvent()
    {
        var note = _fakeNote.Generate();
        _subject.Subscribe(c =>
        {
            var action = c.Type!.Split(".").Last();
            Assert.Equal("Requested", action);
        });
        _service.Requested(note);
    }
    
    [Fact]
    public void PublishesOfferedEvent()
    {
        var note = _fakeNote.Generate();
        _subject.Subscribe(c =>
        {
            var action = c.Type!.Split(".").Last();
            Assert.Equal("Offered", action);
        });
        _service.Offered(note);
    }
    
    [Fact]
    public void PublishesMentionedEvent()
    {
        var note = _fakeNote.Generate();
        _subject.Subscribe(c =>
        {
            var action = c.Type!.Split(".").Last();
            Assert.Equal("Mentioned", action);
        });
        _service.Mentioned(note);
    }
    
    [Fact]
    public void PublishesEventForProfile()
    {
        var profile = _fakeProfile.Generate();
        _subject.Subscribe(c =>
        {
            var actions = c.Type!.Split(".");
            Assert.Contains("Profile", actions);
        });
        _service.Mentioned(profile);
    }
    
    [Fact]
    public void PublishesEventForNote()
    {
        var note = _fakeNote.Generate();
        _subject.Subscribe(c =>
        {
            var actions = c.Type!.Split(".");
            Assert.Contains("Note", actions);
        });
        _service.Mentioned(note);
    }

}