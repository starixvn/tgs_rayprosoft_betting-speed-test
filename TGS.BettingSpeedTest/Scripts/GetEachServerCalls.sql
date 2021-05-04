SELECT
   "server_name",
   COUNT("server_name") AS "Calls" 
FROM
   dbo."api_logs" 
WHERE
   "prefix" = 'sbo' 
   AND "response_code" = 200 
   AND "recorded_on" BETWEEN '2021-05-04 10:00:00.000+08:00' AND '2021-05-04 18:47:00.000+08:00' 
   AND "execution_time_ms" > 0 
   AND 
   (
      "path" LIKE '%deduct%' 
      OR "path" LIKE '%settle%' 
   )
GROUP BY
   "server_name" 
ORDER BY
   1 ;