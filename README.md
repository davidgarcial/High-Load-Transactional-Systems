# Transactional Systems Performance Analysis

## Overview
This repository contains the source code and accompanying data for a comprehensive performance analysis of transactional systems implemented in C\#, C++, and Rust. The project investigates various memory management techniques and their impacts on system performance under high-load conditions.

## Repository Structure

### BankingSystemC
Contains the implementation of the transactional system in C++. This directory includes all source files, build scripts, and dependencies required to compile and run the system.

### BankingSystemCSharp
This directory houses the C\# implementation of the transactional system. It includes the .NET project files, source code, and necessary configuration files for running the application.

### BankingSystemRust
Includes the Rust implementation of the transactional system. This folder contains the Cargo package with all Rust source files and configuration needed to build and execute the system.

### Data
Stores data files and scripts used for analyzing the system's performance. This may include CSV files, data extraction scripts, and other resources used to support data analysis.

### K8
Contains Kubernetes configurations and scripts for deploying the transactional systems in a Kubernetes environment. This includes YAML files for setting up pods, services, and other K8 resources.

### kafka_message_counter
Holds the implementation of a Kafka message counter tool. This utility is used to monitor and count messages passing through Kafka, assisting in performance and throughput analysis.

### WorkloadGenerator
This directory includes scripts and tools for generating workloads on the transactional systems. These scripts simulate various transaction types and loads to test the systems' performances.

### Other Files
- `docker-compose.yml`: Docker Compose file for setting up and running the multi-container application, useful for local development and testing.
- `Kubernetes _ Compute Resources _ Pod-1714268337186.json`: A JSON configuration file for Kubernetes, detailing specific compute resource configurations for pods.

## Getting Started
To get started with this project, clone the repository to your local machine:

```bash
git clone https://github.com/yourusername/transactional-systems.git
