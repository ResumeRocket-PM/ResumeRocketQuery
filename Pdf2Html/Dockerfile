FROM ubuntu:trusty

# Install required dependencies including SSL
RUN apt-get update && apt-get install -y build-essential checkinstall git cmake \
    wget curl libssl-dev zlib1g-dev libbz2-dev libreadline-dev libsqlite3-dev \
    libffi-dev liblzma-dev libgdbm-dev ca-certificates openssl \
    libncurses5-dev libncursesw5-dev xz-utils tk-dev

# Download and install OpenSSL (newer version)
RUN cd /usr/src && \
    wget https://www.openssl.org/source/openssl-1.1.1q.tar.gz && \
    tar xzf openssl-1.1.1q.tar.gz && \
    cd openssl-1.1.1q && \
    ./config --prefix=/usr/local/ssl --openssldir=/usr/local/ssl shared zlib && \
    make && make install

# Update environment variables to use the newly installed OpenSSL
ENV LD_LIBRARY_PATH=/usr/local/ssl/lib
ENV PATH="/usr/local/ssl/bin:$PATH"

# Download and install Python 3.8 from source with SSL support
RUN cd /usr/src && \
    wget https://www.python.org/ftp/python/3.8.13/Python-3.8.13.tgz && \
    tar xzf Python-3.8.13.tgz && \
    cd Python-3.8.13 && \
    ./configure --enable-optimizations --with-openssl=/usr/local/ssl && \
    make altinstall

# Create a symlink to pip for Python 3.8
RUN ln -s /usr/local/bin/pip3.8 /usr/local/bin/pip

# Upgrade pip for Python 3.8
RUN python3.8 -m pip install --upgrade pip

# Check if Python is compiled with SSL support
RUN python3.8 -c "import ssl; print(ssl.OPENSSL_VERSION)"

# Copy and install the Python dependencies
COPY requirements.txt .
RUN python3.8 -m pip install --trusted-host pypi.org --trusted-host files.pythonhosted.org --no-cache-dir -r requirements.txt

# Install additional dependencies (as per your original Dockerfile)
RUN apt-get install -y poppler-data autotools-dev libjpeg-dev libtiff4-dev libpng-dev \
		       libgif-dev libxt-dev autoconf automake libtool bzip2 libxml2-dev libuninameslist-dev \
		       libspiro-dev libpango1.0-dev libcairo2-dev chrpath uuid-dev uthash-dev libopenjpeg-dev \
		       sudo packaging-dev pkg-config g++ xz-utils wget

# Poppler installation
RUN cd / && curl -L http://poppler.freedesktop.org/poppler-0.26.5.tar.xz -o poppler-0.26.5.tar.xz && tar xvf poppler-0.26.5.tar.xz && \
    cd poppler-0.26.5 && ./configure --prefix=/usr --enable-xpdf-headers && make && make install

# Freetype installation
RUN cd / && curl -L https://phoenixnap.dl.sourceforge.net/project/freetype/freetype2/2.6.3/freetype-2.6.3.tar.gz -o freetype-2.6.3.tar.gz && \
    tar xzvf freetype-2.6.3.tar.gz && cd freetype-2.6.3 && ./configure && make && make install

# Fontconfig installation
RUN cd / && wget http://www.freedesktop.org/software/fontconfig/release/fontconfig-2.12.0.tar.bz2 && tar jxvf fontconfig-2.12.0.tar.bz2 && \
    cd fontconfig-2.12.0 && ./configure && make && make install

# Fontforge installation
RUN cd / && git clone https://github.com/coolwanglu/fontforge.git fontforge && cd fontforge && git checkout pdf2htmlEX && \
    ./autogen.sh && ./configure --prefix=/usr && make && make install && ldconfig

# pdf2htmlEX installation
RUN cd / && git clone --depth=1 https://github.com/coolwanglu/pdf2htmlEX.git && \
    cd pdf2htmlEX && cmake . -DENABLE_SVG=ON && make && make install

# Create /pdf directory and set it as a volume and workdir
RUN mkdir -p /pdf

VOLUME /pdf

WORKDIR /pdf

COPY app.py .

EXPOSE 5000

CMD ["python3.8", "app.py"]