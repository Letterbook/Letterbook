# Letterbook Workers

The Workers host implements an asynchronous message queue. This provides or supports a lot of Letterbook's functionality. Message queues introduce a little bit of complexity compared to simple HTTP request controllers. It's important to manage that complexity. The main thing is not to treat these messages like a procedure call. While we expect that they will be, there's no way to know if a message is ever consumed, what consumer(s) received it, and what the outcome was. So, you can't rely on anything the consumers will do, as a producer. And consumers can't communicate anything back to the producer. This loose coupling is great for maintainability and scale, in addition to being just structurally necessary to permit doing work outside of HTTP request/response cycles.

Mostly, messages should represent events—things that have already happened. Consumers can respond to these events in any way, and we're free to evolve that or introduce new consumers with little or no impact to anything else. There are also times you need to initiate something to be done, rather than responding after the fact to an event that has already happened. This is fine, as long as we keep in mind that we can't depend on the implementation or outcome of that work.

The same mechanisms are used in both cases. The important part is how we should think about them. So, we use some naming conventions to signal intent. The message queueing system is broadly composed of message contracts, message producers, and message processors. The following table gives some guidance about how to name these parts: 

| Component | Event     | Command   |
|:----------|:----------|:----------|
| Message   | Event     | Job       |
| Producer  | Publisher | Scheduler |
| Processor | Consumer  | Worker    |

So, in general, messages that represent events are named `Event`. Like `AccountEvent` or `ProfileEvent`. Messages that carry a command are named `Job`. Like `CrawlerJob`. Event producers are named `Publisher`, like `AccountEventPublisher`. Job producers are called `Scheduler`, like `CrawlerScheduler`. Event processors are called Consumer, like `TimelineConsumer` and `OutboundPostConsumer`. Note that there can and often will be multiple consumers of a given event. Those consumers should be named for the purpose they're serving, not the event they're consuming. And finally, job processors should be named `Worker`, like `CrawlerWorker`.
