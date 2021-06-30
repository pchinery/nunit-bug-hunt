# nunit-bug-hunt

The NUnit module suddenly throws an exception while running nunit3-console.exe (target `Test2`). The same manual call to nunit3-console works just fine (target `Test1`).

This most likely happened after an update to dotnet 5.0.301. We had to update `MSBuild.StructuredLogger`, because the log format had changed. With that being the only change in our dependencies, the error now appears and the code is unchanged otherwise. So the most likely root cause is the dotnet update.

Steps to reproduce:
* Ensure that dotnet 5.0.301 is installed
* run `dotnet tool restore`
* run `dotnet fake build`

The target `Test1` now successfully runs the tests, while `Test2` failes with an exception:

```
Unbehandelte Ausnahme: System.IO.FileLoadException: Die Datei oder Assembly "nunit.engine.api, Version=3.0.0.0, Culture=neutral, PublicKeyToken=2638cd05610744eb" oder eine Abhängigkeit davon wurde nicht gefunden. Die Anbieter-DLL wurde nicht richtig initialisiert. (Ausnahme von HRESULT: 0x8009001D)
```

(translates to: Provider DLL failed to initialize correctly)
