FROM golang:alpine

EXPOSE 8080

ADD . /go/src/app

RUN go install app

ENTRYPOINT /go/bin/app
