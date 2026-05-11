CMPE 485 Term Project – Phase I

Student: Asaf Kanlıpıçak 2020400195

Option: B (Simple Game)

- Game: 3D artillery. Aim cannon (yaw+pitch), set power, fire at stacked-block castle.
- Cannonballs apply impact force, explosions apply radial force. Targets destroyed by
high-velocity hits or nearby explosions. Limited ammo.
-Win: destroy all targets. Lose: out of ammo.
-Optional: show previous shot trajectory.
-Software Design: Unity 3D + PhysX. GameManager (FSM), CannonController
(input→velocity), Projectile (Rigidbody), ExplosionManager (OverlapSphere +
AddExplosionForce).

Technical Challenges:
1. 3D Physics Stability (stack jitter) → tune SleepThreshold, collision mode.
2. Radial Explosion Logic → clamp force, distance-based damage.
3. Game State Sync → coroutine FSM + events.


- Profiler Metrics: FPS during explosion, Physics CPU time, GC Alloc per shot.
   
- Assets: Kenney.nl, Unity ProBuilder, default VFX/audio.