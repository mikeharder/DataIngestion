package main

import (
    "bytes"
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

/*
    err = redirectPayload(p, "http://go-backend:8080")
    if err != nil {
        http.Error(w, http.StatusText(502), 502)
    }
*/
}

func redirectPayload(p Payload, url string) error {
    b := new(bytes.Buffer)
    json.NewEncoder(b).Encode(p)
    r, err := http.Post(url, "application/json; charset=utf-8", b)
    if err != nil {
        return err
    }
    defer r.Body.Close()
    return nil
}

func main() {
    http.HandleFunc("/", handler)
    http.ListenAndServe(":8080", nil)
}
