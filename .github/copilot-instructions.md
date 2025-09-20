# Copilot Instructions for AshborneCode

## Project Overview
AshborneCode is a multi-project C# solution with the following major components:
- **AshborneGame**: Core game logic, data, and scene management. Contains subfolders for game data, player, globals, and scene management.
- **AshborneWASM**: Blazor WebAssembly frontend. Contains `Pages/`, `Layout/`, and `wwwroot/` for web assets and UI.
- **AshborneConsole**: Console application interface for the game.
- **AshborneScriptWatcher**: Tooling for script watching and automation.
- **AshborneTests**: Test project for unit and integration tests.

## Key Architectural Patterns
- **Core Logic**: Most core logic and data structures are in `AshborneGame/_Core/`.
- **Frontend/Backend Split**: `AshborneWASM` is the Blazor UI, communicating with game logic via shared code or service interfaces.
- **Console/Web Ports**: `ConsolePort/` and `WebPort/` in `AshborneGame` provide platform-specific input/output handlers.
- **Third-Party Integration**: `AshborneGame/ThirdParty/InkRuntime/` contains external narrative scripting engine code.

## Developer Workflows
- **Build**: Use Visual Studio or `dotnet build` from the solution root (`AshborneGame/AshborneGame.sln`).
- **Run WASM**: Launch `AshborneWASM` as a Blazor WebAssembly app (F5 in VS or `dotnet run` in `AshborneWASM/`).
- **Run Console**: Start `AshborneConsole` for CLI-based gameplay/testing.
- **Tests**: Run tests in `AshborneTests/` via Visual Studio Test Explorer or `dotnet test`.
- **Script Watching**: Use `AshborneScriptWatcher` for monitoring and tooling around script files.

## Project-Specific Conventions
- **_Core/**: Shared code and data for both game and frontend live in `_Core/` subfolders.
- **Dialogue/**: Narrative scripts and dialogue data are in `*_Core/Data/Dialogue/` and `wwwroot/Dialogue/`.
- **No direct DB**: Data is file-based, not database-driven.
- **Web assets**: All static web assets are in `AshborneWASM/wwwroot/` and `wwwroot/` at the root.
- **Blazor Pages**: UI pages are in `AshborneWASM/Pages/`.
- **Platform Ports**: Use `ConsolePort/` and `WebPort/` for platform-specific logic.

## Integration & Communication
- **Shared Data**: Game logic is shared between console and web via shared projects and `_Core/`.
- **Ink Integration**: Narrative scripting is handled via Ink (see `ThirdParty/InkRuntime/`).
- **No REST API**: Communication is via direct method calls/shared code, not HTTP APIs.

## Examples
- To add a new game system, place core logic in `AshborneGame/_Core/`, then expose interfaces in both `ConsolePort/` and `WebPort/` as needed.
- To add a new Blazor page, create a `.razor` file in `AshborneWASM/Pages/` and register it in the router.
- To add new dialogue, place Ink files in `*_Core/Data/Dialogue/` and reference them in game logic.

## Key Files & Directories
- `AshborneGame/AshborneGame.sln`: Solution file for building all projects
- `AshborneGame/_Core/`: Core game logic/data
- `AshborneWASM/Pages/`: Blazor UI pages
- `AshborneGame/ConsolePort/`, `AshborneGame/WebPort/`: Platform-specific handlers
- `AshborneGame/ThirdParty/InkRuntime/`: Narrative scripting engine
- `AshborneTests/`: Test project

---

For questions about project structure or conventions, review the above directories or ask a maintainer.
