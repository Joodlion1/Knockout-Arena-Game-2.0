# Knockout Arena — Creative Core Polish (Assignment 3)

**Course:** SWE 402 — Game Development
**Project:** Knockout Arena
**Scene:** `Assets/Scenes/Prototype 4.unity`
**Unity version:** Unity 6.x (URP 17.3)

This document describes the six Creative Core enhancements added on top of the original Knockout Arena worksheet. Each section explains what was implemented, the GameObject(s) and script(s) it involves, and how to see the effect in action when the project is run.

---

## Section 1 — Shaders and Materials

**What:** A runtime material enhancement that pulses an emissive red/orange tint on every Enemy *only while the player has the powerup active*. It telegraphs to the player that a knockout strike is currently armed.

**GameObjects / Scripts:**
- `Assets/Scripts/EnemyEmissionTint.cs` — attached to the **Enemy** prefab (`Assets/Prefabs/Enemy.prefab`). Uses `Renderer.material` (auto-instanced per enemy) and writes to the URP/Lit shader's `_EmissionColor` property each frame, with the `_EMISSION` keyword enabled at Start. Reads `PlayerController.HasPowerup` (newly exposed property on `Assets/Scripts/PlayerController.cs`).

**How to see it:** Press Play, choose any difficulty, then collect a powerup. Every enemy on the arena will pulse a warm red glow for the 7-second powerup duration and return to its normal albedo when the powerup ends.

---

## Section 2 — Lighting

**What:** Two lighting enhancements:
1. The scene's Directional Light dynamically dims and shifts from a warm cream tone toward a colder blueish tone as the wave count rises (mood/tension build), with a subtle Perlin-noise flicker on top.
2. Each spawned Powerup carries its own flickering Point Light, drawing the eye to the pickup.

**GameObjects / Scripts:**
- `Assets/Scripts/DynamicWaveLighting.cs` — attached to the scene's **Directional Light**. Listens to a new `GameManager.OnWaveChanged` event (added in `Assets/Scripts/GameManager.cs`).
- `Assets/Scripts/PowerupGlow.cs` — attached to the **Powerup** prefab (`Assets/Prefabs/Powerup.prefab`). It creates a child `PowerupPointLight` GameObject at runtime with a flickering `Light` component (Point type).

**How to see it:** Play the game and watch the directional light gradually cool/dim across waves 1 → 6. Each Powerup spawn should have a glowing yellow-orange light flickering around it from a few units away.

---

## Section 3 — Animation

**What:** Two code-driven animation enhancements:
1. The "Wave: N" UI text plays a scale-punch + yellow color flash each time a new wave begins.
2. The Powerup Indicator beneath the player spins, bobs, and pulses while the powerup is active, then snaps back to its rest pose when the powerup ends.

**GameObjects / Scripts:**
- `Assets/Scripts/WaveTextAnimator.cs` — attached to the **Wave Text** TMP element under the Canvas. Hooks `GameManager.OnWaveChanged` and runs a coroutine that lerps `localScale` and `text.color` from a punched state back to the rest state.
- `Assets/Scripts/PowerupIndicatorAnimator.cs` — attached to the **Powerup Indicator** GameObject. Drives `transform.Rotate`, a `Mathf.Sin`-based world-space bob in `LateUpdate` (so it applies after `PlayerController` repositions it each frame), and a pulsing scale.

**How to see it:** On game start the wave text "punches" with a yellow flash; same on every wave change. Collect a powerup — the indicator under the player will visibly spin, bob, and pulse for the 7-second powerup duration.

---

## Section 4 — VFX

**What:** Two new Particle Systems added beyond the original `KnockoutEffect`, both controlled from code with `.Play()` / `.Stop()`:
1. A **movement trail** of soft blue particles emitted while the player is moving and stopped when the player is stationary.
2. A **powerup-collection ring burst** — a one-shot radial ring of warm yellow particles that bursts outward from the player exactly when a powerup is picked up.

**GameObjects / Scripts:**
- `Assets/Scripts/PlayerMovementTrail.cs` — attached to the **Player**. Builds its child `MovementTrail` ParticleSystem fully in code (rate-over-distance emission, world-space simulation, lifetime fade). Each Update it checks `Rigidbody.linearVelocity.magnitude` and toggles `.Play()` / `.Stop()` based on a speed threshold.
- `Assets/Scripts/PowerupCollectBurst.cs` — attached to the **Player**. Builds its child `PowerupBurst` ParticleSystem in code (Circle shape, single burst, looped off, `playOnAwake = false`, GameObject created inactive while configuring to prevent the auto-play one-frame flash). Exposes a public `Play()` method that `PlayerController.OnTriggerEnter` invokes when the player picks up a powerup.

**How to see it:** Move with W / S — a soft blue trail follows the player and disappears when stationary. Walk into a powerup — a yellow ring of particles bursts outward exactly once.

---

## Section 5 — Cameras

**What:** Two camera enhancements beyond the existing focal-point orbit:
1. **Camera shake** on every enemy knockout — applied as a localPosition offset that decays over time.
2. **Dynamic zoom** while the player is powered up — the camera smoothly pulls outward (orthographic size grows from 10 → 12) for the powerup duration and eases back when it ends. The same script also handles perspective cameras via FOV; it auto-detects the projection mode of the attached camera.

**GameObjects / Scripts:**
- `Assets/Scripts/CameraEffects.cs` — attached to the **Main Camera**. Subscribes to `GameManager.OnEnemyKnockedOff` to trigger shake. In `LateUpdate` it lerps `orthographicSize` (or `fieldOfView` for perspective cameras) toward a "powered" or "base" target each frame based on `PlayerController.HasPowerup`, and applies a decaying random offset to `transform.localPosition` while shake intensity is non-zero.

**How to see it:** Knock an enemy off the platform — the camera does a brief shake. Collect a powerup — the camera smoothly zooms out (more arena visible) for ~7 seconds, then eases back when the powerup ends.

---

## Section 6 — Post-Processing

**What:** A URP **Global Volume** with **Bloom** + **Vignette** overrides created entirely at runtime, plus a gameplay-driven override: while the player has the powerup, the vignette intensity grows and its color shifts from black to a warm orange, framing the screen in a "powered-up" mood.

**GameObjects / Scripts:**
- `Assets/Scripts/PostProcessingController.cs` — attached to a new empty GameObject named **Global Volume**. On Awake it adds a `Volume` component (`isGlobal = true`), creates a runtime `VolumeProfile`, and adds Bloom + Vignette overrides to it. Each frame it lerps `vignette.intensity.value` and `vignette.color.value` between idle (black, 0.25) and powered-up (orange, 0.50) targets based on `PlayerController.HasPowerup`.
- **Main Camera → Camera component → Rendering → Post Processing** must be enabled for the volume effects to render.

**How to see it:** On Play, the screen edges immediately have a soft dark vignette and emissive things (Section 1's red enemy glow, Section 2's powerup point light) bloom. When you grab a powerup, the vignette pulls in tighter and shifts to a warm orange tone for the 7-second powerup, then eases back to black/subtle when the powerup ends.

---

## Running the Project

1. Open the project folder `Knockout Arena Game/` in Unity 6.x (URP project).
2. Open `Assets/Scenes/Prototype 4.unity`.
3. Press Play. Pick a difficulty (Easy / Medium / Hard) on the title screen.
4. **Controls:** `W` / `S` — move forward/back. `A` / `D` — orbit the camera (focal point).
5. Knock enemies off the platform; collect powerups to enable powered-up knockouts.

## Files Added / Modified for This Assignment

**New scripts:**
- `Assets/Scripts/EnemyEmissionTint.cs` (Section 1)
- `Assets/Scripts/DynamicWaveLighting.cs` (Section 2)
- `Assets/Scripts/PowerupGlow.cs` (Section 2)
- `Assets/Scripts/WaveTextAnimator.cs` (Section 3)
- `Assets/Scripts/PowerupIndicatorAnimator.cs` (Section 3)
- `Assets/Scripts/PlayerMovementTrail.cs` (Section 4)
- `Assets/Scripts/PowerupCollectBurst.cs` (Section 4)
- `Assets/Scripts/CameraEffects.cs` (Section 5)
- `Assets/Scripts/PostProcessingController.cs` (Section 6)

**Modified scripts:**
- `Assets/Scripts/PlayerController.cs` — exposed `HasPowerup` property; calls `PowerupCollectBurst.Play()` on powerup pickup.
- `Assets/Scripts/GameManager.cs` — added `OnWaveChanged` event raised at game start (wave 1) and on every wave increment.
