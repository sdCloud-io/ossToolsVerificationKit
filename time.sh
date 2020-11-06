#!/bin/sh

_DEFAULT_MOMENT_NAME="defaultMoment"
declare -A _namedMoments;


function time_currentTimeInMillis() {
    date +%s%N | cut -b1-13
}

function time_rememberMoment() {
    local momentName=$1;
    if [ $# -eq 0 ]
    then
        momentName=$_DEFAULT_MOMENT_NAME
    fi
    _namedMoments[$momentName]=`time_currentTimeInMillis`
}

function time_timeSinceRememberedMoment() {
    local currentTime=`time_currentTimeInMillis`
    local momentName=$1;
    if [ $# -eq 0 ]
    then
        momentName=$_DEFAULT_MOMENT_NAME
    fi
    local delta=`expr $currentTime - ${_namedMoments[$momentName]}`
    echo $delta;
}