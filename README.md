# Tweet 'n Beat

A small Unity 2D Flappy Bird like game showcasing applied C# & SE design patterns — concise, not just a clone.

## Showcase
- **Event‑driven flow:** `GameStateManager` broadcasts C# events; UI/audio/gameplay stay decoupled.
- **Data‑driven difficulty:** score → speed, gap, spawn cadence; slomo preserves world spacing (*speed × interval*).
- **Deterministic spawner:** vertical band + safe‑gap math; at most one collectible per pair.
- **Feel & correctness:** input in Update → actions in FixedUpdate; variable‑hold jump, gravity states, soft top clamp, velocity tilt.
- **Audio polish:** SFX library, score pitch‑up, tweened filters/reverb per state (DOTween).

## Patterns
- **Singleton** services (AudioManager, GameStateManager)
- **Observer** via C# events (state, score)
- **Strategy** with **ScriptableObjects** (CollectibleEffect: Heal/Life/Slomo)
- **Factory/Spawner** (PipesSpawner → PipePair + optional collectible)
- **Composition** via interfaces (IDamageable, IFreezable)

Run: open SampleScene → Play. Tap/click/space to jump.
