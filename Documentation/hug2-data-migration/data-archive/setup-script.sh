#!/bin/bash

# Set environment variables
echo "Please enter the database connection details:"
read -p "Server: " DB_HOST
read -p "Database: " DB_NAME
read -s -p "Password: " DB_PASSWORD
read -p "S3 data migration bucket name e.g. hug2-dev-data-migration: " S3_BUCKET
read -s -p "Please enter the password to set on the zip files: " DATA_FILE_PASSWORD
echo
export DB_HOST DB_NAME DB_PASSWORD S3_BUCKET DATA_FILE_PASSWORD

# Create directory structure
mkdir -p /root/data-archive/{non-personal-data,all-other-personal-data,customer-insight-analysis-data}

# Update and install required packages
apt-get update && apt-get install -y postgresql-client zip