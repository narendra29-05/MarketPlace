#!/bin/bash
set -euo pipefail

####################################
# Script: setup-local.sh
# Purpose: Local development setup for Findly service
####################################

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
SRC_DIR="$PROJECT_ROOT/src"
PROJECT_NAME="Findly"
API_PROJECT="$SRC_DIR/${PROJECT_NAME}.Api/${PROJECT_NAME}.Api.csproj"
MIGRATION_PROJECT="$PROJECT_ROOT/db/${PROJECT_NAME}.Migrations/${PROJECT_NAME}.Migrations.csproj"
MIGRATE_SCRIPT="$PROJECT_ROOT/scripts/migrations/migrate.sh"
APPSETTINGS_PATH="$SRC_DIR/${PROJECT_NAME}.Api/appsettings.Development.json"
SOLUTION_PATH="$PROJECT_ROOT/${PROJECT_NAME}.sln"

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

print_info() { echo -e "${GREEN}[INFO]${NC} $1"; }
print_warning() { echo -e "${YELLOW}[WARN]${NC} $1"; }
print_error() { echo -e "${RED}[ERROR]${NC} $1"; }

print_info "Starting local setup for Findly service..."
print_info "Project root: $PROJECT_ROOT"
print_info "API project: $API_PROJECT"
echo ""

if ! command -v dotnet >/dev/null 2>&1; then
    print_error ".NET SDK not found. Install the .NET SDK and retry."
    exit 1
fi
print_info ".NET SDK version $(dotnet --version) detected"

if command -v docker >/dev/null 2>&1; then
    if docker info >/dev/null 2>&1; then
        print_info "Docker is running"
    else
        print_warning "Docker is installed but not running. Start Docker before running SQL Server locally."
    fi
else
    print_warning "Docker not found. Local SQL Server container checks will be skipped."
fi

if [[ -f "$APPSETTINGS_PATH" ]]; then
    print_info "Configuration file exists: $APPSETTINGS_PATH"
else
    print_warning "appsettings.Development.json not found at $APPSETTINGS_PATH"
fi

cd "$PROJECT_ROOT"

if [[ -s "$SOLUTION_PATH" ]]; then
    print_info "Restoring solution packages..."
    dotnet restore "$SOLUTION_PATH"

    print_info "Building solution..."
    dotnet build "$SOLUTION_PATH" --no-restore
else
    print_warning "Solution file is missing or empty, building known projects instead."

    if [[ -f "$API_PROJECT" ]]; then
        print_info "Restoring API project..."
        dotnet restore "$API_PROJECT"

        print_info "Building API project..."
        dotnet build "$API_PROJECT" --no-restore
    else
        print_warning "API project not found at $API_PROJECT"
    fi

    if [[ -f "$MIGRATION_PROJECT" ]]; then
        print_info "Restoring migration project..."
        dotnet restore "$MIGRATION_PROJECT"

        print_info "Building migration project..."
        dotnet build "$MIGRATION_PROJECT" --no-restore
    else
        print_warning "Migration project not found at $MIGRATION_PROJECT"
    fi
fi

if [[ -x "$MIGRATE_SCRIPT" ]]; then
    echo ""
    read -r -p "Apply database migrations now? (y/N): " reply
    if [[ "$reply" =~ ^[Yy]$ ]]; then
        "$MIGRATE_SCRIPT" run
    fi
else
    print_warning "Migration script is not executable at $MIGRATE_SCRIPT"
fi

echo ""
print_info "Findly local setup complete."
print_info "Run API: dotnet run --project $API_PROJECT"
print_info "Run migrations: $MIGRATE_SCRIPT run"
