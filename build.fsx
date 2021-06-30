#r "paket:
source https://api.nuget.org/v3/index.json
nuget Fake.Core.Process
nuget Fake.Core.Target
nuget Fake.DotNet.Cli
nuget Fake.DotNet.MSBuild
nuget Fake.DotNet.Paket
nuget Fake.DotNet.Testing.NUnit
"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing
open Fake.IO.Globbing.Operators
open Fake.DotNet
open Fake.DotNet.Testing

let buildDir = "build"
let unitTestDir = buildDir </> "unit-test"


Target.create "Clean" (fun _ ->
    Shell.cleanDirs [buildDir;]
)

Target.create "Restore" (fun _ ->
    Paket.restore (fun param -> {param with ToolType=ToolType.CreateLocalTool()})
    DotNet.restore id "nunit-bug.sln"
)

Target.create "Compile" (fun _ ->
    !! "nunit-bug.sln"
    |> MSBuild.runRelease id  unitTestDir "Build"
    |> Trace.logItems "AppBuild-Output: "
)

Target.create "Test1" (fun _ ->
    let testAssemblies = !! (unitTestDir </> "TestProject1.dll")

    let args = Arguments.Empty
               |> Arguments.append ["--noheader"; @"--result=build\unit-test\TestResults.xml"]
               |> Arguments.append testAssemblies

    CreateProcess.fromRawCommand @"packages\NUnit.ConsoleRunner\tools\nunit3-console.exe" (Arguments.toArray args)
    |> CreateProcess.ensureExitCode
    |> Proc.run // start with the above configuratio
    |> ignore
)

Target.create "Test2" (fun _ ->
    !! (unitTestDir </> "TestProject1.dll")
    |> NUnit3.run (fun p -> {p with ResultSpecs = [unitTestDir </> "TestResults.xml"] })
)

open Fake.Core.TargetOperators

"Clean"
==> "Restore"
==> "Compile"
==> "Test1"
==> "Test2"

Target.runOrDefault "Test2"