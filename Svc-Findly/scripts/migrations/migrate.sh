#!/bin/bash
set -euo pipefail

####################################
# DbUp Migration Script - Findly Service
####################################
# Commands:
#   new    - Create a new SQL migration file
#   run    - Run Findly.Migrations locally
#   list   - List migration files
####################################

SERVICE_NAME="findly"
PROJECT_NAME="Findly"
SCHEMA_NAME="fin"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
DB_DIR="$PROJECT_ROOT/db"
MIGRATION_RUNNER_DIR="$DB_DIR/${PROJECT_NAME}.Migrations"
SCRIPTS_ROOT="$MIGRATION_RUNNER_DIR/Scripts/$SCHEMA_NAME"
SCHEMA_SCRIPTS_DIR="$SCRIPTS_ROOT/Schemas"
TABLE_SCRIPTS_DIR="$SCRIPTS_ROOT/Tables"

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m'

print_error() { echo -e "${RED}[ERROR]${NC} $1"; }
print_info() { echo -e "${GREEN}[INFO]${NC} $1"; }
print_warn() { echo -e "${YELLOW}[WARN]${NC} $1"; }

sanitize_name() {
    echo "$1" | tr '[:upper:]' '[:lower:]' | sed -E 's/[^a-z0-9]+/_/g; s/^_+|_+$//g'
}

new_migration() {
    local type="${1:-table}"
    local name="${2:-}"

    if [[ -z "$name" ]]; then
        read -r -p "Migration name: " name
    fi

    if [[ -z "$name" ]]; then
        print_error "Migration name is required."
        exit 1
    fi

    local target_dir="$TABLE_SCRIPTS_DIR"
    case "$type" in
        schema|schemas) target_dir="$SCHEMA_SCRIPTS_DIR" ;;
        table|tables|add-column|alter-table|index|indexes) target_dir="$TABLE_SCRIPTS_DIR" ;;
        *) target_dir="$TABLE_SCRIPTS_DIR" ;;
    esac

    mkdir -p "$target_dir"

    local stamp
    stamp="$(date +%Y%m%d%H%M%S)"
    local safe_name
    safe_name="$(sanitize_name "$name")"
    local file="$target_dir/${stamp}_${safe_name}.sql"

    cat > "$file" <<SQL
-- ============================================================
--  ${PROJECT_NAME} ${SCHEMA_NAME} migration
--  Script : $(basename "$file")
-- ============================================================

USE [FindlyDb];
GO

-- Write migration SQL here.
GO
SQL

    print_info "Created migration: $file"
}

run_migrations() {
    if [[ ! -f "$MIGRATION_RUNNER_DIR/${PROJECT_NAME}.Migrations.csproj" ]]; then
        print_error "Migration project not found at $MIGRATION_RUNNER_DIR"
        exit 1
    fi

    print_info "Running DbUp migrations for $PROJECT_NAME using schema '$SCHEMA_NAME'..."
    (cd "$MIGRATION_RUNNER_DIR" && dotnet run)
}

list_migrations() {
    print_info "Migration files for $PROJECT_NAME ($SCHEMA_NAME):"
    echo ""

    if [[ ! -d "$SCRIPTS_ROOT" ]]; then
        print_warn "No migrations directory found at $SCRIPTS_ROOT"
        return
    fi

    find "$SCRIPTS_ROOT" -type f -name '*.sql' | sort | while read -r file; do
        echo -e "  ${GREEN}${file#$SCRIPTS_ROOT/}${NC}"
    done

    local count
    count="$(find "$SCRIPTS_ROOT" -type f -name '*.sql' | wc -l | tr -d ' ')"
    echo ""
    echo -e "Total: ${YELLOW}$count${NC} migration(s)"
}

print_usage() {
    echo ""
    echo -e "${CYAN}Usage:${NC} ./migrate.sh <command> [args...]"
    echo ""
    echo "Commands:"
    echo "  new [type] [name]    Create a SQL migration file"
    echo "  run                  Run Findly.Migrations locally"
    echo "  list                 List migration files"
    echo ""
    echo "Examples:"
    echo "  ./migrate.sh new table create_reviews"
    echo "  ./migrate.sh new schema add_fin_indexes"
    echo "  ./migrate.sh run"
    echo "  ./migrate.sh list"
    echo ""
}

COMMAND="${1:-}"
if [[ $# -gt 0 ]]; then shift; fi

case "$COMMAND" in
    new) new_migration "$@" ;;
    run|update) run_migrations ;;
    list) list_migrations ;;
    -h|--help|help|"") print_usage ;;
    *) print_error "Unknown command: $COMMAND"; print_usage; exit 1 ;;
esac
