FROM codercom/code-server:latest
USER root

# .NET
RUN curl -fsSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin \
      --channel 9.0 --install-dir /usr/local/dotnet
ENV PATH="$PATH:/usr/local/dotnet" DOTNET_ROOT="/usr/local/dotnet"

# Go (cf-microservices) + Node (workers)
RUN wget -q https://go.dev/dl/go1.23.linux-amd64.tar.gz \
    && tar -C /usr/local -xzf go1.23.linux-amd64.tar.gz \
    && rm go1.23.linux-amd64.tar.gz
ENV PATH="$PATH:/usr/local/go/bin"
RUN curl -fsSL https://deb.nodesource.com/setup_20.x | bash - \
    && apt-get install -y nodejs

# cloudflared + wrangler
RUN curl -L https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-linux-amd64 \
      -o /usr/local/bin/cloudflared && chmod +x /usr/local/bin/cloudflared
RUN npm install -g wrangler

USER coder
RUN code-server --install-extension ms-dotnettools.csharp \
    && code-server --install-extension golang.go \
    && code-server --install-extension humao.rest-client \
    && code-server --install-extension eamodio.gitlens

EXPOSE 8080
