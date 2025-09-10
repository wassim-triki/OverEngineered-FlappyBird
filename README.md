# Tweet ’n Beat

A small Unity 2D, Flappy Bird–like game showcasing applied C# and software engineering design patterns — concise, not just a clone. Clean visuals, nice feedback, and a catchy soundtrack tie it all together.

## How to Play

- 🕹️ **Browser:** [Play on Itch.io](https://wassim-triki.itch.io/tweet-n-beat)
- 📱 **Android:** [Download the APK](https://github.com/wassim-triki/Tweet-n-Beat-Unity-2D/releases) from this repository’s Releases and play it on your device.

## Demo
<img src="./tweetnbeat-gif.gif" width="20%"/>

## Under the Hood
- **Event-driven flow:** `GameStateManager` broadcasts C# events; UI, audio, and gameplay stay decoupled.
- **Data-driven difficulty:** score → speed, gap, spawn cadence; slomo preserves world spacing (*speed × interval*).
- **Deterministic spawner:** vertical band + safe-gap math; at most one collectible per pair.
- **Feel & correctness:** input in Update → actions in FixedUpdate; variable-hold jump, gravity states, soft top clamp, velocity tilt.
- **Audio polish:** SFX library, score pitch-up, DOTween-driven filters/reverb per state, and a nice looping soundtrack for atmosphere.

## Patterns
- **Singleton** services (`AudioManager`, `GameStateManager`)
- **Observer** via C# events (state, score)
- **Strategy** with **ScriptableObjects** (`CollectibleEffect`: Heal / Life / Slomo)
- **Factory/Spawner** (`PipesSpawner` → `PipePair` + optional collectible)
- **Composition** via interfaces (`IDamageable`, `IFreezable`)

---
**Run:** Open `Scene` → Play. Tap / click / space to jump.
