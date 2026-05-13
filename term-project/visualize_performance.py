import matplotlib.pyplot as plt
import pandas as pd

physics = pd.read_csv('performance_log.csv')
optimized = pd.read_csv('performance_log_optimized.csv')

plt.figure(figsize=(10, 6))

plt.plot(physics['blocks'], physics['fps'], 'r-o', label='Physics Query', linewidth=2, markersize=8)
plt.plot(optimized['blocks'], optimized['fps'], 'c-o', label='Cached Iteration', linewidth=2, markersize=8)

plt.xlabel('Block Count', fontsize=12)
plt.ylabel('FPS During Explosion', fontsize=12)
plt.title('Explosion Performance: Physics Query vs Cached Iteration', fontsize=14)
plt.legend(fontsize=11)
plt.grid(True, alpha=0.3)

plt.xticks(physics['blocks'])

plt.tight_layout()
plt.savefig('performance_comparison.png', dpi=150)
plt.show()

print("Saved to performance_comparison.png")