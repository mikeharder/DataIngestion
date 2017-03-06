FROM golang:1.8.0-alpine

EXPOSE 8080

ADD . /go/src/app

RUN go install app

ENTRYPOINT /go/bin/app
