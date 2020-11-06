#!/bin/sh

file_listAllVensimModels() {
    find ./testModels -type f -name "*.$CONST_VENSIM_MODEL_EXT"
}

file_listAllXMILEModels() {
    find ./testModels -type f -name "*.$CONST_XMILE_MODEL_EXT"
}