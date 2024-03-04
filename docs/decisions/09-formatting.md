# Formatting

Establishing some minimal style enforcement

## Status

- [ ] Decided
- [ ] Decided Against
- [ ] Deferred
- [ ] Superseded

# Decision

We want to lean into existing tools to take the argument and personal tastes out of applying formatting. This helps to avoid nitpicking, especially if it happens automatically.

## EditorConfig

[EditorConfig](https://editorconfig.org/).
We have an `.editorconfig` file to control basic spacing settings. The initial rules are:

* Covers only `.cs` files
* Covers only basic formatting:
  * utf-8
  * [Tabs, not spaces for accessibility](https://adamtuttle.codes/blog/2021/tabs-vs-spaces-its-an-accessibility-issue/)
  * No trailing newline at end of file, simply because that's what the existing files use.
  * No trailing whitespace at end of line.
  * Newline settings, LF/CRLF not configured, as it's better to let Git settings take care of that via `core.autocrlf`, than to have 2 tools both changing it.

## Making use of these settings

Visual Studio or JetBrains Rider apply these settings on new code automatically. [Visual Studio Code has support via a plug-in](https://marketplace.visualstudio.com/items?itemName=EditorConfig.EditorConfig).

`dotnet format` will also follow `.editorconfig` rules.

We may look into more use of these tools later, in 2 directions

* There are [many dotnet code-style rule options](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/code-style-rule-options) that are supported by some dotnet tools.
* Cover other file types that are used in the repo. `.md`, `.csproj` and `.yml` are currently used.
