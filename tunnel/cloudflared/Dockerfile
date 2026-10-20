FROM cloudflare/cloudflared:latest
COPY config.yaml  /etc/cloudflared/config.yaml
COPY credentials/ /etc/cloudflared/credentials/
CMD ["tunnel", "--config", "/etc/cloudflared/config.yaml", "run"]
