# Add Codeberg support
 
Can we make the same build tasks as Github?

* [Hosted actions](https://codeberg.org/actions/meta)
* [Forgejo Actions | Reference](https://forgejo.org/docs/next/user/actions/reference/)

The runner can be installed locally.

## Log

### Follow quick start

https://forgejo.org/docs/next/user/actions/quick-start/

That does work only the runner must be `runs-on: codeberg-tiny`. A message in the UI says "No matching online runner with label: docker".

### Run build?

We want to run something like:

```sh
dotnet build Letterbook.sln
```

Got this error when using the smallrest runner: "The runner cancelled the job because it exceeds the maximum run time", so trying largest.

### Lazy runners

https://codeberg.org/actions/meta#lazy-runners

> ...we are considering experiments like offsite runners which only run when solar energy is available.