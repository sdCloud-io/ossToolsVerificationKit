export function remove(arr, indexes) {
    return arr.splice(indexes, 1)
}

export function onlyUnique(value, index, self) {
    return self.indexOf(value) === index;
}