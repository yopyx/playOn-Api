# ---- Build stage: produce an optimized Keycloak build ----
FROM quay.io/keycloak/keycloak:26.5.6 AS builder

# >>> ADDED (optional, off by default): single-instance mode, skips clustering.
# >>> Uncomment only after checking the Keycloak docs for your version.
# ENV KC_CACHE=local

ENV KC_DB=postgres
ENV KC_HEALTH_ENABLED=true
ENV KC_METRICS_ENABLED=true

WORKDIR /opt/keycloak
RUN /opt/keycloak/bin/kc.sh build

# ---- Runtime stage ----
FROM quay.io/keycloak/keycloak:26.5.6

COPY --from=builder /opt/keycloak/ /opt/keycloak/

# >>> ADDED: fit a 512 MB container (cap heap, lighter GC, quicker JIT warmup)
ENV JAVA_OPTS_KC_HEAP="-Xms64m -XX:MaxRAMPercentage=50"
ENV JAVA_OPTS_APPEND="-XX:+UseSerialGC -XX:TieredStopAtLevel=1"
# >>> ADDED: smaller DB connection pool (Neon free also limits connections)
ENV KC_DB_POOL_INITIAL_SIZE=2 KC_DB_POOL_MIN_SIZE=2 KC_DB_POOL_MAX_SIZE=5

USER root

ADD https://curl.se/ca/cacert.pem /opt/keycloak/.postgresql/root.crt
RUN chown -R keycloak:keycloak /opt/keycloak/.postgresql

# Realm config, committed to the repo, imported automatically on startup
COPY import/ /opt/keycloak/data/import/
RUN chown -R keycloak:keycloak /opt/keycloak/data/import

USER keycloak

ENTRYPOINT ["/opt/keycloak/bin/kc.sh"]
# "--import-realm" is only added for the deployment to work with the realm config committed to the repo. Remove it if you don't want to import the realm config on startup.
CMD ["start", "--optimized","--import-realm"]