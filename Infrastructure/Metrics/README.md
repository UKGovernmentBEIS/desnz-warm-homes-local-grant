# Metrics

Metrics for this app are exported in Prometheus format using the [alphagov Prometheus exporter](https://github.com/alphagov/paas-prometheus-exporter).
The metrics are ingested and stored in a private [Prometheus](https://prometheus.io/) server and then displayed and alerted on using a private [Grafana](https://grafana.com/docs/grafana/latest/introduction/oss-details/) instance.

## Installation

Create a separate metrics space in GovPaaS to keep the metrics separate.

- Login to Cloud Foundry
- Run `cf create-space sea-beta-metrics`

### Prometheus Exporter

- [Get someone with permissions to] create a GovPaaS user specifically for running the prometheus exporter app. This user needs space auditor permissions for the space being monitored.
- Check out the paas-prometheus-exporter repo - https://github.com/alphagov/paas-prometheus-exporter
- CD into that directory
- Set your Cloud Foundry target space to the right one for the exporter
- Run `cf push --no-start prometheus-exporter --no-route`
- Run `cf map-route prometheus-exporter apps.internal --hostname prometheus-exporter-sea-beta`
- Run `cf set-env prometheus-exporter API_ENDPOINT https://api.cloud.service.gov.uk`
- Run `cf set-env prometheus-exporter USERNAME <your prometheus GovPaaS user username here>`
- Run `cf set-env prometheus-exporter PASSWORD <your prometheus GovPaaS user password here>`
- Run `cf start prometheus-exporter`

### Database for the Prometheus server

- Run `cf create-service influxdb tiny-1.x prometheus-influxdb`

### Prometheus Server
- Cd to the PrometheusServer directory
- Run `cf create-app prometheus-server` <- This name should match the name in `manifest.yml`
- Run `cf apply-manifest -f manifest.yml`
- Run `cf bind-service prometheus-server prometheus-influxdb`
- Run `cf add-network-policy prometheus-server prometheus-exporter --protocol tcp --port 8080`
- Run `cf push prometheus-server`

### Database for Grafana
- Make sure you're targeting the space where you're setting up metrics
- Run `cf create-service postgres small-11 grafana-postgresdb`

### Grafana
- Cd to the Grafana directory
- Run `cf create-app grafana-server --no-route`
- Run `cf bind-service grafana-server grafana-postgresdb`
- Run `cf map-route grafana-server cloudapps.digital --hostname grafana-server-sea-beta`
- Run `cf env grafana-server`
- Using the values from the VCAP_SERVICES output to fill in the GF_DATABASE_* values in `manifest.yml`.
  - Also make up values for ADMIN_USER, ADMIN_PASS, and SECRET_KEY
- Run `cf add-network-policy grafana-server prometheus-server --protocol tcp --port 8080`
- Run `cf apply-manifest -f manifest.yml`
- Run `cf push grafana-server`

## Configuration

### Grafana

- Login to the Grafana site
- Create a dashboard
- Go to the settings (cog icon)
- Select `JSON Model`
- Paste in the contents of `dev-dashboard.json`