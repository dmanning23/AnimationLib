# AnimationLib

A skeletal-based 2D animation system for [MonoGame](https://www.monogame.net/). AnimationLib builds a hierarchical skeleton of bones and joints, drives it with keyframed animations, and can dress it with swappable garments (clothes, armor, weapons, etc.) — all loadable from XML or JSON content files.

It also includes an optional Verlet-integration ragdoll/physics layer, so a skeleton can blend scripted keyframe animation with physics-driven limb movement (e.g. a cape or ponytail that reacts to gravity).

## Where to start

| If you want to… | Look at |
|---|---|
| See the minimum code needed to load a character and play an animation | [AnimationLib.Example](AnimationLib.Example/Game1.cs) — a bare-bones MonoGame game, heavily commented |
| See the more advanced features in action: dynamic lighting, switching animations and playback modes, the Garment system, ragdoll physics, debug overlays | [AnimationTest](AnimationTest/Game1.cs) — a full-featured sample app |
| Create and edit skeletons, animations, and garments | [AnimationTool](https://github.com/dmanning23/AnimationTool) — the companion editor for authoring AnimationLib content |

The recommended path is: build your character in **AnimationTool**, copy the pattern from **AnimationLib.Example** to get it on screen, then borrow from **AnimationTest** as you need lighting, garments, and the rest.

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
AnimationLib.Example/    Bare-bones sample: load a character and play one animation
AnimationTest/           Advanced sample: lighting, garments, animation switching, physics overlays
AnimationLib.Tests/      NUnit test suite, with sample skeleton/animation content
```

## Quick start

This is a condensed version of [AnimationLib.Example/Game1.cs](AnimationLib.Example/Game1.cs) — open that file for the complete, runnable version.

You need four things: a `GameClock` to drive time, an `AnimationContainer` for the character, a `DrawList` to collect sprites, and a `Renderer` (from RenderBuddy) to draw them.

```csharp
using AnimationLib;
using DrawListBuddy;
using FilenameBuddy;
using GameTimer;
using RenderBuddy;
using ResolutionBuddy;

GameClock _clock = new GameClock();
AnimationContainer _character = new AnimationContainer();
DrawList _drawList = new DrawList();
Renderer _renderer;
```

**LoadContent** — set up the renderer, then load the skeleton *before* the animations (animations refer to the skeleton's bones):

```csharp
_renderer = new Renderer(this, Content);
_renderer.LoadContent(GraphicsDevice);

// Paths are relative to the Content folder
_character.ReadSkeletonXml(new Filename("MegaMan/MegaManModel.xml"), _renderer, Content);
_character.ReadAnimationXml(new Filename("MegaMan/MegaManAnimations.xml"), Content);

// Animation names come from the animation file
_character.SetAnimation("Walking", EPlayback.Loop);

_clock.Start();
```

**Update** — advance the clock, pose the skeleton, then run ragdoll physics:

```csharp
_clock.Update(gameTime);
_character.Update(_clock, position, isFlipped: false, rotation: 0f, ignoreRagdoll: false);
_character.UpdateRagdoll(); // after Update, every frame
```

**Draw** — render the character into the draw list, then draw the list through the renderer:

```csharp
_drawList.Flush();
_character.Render(_drawList);

_renderer.SpriteBatchBegin(BlendState.AlphaBlend,
    _renderer.Camera.TranslationMatrix * Resolution.TransformationMatrix());
_drawList.Render(_renderer);
_renderer.SpriteBatchEnd();
```

> The Renderer's lighting shader needs `GraphicsProfile.HiDef`, and the sample uses [ResolutionBuddy](https://github.com/dmanning23/ResolutionBuddy)'s `ResolutionComponent` for a fixed virtual resolution. See the Example's constructor for both.

### Adding garments

Garments layer extra pieces (clothes, weapons, accessories) onto an already-loaded skeleton:

```csharp
var garment = new Garment(new Filename("Character/Clothes/Shirt.xml"), _character.Skeleton, _renderer, Content);
garment.AddToSkeleton();
```

[AnimationTest](AnimationTest/Game1.cs) shows loading several garments, removing them, and loading them from JSON.

`AnimationsLoader` wraps this same pattern (skeleton + animations + garments + additional animation sets) if you'd rather work with one loader object per character. Loading JSON content instead of XML uses the `*Json` equivalents (`ReadSkeletonJson`, `ReadAnimationJson`, `SaveJson`, etc.).

### Playback modes

`EPlayback` controls how an animation's timeline is read each update:

- `Forwards` — play once, clamp at the end
- `Backwards` — play once in reverse, clamp at the start
- `Loop` — repeat forwards indefinitely
- `LoopBackwards` — repeat in reverse indefinitely
- `LoopRandom` — loop forwards starting from a random offset

## Content format

The easiest way to create content is with [AnimationTool](https://github.com/dmanning23/AnimationTool), which edits skeletons, animations, and garments visually and saves them in the formats below.

Skeletons, animations, and garments are serialized as either XML (`XnaContent`/`AnimationLib.*XML` assets, MonoGame content-pipeline style) or JSON. A skeleton file describes the bone hierarchy — each bone's joints, images, and physics data (attachment points, rotation limits, collision circles/lines). An animations file describes one or more named animations as a list of keyframes per joint (time, image, layer, rotation, translation, flip, ragdoll flag). See [AnimationLib.Tests/Content/SuperSimple](AnimationLib.Tests/Content/SuperSimple) for a minimal working example of both.

## Sample apps

### AnimationLib.Example — bare bones

[AnimationLib.Example](AnimationLib.Example/Game1.cs) is the smallest possible game that loads a character and loops one animation. Every step is commented; start here.

```
dotnet run --project AnimationLib.Example
```

### AnimationTest — advanced features

[AnimationTest](AnimationTest/Game1.cs) loads a sample character with garments, dynamic point lights (including flickering fire and flare lights), and lets you interact with it:

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

MIT — see [LICENSE](LICENSE).
