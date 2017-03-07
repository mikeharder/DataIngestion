package main

import (
    "bytes"
    "encoding/json"
    "errors"
    "io/ioutil"
    "net/http"
    "os"
    "strconv"
    "github.com/valyala/fasthttp"
)

type Payload struct {
    Data string `json:"data"`
}

var _, usefasthttp = os.LookupEnv("FASTHTTP")

func handler(w http.ResponseWriter, r *http.Request) {
    decoder := json.NewDecoder(r.Body)

    var p Payload
    err := decoder.Decode(&p)
    if err != nil {
        http.Error(w, http.StatusText(400), 400)
    }

    if usefasthttp {
        err = redirectPayloadFastHttp(p, "http://go-fasthttp-backend:8080/ingest/data")
    } else {
        err = redirectPayloadNetHttp(p, "http://go-nethttp-backend:8080/ingest/data")
    }

    if err != nil {
        http.Error(w, err.Error(), 502)
    }
}

// Increase connection limits to improve connection reuse and prevent client socket exhaustion
// http://tleyden.github.io/blog/2016/11/21/tuning-the-go-http-client-library-for-load-testing/
var httpClient = &http.Client{
    Transport: &http.Transport{
        MaxIdleConns: 1024,
        MaxIdleConnsPerHost: 1024,
    },
}

func redirectPayloadNetHttp(p Payload, url string) error {
    b := new(bytes.Buffer)
    json.NewEncoder(b).Encode(p)
    r, err := httpClient.Post(url, "application/json; charset=utf-8", b)
    if err != nil {
        return err
    }
    defer r.Body.Close()

    // Body must be read so connection can be reused, otherwise client sockets will be exhausted
    // http://tleyden.github.io/blog/2016/11/21/tuning-the-go-http-client-library-for-load-testing/
    _, err = ioutil.ReadAll(r.Body)

    return nil
}

// http://big-elephants.com/2016-12/fasthttp-client/
func redirectPayloadFastHttp(p Payload, url string) error {
    b := new(bytes.Buffer)
    json.NewEncoder(b).Encode(p)

    req := fasthttp.AcquireRequest()
    req.SetRequestURI(url)
    req.Header.SetMethod("POST")
    req.Header.SetContentType("application/json; charset=utf-8")
    req.SetBody(b.Bytes())

    resp := fasthttp.AcquireResponse()
    client := &fasthttp.Client{}
    client.Do(req, resp)

    if resp.StatusCode() != 200 {
        return errors.New(strconv.Itoa(resp.StatusCode()))
    }

    _ = resp.Body()

    fasthttp.ReleaseResponse(resp);
    fasthttp.ReleaseRequest(req);

    return nil
}

func main() {
    http.HandleFunc("/ingest/event", handler)
    http.ListenAndServe(":8080", nil)
}
