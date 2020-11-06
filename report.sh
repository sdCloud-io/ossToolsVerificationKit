#!/bin/sh

REPORT_NAME="testReport.json"

function report_openReport() {
    REPORT_NAME=$1
    json_init $REPORT_NAME
    json_writeObjectStart
}

function report_closeReport() {
    json_writeObjectEnd
}

function report_startResultsArray() {
    json_writeKey $1
    json_writeArrayStart
}

function report_endResultsArray() {
    json_writeArrayEnd
}

function report_writeSuccessfulExecution() {
    local result="success"
    local modelPath=$1
    local log=$2
    local generationTime=$3
    local compilationTime=$4
    local executionTime=$5
    local modelPath=$6
    _report_writeExecution \
                        "$result"    \
                        "$modelPath" \
                        "$log"       \
                        $generationTime \
                        $compilationTime \
                        $executionTime \
                        "$modelPath"
}

function report_writeFailedExecution() {
    local result="failed"
    local modelPath=$1
    local log=$2
    local generationTime=$3
    local compilationTime=$4
    local executionTime=$5
    local modelPath=$6
    _report_writeExecution \
                        "$result"    \
                        "$modelPath" \
                        "$log"       \
                        $generationTime \
                        $compilationTime \
                        $executionTime \
                        "$modelPath"
}

function _report_writeExecution() {
    local result=$1
    local modelPath=$2
    local log=$3
    local generationTime=$4
    local compilationTime=$5
    local executionTime=$6
    local modelPath=$7

    json_writeObjectStart
    
    json_writeKey "result"
    json_writeValueString "$result"
    
    json_writeKey "modelPath"
    json_writeValueString "$modelPath"
    
    json_writeKey "log"
    json_writeValueString "$log"
    
    json_writeKey "codeGenerationTime"
    json_writeValueNumber $generationTime

    json_writeKey "codeCompilationTime"
    json_writeValueNumber $compilationTime

    json_writeKey "codeExecutionTime"
    json_writeValueNumber $executionTime

    if [ -f "$modelPath.result" ]
    then
        json_writeKey "lastModelState"
        json_writeObjectStart

        cat $modelPath.result | \
            jq 'keys' | \
            grep -vE "\[|\]" | \
            sed 's/\\"/~/g' | \
            sed 's/  "//g' | \
            sed 's/\\\\/=^.^=/g' | \
            sed 's/",//g' | \
            sed 's/"$//g' | \
            while read key
            do
                key=`echo $key | sed 's/\~/\\\\"/g' | sed 's/=\^\.\^=/\\\\\\\\/g'` 
                ITEM_VALUE=`cat $modelPath.result | jq ".\"$key\" | to_entries | .[-1].value"`
                if [ "X$ITEM_VALUE" == "X" ]
                then
                    ITEM_VALUE="0"
                fi
                json_writeKey "${key}"
                json_writeValueNumber $ITEM_VALUE
            done
        
        json_writeObjectEnd
        mv "$modelPath.result" "$modelPath.processed.result"
    else
        local modelLocation=$(dirname "${modelPath}")
        local modelName=$(basename -- "${modelPath}")
        local sdeResultPath="${modelLocation}/output/${modelName%.*}.txt"
        if [ -f "$sdeResultPath" ]
        then

            json_writeKey "lastModelState"
            json_writeObjectStart

            local fieldsCount=`head -n 1 $sdeResultPath | gawk -F'\t' '{print NF-1}'`
            local counter=1
            while [ $counter -le $fieldsCount ]
            do
                key=`head -n 1 $sdeResultPath | gawk -v pos=$counter -F'\t' '{print $pos}'`
                value=`tail -n 1 $sdeResultPath | gawk -v pos=$counter -F'\t' '{print $pos}'`
                json_writeKey "${key}"
                json_writeValueNumber "${value}"
                counter=`expr $counter + 1`
            done

            json_writeObjectEnd
            mv "$sdeResultPath" "$sdeResultPath.processed"
        fi
    fi

    json_writeObjectEnd
}

function report_writeEnvironment() {
    local timeStamp=`time_currentTimeInMillis`
    local time=`date`
    json_writeKey "startTimeStamp"
    json_writeValueNumber "$timeStamp"
    json_writeKey "startTime"
    json_writeValueString "$time"
}

function report_writeOverallExecutionMetrics() {
    local totalExecutionTime=$1
    local totalModelsProcesses=$2
    local successCount=$3
    local failureCount=$4
    
    json_writeKey "summary"

    json_writeObjectStart

    json_writeKey "totalExecutionTime"
    json_writeValueNumber $totalExecutionTime

    json_writeKey "totlaModelsCount"
    json_writeValueNumber $totalModelsProcesses

    json_writeKey "succeededModelsCount"
    json_writeValueNumber $successCount

    json_writeKey "failedModelsCount"
    json_writeValueNumber $failureCount

    json_writeObjectEnd
}