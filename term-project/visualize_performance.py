#!/usr/bin/env python3
"""Performance test visualization for Unity artillery game."""

import matplotlib.pyplot as plt
import seaborn as sns
import pandas as pd

# Extracted data from Unity console logs
data = {
    'Blocks': [10, 20, 30, 40, 50],
    'PhysicsObjects': [40, 51, 61, 70, 80],
    'FPS': [479.5, 541.4, 515.0, 436.5, 461.1],
    'GCAlloc_KB': [292, 324, 64, -56, 204],
    'Time_ms': [998.0, 995.6, 995.4, 996.4, 997.8],
}

df = pd.DataFrame(data)

# Set style
sns.set_style('darkgrid')
sns.set_palette('husl')

# Create figure with 3 subplots
fig, axes = plt.subplots(1, 3, figsize=(15, 5))

# Plot 1: Blocks vs FPS
axes[0].plot(df['Blocks'], df['FPS'], 'o-', linewidth=2, markersize=8, color='#2ecc71')
axes[0].fill_between(df['Blocks'], df['FPS'], alpha=0.3, color='#2ecc71')
axes[0].set_xlabel('Number of Blocks (Independent Variable)', fontsize=11, fontweight='bold')
axes[0].set_ylabel('FPS (Performance Metric)', fontsize=11, fontweight='bold')
axes[0].set_title('Effect of Block Count on FPS', fontsize=13, fontweight='bold')
axes[0].grid(True, alpha=0.3)
for i, (x, y) in enumerate(zip(df['Blocks'], df['FPS'])):
    axes[0].annotate(f'{y:.1f}', (x, y), textcoords='offset points', xytext=(0, 10), ha='center', fontsize=9)

# Plot 2: Physics Objects vs FPS
axes[1].plot(df['PhysicsObjects'], df['FPS'], 's-', linewidth=2, markersize=8, color='#3498db')
axes[1].fill_between(df['PhysicsObjects'], df['FPS'], alpha=0.3, color='#3498db')
axes[1].set_xlabel('Physics Objects Count', fontsize=11, fontweight='bold')
axes[1].set_ylabel('FPS (Performance Metric)', fontsize=11, fontweight='bold')
axes[1].set_title('Effect of Physics Objects on FPS', fontsize=13, fontweight='bold')
axes[1].grid(True, alpha=0.3)
for i, (x, y) in enumerate(zip(df['PhysicsObjects'], df['FPS'])):
    axes[1].annotate(f'{y:.1f}', (x, y), textcoords='offset points', xytext=(0, 10), ha='center', fontsize=9)

# Plot 3: Blocks vs GC Allocation
axes[2].bar(df['Blocks'], df['GCAlloc_KB'], color='#e74c3c', alpha=0.7, edgecolor='#c0392b', linewidth=1.5)
axes[2].set_xlabel('Number of Blocks (Independent Variable)', fontsize=11, fontweight='bold')
axes[2].set_ylabel('GC Allocation (KB)', fontsize=11, fontweight='bold')
axes[2].set_title('Memory Allocation vs Block Count', fontsize=13, fontweight='bold')
axes[2].grid(True, alpha=0.3, axis='y')
for i, (x, y) in enumerate(zip(df['Blocks'], df['GCAlloc_KB'])):
    axes[2].annotate(f'{y:.0f}', (x, y), textcoords='offset points', xytext=(0, 10 if y > 0 else -15), ha='center', fontsize=9)

plt.tight_layout()
plt.savefig('performance_analysis.png', dpi=300, bbox_inches='tight')
plt.savefig('performance_analysis.pdf', bbox_inches='tight')
print('✓ Saved: performance_analysis.png, performance_analysis.pdf')

# Print summary statistics
print('\n=== PERFORMANCE SUMMARY ===')
print(f'Average FPS: {df["FPS"].mean():.1f}')
print(f'FPS Std Dev: {df["FPS"].std():.1f}')
print(f'FPS Range: {df["FPS"].min():.1f} - {df["FPS"].max():.1f}')
print(f'\nGC Alloc Range: {df["GCAlloc_KB"].min():.0f} - {df["GCAlloc_KB"].max():.0f} KB')
print(f'Physics Objects Range: {df["PhysicsObjects"].min()} - {df["PhysicsObjects"].max()}')
print('\n✓ Graph shows interaction between block count (x-axis) and FPS/GC (y-axis)')
