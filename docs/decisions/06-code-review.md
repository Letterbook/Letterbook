# The Purpose of Code Review

Informational discussion of the role code review should play in the project.

## Status

- [ ] Decided
- [ ] Decided Against
- [ ] Deferred
- [ ] Superseded

# Decision

Over time, some of this will change. I can imagine a future where PRs become a way to solicit community input, for instance. But this is how I think our priorities should stand right now.

### First, Education

Code review serves several functions, but the most important and valuable is to share knowledge with each other. It's best if more than one person understands how things work. That includes how things are organized, why they're this way and not some other way, and the path it's taken to get there. Pull requests and code review are not necessarily the best way to do that, but they do offer a check point where that knowledge sharing can be foregrounded. It's a good thing to ask questions and make suggestions. Please do that. As such, it's also a good thing to leave PRs open for at least a couple of days, so that multiple contributors have a chance to see it and ask those questions, while it's still topical and easy to change.

Code review also offers a unique chance to see things change as they're changing. This may be the best way to actually gain a sense of history with a code base. Having that historical perspective is very valuable. It's not possible to have a robust discussion of every potential way things could be designed before implementation. People are going to just pick an option and run with it. And sometimes those choices will become something we regret over time. Undoing decisions we no longer benefit from is much more viable when people know something about what influenced the decision in the first place.

### Then Quality

At some level, code review is also a point to perform some more mechanical analysis. Reviewing for correctness and completeness are good things. This is particularly true when reviewing contributions from newcomers to the project. This is a relationship, and that means there's an element of trust involved. Trust takes time. It's almost always reasonable to ask for more tests or docs, to cover more scenarios, or to change things that could become problems. If something is unreviewably large, it's fair to ask it be broken up into more digestible chunks.

### Never Style

Nitpicking is counterproductive to learning. There are some benefits to maintaining consistent styles or decluttering. But those benefits are small compared to the dynamic it introduces to the review process. It turns what should be a collaborative learning exercise into a sort of source code gramar policing. To the extent that enforcing style, convention, or formatting are important, we should have CI do it. We can always refactor code. On the other hand, repairing relationship is hard, at best.

### Not an Obligation

This project isn't anyone's job. We're all here voluntarily, mostly for fun. Or something like fun, at least. No one is obligated to review PRs. Just like code contributions, contributing to some review is a gift.

## Impact

Details about what changes because of the decision

## Context

I did just put up a monster PR, and I feel a little bad about that. This ADR isn't about that so much as it's just on my mind. And also that monster PR is behind me, and it freed up some neurons for other purposes.

## Discussion

- Should we update our PR templates to invite questions, or something like that?
- Should we update any other docs?