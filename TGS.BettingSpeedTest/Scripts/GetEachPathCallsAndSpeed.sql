SELECT
   "path",
   COUNT("path") AS "Calls",
   MIN("execution_time_ms") AS "Min",
   MAX("execution_time_ms") AS "Max",
   to_char(AVG("execution_time_ms"), '99999D99') AS "Avg" 
FROM
   dbo."api_logs" 

WHERE
   "prefix" = 'sbo' 
   AND "response_code" = 200 
   AND "recorded_on" BETWEEN '2021-05-04 10:45:00.000+08:00' AND '2021-05-04 18:48:00.000+08:00' 
   AND "execution_time_ms" > 0 
   AND 
   (
      "path" LIKE '%deduct%' 
      OR "path" LIKE '%settle%' 
   )
   GROUP BY
   "path" 
ORDER BY
   1 ;