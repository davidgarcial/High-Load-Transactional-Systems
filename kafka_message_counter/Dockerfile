# Build stage
FROM ubuntu:22.04 AS build

# Avoid user interaction with tzdata
ARG DEBIAN_FRONTEND=noninteractive

# Install necessary packages for building
RUN apt-get update && \
    apt-get install -y curl build-essential pkg-config libssl-dev

# Install Rust
RUN curl https://sh.rustup.rs -sSf | bash -s -- -y
ENV PATH="/root/.cargo/bin:${PATH}"

# Specify Rust version if necessary
# RUN rustup install 1.75.0
# RUN rustup default 1.75.0

# Copy your source code
WORKDIR /usr/src/kafka_message_counter
COPY . .

# Build your application
RUN cargo build --release

# Final stage
FROM ubuntu:22.04

# Install runtime dependencies
RUN apt-get update && \
    apt-get install -y libssl-dev ca-certificates && \
    rm -rf /var/lib/apt/lists/*

# Copy the binary from the build stage
COPY --from=build /usr/src/kafka_message_counter/target/release/kafka_message_counter /usr/local/bin/

# Setup runtime environment
WORKDIR /usr/local/bin
CMD ["./kafka_message_counter"]
