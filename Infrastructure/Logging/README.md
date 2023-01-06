# Logging

## Logit.io

GovPaaS has instructions on how to export console logs to Logit.io. However, the ASP.Net Core default console logging
is not very good so the default GovPaaS solution doesn't work very well for us.

Logit.io have [instructions on their site](https://logit.io/sources/configure/dotnetcore) on how to use
[Serilog](https://github.com/serilog/serilog) instead. This is what we are using on this project along with a
[Serilog network sink](https://github.com/serilog-contrib/Serilog.Sinks.Network) to push the logs to Logit.io.

We have a single alerting rule configured on Logit.io to alert us every time a Critical or Error log is raised in the
website code.

### Configuration

We are currently using the default logit.io logstash pipeline configuration. There is a copy of this in
`Logit.io/LogstashPipeline.txt`. Any changes made to the pipeline should be tracked here and uploaded to Logit.io.

The configuration for alerting rules is in `Logit.io/AlertRules/*.yaml`. Changes to the alert rules should be tracked
here and uploaded to Logit.io.
