SELECT
   "ServerName",
   COUNT("ServerName") AS "Calls" 
FROM
   dbo."ApiCallLogs" 
WHERE
   "Prefix" = 'dopamine' 
   AND "ResponseCode" = 200 
   AND "RecordedOn" BETWEEN '2020-03-13 18:45:00.000+08:00' AND '2020-03-13 18:47:00.000+08:00' 
   AND "ExecutionTimeMs" > 0 
   AND 
   (
      "Path" LIKE '%bets/place%' 
      OR "Path" LIKE '%bets/settle%' 
   )
GROUP BY
   "ServerName" 
ORDER BY
   1 ;