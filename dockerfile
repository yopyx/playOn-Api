# ---- Build stage: produce an optimized Keycloak build ----
FROM quay.io/keycloak/keycloak:26.5.6 AS builder

ENV KC_DB=postgres
ENV KC_HEALTH_ENABLED=true
ENV KC_METRICS_ENABLED=true

WORKDIR /opt/keycloak
RUN /opt/keycloak/bin/kc.sh build

# ---- Runtime stage ----
FROM quay.io/keycloak/keycloak:26.5.6

COPY --from=builder /opt/keycloak/ /opt/keycloak/

USER root

ADD https://curl.se/ca/cacert.pem /opt/keycloak/.postgresql/root.crt
RUN chown -R keycloak:keycloak /opt/keycloak/.postgresql

# Realm config, committed to the repo, imported automatically on startup
COPY import/ /opt/keycloak/data/import/
RUN chown -R keycloak:keycloak /opt/keycloak/data/import

USER keycloak

ENTRYPOINT ["/opt/keycloak/bin/kc.sh"]
CMD ["start", "--optimized"]