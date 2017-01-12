package main

import (
    "encoding/json"
    "net/http"
)

type Payload struct {
    Data string `json:"data"`
}

func handler(w http.ResponseWriter, r *http.Request) {
    decoder := json.NewDecoder(r.Body)

    var p Payload
    err := decoder.Decode(&p)
    if err != nil {
        http.Error(w, http.StatusText(400), 400)
    }
}

func main() {
    http.HandleFunc("/", handler)
    http.ListenAndServe(":8080", nil)
}
