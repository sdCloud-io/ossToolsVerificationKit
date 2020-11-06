#!/bin/sh

JSON_FILE_NAME=""

_CURRENT_INDENT=""
_INDENT_CHARS="    "
_INDENT_SIZE=${#_INDENT_CHARS}

_LAST_STATE=""

function json_init() {
    JSON_FILE_NAME=$1;

    echo > $JSON_FILE_NAME;
}


function json_writeObjectStart() {
    if [ "$_LAST_STATE" == "value" -o  "$_LAST_STATE" == "objEnd" -o "$_LAST_STATE" == "arrayEnd" ]
    then
        _json_noIndentPrintln ","
    fi
    _json_println "{"
    _json_indentUp
    _LAST_STATE="objStart"
}

function json_writeObjectEnd() {
    if [ "$_LAST_STATE" == "value" ]
    then
        _json_noIndentPrintln " "
    fi
    _json_indentDown
    _json_println "}"
    _LAST_STATE="objEnd"
}

function json_writeArrayStart() {
    if [ "$_LAST_STATE" == "value" -o  "$_LAST_STATE" == "objEnd" -o "$_LAST_STATE" == "arrayEnd" ]
    then
        _json_noIndentPrintln ","
    fi
    _json_println "["
    _json_indentUp
    _LAST_STATE="arrayStart"
}

function json_writeArrayEnd() {
    if [ "$_LAST_STATE" == "value" ]
    then
        _json_noIndentPrintln " "
    fi
    _json_indentDown
    _json_println "]"
    _LAST_STATE="arrayEnd"
}

function json_writeKey() {
    local keyName=$1
    keyName=`echo $keyName | tr '"' '===='`
    keyName=`echo $keyName | tr '\\' '===='`
    keyName=`echo $keyName | sed 's/\==//g'`
    if [ "$_LAST_STATE" == "value" -o  "$_LAST_STATE" == "objEnd" -o "$_LAST_STATE" == "arrayEnd" ]
    then
        _json_noIndentPrintln ","
    fi
    _json_print "\"$keyName\": "
    _LAST_STATE="key"
}

function json_writeValueString() {
    local value=$1
    _json_noIndentPrint "\"$value\""
    _LAST_STATE="value"
}

function json_writeValueNumber() {
    local value=$1
    _json_noIndentPrint "$value"
    _LAST_STATE="value"
}


function _json_print() {
    local value=$1
    echo -n "$_CURRENT_INDENT$value" >> $JSON_FILE_NAME
}

function _json_println() {
    local value=$1
    echo "$_CURRENT_INDENT$value" >> $JSON_FILE_NAME
}

function _json_noIndentPrint() {
    local value=$1
    echo -n "$value" >> $JSON_FILE_NAME
}

function _json_noIndentPrintln() {
    local value=$1
    echo "$value" >> $JSON_FILE_NAME
}

function _json_indentUp() {
    _CURRENT_INDENT="$_CURRENT_INDENT$_INDENT_CHARS"
}

# https://stackoverflow.com/a/5863742
function _json_indentDown() {
    local shiftSize=`expr $_INDENT_SIZE + 1`
    _CURRENT_INDENT=`echo "$_CURRENT_INDENT" | rev | cut -c $shiftSize- | rev`
}