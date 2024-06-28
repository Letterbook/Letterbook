# Dotnet API Docs

This is an initial configuration for DocFX. The intent is to start generating code-level documentation. This can be a more accessible reference than the source code. Hopefully this makes the project more approachable for new contributors or 3rd party clients. It's also hopefully useful to established contributors.

However, the goal is to incorporate these docs into a cohesive documentation site that includes a project blog, guides, and other purpose-made articles. Preferably one that reuses some of the tooling and assets from the app itself. That makes it a more consistent experience, and also means expertise is more transferable between the docs site and the app itself.

So, the plan is to use DocFX to extract XMLDoc strings from the source code and build them into reusable markdown. Then, use them as input to a static site built with Razor Press.