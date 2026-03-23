# Aspire 13.2 Update Notes

This document describes the changes made to update all chapters to .NET Aspire 13.2 and the results of the build verification.

## Changes Made

### 1. `Directory.Packages.props` (root)
Updated all .NET Aspire NuGet package versions to 13.2.0:
- `Aspire.AppHost.Sdk`: `13.0.0` → `13.2.0` (in all AppHost project files)
- All `Aspire.*` packages at `13.0.0` → `13.2.0`
- `Aspire.Hosting.Azure.AppService`: `13.0.0-preview.1.25560.3` → `13.2.0` (now stable)
- `Aspire.Hosting.Docker`: `13.0.0-preview.1.25560.3` → `13.2.0` (now stable)
- `Aspire.Hosting.Keycloak`: `9.2.1-preview.1.25222.1` → `13.2.0-preview.1.26170.3`
- `Aspire.Hosting.Kubernetes`: `9.2.1-preview.1.25222.1` → `13.2.0-preview.1.26170.3`
- `Aspire.Microsoft.Azure.Cosmos`: `9.3.0` → `13.2.0`
- `Aspire.MongoDB.Driver.v3`: `9.4.0` → `9.5.2` (no 13.x version available)
- `Aspire.Npgsql.EntityFrameworkCore.PostgreSQL`: `9.5.2` → `13.2.0`
- `Aspire.StackExchange.Redis`: `9.4.0` → `13.2.0`
- `CommunityToolkit.Aspire.Hosting.OpenTelemetryCollector`: `13.0.0-beta.439` → `13.2.1-beta.532`
- `Azure.Identity`: `1.17.0` → `1.18.0` (required by `Aspire.Azure.Storage.Queues 13.2.0`)

### 2. AppHost SDK version updated in all AppHost projects
All projects using `Sdk="Aspire.AppHost.Sdk/13.0.0"` were updated to `Sdk="Aspire.AppHost.Sdk/13.2.0"`.

### 3. ch13 and ch14: `AppHostPrometheus` projects migrated
- `ch13/Codebreaker.AppHostPrometheus/Codebreaker.AppHostPrometheus.csproj`
- `ch14/Codebreaker.AppHostPrometheus/Codebreaker.AppHostPrometheus.csproj`

These projects used the legacy Aspire host format (`Sdk="Microsoft.NET.Sdk"` with `IsAspireHost=true` and direct `Aspire.Hosting` 8.0.0-preview package references on `net8.0`). They were migrated to the modern format using `Sdk="Aspire.AppHost.Sdk/13.2.0"` on `net10.0`.

### 4. ch04: Fixed case mismatch in solution and project references
- `ch04/Chapter04.Client.slnx`: Fixed reference from `Codebreaker.GameApis.KiotaClient` → `Codebreaker.GameAPIs.KiotaClient`
- `ch04/client/Codebreaker.KiotaConsole/Codebreaker.KiotaConsole.csproj`: Fixed project reference case

## Build Verification Results

| Chapter / Project | Build Result | Notes |
|---|---|---|
| ch01/Aspire (`AspireSample.slnx`) | ✅ Succeeded | |
| ch02 | ✅ Succeeded | |
| ch03 | ✅ Succeeded | |
| ch04 Server (`Chapter04.Server.slnx`) | ✅ Succeeded | |
| ch04 Client (`Chapter04.Client.slnx`) | ✅ Succeeded | Fixed case mismatch in solution and project references |
| ch05/FinalAspire | ✅ Succeeded | |
| ch05/FinalDocker | ✅ Succeeded | |
| ch05/NativeAOT | ✅ Succeeded | |
| ch05/StartAspire | ✅ Succeeded | |
| ch05/StartDocker | ❌ Failed | Pre-existing issue (see below) |
| ch06 | ✅ Succeeded | |
| ch07 | ✅ Succeeded | |
| ch08 (`Chapter08.slnx`, `ClientLib.slnx`) | ✅ Succeeded | |
| ch09 (`Chapter9.slnx`) | ✅ Succeeded | |
| ch09/BlazorWASMAuth | ❌ Failed | Pre-existing issue (see below) |
| ch10/Start | ✅ Succeeded | |
| ch10/Final | ❌ Failed | Pre-existing issues (see below) |
| ch11 | ✅ Succeeded | |
| ch12 | ✅ Succeeded | |
| ch13 | ✅ Succeeded | |
| ch14 | ✅ Succeeded | |
| ch15 | ✅ Succeeded | |
| ch16 | ✅ Succeeded | |

## Pre-existing Compilation Issues

The following issues existed before the Aspire 13.2 update and are not caused by it:

### ch05/StartDocker — `Codebreaker.GameAPIs`
**Error:** `CS0234: The type or namespace name 'Models' does not exist in the namespace 'Microsoft.OpenApi'`  
**File:** `ch05/StartDocker/Codebreaker.GameAPIs/Program.cs:5`  
**Cause:** `using Microsoft.OpenApi.Models;` is no longer valid with `Swashbuckle.AspNetCore 10.x` which moved to `Microsoft.OpenApi v2.x`, where types were reorganized. The code is using the old namespace.

### ch09/BlazorWASMAuth
**Error:** `NU1008: Projects using Central Package Management must define a Version value on a PackageVersion item`  
**Files:** `BlazorWASMAuth.csproj`, `BlazorWASMAuth.Client.csproj`  
**Cause:** These projects define `Version=` on `PackageReference` items, which conflicts with Central Package Management (CPM). Either the projects should opt out of CPM (as ch01 does via a local `Directory.Packages.props`) or the version attributes should be removed from the `PackageReference` items.

### ch10/Final — `Codebreaker.GameAPIs` and `CodeBreaker.Bot`
**Errors:**
1. `CS0234: The type or namespace name 'Models' does not exist in the namespace 'Microsoft.OpenApi'`  
   **File:** `ch10/Final/Codebreaker.GameAPIs/Program.cs:4`  
   **Cause:** Same as ch05/StartDocker — outdated `Microsoft.OpenApi.Models` namespace.
2. `CS1002: ; expected`  
   **File:** `ch10/Final/CodeBreaker.Bot/Endpoints/BotEndpoints.cs:48`  
   **Cause:** Syntax error in source code.
