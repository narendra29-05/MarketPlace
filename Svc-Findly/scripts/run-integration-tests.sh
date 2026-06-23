#!/bin/bash
set -euo pipefail

####################################
# Script: run-integration-tests.sh
# Purpose: Run Findly integration tests
####################################

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
TEST_PROJECT="$PROJECT_ROOT/tests/Findly.IntegrationTests/Findly.IntegrationTests.csproj"

GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

print_info() { echo -e "${GREEN}[INFO]${NC} $1"; }
print_warning() { echo -e "${YELLOW}[WARN]${NC} $1"; }
print_error() { echo -e "${RED}[ERROR]${NC} $1"; }

FILTER=""
VERBOSITY="minimal"
COVERAGE=false

while [[ $# -gt 0 ]]; do
    case "$1" in
        --filter)
            FILTER="${2:-}"
            shift 2
            ;;
        --verbose)
            VERBOSITY="detailed"
            shift
            ;;
        --coverage)
            COVERAGE=true
            shift
            ;;
        --help|-h)
            echo "Usage: $0 [options]"
            echo ""
            echo "Options:"
            echo "  --filter <pattern>  Run tests matching the pattern"
            echo "  --verbose           Show detailed test output"
            echo "  --coverage          Generate code coverage report"
            echo "  --help              Show this help message"
            exit 0
            ;;
        *)
            print_error "Unknown option: $1"
            echo "Use --help for usage information."
            exit 1
            ;;
    esac
done

if [[ ! -f "$TEST_PROJECT" ]]; then
    print_warning "Integration test project not found at $TEST_PROJECT"
    print_warning "Create tests/Findly.IntegrationTests before running this script."
    exit 0
fi

if command -v docker >/dev/null 2>&1; then
    if docker info >/dev/null 2>&1; then
        print_info "Docker is running"
    else
        print_error "Docker is not running. Start Docker before running integration tests."
        exit 1
    fi
else
    print_warning "Docker not found. Continuing because tests may not require containers."
fi

print_info "Restoring integration test packages..."
dotnet restore "$TEST_PROJECT"

print_info "Building integration test project..."
dotnet build "$TEST_PROJECT" --configuration Release --no-restore

TEST_COMMAND=(dotnet test "$TEST_PROJECT" --configuration Release --no-build --logger "console;verbosity=$VERBOSITY")

if [[ -n "$FILTER" ]]; then
    TEST_COMMAND+=(--filter "$FILTER")
    print_info "Filter: $FILTER"
fi

if [[ "$COVERAGE" == true ]]; then
    TEST_COMMAND+=(/p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./TestResults/)
    print_info "Coverage enabled"
fi

print_info "Running Findly integration tests..."
"${TEST_COMMAND[@]}"

print_info "Integration test run complete."
