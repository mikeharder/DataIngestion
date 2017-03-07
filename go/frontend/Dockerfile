FROM golang:1.8.0-alpine

RUN apk add --no-cache git

EXPOSE 8080

RUN go get -u github.com/valyala/fasthttp/...

ADD . /go/src/app
RUN go install app

ENTRYPOINT /go/bin/app
