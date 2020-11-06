#!/bin/sh


util_lastOperationFailed() {
    if [ $1 -eq 0 ]
    then
        return 1
    else
        return 0
    fi
}