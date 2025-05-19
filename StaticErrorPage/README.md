This directory contains the static error page for when the WH:LG service is unavailable.

This page is not deployed as part of our pipeline. All changes must be sent to ICS for them to update the S3 bucket where this page is kept.

Version 4.9.0 of GDS is included here. Modifications to make it run statically are:

- All path references are made relative (/assets/... -> ./assets/...).

