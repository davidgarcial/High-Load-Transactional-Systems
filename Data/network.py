import pandas as pd
import matplotlib.pyplot as plt

# Load the CSV files
csharp_bandwidth_data = pd.read_csv('C# Receive Bandwidth-data.csv')
rust_bandwidth_data = pd.read_csv('Rust Receive Bandwidth-data.csv')
cpp_bandwidth_data = pd.read_csv('C++ Receive Bandwidth-data.csv')

# Convert 'Time' columns to integer for consistency in plotting
csharp_bandwidth_data['Time'] = csharp_bandwidth_data['Time'].astype(int)
rust_bandwidth_data['Time'] = rust_bandwidth_data['Time'].astype(int)
cpp_bandwidth_data['Time'] = cpp_bandwidth_data['Time'].astype(int)

# Convert the bandwidth data from bytes per second to kilobytes per second
csharp_bandwidth_data.iloc[:, 1] /= 1024
rust_bandwidth_data.iloc[:, 1] /= 1024
cpp_bandwidth_data.iloc[:, 1] /= 1024

# Create a plot
plt.figure(figsize=(10, 6))
plt.plot(csharp_bandwidth_data['Time'], csharp_bandwidth_data.iloc[:, 1], label='C#', marker='o')
plt.plot(rust_bandwidth_data['Time'], rust_bandwidth_data.iloc[:, 1], label='Rust', marker='o')
plt.plot(cpp_bandwidth_data['Time'], cpp_bandwidth_data.iloc[:, 1], label='C++', marker='o')

plt.title('Receive Bandwidth Over Time (KB/s)')
plt.xlabel('Time (Minutes)')
plt.ylabel('Receive Bandwidth (KB/s)')
plt.legend()
plt.grid(True)

# Save the plot to a file
plt.savefig('Receive_Bandwidth_Over_Time_KBs.png')
plt.close()

# To display the plot (optional)
plt.show()
