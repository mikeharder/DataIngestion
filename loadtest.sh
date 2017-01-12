loadtest -k -p payload.json -T application.json -c 50 -n 10000 http://$1:$2/ingest/event
