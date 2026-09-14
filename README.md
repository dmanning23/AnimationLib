# AnimationLib

A skeletal-based 2D animation system for [MonoGame](https://www.monogame.net/). AnimationLib builds a hierarchical skeleton of bones and joints, drives it with keyframed animations, and can dress it with swappable garments (clothes, armor, weapons, etc.) — all loadable from XML or JSON content files.

It also includes an optional Verlet-integration ragdoll/physics layer, so a skeleton can blend scripted keyframe animation with physics-driven limb movement (e.g. a cape or ponytail that reacts to gravity).

## Features

- **Skeletal hierarchy** — a `Skeleton` of parent/child `Bone`s connected at `Joint`s, each bone carrying one or more `Image`s that can be swapped per-frame.
- **Keyframe animation** — `Animation`s are built from `KeyElement`s (image, layer, rotation, translation, flip, ragdoll on/off) recorded per joint over time, played back forwards, backwards, looping, or randomized (`EPlayback`).
- **Garments** — `Garment`/`GarmentFragment` let you layer separate, independently-loaded pieces (clothing, weapons, accessories) onto a base skeleton without modifying the base model.
- **Physics/ragdoll** — `PhysicsCircle`/`PhysicsLine` and Verlet integration on bones for gravity, spring limits, and rag-doll style limb behavior.
- **Normal Mapping** — Allows the use of normal maps to light your characters
- **Color tagging** — `ColorRepository` lets you tint tagged parts of a model (skin, hair, eyes, etc.) at runtime.
- **Undo/redo aware** — animation and garment edits go through [UndoRedoBuddy](https://github.com/dmanning23/UndoRedoBuddy) `Command`s, making the library friendly to an in-game or tool-based editor.
- **XML and JSON content** — every model, animation, and garment can be read and written as XML or JSON.

## Installation

AnimationLib is distributed as a NuGet package targeting `net8.0`:

```
dotnet add package AnimationLib
```

## Requirements

- .NET 8.0
- MonoGame (`MonoGame.Framework.DesktopGL`, 3.8.x)

AnimationLib also depends on several other packages from the same author, all pulled in automatically via NuGet:

| Package | Purpose |
|---|---|
| [CameraBuddy](https://github.com/dmanning23/CameraBuddy) | Camera/viewport management |
| [CollisionBuddy](https://github.com/dmanning23/CollisionBuddy) | Circle/line collision primitives |
| [DrawListBuddy](https://github.com/dmanning23/DrawListBuddy) | Deferred/sorted sprite draw lists |
| [FilenameBuddy](https://github.com/dmanning23/FilenameBuddy) | Content-relative filename handling |
| [GameTimer](https://github.com/dmanning23/GameTimer) | Frame-independent game clock |
| [MatrixExtensions](https://github.com/dmanning23/MatrixExtensions) | Transform matrix helpers |
| [PrimitiveBuddy](https://github.com/dmanning23/PrimitiveBuddy) | Debug primitive rendering |
| [RandomExtensions](https://github.com/dmanning23/RandomExtensions) | Random number helpers |
| [RenderBuddy](https://github.com/dmanning23/RenderBuddy) | Lighting/rendering abstraction (`IRenderer`) |
| [ResolutionBuddy](https://github.com/dmanning23/ResolutionBuddy) | Resolution-independent rendering |
| [UndoRedoBuddy](https://github.com/dmanning23/UndoRedoBuddy) | Undo/redo command stack |
| [Vector2Extensions](https://github.com/dmanning23/Vector2Extensions) | Vector2 helper methods |
| [XmlBuddy](https://github.com/dmanning23/XmlBuddy) | XML content serialization |

## Repository layout

```
AnimationLib/            The library (this is what ships as the NuGet package)
AnimationLib.Tests/      NUnit test suite, with sample skeleton/animation content
AnimationTest/           A runnable MonoGame sample app that loads and plays a character
```

## Quick start

The typical flow is: create an `AnimationContainer`, load a skeleton file, load an animations file, optionally add garments, then update/render it each frame.

```csharp
using AnimationLib;
using FilenameBuddy;

// renderer implements RenderBuddy.IRenderer and is used to load images
var animations = new AnimationContainer(scale: 1f);

var modelFile = new Filename(@"Content\Character\CharacterModel.xml");
var animFile = new Filename(@"Content\Character\CharacterAnimations.xml");

animations.ReadSkeletonXml(modelFile, renderer);
animations.ReadAnimationXml(animFile);

// optionally layer a garment onto the loaded skeleton
var garmentFile = new Filename(@"Content\Character\Clothes\Shirt.xml");
var garment = new Garment(garmentFile, animations.Skeleton, renderer);
garment.AddToSkeleton();

animations.SetAnimation("Walk", EPlayback.Loop);
```

Each frame:

```csharp
animations.Update(clock, position, isFlipped, rotation, ignoreRagdoll: false);
animations.UpdateRagdoll(); // only needed if the model uses physics/ragdoll bones
animations.Render(drawList);
```

`AnimationsLoader` wraps this same pattern (skeleton + animations + garments + additional animation sets) if you'd rather work with one loader object per character. Loading JSON content instead of XML uses the `*Json` equivalents (`ReadSkeletonJson`, `ReadAnimationJson`, `SaveJson`, etc.).

### Playback modes

`EPlayback` controls how an animation's timeline is read each update:

- `Forwards` — play once, clamp at the end
- `Backwards` — play once in reverse, clamp at the start
- `Loop` — repeat forwards indefinitely
- `LoopBackwards` — repeat in reverse indefinitely
- `LoopRandom` — loop forwards starting from a random offset

## Content format

Skeletons, animations, and garments are serialized as either XML (`XnaContent`/`AnimationLib.*XML` assets, MonoGame content-pipeline style) or JSON. A skeleton file describes the bone hierarchy — each bone's joints, images, and physics data (attachment points, rotation limits, collision circles/lines). An animations file describes one or more named animations as a list of keyframes per joint (time, image, layer, rotation, translation, flip, ragdoll flag). See [AnimationLib.Tests/Content/SuperSimple](AnimationLib.Tests/Content/SuperSimple) for a minimal working example of both.

## Sample app

[AnimationTest](AnimationTest/Game1.cs) is a small MonoGame program that loads a sample character and lets you interact with it:

| Key | Action |
|---|---|
| `A` / `Z` | Next / previous animation |
| `Q` / `W` / `E` / `R` / `T` | Set playback mode: Forwards / Backwards / Loop / LoopBackwards / LoopRandom |
| `X` / `C` | Rotate the model |
| `F` | Flip the model |
| `P` | Toggle joint-skeleton overlay |
| `O` | Toggle physics overlay |
| `G` | Spawn a flare light |
| Arrow keys / IJKL | Move the two point lights |
| `Esc` | Quit |

Run it with:

```
dotnet run --project AnimationTest
```

## Building the NuGet package

`AnimationLib/build.sh` cleans, builds, packs, and pushes the package to nuget.org (requires `NUGET_API_KEY` in the environment). For a local build without publishing:

```
dotnet build AnimationLib/AnimationLib.csproj --configuration Release
dotnet pack AnimationLib/AnimationLib.csproj --configuration Release --no-build
```

## Testing

The test suite in `AnimationLib.Tests` uses NUnit and Shouldly, and exercises skeleton/animation/garment loading against the sample content in `AnimationLib.Tests/Content`.

```
dotnet test AnimationLib.Tests
```

> **Note:** `AnimationLib.Tests.csproj` is currently a legacy .NET Framework project file that imports `AnimationLib.SharedProject/AnimationLib.SharedProject.projitems` and a `packages/` folder that aren't present in this repository, so it won't build as-is. It (along with `AnimationTest`) is also not yet added to `AnimationLib.sln`. These need to be modernized/reconnected before the suite can run.

## License

MIT — see the [PackageLicenseExpression](AnimationLib/AnimationLib.csproj) in the project file.
