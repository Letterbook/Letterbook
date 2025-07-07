# Web Mocks

This is a test project with access to the fake data generators used in unit tests.
This allows us to more easily generate mock data to use when developing UI views.
Please add any mock behavior you need by creating mock service classes.

This project loads and serves razor pages from the `Letterbook.Web` project, so continue to make actual UI changes there.

## Profiles

- [A mocked profile](http://localhost:5127/@anything@mock)

## Threads

- [Post not found](http://localhost:5127/thread/000000000000000000000007b)
- [A single post](http://localhost:5127/thread/00000000000000000000000en)
- [A post with ancestors and no children](http://localhost:5127/thread/00000000000000000000000u9)
- [A post with children and no ancestors](http://localhost:5127/thread/00000000000000000000001oi)
- [A post with both children and ancestors](http://localhost:5127/thread/00000000000000000000003c1)