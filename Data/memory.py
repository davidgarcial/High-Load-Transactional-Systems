# Plot the data with updated colors (darker yellow to orange)
plt.figure(figsize=(12, 6))

# Plot Rust memory usage in blue
plt.plot(rust_memory['Time'], rust_memory['bankingsystemrust'], label='Rust', color='blue', marker='o')

# Plot C# memory usage in red
plt.plot(csharp_memory['Time'], csharp_memory['bankingsystemcsharp'], label='C#', color='red', marker='o')

# Plot C++ memory usage in darker yellow (orange)
plt.plot(cpp_memory['Time'], cpp_memory['bankingsystemc'], label='C++', color='orange', marker='o')

# Adding titles and labels
plt.title('Memory Usage Over Time (Interval: 5 Minutes)')
plt.xlabel('Time (Minutes)')
plt.ylabel('Memory Usage (MB)')
plt.legend(loc='upper left')
plt.grid(True)

# Show the plot
plt.savefig('/mnt/data/Memory_Usage_Comparison_All_MB_Colors_Updated.png')
plt.show()
