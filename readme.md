# Introduction
When you build a .NET Framework C# project, it will automatically generate some binding redirects for you for the assemblies that it determines need it.
This means that sometimes when you specify one explicitly, it may actually be detrimental because it may not be required, and it will force that binding redirect which is tech debt for when you upgrade the assembly.
An explicit binding redirect could also cause runtime issues that may be hard to debug.

Note that binding redirects are only applicable to .NET Framework projects. .NET Standard/Core/5+ libraries and applications use .deps.json files instead.

# How To Use
1. Build your C# project with any explicit assembly binding redirects in place.
2. Copy the resulting .config file out of the output folder to a separate location since it will be overwritten by the next build.
	* Ex: If you're working with ClassLibrary1, the file you want to copy out is ClassLibrary1.dll.config. For applications, it will instead be .exe.config suffix.
	* I recommend appending (before) or (original) or some other text to the end of the filename to denote that this is the original config with the explicit binding redirects.
3. Update the app.config as needed to remove the binding redirects that you think you don't need.
4. Clean and rebuild your project to ensure that a new output config file is generated.
5. Repeat step 2 for the newly generated file, this time calling it (after) or (new).
6. Run this application and provide the absolute path for the file from step 2, then for the file from step 5.
7. Observe the text displayed detailing which binding redirects are now different or removed. If no text is displayed and you only see "Finished" then the content is the same and you know that you did not need to explicitly add the binding redirects.