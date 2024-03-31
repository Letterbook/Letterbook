using System.Reactive;
using System.Reactive.Subjects;
using CloudNative.CloudEvents;
using Letterbook.Core.Tests.Fakes;
using Moq;

namespace Letterbook.Core.Tests;


public class AccountEventServiceTest : WithMocks
{
	private AccountEventService _service;
	private Subject<CloudEvent> _subject;
	private FakeAccount _fakeAccount;

	public AccountEventServiceTest()
	{
		_subject = new Subject<CloudEvent>();
		MessageBusAdapterMock.Setup(m => m.OpenChannel<It.IsAnyType>(It.IsAny<string?>()))
			.Returns(_subject.AsObserver());
		_fakeAccount = new FakeAccount();

		_service = new AccountEventService(CoreOptionsMock, MessageBusAdapterMock.Object);
	}

	[Fact]
	public void PublishesCreatedEvent()
	{
		var account = _fakeAccount.Generate();
		_subject.Subscribe(c =>
		{
			var action = c.Type!.Split(".").Last();
			Assert.Equal("Created", action);
		});
		_service.Created(account);
	}

	[Fact]
	public void PublishesDeletedEvent()
	{
		var account = _fakeAccount.Generate();
		_subject.Subscribe(c =>
		{
			var action = c.Type!.Split(".").Last();
			Assert.Equal("Deleted", action);
		});
		_service.Deleted(account);
	}

	[Fact]
	public void PublishesSuspendedEvent()
	{
		var account = _fakeAccount.Generate();
		_subject.Subscribe(c =>
		{
			var action = c.Type!.Split(".").Last();
			Assert.Equal("Suspended", action);
		});
		_service.Suspended(account);
	}

	[Fact]
	public void PublishesUpdatedEvent()
	{
		var account = _fakeAccount.Generate();
		var updated = account.ShallowClone();
		_subject.Subscribe(c =>
		{
			var action = c.Type!.Split(".").Last();
			Assert.Equal("Updated", action);
		});
		_service.Updated(account, updated);
	}

	[Fact]
	public void PublishesVerifiedEvent()
	{
		var account = _fakeAccount.Generate();
		_subject.Subscribe(c =>
		{
			var action = c.Type!.Split(".").Last();
			Assert.Equal("Verified", action);
		});
		_service.Verified(account);
	}
}