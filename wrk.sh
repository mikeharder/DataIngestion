wrk \
-H "Host: localhost" \
-H "Accept: text/plain,text/html;q=0.9,application/xhtml+xml;q=0.9,application/xml;q=0.8,*/*;q=0.7" \
-H "Connection: keep-alive" \
--latency \
-d 15 \
-c $3 \
--timeout 8 \
-t 32 \
http://$1:$2/ingest/event \
-s post.lua
