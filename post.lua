local pipelineDepth = 1

function init(args)
   wrk.method = "POST"
   wrk.body   = "{ \"data\": \"{'job_id':'c4bb6d130003','container_id':'ab7b85dcac72','status':'Success: process exited with code 0.'}\" }"
   wrk.headers["Content-Type"] = "Content-Type: application/json"

   if args[1] ~= nil then
      pipelineDepth = args[1]
   end

   local r = {}
   for i = 1, pipelineDepth, 1 do
      r[i] = wrk.format(nil)
   end

   req = table.concat(r)
end

function request()
   return req
end
