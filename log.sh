#!/bin/sh

LOG_LEVEL_INFO="INFO"
LOG_PATH="/tmp/certToolLog.log"


_LAST_CAPTURED_LOG=""

function _log_printLogLine() {
    local LOG_PREFIX=$1
    local LOG_LINE=$2
    local TIME_STAMP=`date +%T`
    echo "[$TIME_STAMP] [$LOG_PREFIX]: $LOG_LINE"
}

function log_printInfoLine() {
    _log_printLogLine "$LOG_LEVEL_INFO" "$1"
}

function log_printScriptHeader() {
    log_printInfoLine ""
    log_printInfoLine "* ***********************************************************"
    log_printInfoLine "*  System Dynamics Tools Certification Kit"
    log_printInfoLine "* ***********************************************************"
    log_printInfoLine ""
}

function log_printSectionHeader() {
    log_printInfoLine ""
    log_printInfoLine "* ==========================================================="
    log_printInfoLine "* === $1"
    log_printInfoLine "* ==========================================================="
    log_printInfoLine ""
}

function log_printVariable() {
    log_printInfoLine "$1 = ${!1}"
}

function log_wrapExternalLogOutput() {
    _LAST_CAPTURED_LOG=""
    while read logLine
    do
        _LAST_CAPTURED_LOG="$_LAST_CAPTURED_LOG\n$logLine"
        log_printInfoLine "[LOG]: $logLine"
    done
    echo "$_LAST_CAPTURED_LOG" > $LOG_PATH
}