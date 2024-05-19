FROM debian:buster-slim

ENV DOCKER_BUILDKIT=1

RUN apt-get update && apt-get install -y --fix-missing \
    g++ \
    cmake \
    make \
    pkg-config \
    libssl-dev \
    libsasl2-dev \
    wget \
    libbson-1.0-0 \
    libmongoc-1.0-0 \
    libboost-all-dev \
    librdkafka-dev \
    librdkafka1 \
    nlohmann-json3-dev

# Install newer version of CMake
RUN wget -O cmake-linux.sh https://github.com/Kitware/CMake/releases/download/v3.21.3/cmake-3.21.3-linux-x86_64.sh && \
    mkdir /opt/cmake && \
    sh cmake-linux.sh --prefix=/opt/cmake --skip-license && \
    ln -s /opt/cmake/bin/cmake /usr/local/bin/cmake

RUN wget https://github.com/mongodb/mongo-c-driver/releases/download/1.17.0/mongo-c-driver-1.17.0.tar.gz && \
    tar xzf mongo-c-driver-1.17.0.tar.gz

RUN cd mongo-c-driver-1.17.0 && \
    mkdir cmake-build && \
    cd cmake-build && \
    cmake -DENABLE_AUTOMATIC_INIT_AND_CLEANUP=OFF .. && \
    make && \
    make install

RUN wget https://github.com/mongodb/mongo-cxx-driver/releases/download/r3.6.0/mongo-cxx-driver-r3.6.0.tar.gz && \
    tar -xzf mongo-cxx-driver-r3.6.0.tar.gz

RUN cd mongo-cxx-driver-r3.6.0/build && \
    cmake .. -DCMAKE_BUILD_TYPE=Release -DBSONCXX_POLY_USE_BOOST=1 -DCMAKE_INSTALL_PREFIX=/usr/local -DBoost_INCLUDE_DIR=/usr/include -DCMAKE_VERBOSE_MAKEFILE:BOOL=ON && \
    make VERBOSE=1 && \
    make install

# Install simdjson version 3.9.3
RUN wget https://github.com/simdjson/simdjson/archive/refs/tags/v3.9.1.tar.gz && \
    tar xzf v3.9.1.tar.gz && \
    cd simdjson-3.9.1 && \
    cmake . && \
    make && \
    make install

COPY src/ /app/
WORKDIR /app
ENV LD_LIBRARY_PATH /usr/local/lib:$LD_LIBRARY_PATH

RUN g++ -o app *.cpp -lrdkafka++ -lrdkafka -lmongocxx -lbsoncxx -lsimdjson \
    -I/usr/include/librdkafka \
    -I/usr/local/include/mongocxx/v_noabi \
    -I/usr/local/include/bsoncxx/v_noabi \
    -I/usr/local/include/simdjson \
    -L/usr/lib \
    -L/usr/local/lib

CMD ["./app"]
