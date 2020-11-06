#!/bin/sh

OPS_MOMENT_MODEL_CODE_GENERATION="modelCodeGenerationTime"
OPS_MOMENT_MODEL_CODE_COMPILATION="modelCodeCompilationTime"
OPS_MOMENT_MODEL_CODE_EXECUTION="modelCodeExecutionTime"

declare -A LAST_OPERATION_RESULT

ops_executeModelWithSDE() {
    local SDE_CMD=$1
    local PATH_TO_MODEL=$2
    local timeDelta=0

    LAST_OPERATION_RESULT["$OPS_MOMENT_MODEL_CODE_GENERATION"]=0
    LAST_OPERATION_RESULT["$OPS_MOMENT_MODEL_CODE_COMPILATION"]=0
    LAST_OPERATION_RESULT["$OPS_MOMENT_MODEL_CODE_EXECUTION"]=0

    log_printSectionHeader "Running model with SDEverywhere"

    log_printInfoLine "Model paht: $PATH_TO_MODEL"
    time_rememberMoment "modelProcessingStart"

    log_printInfoLine "Generating model code..."
    time_rememberMoment "modelCodeGenerationStart"
    $SDE_CMD generate $PATH_TO_MODEL 2>&1 | log_wrapExternalLogOutput
    if util_lastOperationFailed ${PIPESTATUS[0]}
    then
        return 1
    fi
    $SDE_CMD generate --genc $PATH_TO_MODEL 2>&1 | log_wrapExternalLogOutput
    if util_lastOperationFailed ${PIPESTATUS[0]}
    then
        return 2
    fi
    timeDelta=`time_timeSinceRememberedMoment "modelCodeGenerationStart"`
    LAST_OPERATION_RESULT["$OPS_MOMENT_MODEL_CODE_GENERATION"]=${timeDelta}
    log_printInfoLine " - completed in ${timeDelta}"

    log_printInfoLine "Compiling model code"
    time_rememberMoment "modelGeneratedCodeCompilationStart"
    $SDE_CMD compile $PATH_TO_MODEL 2>&1 | log_wrapExternalLogOutput
    if util_lastOperationFailed ${PIPESTATUS[0]}
    then
        return 3
    fi
    timeDelta=`time_timeSinceRememberedMoment "modelGeneratedCodeCompilationStart"`
    LAST_OPERATION_RESULT["$OPS_MOMENT_MODEL_CODE_COMPILATION"]=${timeDelta}
    log_printInfoLine " - completed in ${timeDelta}"

    log_printInfoLine "Running compiled model"
    time_rememberMoment "modelExecutionStart"
    $SDE_CMD exec $PATH_TO_MODEL 2>&1 | log_wrapExternalLogOutput
    if util_lastOperationFailed ${PIPESTATUS[0]}
    then
        return 4
    fi
    timeDelta=`time_timeSinceRememberedMoment "modelExecutionStart"`
    LAST_OPERATION_RESULT["$OPS_MOMENT_MODEL_CODE_EXECUTION"]=${timeDelta}
    log_printInfoLine " - completed in ${timeDelta}"

    timeDelta=`time_timeSinceRememberedMoment "modelProcessingStart"`
    log_printInfoLine "================================================================"
    log_printInfoLine " Total model processing time with SDEverywhere was $timeDelta ms"
    log_printInfoLine "================================================================"
}

ops_executeModelWithPySD() {
    local PySD_CMD=$1
    local PATH_TO_MODEL=$2
    local timeDelta=0
    local modelCompilationTime=0

    LAST_OPERATION_RESULT["$OPS_MOMENT_MODEL_CODE_GENERATION"]=0
    LAST_OPERATION_RESULT["$OPS_MOMENT_MODEL_CODE_COMPILATION"]=0
    LAST_OPERATION_RESULT["$OPS_MOMENT_MODEL_CODE_EXECUTION"]=0

    log_printSectionHeader "Running model with PySD"

    log_printInfoLine "Model paht: $PATH_TO_MODEL"
    time_rememberMoment "modelProcessingStart"

    log_printInfoLine "Generating model code..."
    LAST_OPERATION_RESULT["$OPS_MOMENT_MODEL_CODE_GENERATION"]=0
    log_printInfoLine " - completed in 0"

    log_printInfoLine "Compiling model code"
    time_rememberMoment "modelGeneratedCodeCompilationStart"
    $PySD_CMD compileMdl $PATH_TO_MODEL 2>&1 | log_wrapExternalLogOutput
    if util_lastOperationFailed ${PIPESTATUS[0]}
    then
        return 3
    fi
    modelCompilationTime=`time_timeSinceRememberedMoment "modelGeneratedCodeCompilationStart"`
    LAST_OPERATION_RESULT["$OPS_MOMENT_MODEL_CODE_COMPILATION"]=${modelCompilationTime}
    log_printInfoLine " - completed in ${modelCompilationTime}"

    log_printInfoLine "Running compiled model"
    time_rememberMoment "modelExecutionStart"
    $PySD_CMD compileAndRunMdl $PATH_TO_MODEL 2>&1 | log_wrapExternalLogOutput
    if util_lastOperationFailed ${PIPESTATUS[0]}
    then
        return 4
    fi
    timeDelta=`time_timeSinceRememberedMoment "modelExecutionStart"`
    #timeDelta=`expr $timeDelta - $modelCompilationTime`
    LAST_OPERATION_RESULT["$OPS_MOMENT_MODEL_CODE_EXECUTION"]=${timeDelta}
    log_printInfoLine " - completed in ${timeDelta}"

    timeDelta=`time_timeSinceRememberedMoment "modelProcessingStart"`
    log_printInfoLine "========================================================"
    log_printInfoLine " Total model processing time with PySD was $timeDelta ms"
    log_printInfoLine "========================================================"
}

ops_executeXmileModelWithPySD() {
    local PySD_CMD=$1
    local PATH_TO_MODEL=$2
    local timeDelta=0
    local modelCompilationTime=0

    LAST_OPERATION_RESULT["$OPS_MOMENT_MODEL_CODE_GENERATION"]=0
    LAST_OPERATION_RESULT["$OPS_MOMENT_MODEL_CODE_COMPILATION"]=0
    LAST_OPERATION_RESULT["$OPS_MOMENT_MODEL_CODE_EXECUTION"]=0

    log_printSectionHeader "Running model with PySD"

    log_printInfoLine "Model paht: $PATH_TO_MODEL"
    time_rememberMoment "modelProcessingStart"

    log_printInfoLine "Generating model code..."
    LAST_OPERATION_RESULT["$OPS_MOMENT_MODEL_CODE_GENERATION"]=0
    log_printInfoLine " - completed in 0"

    log_printInfoLine "Compiling model code"
    time_rememberMoment "modelGeneratedCodeCompilationStart"
    $PySD_CMD compileXmile $PATH_TO_MODEL 2>&1 | log_wrapExternalLogOutput
    if util_lastOperationFailed ${PIPESTATUS[0]}
    then
        return 3
    fi
    modelCompilationTime=`time_timeSinceRememberedMoment "modelGeneratedCodeCompilationStart"`
    LAST_OPERATION_RESULT["$OPS_MOMENT_MODEL_CODE_COMPILATION"]=${modelCompilationTime}
    log_printInfoLine " - completed in ${modelCompilationTime}"

    log_printInfoLine "Running compiled model"
    time_rememberMoment "modelExecutionStart"
    $PySD_CMD compileAndRunXmile $PATH_TO_MODEL 2>&1 | log_wrapExternalLogOutput
    if util_lastOperationFailed ${PIPESTATUS[0]}
    then
        return 4
    fi
    timeDelta=`time_timeSinceRememberedMoment "modelExecutionStart"`
    #timeDelta=`expr $timeDelta - $modelCompilationTime`
    LAST_OPERATION_RESULT["$OPS_MOMENT_MODEL_CODE_EXECUTION"]=${timeDelta}
    log_printInfoLine " - completed in ${timeDelta}"

    timeDelta=`time_timeSinceRememberedMoment "modelProcessingStart"`
    log_printInfoLine "========================================================"
    log_printInfoLine " Total model processing time with PySD was $timeDelta ms"
    log_printInfoLine "========================================================"
}