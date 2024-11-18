# Worker Contracts

These contracts define the messages that workers will consume and process. Message queues introduce a little bit of complexity, and it's important to manage that well. The main thing is not to treat it like a procedure call. While we expect that they will be, there's no way to know if a message is ever consumed, what consumer(s) received it, and what the outcome was. So, you can't rely on anything the consumers will do, as a producer. This loose coupling is great for maintainability and scale, in addition to being just structurally necessary to permit doing work outside of HTTP request/response cycles.

Mostly, messages should represent events: things that have already happened. Consumers can respond to these events in any way, and we're free to evolve that or introduce new consumers with little or no impact to anything else. Sometimes, we instead there's no event that has already happened, but instead we need to initiate some out of band work. This is fine, as long as we keep in mind that we can't depend on the implementation or outcome of that work.

So, in general, messages that represent events are named `Event`. Like `AccountEvent` or `ProfileEvent`. Messages that carry a command are named `Message`. Like `CrawlerMessage`.
Still, there are times we need to use the message system to 