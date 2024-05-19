# Plot the data with percentages in the y-axis labels
plt.figure(figsize=(12, 6))

# Plot Rust CPU usage in blue
plt.plot(rust_cpu['Time'], rust_cpu['bankingsystemrust'], label='Rust', color='blue', marker='o')

# Plot C# CPU usage in red
plt.plot(csharp_cpu['Time'], csharp_cpu['bankingsystemcsharp'], label='C#', color='red', marker='o')

# Plot C++ CPU usage in orange
plt.plot(cpp_cpu['Time'], cpp_cpu['bankingsystemc'], label='C++', color='orange', marker='o')

# Adding titles and labels
plt.title('CPU Usage Over Time (Interval: 5 Minutes)')
plt.xlabel('Time (Minutes)')
plt.ylabel('CPU Usage (%)')
plt.legend(loc='upper left')
plt.grid(True)

# Adding percentage sign to the y-axis labels
plt.gca().yaxis.set_major_formatter(plt.FuncFormatter(lambda x, _: f'{x:.0f}%'))

# Show the plot
plt.savefig('/mnt/data/CPU_Usage_Comparison_Percentage_Updated.png')
plt.show()
