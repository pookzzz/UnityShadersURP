# Realtime Effects Lab — Sample 001: Holographic Dissolve Card

Small, shippable Unity URP shader prototype focused on a game-card reveal / materialization beat.

## What is implemented

- Procedural 4-octave FBM breakup; no texture dependency.
- Animated dissolve threshold with a bright emissive transition edge.
- Fresnel-driven holographic rim.
- Moving scanline modulation and subtle two-color holo shift.
- Transparent, double-sided URP pass suitable for a thin card mesh.
- `MaterialPropertyBlock` animation so the demo does not instantiate materials at runtime.
- One-click editor scene builder that creates a camera, card, two accent lights, ground plane, demo materials, and a saved demo scene.

## Demo

Project baseline: Unity 2021.3.10f1 / URP 12.1.7.

1. Open the project and allow shaders/scripts to compile.
2. Run **Realtime Effects Lab → Sample 001 → Create Holo Dissolve Demo**.
3. Unity saves the generated scene to `Generated/HoloDissolveCard_Demo.unity` and opens it.
4. Press **Play**. The card rotates slowly while the dissolve loops between a readable materialized state and a broken-up holographic state.
5. Tweak the generated card material to art-direct the result.

## Useful controls

| Property | Role |
| --- | --- |
| `_Dissolve` | Main reveal / breakup threshold |
| `_EdgeWidth` | Width of the hot transition band |
| `_NoiseScale` | Chunk size of the procedural breakup |
| `_NoiseSpeed` | Drift speed through the noise field |
| `_HoloColor` | Primary cyan/blue holographic contribution |
| `_EdgeColor` | Hot dissolve edge and secondary hue |
| `_ScanlineDensity` | Frequency of horizontal holo lines |
| `_ScanlineStrength` | Visibility of scanlines |
| `_FresnelPower` | Tightness of view-angle rim |
| `_Emission` | Overall VFX punch |

## Progress capture — 2026-08-31

**Goal:** leave one Effects Lab sample in a demonstrable state rather than expanding into a larger card-opening system.

**Completed:** shader core, runtime loop controller, reusable material controls, and one-click demo-scene generation.

**Intentional scope cuts:** no particles, no texture authoring, no bloom configuration changes, no Timeline, no card artwork/UI, no pack-tearing animation, and no VFX Graph dependency. Those can become later samples instead of blocking this one.

**Demo state:** code and scene-generation path are complete in-repo. The final visual capture should be made after opening the branch in Unity so the exact URP/device output is represented rather than approximated outside the engine.

## Next tiny extension ideas

Keep these separate from Sample 001 unless needed for polish:

- Add a directional dissolve bias so the card reveals bottom-to-top.
- Feed a card mask/illustration into emission without changing the procedural edge logic.
- Add a single burst of particles at the dissolve frontier.
- Turn the shader into the reveal beat for the existing pack/card prototype.
