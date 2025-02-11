#!/bin/bash

# Set environment variables
echo "Please enter the database connection details:"
read -p "Server: " DB_HOST
read -p "Database: " DB_NAME
read -s -p "Password: " DB_PASSWORD
echo
export DB_HOST DB_NAME DB_PASSWORD

# Update and install required packages
apt-get update && apt-get install -y postgresql-client