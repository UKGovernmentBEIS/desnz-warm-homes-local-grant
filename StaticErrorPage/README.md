This directory contains the static error page for when the WH:LG service is unavailable.

This page is not deployed as part of our pipeline. All changes must be sent to ICS for them to update the S3 bucket
where this page is kept.

Version 5.10.2 of GDS is included here. To update GDS library for the new version:

- Download a zip of the latest release from https://github.com/alphagov/govuk-frontend/releases
- Copy paste the .css file into unavailable.css
- Copy paste the assets files into static-assets
- Make changes to the unavailable.html file as necessary
- Apply the following changes to unavailable.html and unavailable.css:
    - All path references are made relative
    - All assets references are replaced with static-assets
    - (/assets/... -> ./static-assets/...).
    - These changes are required to ensure that the assets are found successfully when served from S3. The path
      ./static-assets/ prevents these files from being prioritised on the main site over ./assets.

