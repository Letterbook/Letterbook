# Dashboards

This is a set of configurations and settings for prometheus and grafana monitoring services.

These settings are used by the default monitors configured in the [Docker Compose](../docker-compose.yml) file.



## Grafana

Grafana is available at [localhost:3000](localhost:3000) and will use the default `admin` user with a default password
of `admin`. Once you connect you will be prompted to set a new password.

Grafana should automatically provision a connection to the local Prometheus data source.

## Prometheus

Prometheus is configured with [prometheus.yml](Letterbook.Dashboards/Prometheus/prometheus.yml) to connect to a local
version of Letterbook running on your host computer. If you are running Letterbook on another port or in any other way
this connection may not work out of the box and your configuration may need to be updated for prometheus scrapping to
work correctly.